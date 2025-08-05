using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BattleManager))]
public class BattleSceneController : MonoBehaviour
{
    [Header("Battle Configuration")]
    [Tooltip("Reference to the BattleManager handling the combat flow")]
    [SerializeField] private BattleManager battleManager;

    [Tooltip("Difficulty level for the current stage")]
    [SerializeField] private StageDifficulty stageDifficulty;

    [Tooltip("List of player character IDs to include in the battle")]
    [SerializeField] private List<string> playerIdList = new List<string>();

    [Tooltip("List of enemy character IDs to include in the battle")]
    [SerializeField] private List<string> enemyIdList = new List<string>();

    private void Start()
    {
        Debug.Log("Battle Scene Start");
     
        // Ensure BattleManager reference
        if (battleManager == null)
            battleManager = GetComponent<BattleManager>();

        //var partyModels = StageController.Instance.GetParty();
        //var enemyModels = StageController.Instance.GetEnemies();

        // Initialize battle with configured IDs and difficulty
        battleManager.SetupBattle(playerIdList, enemyIdList, stageDifficulty);
        //battleManager.SetupBattle(partyModels, enemyModels, stageDifficulty);


        // 전투 맵 그리기


        // 맵에 캐릭터 시작상태 설정


        // Begin the turn-based combat loop
        StartCoroutine(battleManager.TurnLoop());
    }
}