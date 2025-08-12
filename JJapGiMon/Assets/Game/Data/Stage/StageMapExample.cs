using UnityEngine;

public class StageMapExample : MonoBehaviour
{
    [Header("스테이지 데이터")]
    [SerializeField] private StageData stageData;

    [Header("스테이지 생성 설정 (StageData가 없을 때 사용)")]
    [SerializeField] private int stageSeed = 12345;
    [SerializeField] private int stageDepth = 5;
    [SerializeField] private int maxWidth = 3;

    [Header("디버그 출력")]
    [SerializeField] private bool printToConsole = true;

    private StageMapGenerator generator;
    private StageNode stageMap;

    void Start()
    {
        // StageData가 없으면 기본값으로 생성
        if (stageData == null)
        {
            stageData = new StageData(
                stageId: 1,
                stageName: "테스트 스테이지",
                stageDescription: "테스트용 스테이지입니다.",
                stageLength: stageDepth,
                choicesPerStep: maxWidth,
                randomSeed: stageSeed,
                lastRoomCount: 3
            );
        }

        // 스테이지 맵 생성기 초기화
        generator = new StageMapGenerator(stageData);
        
        // 스테이지 맵 생성
        stageMap = generator.GenerateStageMap();
        
        if (printToConsole)
        {
            Debug.Log($"=== {stageData.stageName} 생성 완료 ===");
            Debug.Log($"스테이지 정보: {stageData}");
            PrintStageMapToConsole(stageMap);
        }
    }

    private void PrintStageMapToConsole(StageNode node, string indent = "")
    {
        string nodeInfo = $"{indent}{node}";
        Debug.Log(nodeInfo);
        
        foreach (var child in node.children)
        {
            PrintStageMapToConsole(child, indent + "  ");
        }
    }

    /// <summary>
    /// 특정 시드값으로 새로운 스테이지 맵을 생성합니다.
    /// </summary>
    public void GenerateNewStageMap(int newSeed)
    {
        stageData.randomSeed = newSeed;
        generator = new StageMapGenerator(stageData);
        stageMap = generator.GenerateStageMap();
        
        if (printToConsole)
        {
            Debug.Log($"=== 새로운 시드({newSeed})로 생성된 스테이지 맵 ===");
            PrintStageMapToConsole(stageMap);
        }
    }

    /// <summary>
    /// 새로운 StageData로 스테이지 맵을 생성합니다.
    /// </summary>
    public void GenerateStageMapWithData(StageData newStageData)
    {
        stageData = newStageData;
        generator = new StageMapGenerator(stageData);
        stageMap = generator.GenerateStageMap();
        
        if (printToConsole)
        {
            Debug.Log($"=== 새로운 스테이지 데이터로 생성된 맵 ===");
            Debug.Log($"스테이지 정보: {stageData}");
            PrintStageMapToConsole(stageMap);
        }
    }

    /// <summary>
    /// 현재 스테이지 맵에서 특정 노드를 찾습니다.
    /// </summary>
    public StageNode FindNode(int targetDepth, int targetIndex)
    {
        return FindNodeRecursive(stageMap, targetDepth, targetIndex);
    }

    private StageNode FindNodeRecursive(StageNode node, int targetDepth, int targetIndex)
    {
        if (node.depth == targetDepth && node.index == targetIndex)
        {
            return node;
        }

        foreach (var child in node.children)
        {
            var result = FindNodeRecursive(child, targetDepth, targetIndex);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// 특정 깊이의 모든 노드를 반환합니다.
    /// </summary>
    public StageNode[] GetNodesAtDepth(int depth)
    {
        var nodes = generator.GetNodesAtDepth(stageMap, depth);
        return nodes.ToArray();
    }

    /// <summary>
    /// 모든 노드를 반환합니다.
    /// </summary>
    public StageNode[] GetAllNodes()
    {
        var nodes = generator.GetAllNodes(stageMap);
        return nodes.ToArray();
    }

    /// <summary>
    /// 현재 방문 가능한 노드들을 반환합니다.
    /// </summary>
    public StageNode[] GetAvailableNodes()
    {
        var allNodes = GetAllNodes();
        var availableNodes = new System.Collections.Generic.List<StageNode>();
        
        foreach (var node in allNodes)
        {
            if (node.isAvailable && !node.isVisited)
            {
                availableNodes.Add(node);
            }
        }
        
        return availableNodes.ToArray();
    }

    /// <summary>
    /// 특정 노드를 방문 처리하고 자식 노드들을 접근 가능하게 만듭니다.
    /// </summary>
    public void VisitNode(StageNode node)
    {
        if (node != null && node.isAvailable)
        {
            node.Visit();
            node.MakeChildrenAvailable();
            
            if (printToConsole)
            {
                Debug.Log($"방문: {node.roomName}");
            }
        }
    }

    /// <summary>
    /// 스테이지 맵의 진행 상황을 출력합니다.
    /// </summary>
    public void PrintProgress()
    {
        var allNodes = GetAllNodes();
        int totalNodes = allNodes.Length;
        int visitedNodes = 0;
        int availableNodes = 0;

        foreach (var node in allNodes)
        {
            if (node.isVisited) visitedNodes++;
            if (node.isAvailable && !node.isVisited) availableNodes++;
        }

        Debug.Log($"스테이지 진행률: {visitedNodes}/{totalNodes} (방문됨), {availableNodes}개 접근 가능");
    }
}
