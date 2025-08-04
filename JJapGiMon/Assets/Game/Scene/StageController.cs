using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public static StageController Instance { get; private set; }

    [Header("시퀀스 데이터")]
    [SerializeField] private StageSequenceData sequenceData;

    public int CurrentStageIndex { get; private set; } = 0;

    private List<CharacterModel> party;    // 선택된 아군
    private List<CharacterModel> enemies;  // 현재 스테이지 적

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 게임 시작 직후, 캐릭터 선택 & 진입 전 조건 모두 끝난 뒤 호출
    /// </summary>
    public void InitGame(List<CharacterModel> selectedParty)
    {
        party = selectedParty;
        CurrentStageIndex = 0;
        LoadStage(CurrentStageIndex);
    }

    private void LoadStage(int index)
    {
        if (index >= sequenceData.stages.Count)
        {
            Debug.Log("모든 스테이지 완료!");
            // TODO: 엔딩 씬 로드 등
            return;
        }

        var def = sequenceData.stages[index];

        // 적 생성 로직. 필요에 따라 EnemyGenerator 등에서 뽑아 옵니다.
        if (def.isBattle || def.isBoss)
        {
            enemies = EnemyGenerator.GenerateForStage(index + 1);
        }
        else
        {
            enemies = new List<CharacterModel>(); // 이벤트맵은 직접 이벤트 컴포넌트가 처리
        }

        // 해당 씬으로 이동
        SceneManager.LoadScene(def.sceneName);
    }

    /// <summary>
    /// BattleSceneController 등에서 파티·적 정보 조회할 때 사용
    /// </summary>
    public List<CharacterModel> GetParty() => party;
    public List<CharacterModel> GetEnemies() => enemies;

    /// <summary>
    /// 각 맵(스테이지) 완료 시 호출
    /// </summary>
    public void OnStageComplete(bool cleared)
    {
        if (!cleared)
        {
            SceneManager.LoadScene("GameOverScene");
            return;
        }

        CurrentStageIndex++;
        LoadStage(CurrentStageIndex);
    }
}