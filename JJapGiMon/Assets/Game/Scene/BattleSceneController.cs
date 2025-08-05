using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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

    private void Start()
    {
        Debug.Log("Battle Scene Start");
     
        // Ensure BattleManager reference
        if (battleManager == null)
            battleManager = GetComponent<BattleManager>();

        // 1) 배경 설정
        if (backgroundRenderer != null && battleBackground != null)
        {
            backgroundRenderer.sprite = battleBackground;
        }
        else
        {
            if (backgroundRenderer == null)
                Debug.LogError("backgroundRenderer가 할당되지 않았습니다!");
            if (battleBackground == null)
                Debug.LogError("battleBackground가 할당되지 않았습니다!");

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
        var bounds = backgroundRenderer.sprite.bounds.size;
        Debug.Log($"Sprite world size (before scale): {bounds}");
        Debug.Log($"Transform localScale: {backgroundRenderer.transform.localScale}");

        //var partyModels = StageController.Instance.GetParty();
        //var enemyModels = StageController.Instance.GetEnemies();

        // 캐릭터 정보 세팅
        ICharacterRepository _characterRepo = new LocalCharacterRepository();
        CharacterFactory characterFactory = new(_characterRepo);
        var partyModels = playerIdList.Select(id => characterFactory.Create(id)).ToList();
        var enemyModels = enemyIdList.Select(id => characterFactory.Create(id)).ToList();



        // Initialize battle with configured IDs and difficulty
        battleManager.SetupBattle(partyModels, enemyModels, stageDifficulty);
        //battleManager.SetupBattle(partyModels, enemyModels, stageDifficulty);



        // 3) 플레이어 뷰 인스턴스화
        for (int i = 0; i < partyModels.Count && i < playerSpawnPoints.Length; i++)
        {
            var model = partyModels[i];
            var spawn = playerSpawnPoints[i].position;
            spawn.z = 0f;
            var view = Instantiate(playerViewPrefab, spawn, Quaternion.identity);
            view.Initialize(model);
            views[model] = view;
            Debug.Log($"[UI] Instantiated PlayerUI for {model.DisplayName} ");
        }

        // 4) 적 뷰 인스턴스화
        for (int i = 0; i < enemyModels.Count && i < enemySpawnPoints.Length; i++)
        {
            var model = enemyModels[i];
            var spawn = enemySpawnPoints[i].position;
            spawn.z = 0f;
            var view = Instantiate(enemyViewPrefab, spawn, Quaternion.identity);
            view.Initialize(model);
            views[model] = view;
        }

        // 전투 맵 그리기


        // 맵에 캐릭터 시작상태 설정


        // Begin the turn-based combat loop
        StartCoroutine(battleManager.TurnLoop());
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
}