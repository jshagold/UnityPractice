using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StageManager))]
[RequireComponent(typeof(StageInputManager))]
public class StageSceneController : MonoBehaviour
{
    [Header("Stage Configuration")]
    [SerializeField] private StageData stageData;
    
    [Header("Legacy Configuration (StageData가 없을 때 사용)")]
    [SerializeField] private int stageLength = 5;
    [SerializeField] private int choicesPerStep = 3;
    [SerializeField] private int? randomSeed = null;

    [Header("Manager References")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageInputManager stageInputManager;

    [Header("Party Configuration")]
    [Tooltip("List of player character IDs to include in the stage")]
    [SerializeField] private List<string> playerIdList = new List<string>();

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private void Awake()
    {
        Debug.Log("Stage Scene Awake");
        // Ensure Manager references
        if (stageManager == null)
            stageManager = GetComponent<StageManager>();
        if (stageInputManager == null)
            stageInputManager = GetComponent<StageInputManager>();
    }

    private void Start()
    {
        Debug.Log("Stage Scene Start");

        // StageData가 없으면 기본값으로 생성
        if (stageData == null)
        {
            stageData = new StageData(
                stageId: 1,
                stageName: "기본 스테이지",
                stageDescription: "기본 설정으로 생성된 스테이지",
                stageLength: stageLength,
                choicesPerStep: choicesPerStep,
                randomSeed: randomSeed,
                lastRoomCount: 3
            );
        }

        // 캐릭터 정보 세팅
        var partyModels = CreatePartyModels();

        // 새로운 스테이지 시스템으로 초기화
        InitializeNewStageSystem(partyModels);

        // UI 초기화 (기존 시스템과의 호환성을 위해)
        InitializeUI();
    }

    /// <summary>
    /// 새로운 스테이지 시스템 초기화
    /// </summary>
    private void InitializeNewStageSystem(List<CharacterModel> partyModels)
    {
        // StageManager에 스테이지 시작
        stageManager.StartStage(stageData);
        
        if (debugMode)
        {
            Debug.Log($"새로운 스테이지 시스템 초기화 완료: {stageData.stageName}");
            stageManager.PrintStageMap();
        }
    }

    /// <summary>
    /// UI 초기화 (기존 시스템과의 호환성)
    /// </summary>
    private void InitializeUI()
    {
        // 기존 StageInputManager가 새로운 시스템과 호환되도록 수정 필요
        // 현재는 기본적인 초기화만 수행
        if (stageInputManager != null)
        {
            // 새로운 스테이지 맵 정보를 UI에 전달
            var currentMap = stageManager.CurrentStageMap;
            var currentNode = stageManager.CurrentNode;
            
            // StageInputManager의 InitializeStageMap 메서드가 새로운 구조를 지원하도록 수정 필요
            // stageInputManager.InitializeStageMap(currentMap, currentNode);
        }
    }

    /// <summary>
    /// 파티 모델 생성
    /// </summary>
    private List<CharacterModel> CreatePartyModels()
    {
        ICharacterRepository characterRepo = new LocalCharacterRepository();
        CharacterFactory characterFactory = new(characterRepo);
        
        var partyModels = playerIdList.Select(id => characterFactory.Create(id)).ToList();
        
        // 캐릭터 설정 초기화
        partyModels.ForEach(player => { player.SaveData.CurrentHealth = player.MaxHp; });
        
        return partyModels;
    }

    /// <summary>
    /// 현재 노드에서 특정 자식 노드로 이동
    /// </summary>
    public bool MoveToNode(int childIndex)
    {
        bool success = stageManager.MoveToNode(childIndex);
        
        if (success)
        {
            // 노드 이동 후 처리
            ProcessCurrentNode();
            
            if (debugMode)
            {
                Debug.Log($"노드 이동 성공: {stageManager.CurrentNode.roomName}");
                stageManager.PrintStageMap();
            }
        }
        else
        {
            Debug.LogWarning($"노드 이동 실패: 인덱스 {childIndex}");
        }
        
        return success;
    }

    /// <summary>
    /// 현재 노드 처리
    /// </summary>
    private void ProcessCurrentNode()
    {
        stageManager.ProcessCurrentNode();
        
        // 노드 타입에 따른 추가 처리
        var currentNode = stageManager.CurrentNode;
        if (currentNode != null)
        {
            switch (currentNode.type)
            {
                case StageRoomType.Battle:
                    // 전투 씬으로 전환
                    SceneManager.LoadScene("BattleScene");
                    break;
                    
                case StageRoomType.Boss:
                    // 보스 전투 씬으로 전환
                    SceneManager.LoadScene("BossBattleScene");
                    break;
                    
                case StageRoomType.Event:
                    // 이벤트 처리 (현재 씬에서 처리)
                    HandleEventRoom();
                    break;
            }
        }
    }

    /// <summary>
    /// 이벤트 방 처리
    /// </summary>
    private void HandleEventRoom()
    {
        var currentNode = stageManager.CurrentNode;
        if (currentNode?.eventType != null)
        {
            switch (currentNode.eventType)
            {
                case EventRoomType.Rest:
                    // 휴식 이벤트 UI 표시
                    ShowRestEventUI();
                    break;
                    
                case EventRoomType.Shop:
                    // 상점 UI 표시
                    ShowShopUI();
                    break;
                    
                case EventRoomType.Maintenance:
                    // 정비 UI 표시
                    ShowMaintenanceUI();
                    break;
                    
                case EventRoomType.Event:
                    // 특별 이벤트 UI 표시
                    ShowSpecialEventUI();
                    break;
            }
        }
    }

    /// <summary>
    /// 휴식 이벤트 UI 표시
    /// </summary>
    private void ShowRestEventUI()
    {
        Debug.Log("휴식 이벤트 UI 표시");
        // TODO: 실제 UI 구현
    }

    /// <summary>
    /// 상점 UI 표시
    /// </summary>
    private void ShowShopUI()
    {
        Debug.Log("상점 UI 표시");
        // TODO: 실제 UI 구현
    }

    /// <summary>
    /// 정비 UI 표시
    /// </summary>
    private void ShowMaintenanceUI()
    {
        Debug.Log("정비 UI 표시");
        // TODO: 실제 UI 구현
    }

    /// <summary>
    /// 특별 이벤트 UI 표시
    /// </summary>
    private void ShowSpecialEventUI()
    {
        Debug.Log("특별 이벤트 UI 표시");
        // TODO: 실제 UI 구현
    }

    /// <summary>
    /// 스테이지 완료 시 호출 (BattleSceneController 등에서 호출)
    /// </summary>
    public void OnStageComplete(bool cleared)
    {
        stageManager.CompleteStage(cleared);

        if (!cleared)
        {
            // 게임 오버
            SceneManager.LoadScene("GameOverScene");
            return;
        }

        // 스테이지 맵 씬으로 돌아가기
        SceneManager.LoadScene("StageMapScene");
    }

    /// <summary>
    /// 현재 방문 가능한 노드들 반환
    /// </summary>
    public List<StageNode> GetAvailableNodes()
    {
        return stageManager.GetAvailableChildren();
    }

    /// <summary>
    /// 스테이지 진행 상황 반환
    /// </summary>
    public StageProgress GetStageProgress()
    {
        return stageManager.GetProgress();
    }

    /// <summary>
    /// 현재 노드 정보 반환
    /// </summary>
    public StageNode GetCurrentNode()
    {
        return stageManager.CurrentNode;
    }

    /// <summary>
    /// 스테이지 맵 재생성 (새로운 시드로)
    /// </summary>
    public void RegenerateMap(int? newSeed = null)
    {
        if (stageData != null)
        {
            stageData.randomSeed = newSeed;
            stageManager.StartStage(stageData);
            
            if (debugMode)
            {
                Debug.Log($"새로운 시드({newSeed})로 스테이지 맵 재생성");
                stageManager.PrintStageMap();
            }
        }
    }

    /// <summary>
    /// 새로운 StageData로 스테이지 시작
    /// </summary>
    public void StartStageWithData(StageData newStageData)
    {
        stageData = newStageData;
        stageManager.StartStage(stageData);
        
        if (debugMode)
        {
            Debug.Log($"새로운 스테이지 데이터로 시작: {stageData.stageName}");
            stageManager.PrintStageMap();
        }
    }

    /// <summary>
    /// 디버그 정보 출력
    /// </summary>
    [ContextMenu("디버그 정보 출력")]
    public void PrintDebugInfo()
    {
        Debug.Log("=== StageSceneController 디버그 정보 ===");
        Debug.Log($"현재 스테이지: {stageData?.stageName ?? "없음"}");
        Debug.Log($"현재 노드: {stageManager.CurrentNode?.roomName ?? "없음"}");
        Debug.Log($"스테이지 활성화: {stageManager.IsStageActive}");
        
        var progress = stageManager.GetProgress();
        Debug.Log($"진행률: {progress.ProgressPercentage:F1}% ({progress.visitedNodes}/{progress.totalNodes})");
        
        var availableNodes = stageManager.GetAvailableChildren();
        Debug.Log($"접근 가능한 노드: {availableNodes.Count}개");
        
        foreach (var node in availableNodes)
        {
            Debug.Log($"  - {node.roomName}");
        }
    }
}