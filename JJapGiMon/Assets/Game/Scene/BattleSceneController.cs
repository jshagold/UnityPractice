using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(BattleManager))]
public class BattleSceneController : MonoBehaviour
{
    [Header("Battle Configuration")]
    [Tooltip("Reference to the BattleManager handling the combat flow")]
    [SerializeField] private BattleManager battleManager;

    [Header("Background")]
    [SerializeField] private SpriteRenderer backgroundRenderer;  // 씬에 배치된 SpriteRenderer
    [SerializeField] private Sprite battleBackground;    // 스테이지별 배경 스프라이트

    [Tooltip("Difficulty level for the current stage")]
    [SerializeField] private StageDifficulty stageDifficulty;

    [Header("Character Container (World-Space Canvas)")]
    [SerializeField] private Transform playerUiContainer;
    [SerializeField] private Transform enemyUiContainer;

    [Header("Prefabs")]
    [SerializeField] private CharacterView playerViewPrefab;    // CharacterView 프리팹 :contentReference[oaicite:5]{index=5}
    [SerializeField] private CharacterView enemyViewPrefab;

    [Header("Spawn Points")]
    [Tooltip("플레이어가 배치될 위치들 (1~3개)")]
    [SerializeField] private Transform[] playerSpawnPoints;
    [Tooltip("적이 배치될 위치들 (1~3개)")]
    [SerializeField] private Transform[] enemySpawnPoints;

    [Tooltip("List of player character IDs to include in the battle")]
    [SerializeField] private List<string> playerIdList = new List<string>();

    [Tooltip("List of enemy character IDs to include in the battle")]
    [SerializeField] private List<string> enemyIdList = new List<string>();

    // 캐릭터 모델 ↔ 뷰 매핑
    private readonly Dictionary<CharacterModel, CharacterView> views
        = new Dictionary<CharacterModel, CharacterView>();

    private void Awake()
    {
        Debug.Log("Battle Scene Awake");
        // Ensure BattleManager reference
        if (battleManager == null)
            battleManager = GetComponent<BattleManager>();
    }

    private void OnEnable()
    {
        BattleManager.Instance.OnBattleEnd += HandleBattleEnd;
        BattleManager.Instance.OnQTEPhaseStart += HandleQTEPhaseStart;
    }

    private void OnDisable()
    {
        BattleManager.Instance.OnBattleEnd -= HandleBattleEnd;
        BattleManager.Instance.OnQTEPhaseStart -= HandleQTEPhaseStart;
    }

    private void Start()
    {
        Debug.Log("Battle Scene Start");

        // 1) 배경 설정
        if (backgroundRenderer != null && battleBackground != null)
        {
            backgroundRenderer.sprite = battleBackground;
        }
        else
        {
            if (backgroundRenderer == null)
                Debug.LogError("backgroundRenderer가 할당되지 않았습니다!");

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

        //var partyModels = StageController.Instance.GetParty();
        //var enemyModels = StageController.Instance.GetEnemies();

        // 캐릭터 정보 세팅
        ICharacterRepository _characterRepo = new LocalCharacterRepository();
        CharacterFactory characterFactory = new(_characterRepo);
        var partyModels = playerIdList.Select(id => characterFactory.Create(id)).ToList();
        var enemyModels = enemyIdList.Select(id => characterFactory.Create(id)).ToList();

        // todo 캐릭터 설정 초기화
        partyModels.ForEach(player => { player.SaveData.CurrentHealth = player.MaxHp; });
        enemyModels.ForEach(enemy => { enemy.SaveData.CurrentHealth = enemy.MaxHp; });

        // Initialize battle with configured IDs and difficulty
        battleManager.SetupBattle(partyModels, enemyModels, stageDifficulty);

        // 3) 플레이어 뷰 인스턴스화
        InitCharacters(partyModels, playerSpawnPoints, playerViewPrefab, playerUiContainer);

        // 4) 적 뷰 인스턴스화
        InitCharacters(enemyModels, enemySpawnPoints, enemyViewPrefab, enemyUiContainer);



        // Begin the turn-based combat loop
        StartCoroutine(battleManager.TurnLoop());
    }

    /*
     * 캐릭터 세팅
     */
    private void InitCharacters(
        List<CharacterModel> models,
        Transform[] spawnPoints,
        CharacterView prefab,
        Transform container)
    {
        for (int i = 0; i < models.Count && i < spawnPoints.Length; i++)
        {
            var model = models[i];
            Vector3 worldPos = spawnPoints[i].position;
            worldPos.z = 0f;

            // World-Space Canvas 하위에 생성
            var view = Instantiate(prefab, container);
            view.transform.position = worldPos;
            view.transform.rotation = Quaternion.identity;

            view.Initialize(model);
            views[model] = view;
        }
    }


    private void HandleQTEPhaseStart(CharacterModel caster)
    {

    }

    /// <summary>
    /// 전투 중에 특정 캐릭터를 새로운 월드좌표로 이동시킵니다.
    /// </summary>
    public void MoveCharacter(CharacterModel model, Vector3 newPos, float duration = 0.5f)
    {
        if (views.TryGetValue(model, out var view))
        {
            StartCoroutine(view.MoveToPosition(newPos, duration));
        }
    }

    /*
     * BattleManager에서 전투가 끝났을 때
     */
    public void HandleBattleEnd()
    {

    }
}