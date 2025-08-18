using System;
using System.Collections.Generic;
using System.Linq;

public class StageMapGenerator
{
    private readonly Random random;
    private readonly StageData stageData;
    private int nextNodeId = 0;

    public StageMapGenerator(StageData stageData)
    {
        this.stageData = stageData;
        this.random = new Random(stageData.randomSeed ?? Environment.TickCount);
    }

    public StageMapGenerator(int seed)
    {
        this.stageData = new StageData();
        this.random = new Random(seed);
    }

    /// <summary>
    /// 완전한 스테이지 데이터를 생성합니다.
    /// </summary>
    public StageData GenerateCompleteStageData()
    {
        // 1. 깊이별 노드 생성
        var nodesByDepth = GenerateNodesByDepth();
        
        // 2. 노드들 간의 무작위 연결 생성
        GenerateRandomConnections(nodesByDepth);
        
        // 3. 모든 노드를 StageNodeData로 변환
        var allNodes = ConvertAllNodesToData(nodesByDepth);
        
        // 4. 완전한 StageData 생성
        var completeStageData = new StageData
        {
            stageId = stageData.stageId,
            stageName = stageData.stageName,
            stageDescription = stageData.stageDescription,
            stageLength = stageData.stageLength,
            MaxNodeCountByDepth = stageData.MaxNodeCountByDepth,
            randomSeed = stageData.randomSeed,
            lastRoomCount = stageData.lastRoomCount,
            
            // 맵 데이터
            rootNode = allNodes.First(n => n.depth == 0),
            allNodes = allNodes,
            
            // 초기 상태 설정
            currentNodeId = allNodes.First(n => n.depth == 0).nodeId,
            visitedNodeIds = new List<int>(),
            availableNodeIds = new List<int>(),
            characterStates = stageData.characterStates ?? new List<CharacterSaveData>(),
            isCompleted = false,
            isFailed = false
        };
        
        // 5. 시작 노드의 자식들을 접근 가능하게 설정
        foreach (var childId in completeStageData.rootNode.childNodeIds)
        {
            completeStageData.MakeNodeAvailable(childId);
        }
        
        // 6. 노드 맵 초기화
        completeStageData.InitializeNodeMap();
        
        return completeStageData;
    }

    /// <summary>
    /// 깊이별로 노드들을 생성합니다.
    /// </summary>
    private Dictionary<int, List<StageNode>> GenerateNodesByDepth()
    {
        var nodesByDepth = new Dictionary<int, List<StageNode>>();
        
        // 깊이 0: 시작 노드 (1개)
        var startNode = new StageNode(0, 0, StageRoomType.Start, null, null, random.Next());
        startNode.nodeId = GetNextNodeId();
        startNode.state = StageStateType.NEUTRAL;
        nodesByDepth[0] = new List<StageNode> { startNode };
        
        // 깊이 1 ~ stageLength-2: 중간 노드들 (MinNodeCountByDepth~MaxNodeCountByDepth개)
        for (int depth = 1; depth < stageData.stageLength - 1; depth++)
        {
            int nodeCount = random.Next(stageData.MinNodeCountByDepth, stageData.MaxNodeCountByDepth + 1);
            var nodes = new List<StageNode>();
            
            for (int i = 0; i < nodeCount; i++)
            {
                var node = CreateRandomNode(depth, i);
                nodes.Add(node);
            }
            
            nodesByDepth[depth] = nodes;
        }
        
        // 마지막 깊이: lastRoomCount개 노드
        int lastDepth = stageData.stageLength - 1;
        var lastNodes = new List<StageNode>();
        
        // 보스방의 위치를 무작위로 결정
        int bossRoomIndex = random.Next(0, stageData.lastRoomCount);
        
        for (int i = 0; i < stageData.lastRoomCount; i++)
        {
            bool isBossRoom = (i == bossRoomIndex);
            StageRoomType roomType = isBossRoom ? StageRoomType.Boss : StageRoomType.Event;
            EventRoomType? eventType = isBossRoom ? null : DetermineEventType(random.Next());
            
            var node = new StageNode(lastDepth, i, roomType, eventType, null, random.Next());
            node.nodeId = GetNextNodeId();
            node.state = isBossRoom ? StageStateType.SUCCESS : StageStateType.FAIL;
            lastNodes.Add(node);
        }
        
        nodesByDepth[lastDepth] = lastNodes;
        
        return nodesByDepth;
    }

    /// <summary>
    /// 랜덤한 중간 노드를 생성합니다.
    /// </summary>
    private StageNode CreateRandomNode(int depth, int index)
    {
        int seed = random.Next();
        StageRoomType roomType = DetermineRoomType(seed);
        
        EventRoomType? eventType = null;
        BattleRoomType? battleType = null;
        
        if (roomType == StageRoomType.Event)
        {
            eventType = DetermineEventType(seed);
        }
        else if (roomType == StageRoomType.Battle)
        {
            battleType = DetermineBattleType(seed);
        }
        
        var node = new StageNode(depth, index, roomType, eventType, battleType, seed);
        node.nodeId = GetNextNodeId();
        node.state = StageStateType.NEUTRAL;
        
        return node;
    }

    /// <summary>
    /// 노드들 간의 무작위 연결을 생성합니다.
    /// </summary>
    private void GenerateRandomConnections(Dictionary<int, List<StageNode>> nodesByDepth)
    {
        // 각 깊이의 노드들을 순회하면서 인접한 깊이의 노드들과 연결
        for (int depth = 0; depth < stageData.stageLength - 1; depth++)
        {
            var currentNodes = nodesByDepth[depth];
            var nextNodes = nodesByDepth[depth + 1];
            
            // 먼저 모든 노드가 최소 하나의 연결을 가지도록 보장
            EnsureMinimumConnections(currentNodes, nextNodes);
            
            // 간선 수 제한을 적용한 추가 연결 생성
            GenerateLimitedRandomConnections(currentNodes, nextNodes);
        }
    }

    /// <summary>
    /// 모든 노드가 최소 하나의 연결을 가지도록 보장합니다.
    /// </summary>
    private void EnsureMinimumConnections(List<StageNode> currentNodes, List<StageNode> nextNodes)
    {
        // 현재 깊이의 노드들이 다음 깊이의 노드들과 연결되지 않은 경우 처리
        var unconnectedCurrentNodes = currentNodes.Where(n => n.children.Count == 0).ToList();
        var unconnectedNextNodes = nextNodes.Where(n => n.parent == null).ToList();
        
        // 연결되지 않은 현재 노드들을 다음 노드들과 연결
        foreach (var currentNode in unconnectedCurrentNodes)
        {
            // 아직 부모가 없는 다음 노드 중에서 선택
            var availableNextNodes = nextNodes.Where(n => n.parent == null).ToList();
            if (availableNextNodes.Count == 0)
            {
                // 모든 다음 노드가 이미 연결되어 있다면 랜덤하게 선택
                availableNextNodes = nextNodes;
            }
            
            var targetNextNode = availableNextNodes[random.Next(availableNextNodes.Count)];
            currentNode.AddChild(targetNextNode);
        }
        
        // 연결되지 않은 다음 노드들을 현재 노드들과 연결
        var stillUnconnectedNextNodes = nextNodes.Where(n => n.parent == null).ToList();
        foreach (var nextNode in stillUnconnectedNextNodes)
        {
            var targetCurrentNode = currentNodes[random.Next(currentNodes.Count)];
            targetCurrentNode.AddChild(nextNode);
        }
    }

    /// <summary>
    /// 간선 수 제한을 적용한 무작위 연결을 생성합니다.
    /// </summary>
    private void GenerateLimitedRandomConnections(List<StageNode> currentNodes, List<StageNode> nextNodes)
    {
        // 현재 깊이 전체가 가질 수 있는 최대 간선 수 (자식 노드 수 + 1개)
        int maxTotalConnections = nextNodes.Count + 1;
        
        // 현재 깊이의 모든 노드가 이미 가진 간선 수의 총합
        int currentTotalConnections = currentNodes.Sum(n => n.children.Count);
        
        // 추가로 생성할 수 있는 총 간선 수
        int remainingConnections = maxTotalConnections - currentTotalConnections;
        
        if (remainingConnections <= 0)
        {
            // 이미 최대 간선 수에 도달했으면 추가 연결하지 않음
            return;
        }
        
        // 추가 연결을 무작위로 분배
        for (int i = 0; i < remainingConnections; i++)
        {
            // 랜덤하게 부모 노드 선택
            var randomParent = currentNodes[random.Next(currentNodes.Count)];
            
            // 아직 연결되지 않은 자식 노드들 찾기
            var availableChildren = nextNodes.Where(n => !randomParent.children.Contains(n)).ToList();
            
            if (availableChildren.Count > 0)
            {
                // 랜덤하게 자식 노드 선택하여 연결
                var randomChild = availableChildren[random.Next(availableChildren.Count)];
                randomParent.AddChild(randomChild);
            }
        }
    }

    /// <summary>
    /// 모든 노드를 StageNodeData로 변환합니다.
    /// </summary>
    private List<StageNodeData> ConvertAllNodesToData(Dictionary<int, List<StageNode>> nodesByDepth)
    {
        var allNodes = new List<StageNodeData>();
        
        foreach (var kvp in nodesByDepth)
        {
            foreach (var node in kvp.Value)
            {
                var nodeData = ConvertToNodeData(node);
                allNodes.Add(nodeData);
            }
        }
        
        return allNodes;
    }

    /// <summary>
    /// StageNode를 StageNodeData로 변환
    /// </summary>
    private StageNodeData ConvertToNodeData(StageNode node)
    {
        var nodeData = new StageNodeData
        {
            nodeId = node.nodeId,
            depth = node.depth,
            index = node.index,
            type = node.type,
            seed = node.seed,
            eventType = node.eventType,
            battleType = node.battleType,
            state = node.state,
            childNodeIds = node.children.Select(c => c.nodeId).ToList(),
            parentNodeId = node.parent?.nodeId
        };

        return nodeData;
    }

    /// <summary>
    /// 다음 노드 ID 생성
    /// </summary>
    private int GetNextNodeId()
    {
        return nextNodeId++;
    }

    /// <summary>
    /// 방 타입 결정
    /// </summary>
    private StageRoomType DetermineRoomType(int seed)
    {
        var localRandom = new Random(seed);
        return localRandom.Next(2) == 0 ? StageRoomType.Event : StageRoomType.Battle;
    }

    /// <summary>
    /// 이벤트 방 타입 결정
    /// </summary>
    private EventRoomType DetermineEventType(int seed)
    {
        var localRandom = new Random(seed);
        var eventTypes = (EventRoomType[])Enum.GetValues(typeof(EventRoomType));
        return eventTypes[localRandom.Next(eventTypes.Length)];
    }

    /// <summary>
    /// 전투 방 타입 결정
    /// </summary>
    private BattleRoomType DetermineBattleType(int seed)
    {
        var localRandom = new Random(seed);
        var battleTypes = (BattleRoomType[])Enum.GetValues(typeof(BattleRoomType));
        return battleTypes[localRandom.Next(battleTypes.Length)];
    }

    // 기존 호환성을 위한 메서드들
    public StageNode GenerateStageMap()
    {
        var completeStageData = GenerateCompleteStageData();
        return ReconstructStageNode(completeStageData.rootNode, completeStageData);
    }

    public StageNode GenerateStageMap(int depth, int maxWidth = 3)
    {
        var tempStageData = new StageData
        {
            stageLength = depth,
            MaxNodeCountByDepth = maxWidth,
            lastRoomCount = 3
        };
        
        var generator = new StageMapGenerator(tempStageData);
        return generator.GenerateStageMap();
    }

    private StageNode ReconstructStageNode(StageNodeData nodeData, StageData stageData)
    {
        var node = new StageNode(nodeData.depth, nodeData.index, nodeData.type, nodeData.eventType, nodeData.battleType, nodeData.seed);
        node.nodeId = nodeData.nodeId;
        node.state = nodeData.state;
        
        foreach (var childId in nodeData.childNodeIds)
        {
            var childData = stageData.GetNodeById(childId);
            if (childData != null)
            {
                var childNode = ReconstructStageNode(childData, stageData);
                node.AddChild(childNode);
            }
        }
        
        return node;
    }

    /// <summary>
    /// 특정 깊이의 모든 노드를 반환합니다.
    /// </summary>
    public List<StageNode> GetNodesAtDepth(StageNode root, int targetDepth)
    {
        var nodes = new List<StageNode>();
        CollectNodesAtDepth(root, targetDepth, nodes);
        return nodes;
    }

    private void CollectNodesAtDepth(StageNode node, int targetDepth, List<StageNode> nodes)
    {
        if (node.depth == targetDepth)
        {
            nodes.Add(node);
        }

        foreach (var child in node.children)
        {
            CollectNodesAtDepth(child, targetDepth, nodes);
        }
    }

    /// <summary>
    /// 스테이지 맵의 모든 노드를 리스트로 반환합니다.
    /// </summary>
    public List<StageNode> GetAllNodes(StageNode root)
    {
        var nodes = new List<StageNode>();
        CollectAllNodes(root, nodes);
        return nodes;
    }

    private void CollectAllNodes(StageNode node, List<StageNode> nodes)
    {
        nodes.Add(node);
        foreach (var child in node.children)
        {
            CollectAllNodes(child, nodes);
        }
    }

    /// <summary>
    /// 스테이지 맵을 시각적으로 출력합니다 (디버깅용)
    /// </summary>
    public void PrintStageMap(StageNode node, string indent = "")
    {
        Console.WriteLine($"{indent}{node}");
        
        foreach (var child in node.children)
        {
            PrintStageMap(child, indent + "  ");
        }
    }
}
