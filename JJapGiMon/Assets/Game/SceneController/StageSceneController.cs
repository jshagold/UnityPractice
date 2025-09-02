using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSceneController : MonoBehaviour
{
    [Header("Stage Configuration")]
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private StageGraph stageGraph;
    [SerializeField] private StageState stageState;

    [Header("Manager References")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageInputManager stageInputManager;

    [Header("UI References")]
    [SerializeField] private StageMapUI stageMapUI;
    [SerializeField] private StageHUDController stageHUDController;
    [SerializeField] private StageBackgroundTable stageBackgroundTable;
    [SerializeField] private Image backgroundImage;

    [Header("Party Configuration")]
    [Tooltip("List of player character IDs to include in the stage")]
    [SerializeField] private List<string> playerIdList = new List<string>();

    // 이전 씬에서 받은 정보 세션
    private StageLaunchArgs sessionArgs;

    // 이벤트 핸들러들
    private System.Action<bool> onStageComplete;

    private void Awake()
    {
        Debug.Log("Stage Scene Awake");

        // 1) 세선에서 DTO 가져오기. (null 일때는 기본값.)
        if (GameSession.I.TryConsume<StageLaunchArgs>(out var args))
        {
            this.sessionArgs = args;
        } else {
            this.sessionArgs = new StageLaunchArgs { StageId = -1 };
        }

        // 2) 참조 확보 Ensure Manager references
        if (stageManager == null)
            stageManager = GetComponentInChildren<StageManager>(true);

        if (stageInputManager == null)
            stageInputManager = GetComponent<StageInputManager>();

        if(stageMapUI == null)
            stageMapUI = GetComponent<StageMapUI>();

        if (stageHUDController == null)
            stageHUDController = GetComponent<StageHUDController>();

        // Background Image 세팅
        backgroundImage.preserveAspect = true;
        backgroundImage.raycastTarget = false;

        // TODO StageManager, stageInputManager는 Start에서 초기(Initialize)화 하자! 여기서 작업할거라면 Bind만
    }

    private void OnEnable()
    {
        // StageManager 이벤트 구독
        if (stageManager != null)
        {
            stageManager.OnEventRoomEntered += HandleEventRoomEntered;
            stageManager.OnBattleRoomEntered += HandleBattleRoomEntered;
            stageManager.OnBossRoomEntered += HandleBossRoomEntered;
        }

        // StageHUDController 이벤트 구독
        if (stageHUDController != null)
        {
            stageHUDController.OpenOptionRequested += HandleOpenOptionRequested;
            stageHUDController.OnSaveAndExitStage += HandleSaveAndExitStage;
            stageHUDController.OnGiveUpStage += HandleGiveUpStage;
            stageHUDController.OpenSettingRequested += HandleOpenSettingRequested;
        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        if (stageManager != null)
        {
            stageManager.OnEventRoomEntered -= HandleEventRoomEntered;
            stageManager.OnBattleRoomEntered -= HandleBattleRoomEntered;
            stageManager.OnBossRoomEntered -= HandleBossRoomEntered;
        }

        // StageHUDController 이벤트 구독 해제      
        if (stageHUDController != null)
        {
            stageHUDController.OpenOptionRequested -= HandleOpenOptionRequested;
            stageHUDController.OnSaveAndExitStage -= HandleSaveAndExitStage;
            stageHUDController.OnGiveUpStage -= HandleGiveUpStage;
            stageHUDController.OpenSettingRequested -= HandleOpenSettingRequested;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제 (안전장치)
        if (stageManager != null)
        {
            stageManager.OnEventRoomEntered -= HandleEventRoomEntered;
            stageManager.OnBattleRoomEntered -= HandleBattleRoomEntered;
            stageManager.OnBossRoomEntered -= HandleBossRoomEntered;
        }
        
        // StageHUDController 이벤트 구독 해제      
        if (stageHUDController != null)
        {
            stageHUDController.OpenOptionRequested -= HandleOpenOptionRequested;
            stageHUDController.OnSaveAndExitStage -= HandleSaveAndExitStage;
            stageHUDController.OnGiveUpStage -= HandleGiveUpStage;
            stageHUDController.OpenSettingRequested -= HandleOpenSettingRequested;
        }
    }

    private void Start()
    {
        Debug.Log("Stage Scene Start");

        Initialize();

        // 고정 UI (HUD) 세팅
        stageHUDController.SetStageTitle(sessionArgs.StageName);

        // 캐릭터 정보 세팅 todo

        // Background Image 세팅
        backgroundImage.sprite = stageBackgroundTable.Get(sessionArgs.StageId);

    }


    private void Initialize()
    {
        (stageConfig, stageGraph, stageState) = DataLoadOrGenerate();

        // StageManager 초기화 및 시작
        stageManager.Initialize(stageConfig, stageGraph, stageState);
        Debug.Log($"StageManager Initialize: {stageGraph.rootNode}");
        Debug.Log($"StageManager Initialize: {stageGraph.rootNodeData.nodeId}");
        stageMapUI.RenderMap(stageGraph.rootNode);

    }

    private (StageConfig, StageGraph, StageState) DataLoadOrGenerate() {
        sessionArgs = sessionArgs ?? new StageLaunchArgs { StageId = -1 };

        // 1) Repository 준비
        IStageRepository stageRepository = new LocalStageRepository();

        // 2) Stage 데이터 로드 (StageConfig, StageGraph, StageState 불러오기)
        // currentStageData = LoadStage(sessionArgs.ContentId) ?? new StageData();

        // 로드할 데이터가 없을 때, 생성
        // 3) StageConfig 준비
        var config = new StageConfig { 
            stageId = sessionArgs.StageId,
            randomSeed = sessionArgs.Seed
        };

        // 4) 스테이지 생성기 준비
        StageMapGenerator stageMapGenerator = new StageMapGenerator(config);

        // 5) 스테이지 데이터 생성
        var graph = stageMapGenerator.GenerateCompleteStageData();

        // 6) StageState 준비
        var state = new StageState
        {
            currentNodeId = graph.rootNodeData.nodeId,
            visitedNodeIds = new List<int>(),
            isCompleted = false,
            isFailed = false
        };

        return (config, graph, state);
    }


    // 이벤트 처리

    // StageManager 이벤트 처리
    /// 전투 씬 로드
    private void HandleBattleRoomEntered(StageNode node)
    {
        Debug.Log("전투 씬으로 전환합니다.");
        switch (node.battleType)
        {
            case BattleRoomType.Normal:
                break;
        }

        // TODO: 전투 씬에 필요한 데이터 전달
        SceneManager.LoadScene("BattleScene");
    }

    /// 보스 전투 씬 로드
    private void HandleBossRoomEntered(StageNode node)
    {
        Debug.Log("보스 전투 씬으로 전환합니다.");
        
        // TODO: 보스 전투 씬에 필요한 데이터 전달
        SceneManager.LoadScene("BossBattleScene");
    }

    /// 이벤트 씬 처리
    private void HandleEventRoomEntered(StageNode node)
    {   
        Debug.Log($"이벤트 씬으로 전환합니다: {node.roomName}");
        switch (node.eventType)
        {
            case EventRoomType.Event:
                break;
            case EventRoomType.Maintenance:
                break;
            case EventRoomType.Rest:
                break;
            case EventRoomType.Story:
                break;
        }

        // TODO: 이벤트 씬에 필요한 데이터 전달
        SceneManager.LoadScene("EventScene");
    }

    // StageHUDController 이벤트 처리
    private void HandleOpenOptionRequested()
    {
        Debug.Log("옵션 창이 열렸습니다.");
    }
    
    private void HandleSaveAndExitStage()
    {
        Debug.Log("저장 후 메인화면으로 이동합니다.");
    }
    private void HandleGiveUpStage()
    {
        Debug.Log("스테이지를 포기합니다.");
    }
    private void HandleOpenSettingRequested()
    {
        Debug.Log("게임 설정 창이 열렸습니다.");
    }

    /// <summary>
    /// 스테이지 포기
    /// </summary>
    public void AbandonStage()
    {
        Debug.Log("스테이지를 포기했습니다.");
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("MainScene");
    }

    // TODO 스테이지 종료 결과 처리 후 씬 이동 


    /// <summary>
    /// 스테이지 완료 시 호출 (BattleSceneController 등에서 호출)
    /// </summary>
    public void OnStageComplete(bool cleared)
    {

        if (!cleared)
        {
            // 게임 오버
            SceneManager.LoadScene("GameOverScene");
            return;
        }

        // 스테이지 맵 씬으로 돌아가기
        SceneManager.LoadScene("StageMapScene");
    }

}