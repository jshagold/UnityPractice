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

    [Header("Party Configuration")]
    [Tooltip("List of player character IDs to include in the stage")]
    [SerializeField] private List<string> playerIdList = new List<string>();

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    // 이전 씬에서 받은 정보 세션
    private StageLaunchArgs args;

    // 이벤트 핸들러들
    private System.Action<StageNode> onNodeProcessed;
    private System.Action<bool> onStageComplete;

    private void Awake()
    {
        Debug.Log("Stage Scene Awake");

        // 1) 세선에서 DTO 가져오기. (null 일때는 기본값.)
        args = GameSession.I.ConsumeStageLaunchArgs()
            ?? new StageLaunchArgs { StageId = -1 };

        // 2) 참조 확보 Ensure Manager references
        if (stageManager == null)
            stageManager = GetComponent<StageManager>();

        if(stageMapUI == null)
            stageMapUI = GetComponent<StageMapUI>();

        if (stageInputManager == null)
            stageInputManager = GetComponent<StageInputManager>();

        // TODO StageManager, stageInputManager는 Start에서 초기(Initialize)화 하자! 여기서 작업할거라면 Bind만
    }

    private void OnEnable()
    {
        // StageInputManager 이벤트 구독
        if (stageInputManager != null)
        {

        }
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        if (stageInputManager != null)
        {

        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제 (안전장치)
        if (stageInputManager != null)
        {

        }
    }

    private void Start()
    {
        Debug.Log("Stage Scene Start");

        // 캐릭터 정보 세팅 todo

        // StageManager 초기화 및 시작
        stageManager.Initialize(args);
        stageManager.StartStage();
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
    /// 이벤트 씬 처리
    /// </summary>
    private void LoadEventScene()
    {
        Debug.Log("이벤트 씬으로 전환합니다.");
        // TODO: 이벤트 씬에 필요한 데이터 전달
        SceneManager.LoadScene("EventScene");
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
    /// 스테이지 포기
    /// </summary>
    public void AbandonStage()
    {
        stageManager.AbandonStage();
        Debug.Log("스테이지를 포기했습니다.");
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("MainScene");
    }

    // TODO 스테이지 종료 결과 처리 후 씬 이동 

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