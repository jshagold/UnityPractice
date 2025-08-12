using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StageManager : MonoBehaviour
{
    [Header("스테이지 설정")]
    [SerializeField] private StageData currentStageData;
    [SerializeField] private StageNode currentStageMap;
    [SerializeField] private StageNode currentNode;

    [Header("스테이지 진행 상태")]
    [SerializeField] private bool isStageActive = false;
    [SerializeField] private int currentDepth = 0;

    private StageMapGenerator stageMapGenerator;

    public StageData CurrentStageData => currentStageData;
    public StageNode CurrentStageMap => currentStageMap;
    public StageNode CurrentNode => currentNode;
    public bool IsStageActive => isStageActive;
    public int CurrentDepth => currentDepth;

    void Awake()
    {
        stageMapGenerator = new StageMapGenerator(currentStageData ?? new StageData());
    }

    /// <summary>
    /// 새로운 스테이지를 시작합니다.
    /// </summary>
    public void StartStage(StageData stageData)
    {
        currentStageData = stageData;
        
        // 🆕 완전한 스테이지 데이터 생성
        if (stageData.allNodes == null || stageData.allNodes.Count == 0)
        {
            // 맵 데이터가 없으면 생성
            stageMapGenerator = new StageMapGenerator(stageData);
            currentStageData = stageMapGenerator.GenerateCompleteStageData();
        }
        
        // 🆕 저장된 맵 데이터로 복원
        currentStageMap = RestoreStageMap(currentStageData.rootNode, currentStageData.allNodes);
        
        // 현재 노드 설정
        currentNode = GetNodeById(currentStageData.currentNodeId);
        if (currentNode == null)
        {
            currentNode = currentStageMap; // 시작 노드로 설정
            currentStageData.currentNodeId = currentNode.nodeId;
        }
        
        currentDepth = currentNode.depth;
        isStageActive = true;
        
        Debug.Log($"스테이지 시작: {stageData.stageName}");
    }

    /// <summary>
    /// 저장된 맵 데이터로 StageNode 구조 복원
    /// </summary>
    private StageNode RestoreStageMap(StageNodeData rootData, List<StageNodeData> allNodes)
    {
        var nodeMap = allNodes.ToDictionary(n => n.nodeId);
        return RestoreNodeRecursive(rootData, nodeMap);
    }

    private StageNode RestoreNodeRecursive(StageNodeData nodeData, Dictionary<int, StageNodeData> nodeMap)
    {
        // StageNode 생성
        var node = new StageNode(nodeData.depth, nodeData.index, nodeData.type, nodeData.seed)
        {
            nodeId = nodeData.nodeId,
            isGoal = nodeData.isGoal,
            eventType = nodeData.eventType,
            battleType = nodeData.battleType,
            roomName = nodeData.nodeName,
            roomDescription = nodeData.nodeDescription
        };

        // 자식 노드들 복원
        foreach (var childId in nodeData.childNodeIds)
        {
            if (nodeMap.ContainsKey(childId))
            {
                var childNode = RestoreNodeRecursive(nodeMap[childId], nodeMap);
                node.AddChild(childNode);
            }
        }

        return node;
    }

    /// <summary>
    /// 노드 ID로 노드 찾기
    /// </summary>
    private StageNode GetNodeById(int nodeId)
    {
        return FindNodeRecursive(currentStageMap, nodeId);
    }

    private StageNode FindNodeRecursive(StageNode node, int targetId)
    {
        if (node.nodeId == targetId) return node;
        
        foreach (var child in node.children)
        {
            var result = FindNodeRecursive(child, targetId);
            if (result != null) return result;
        }
        
        return null;
    }

    /// <summary>
    /// 현재 노드에서 특정 자식 노드로 이동합니다.
    /// </summary>
    public bool MoveToNode(int childIndex)
    {
        if (currentNode == null || childIndex < 0 || childIndex >= currentNode.children.Count)
        {
            Debug.LogWarning($"유효하지 않은 노드 인덱스: {childIndex}");
            return false;
        }

        var targetNode = currentNode.children[childIndex];
        
        // StageData를 통한 이동 처리
        bool success = currentStageData.MoveToNode(childIndex);
        if (success)
        {
            // StageNode도 업데이트
            currentNode = targetNode;
            currentDepth = currentNode.depth;
            
            Debug.Log($"노드 이동: {currentNode.roomName} (깊이: {currentDepth})");
        }
        else
        {
            Debug.LogWarning($"접근할 수 없는 노드: {targetNode.roomName}");
        }
        
        return success;
    }

    /// <summary>
    /// 현재 노드에서 방문 가능한 자식 노드들을 반환합니다.
    /// </summary>
    public List<StageNode> GetAvailableChildren()
    {
        var availableChildren = new List<StageNode>();
        
        if (currentNode != null)
        {
            var availableNodeData = currentStageData.GetAvailableChildren();
            foreach (var nodeData in availableNodeData)
            {
                var node = GetNodeById(nodeData.nodeId);
                if (node != null)
                {
                    availableChildren.Add(node);
                }
            }
        }
        
        return availableChildren;
    }

    /// <summary>
    /// 현재 노드의 타입에 따라 적절한 처리를 수행합니다.
    /// </summary>
    public void ProcessCurrentNode()
    {
        if (currentNode == null) return;

        switch (currentNode.type)
        {
            case StageRoomType.Start:
                Debug.Log("시작 지점에 도착했습니다.");
                break;
                
            case StageRoomType.Event:
                ProcessEventRoom();
                break;
                
            case StageRoomType.Battle:
                ProcessBattleRoom();
                break;
                
            case StageRoomType.Boss:
                ProcessBossRoom();
                break;
        }
    }

    private void ProcessEventRoom()
    {
        Debug.Log($"이벤트 방 처리: {currentNode.roomName}");
        
        switch (currentNode.eventType)
        {
            case EventRoomType.Rest:
                Debug.Log("휴식 공간에 입장했습니다. 체력을 회복할 수 있습니다.");
                break;
                
            case EventRoomType.Shop:
                Debug.Log("상점에 입장했습니다. 아이템을 구매할 수 있습니다.");
                break;
                
            case EventRoomType.Maintenance:
                Debug.Log("정비소에 입장했습니다. 장비를 강화할 수 있습니다.");
                break;
                
            case EventRoomType.Event:
                Debug.Log("특별한 이벤트가 발생했습니다.");
                break;
        }
        
        // 이벤트 씬으로 전환
        // SceneManager.LoadScene("EventScene");
    }

    private void ProcessBattleRoom()
    {
        Debug.Log($"전투 방 처리: {currentNode.roomName}");
        
        switch (currentNode.battleType)
        {
            case BattleRoomType.NormalBattle:
                Debug.Log("일반 전투를 시작합니다.");
                break;
                
            case BattleRoomType.EliteBattle:
                Debug.Log("정예 전투를 시작합니다.");
                break;
                
            case BattleRoomType.AmbushBattle:
                Debug.Log("매복 전투를 시작합니다.");
                break;
                
            case BattleRoomType.ArenaBattle:
                Debug.Log("아레나 전투를 시작합니다.");
                break;
        }
        
        // 전투 씬으로 전환
        // SceneManager.LoadScene("BattleScene");
    }

    private void ProcessBossRoom()
    {
        Debug.Log($"보스 방 처리: {currentNode.roomName}");
        
        if (currentNode.isGoal)
        {
            Debug.Log("목표 보스를 처치했습니다!");
            CompleteStage(true);
        }
        else
        {
            Debug.Log("보스 전투를 시작합니다.");
            // 보스 전투 씬으로 전환
            // SceneManager.LoadScene("BossBattleScene");
        }
    }

    /// <summary>
    /// 스테이지를 완료합니다.
    /// </summary>
    public void CompleteStage(bool isSuccess)
    {
        isStageActive = false;
        
        if (isSuccess)
        {
            currentStageData.isCompleted = true;
            Debug.Log($"스테이지 클리어: {currentStageData.stageName}");
            // 성공 처리 - 보상 지급 등
        }
        else
        {
            currentStageData.isFailed = true;
            Debug.Log($"스테이지 실패: {currentStageData.stageName}");
            // 실패 처리
        }
    }

    /// <summary>
    /// 스테이지 진행 상황을 반환합니다.
    /// </summary>
    public StageProgress GetProgress()
    {
        if (currentStageData == null) return new StageProgress();

        return new StageProgress
        {
            totalNodes = currentStageData.allNodes.Count,
            visitedNodes = currentStageData.visitedNodeIds.Count,
            availableNodes = currentStageData.availableNodeIds.Count,
            currentDepth = currentDepth,
            isCompleted = currentStageData.IsCompleted()
        };
    }

    /// <summary>
    /// 스테이지 맵 정보를 디버그 출력합니다.
    /// </summary>
    public void PrintStageMap()
    {
        if (currentStageMap != null)
        {
            Debug.Log("=== 현재 스테이지 맵 ===");
            PrintNodeRecursive(currentStageMap, "");
        }
    }

    private void PrintNodeRecursive(StageNode node, string indent)
    {
        string status = node == currentNode ? " [현재]" : "";
        string visited = currentStageData.visitedNodeIds.Contains(node.nodeId) ? " [방문됨]" : "";
        string available = currentStageData.availableNodeIds.Contains(node.nodeId) ? " [접근가능]" : "";
        
        Debug.Log($"{indent}{node}{status}{visited}{available}");
        
        foreach (var child in node.children)
        {
            PrintNodeRecursive(child, indent + "  ");
        }
    }

    /// <summary>
    /// 현재 스테이지 데이터를 JSON으로 저장
    /// </summary>
    public string SaveStageDataToJson()
    {
        if (currentStageData != null)
        {
            return JsonUtility.ToJson(currentStageData, true);
        }
        return "";
    }

    /// <summary>
    /// JSON에서 스테이지 데이터 로드
    /// </summary>
    public void LoadStageDataFromJson(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            var stageData = JsonUtility.FromJson<StageData>(json);
            if (stageData != null)
            {
                stageData.InitializeNodeMap();
                StartStage(stageData);
            }
        }
    }

    /// <summary>
    /// 자동 저장
    /// </summary>
    public void AutoSave()
    {
        if (currentStageData != null)
        {
            string json = SaveStageDataToJson();
            PlayerPrefs.SetString("StageAutoSave", json);
            PlayerPrefs.Save();
            Debug.Log("스테이지 자동 저장 완료");
        }
    }

    /// <summary>
    /// 자동 저장 데이터 로드
    /// </summary>
    public bool LoadAutoSave()
    {
        string json = PlayerPrefs.GetString("StageAutoSave", "");
        if (!string.IsNullOrEmpty(json))
        {
            LoadStageDataFromJson(json);
            Debug.Log("자동 저장 데이터 로드 완료");
            return true;
        }
        return false;
    }

    /// <summary>
    /// 스테이지 포기
    /// </summary>
    public void AbandonStage()
    {
        isStageActive = false;
        currentStageData.isFailed = true;
        Debug.Log("스테이지를 포기했습니다.");
        // 메인 씬으로 돌아가기
        // SceneManager.LoadScene("MainScene");
    }
}

[System.Serializable]
public class StageProgress
{
    public int totalNodes;
    public int visitedNodes;
    public int availableNodes;
    public int currentDepth;
    public bool isCompleted;

    public float ProgressPercentage => totalNodes > 0 ? (float)visitedNodes / totalNodes * 100f : 0f;
}
