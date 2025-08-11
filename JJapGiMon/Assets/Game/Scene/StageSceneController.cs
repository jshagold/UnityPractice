using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StageManager))]
[RequireComponent(typeof(StageInputManager))]
public class StageSceneController : MonoBehaviour
{
    [Header("Stage Configuration")]
    [SerializeField] private int stageLength = 5;
    [SerializeField] private int choicesPerStep = 3;
    [SerializeField] private int? randomSeed = null;

    [Header("Manager References")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageInputManager stageInputManager;

    [Header("Party Configuration")]
    [Tooltip("List of player character IDs to include in the stage")]
    [SerializeField] private List<string> playerIdList = new List<string>();

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
        // StageManager 이벤트 구독
        if (stageManager != null)
        {
            stageManager.OnRoomEntered += HandleRoomEntered;
        }
    }

    private void OnDisable()
    {
        // StageManager 이벤트 구독 해제
        if (stageManager != null)
        {
            stageManager.OnRoomEntered -= HandleRoomEntered;
        }
    }

    private void Start()
    {
        Debug.Log("Stage Scene Start");

        // 캐릭터 정보 세팅
        var partyModels = CreatePartyModels();

        // 스테이지 초기화
        stageManager.SetupStage(stageLength, choicesPerStep, randomSeed, partyModels);

        // UI 초기화
        stageInputManager.InitializeStageMap(
            stageManager.GetStageMapModel(), 
            stageManager.GetCurrentNode()
        );
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
    /// 방 진입 처리
    /// </summary>
    private void HandleRoomEntered(StageRoomType roomType)
    {
        Debug.Log($"StageSceneController: Room entered - {roomType}");
        
        // 방 타입에 따른 씬 전환은 StageInputManager에서 처리됨
        // 여기서는 추가적인 씬별 설정이나 로직을 처리할 수 있음
    }

    /// <summary>
    /// 스테이지 완료 시 호출 (BattleSceneController 등에서 호출)
    /// </summary>
    public void OnStageComplete(bool cleared)
    {
        if (stageManager != null)
        {
            stageManager.OnStageComplete(cleared);
        }

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
    /// 파티 정보 조회
    /// </summary>
    public List<CharacterModel> GetParty() => stageManager?.GetParty() ?? new List<CharacterModel>();

    /// <summary>
    /// 현재 노드가 마지막 방인지 확인
    /// </summary>
    public bool IsLastRoom() => stageManager?.IsLastRoom() ?? false;

    /// <summary>
    /// 현재 노드가 목표 방인지 확인
    /// </summary>
    public bool IsGoalRoom() => stageManager?.IsGoalRoom() ?? false;

    /// <summary>
    /// 스테이지 맵 재생성 (새로운 시드로)
    /// </summary>
    public void RegenerateMap(int? newSeed = null)
    {
        if (stageManager != null)
        {
            stageManager.RegenerateMap(stageLength, choicesPerStep, newSeed);
            stageInputManager.InitializeStageMap(
                stageManager.GetStageMapModel(), 
                stageManager.GetCurrentNode()
            );
        }
    }
}