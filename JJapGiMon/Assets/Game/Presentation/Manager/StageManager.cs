using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class StageManager : MonoBehaviour
{
    [Header("스테이지 설정")]
    [SerializeField] private StageData currentStageData;
    [SerializeField] private StageNode rootNode;
    [SerializeField] private StageNode currentNode;

    [Header("스테이지 진행 상태")]
    [SerializeField] private bool isStageActive = false;
    [SerializeField] private int currentDepth = 0;

    // 초기화 여부
    private bool _initialized;

    // 세션 정보
    private StageLaunchArgs sessionArgs;

    private StageMapGenerator stageMapGenerator;
    private IStageRepository stageRepository;

    public StageData CurrentStageData => currentStageData;
    public StageNode CurrentStageMap => rootNode;
    public StageNode CurrentNode => currentNode;
    public bool IsStageActive => isStageActive;
    public int CurrentDepth => currentDepth;


    // 이벤트
    public event Action<int, StageNode> OnStageGenerated;

    
    void Awake()
    {
        // Awake는 최소셋업만 유지하거나 비우는걸 권장!
    }


    // 초기화
    public void Initialize(StageLaunchArgs args)
    {
        sessionArgs = args ?? new StageLaunchArgs { StageId = -1 };

        // 1) Repository 준비
        stageRepository = new LocalStageRepository();

        // 2) Stage 데이터 로드 (args.contentID 사용)
        var stageConfig = new StageConfig { 
            stageId = sessionArgs.StageId,
            randomSeed = sessionArgs.Seed
        };
        currentStageData = LoadStage(sessionArgs.ContentId) ?? new StageData();

        // 3) 스테이지 생성기 준비
        stageMapGenerator = new StageMapGenerator(stageConfig);

        _initialized = true;
    }

    /// <summary>
    /// 새로운 스테이지를 시작합니다.
    /// </summary>
    public void StartStage()
    {
        if (!_initialized) 
        {
            Debug.LogError("StageManager 초기화 되지 않음.");
            return;
        }
        
        // 🆕 완전한 스테이지 데이터 생성
        if (currentStageData.allNodes == null || currentStageData.allNodes.Count == 0)
        {
            // 맵 데이터가 없으면 생성
            stageMapGenerator = new StageMapGenerator(sessionArgs.Seed ?? Environment.TickCount);
            currentStageData = stageMapGenerator.GenerateCompleteStageData();
        }
        
        // 🆕 저장된 맵 데이터로 복원
        rootNode = RestoreStageMap(currentStageData.rootNode, currentStageData.allNodes);
        
        // 디버깅: 복원된 맵 구조 출력
        Debug.Log("=== 복원된 맵 구조 ===");
        PrintMapStructure(rootNode);
        
        // 현재 노드 설정
        currentNode = GetNodeById(currentStageData.currentNodeId);
        if (currentNode == null)
        {
            currentNode = rootNode; // 시작 노드로 설정
            currentStageData.currentNodeId = currentNode.nodeId;
        }
        
        currentDepth = currentNode.depth;
        isStageActive = true;
        
        Debug.Log($"스테이지 시작: {currentStageData.stageName}");


        OnStageGenerated?.Invoke(sessionArgs.StageId, rootNode);
    }

    /// <summary>
    /// 저장된 맵 데이터로 StageNode 구조 복원
    /// </summary>
    private StageNode RestoreStageMap(StageNodeData rootData, List<StageNodeData> allNodes)
    {
        var nodeMap = allNodes.ToDictionary(n => n.nodeId);
        var restoredNodes = new Dictionary<int, StageNode>(); // 노드 캐싱
        return RestoreNodeRecursive(rootData, nodeMap, restoredNodes);
    }

    private StageNode RestoreNodeRecursive(StageNodeData nodeData, Dictionary<int, StageNodeData> nodeMap, Dictionary<int, StageNode> restoredNodes)
    {
        // 이미 복원된 노드가 있으면 반환
        if (restoredNodes.ContainsKey(nodeData.nodeId))
        {
            return restoredNodes[nodeData.nodeId];
        }

        // StageNode 생성 (런타임 정보는 생성자에서 자동으로 설정됨)
        var node = new StageNode(nodeData.depth, nodeData.index, nodeData.type, nodeData.eventType, nodeData.battleType, nodeData.seed)
        {
            nodeId = nodeData.nodeId,
            state = nodeData.state
        };

        // 캐시에 추가
        restoredNodes[nodeData.nodeId] = node;

        // 자식 노드들 복원
        foreach (var childId in nodeData.childNodeIds)
        {
            if (nodeMap.ContainsKey(childId))
            {
                var childNode = RestoreNodeRecursive(nodeMap[childId], nodeMap, restoredNodes);
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
        return FindNodeRecursive(rootNode, nodeId);
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
    /// 맵 구조를 출력합니다 (디버깅용)
    /// </summary>
    private void PrintMapStructure(StageNode node, string indent = "")
    {
        Debug.Log($"{indent}{node.roomName} (ID: {node.nodeId}, Depth: {node.depth}, Type: {node.type})");
        
        foreach (var child in node.children)
        {
            PrintMapStructure(child, indent + "  ");
        }
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
                
            case EventRoomType.Story:
                Debug.Log("스토리 이벤트가 발생했습니다.");
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
            case BattleRoomType.Normal:
                Debug.Log("일반 전투를 시작합니다.");
                break;
        }
        
        // 전투 씬으로 전환
        // SceneManager.LoadScene("BattleScene");
    }

    private void ProcessBossRoom()
    {
        Debug.Log($"보스 방 처리: {currentNode.roomName}");
        
        if (currentNode.state == StageStateType.SUCCESS)
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
        if (rootNode != null)
        {
            Debug.Log("=== 현재 스테이지 맵 ===");
            PrintNodeRecursive(rootNode, "");
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
    /// 현재 스테이지 데이터를 저장합니다.
    /// </summary>
    public void SaveStage()
    {
        if (currentStageData != null)
        {
            try
            {
                stageRepository.Save(sessionArgs.ContentId, currentStageData);
                Debug.Log("스테이지 데이터 저장 완료");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"스테이지 저장 중 오류 발생: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("저장할 스테이지 데이터가 없습니다.");
        }
    }

    /// <summary>
    /// 가장 최근 저장된 스테이지 데이터를 로드합니다.
    /// </summary>
    /// <returns>로드 성공 여부</returns>
    #nullable enable
    public StageData? LoadStage(string contentId)
    {
        try
        {
            var stageData = stageRepository.Load(contentId);
            if (stageData != null)
            {
                Debug.Log("스테이지 데이터 로드 완료");
                return stageData;
            }
            else
            {
                Debug.Log("저장된 스테이지 데이터가 없습니다.");
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"스테이지 로드 중 오류 발생: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 저장된 스테이지 데이터가 있는지 확인합니다.
    /// </summary>
    /// <returns>저장 데이터 존재 여부</returns>
    public bool HasSaveData(string contentId)
    {
        return stageRepository.HasSaveData(contentId);
    }

    /// <summary>
    /// 저장된 스테이지 데이터를 삭제합니다.
    /// </summary>
    public void DeleteSaveData(string contentId)
    {
        try
        {
            stageRepository.Delete(contentId);
            Debug.Log("저장된 스테이지 데이터 삭제 완료");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"스테이지 데이터 삭제 중 오류 발생: {ex.Message}");
        }
    }

    // 기존 JSON 저장/로드 메서드들 (호환성 유지)
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
    /// 자동 저장 (PlayerPrefs 사용)
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
    /// 스테이지 포기
    /// </summary>
    public void AbandonStage()
    {
        isStageActive = false;
        currentStageData.isFailed = true;
        Debug.Log("스테이지를 포기했습니다.");
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
