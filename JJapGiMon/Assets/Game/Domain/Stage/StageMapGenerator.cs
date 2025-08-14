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
        // 1. 기본 맵 구조 생성
        var rootNode = GenerateStageMap();
        
        // 2. 모든 노드에 기본 데이터 할당
        var allNodes = GetAllNodesWithData(rootNode);
        
        // 3. 완전한 StageData 생성
        var completeStageData = new StageData
        {
            stageId = stageData.stageId,
            stageName = stageData.stageName,
            stageDescription = stageData.stageDescription,
            stageLength = stageData.stageLength,
            choicesPerStep = stageData.choicesPerStep,
            randomSeed = stageData.randomSeed,
            lastRoomCount = stageData.lastRoomCount,
            
            // 🆕 맵 데이터
            rootNode = ConvertToNodeData(rootNode),
            allNodes = allNodes,
            
            // 🆕 초기 상태 설정
            currentNodeId = rootNode.nodeId,
            visitedNodeIds = new List<int>(),
            availableNodeIds = new List<int>(),
            characterStates = stageData.characterStates ?? new List<CharacterSaveData>(),
            isCompleted = false,
            isFailed = false
        };
        
        // 4. 시작 노드의 자식들을 접근 가능하게 설정
        foreach (var childId in completeStageData.rootNode.childNodeIds)
        {
            completeStageData.MakeNodeAvailable(childId);
        }
        
        // 5. 노드 맵 초기화
        completeStageData.InitializeNodeMap();
        
        return completeStageData;
    }

    /// <summary>
    /// 스테이지 맵을 생성합니다.
    /// </summary>
    public StageNode GenerateStageMap()
    {
        var root = new StageNode(0, 0, StageRoomType.Start, null, null, random.Next());
        root.nodeId = GetNextNodeId();
        
        if (stageData.stageLength > 0)
        {
            GenerateChildren(root, 1, stageData.stageLength);
        }
        
        return root;
    }

    /// <summary>
    /// 스테이지 맵을 생성합니다. (기존 호환성)
    /// </summary>
    public StageNode GenerateStageMap(int depth, int maxWidth = 3)
    {
        var root = new StageNode(0, 0, StageRoomType.Start, null, null, random.Next());
        root.nodeId = GetNextNodeId();
        
        if (depth > 1)
        {
            GenerateChildren(root, 1, depth, maxWidth);
        }
        
        return root;
    }

    private void GenerateChildren(StageNode parent, int currentDepth, int maxDepth, int maxWidth = 3)
    {
        if (currentDepth >= maxDepth)
        {
            // 마지막 깊이에서는 보스 방들 생성 (lastRoomCount만큼)
            for (int i = 0; i < stageData.lastRoomCount; i++)
            {
                var bossNode = new StageNode(currentDepth, i, StageRoomType.Boss, null, null, random.Next());
                bossNode.nodeId = GetNextNodeId();
                bossNode.isGoal = true;
                parent.AddChild(bossNode);
            }
            return;
        }

        // 현재 깊이에서 생성할 노드 수 결정 (최소 1개, 최대 choicesPerStep)
        int nodeCount = random.Next(1, Math.Min(stageData.choicesPerStep + 1, maxWidth + 1));
        
        for (int i = 0; i < nodeCount; i++)
        {
            // 방 타입 결정 (시드값을 다르게 하여 다양한 결과 생성)
            int seed = random.Next();
            StageRoomType roomType = DetermineRoomType(currentDepth, maxDepth, seed);
            
            // 방 타입에 따른 세부 타입 결정
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
            
            var childNode = new StageNode(currentDepth, i, roomType, eventType, battleType, seed);
            childNode.nodeId = GetNextNodeId();
            parent.AddChild(childNode);
            
            // 다음 깊이로 재귀
            GenerateChildren(childNode, currentDepth + 1, maxDepth, maxWidth);
        }
    }

    private StageRoomType DetermineRoomType(int currentDepth, int maxDepth, int seed)
    {
        var localRandom = new Random(seed);
        
        // 마지막은 보스 전용 전투
        if (currentDepth == maxDepth)
        {
            return StageRoomType.Boss; // 보스 전투
        }
        
        // 첫 번째 깊이는 시작 방 다음이므로 이벤트나 전투
        if (currentDepth == 1)
        {
            return localRandom.Next(2) == 0 ? StageRoomType.Event : StageRoomType.Battle;
        }
        
        // 중간 깊이에서는 이벤트, 전투 중 선택
        return localRandom.Next(2) == 0 ? StageRoomType.Event : StageRoomType.Battle;
    }

    /// <summary>
    /// 모든 노드에 기본 데이터를 할당하여 StageNodeData 리스트 생성
    /// </summary>
    private List<StageNodeData> GetAllNodesWithData(StageNode rootNode)
    {
        var allNodes = new List<StageNodeData>();
        CollectNodesWithData(rootNode, allNodes);
        return allNodes;
    }

    private void CollectNodesWithData(StageNode node, List<StageNodeData> allNodes)
    {
        var nodeData = ConvertToNodeData(node);
        allNodes.Add(nodeData);
        
        foreach (var child in node.children)
        {
            CollectNodesWithData(child, allNodes);
        }
    }

    /// <summary>
    /// StageNode를 StageNodeData로 변환 (저장용 데이터만 추출)
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
            isGoal = node.isGoal,
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
}
