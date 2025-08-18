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
    [SerializeField] private int MaxNodeCountByDepth = 3;
    [SerializeField] private int? randomSeed = null;

    [Header("Manager References")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageInputManager stageInputManager;

    [Header("UI References")]
    [SerializeField] private StageMapUI stageMapUI;

    [Header("Background")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Sprite stageBackground;

    [Header("Party Configuration")]
    [Tooltip("List of player character IDs to include in the stage")]
    [SerializeField] private List<string> playerIdList = new List<string>();

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    // 이벤트 핸들러들
    private System.Action<StageNode> onNodeProcessed;
    private System.Action<bool> onStageComplete;

    private void Awake()
    {
        Debug.Log("Stage Scene Awake");
        // Ensure Manager references
        if (stageManager == null)
            stageManager = GetComponent<StageManager>();
        if (stageInputManager == null)
            stageInputManager = GetComponent<StageInputManager>();
    }

    private void OnEnable()
    {
        // StageInputManager 이벤트 구독
        if (stageInputManager != null)
        {
            stageInputManager.OnNodeClicked += HandleNodeClicked;
        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        if (stageInputManager != null)
        {
            stageInputManager.OnNodeClicked -= HandleNodeClicked;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제 (안전장치)
        if (stageInputManager != null)
        {
            stageInputManager.OnNodeClicked -= HandleNodeClicked;
        }
    }

    private void Start()
    {
        Debug.Log("Stage Scene Start");
        
        // 1) 배경 설정
        SetupBackground();

        // 2) 저장된 StageData 불러오기 / 없다면 기본값으로 생성
        InitializeStageData();

        // 3) 캐릭터 정보 세팅
        var partyModels = CreatePartyModels();

        // 4) 새로운 스테이지 시스템으로 초기화
        InitializeNewStageSystem();

        // 5) UI 초기화
        InitializeUI();
    }

    /// <summary>
    /// 배경 설정
    /// </summary>
    private void SetupBackground()
    {
        if (backgroundRenderer != null && stageBackground != null)
        {
            backgroundRenderer.sprite = stageBackground;
        }
        else
        {
            if (backgroundRenderer == null)
                Debug.LogError("backgroundRenderer가 할당되지 않았습니다!");

            // 기본 배경 로드
            Sprite tempBg = Resources.Load<Sprite>("Images/temp_battle_bg");
            if (tempBg != null)
            {
                backgroundRenderer.sprite = tempBg;
            }
            else
            {
                Debug.LogError("temp_bg.png를 Resources/Images에서 찾을 수 없습니다.");
            }
        }
    }

    /// <summary>
    /// StageData 초기화
    /// </summary>
    private void InitializeStageData()
    {
        if (stageData == null)
        {
            stageData = new StageData(
                stageId: 1,
                stageName: "기본 스테이지",
                stageDescription: "기본 설정으로 생성된 스테이지",
                stageLength: stageLength,
                MaxNodeCountByDepth: MaxNodeCountByDepth,
                randomSeed: randomSeed,
                lastRoomCount: 3
            );
        }
    }

    /// <summary>
    /// 새로운 스테이지 시스템 초기화
    /// </summary>
    private void InitializeNewStageSystem()
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
    /// UI 초기화
    /// </summary>
    private void InitializeUI()
    {
        if (stageMapUI == null)
        {
            Debug.LogError("StageMapUI가 할당되지 않았습니다!");
            return;
        }

        if (stageInputManager == null)
        {
            Debug.LogError("StageInputManager가 할당되지 않았습니다!");
            return;
        }

        // StageInputManager 초기화
        stageInputManager.InitializeStageMap(stageManager.CurrentStageMap, stageManager.CurrentNode);
    }

    /// <summary>
    /// 노드 클릭 처리
    /// </summary>
    private void HandleNodeClicked(StageNode clickedNode)
    {
        if (!stageManager.IsStageActive)
        {
            Debug.LogWarning("스테이지가 활성화되지 않았습니다.");
            return;
        }

        // 클릭된 노드가 현재 노드의 자식인지 확인
        var availableChildren = stageManager.GetAvailableChildren();
        int childIndex = availableChildren.IndexOf(clickedNode);
        
        if (childIndex >= 0)
        {
            // 노드 이동
            MoveToNode(childIndex);
        }
        else
        {
            Debug.LogWarning($"클릭된 노드가 접근 가능하지 않습니다: {clickedNode.roomName}");
        }
    }

    /// <summary>
    /// 현재 노드에서 특정 자식 노드로 이동
    /// </summary>
    public bool MoveToNode(int childIndex)
    {
        bool success = stageManager.MoveToNode(childIndex);
        
        if (success)
        {
            // UI 업데이트
            UpdateUI();
            
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
    /// UI 업데이트
    /// </summary>
    private void UpdateUI()
    {
        if (stageInputManager != null)
        {
            stageInputManager.UpdateCurrentNode(stageManager.CurrentNode);
        }
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
                    LoadBattleScene();
                    break;
                    
                case StageRoomType.Boss:
                    // 보스 전투 씬으로 전환
                    LoadBossBattleScene();
                    break;
                    
                case StageRoomType.Event:
                    // 이벤트 처리 (현재 씬에서 처리)
                    HandleEventRoom();
                    break;
            }
        }
    }

    /// <summary>
    /// 전투 씬 로드
    /// </summary>
    private void LoadBattleScene()
    {
        Debug.Log("전투 씬으로 전환합니다.");
        // TODO: 전투 씬에 필요한 데이터 전달
        SceneManager.LoadScene("BattleScene");
    }

    /// <summary>
    /// 보스 전투 씬 로드
    /// </summary>
    private void LoadBossBattleScene()
    {
        Debug.Log("보스 전투 씬으로 전환합니다.");
        // TODO: 보스 전투 씬에 필요한 데이터 전달
        SceneManager.LoadScene("BossBattleScene");
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
                    
                case EventRoomType.Story:
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
        // UI를 표시하고 플레이어가 선택할 수 있도록 함
    }

    /// <summary>
    /// 상점 UI 표시
    /// </summary>
    private void ShowShopUI()
    {
        Debug.Log("상점 UI 표시");
        // TODO: 실제 UI 구현
        // 상점 UI를 표시하고 아이템 구매 기능 제공
    }

    /// <summary>
    /// 정비 UI 표시
    /// </summary>
    private void ShowMaintenanceUI()
    {
        Debug.Log("정비 UI 표시");
        // TODO: 실제 UI 구현
        // 정비 UI를 표시하고 장비 강화 기능 제공
    }

    /// <summary>
    /// 특별 이벤트 UI 표시
    /// </summary>
    private void ShowSpecialEventUI()
    {
        Debug.Log("특별 이벤트 UI 표시");
        // TODO: 실제 UI 구현
        // 특별 이벤트 UI를 표시하고 선택지 제공
    }

    /// <summary>
    /// 스테이지 일시정지
    /// </summary>
    public void PauseStage()
    {
        SetInputEnabled(false);
        Debug.Log("스테이지가 일시정지되었습니다.");
    }

    /// <summary>
    /// 스테이지 재개
    /// </summary>
    public void ResumeStage()
    {
        SetInputEnabled(true);
        Debug.Log("스테이지가 재개되었습니다.");
    }

    /// <summary>
    /// 스테이지 재시작
    /// </summary>
    public void RestartStage()
    {
        Debug.Log("스테이지를 재시작합니다.");
        // 현재 스테이지 데이터로 다시 시작
        if (stageData != null)
        {
            stageManager.StartStage(stageData);
            if (stageInputManager != null)
            {
                stageInputManager.RefreshMap();
            }
        }
    }

    /// <summary>
    /// 스테이지 포기
    /// </summary>
    public void AbandonStage()
    {
        stageManager.AbandonStage();
        Debug.Log("스테이지를 포기했습니다.");
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("MainScene");
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
            
            // UI 새로고침
            if (stageInputManager != null)
            {
                stageInputManager.RefreshMap();
            }
            
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
        
        // UI 새로고침
        if (stageInputManager != null)
        {
            stageInputManager.RefreshMap();
        }
        
        if (debugMode)
        {
            Debug.Log($"새로운 스테이지 데이터로 시작: {stageData.stageName}");
            stageManager.PrintStageMap();
        }
    }

    /// <summary>
    /// 스테이지 저장
    /// </summary>
    public void SaveStage()
    {
        stageManager.SaveStage();
    }

    /// <summary>
    /// 스테이지 로드
    /// </summary>
    public bool LoadStage()
    {
        bool success = stageManager.LoadStage();
        if (success && stageInputManager != null)
        {
            stageInputManager.RefreshMap();
        }
        return success;
    }

    /// <summary>
    /// UI 활성화/비활성화
    /// </summary>
    public void SetUIEnabled(bool enabled)
    {
        if (stageInputManager != null)
        {
            stageInputManager.SetUIEnabled(enabled);
        }
    }

    /// <summary>
    /// 입력 처리 활성화/비활성화
    /// </summary>
    public void SetInputEnabled(bool enabled)
    {
        if (stageInputManager != null)
        {
            stageInputManager.SetInputEnabled(enabled);
        }
    }

    /// <summary>
    /// 접근 가능한 노드들 강조 표시
    /// </summary>
    public void HighlightAvailableNodes()
    {
        if (stageInputManager != null)
        {
            stageInputManager.HighlightAvailableNodes();
        }
    }

    /// <summary>
    /// 모든 노드 강조 해제
    /// </summary>
    public void ClearNodeHighlights()
    {
        if (stageInputManager != null)
        {
            stageInputManager.ClearHighlights();
        }
    }

    /// <summary>
    /// 특정 노드 강조 표시
    /// </summary>
    public void HighlightNode(StageNode node, bool highlight = true)
    {
        if (stageInputManager != null)
        {
            stageInputManager.HighlightNode(node, highlight);
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

        // UI 상태 확인
        if (stageMapUI != null)
        {
            Debug.Log($"StageMapUI 활성화: {stageMapUI.gameObject.activeInHierarchy}");
        }
        if (stageInputManager != null)
        {
            Debug.Log($"StageInputManager 활성화: {stageInputManager.gameObject.activeInHierarchy}");
        }
    }
}