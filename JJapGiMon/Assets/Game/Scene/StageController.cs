using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public static StageController Instance { get; private set; }

    [Header("������ ������")]
    [SerializeField] private StageSequenceData sequenceData;

    public int CurrentStageIndex { get; private set; } = 0;

    private List<CharacterModel> party;    // ���õ� �Ʊ�
    private List<CharacterModel> enemies;  // ���� �������� ��

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
    /// ���� ���� ����, ĳ���� ���� & ���� �� ���� ��� ���� �� ȣ��
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
            Debug.Log("��� �������� �Ϸ�!");
            // TODO: ���� �� �ε� ��
            return;
        }

        var def = sequenceData.stages[index];

        // �� ���� ����. �ʿ信 ���� EnemyGenerator ��� �̾� �ɴϴ�.
        if (def.isBattle || def.isBoss)
        {
            enemies = EnemyGenerator.GenerateForStage(index + 1);
        }
        else
        {
            enemies = new List<CharacterModel>(); // �̺�Ʈ���� ���� �̺�Ʈ ������Ʈ�� ó��
        }

        // �ش� ������ �̵�
        SceneManager.LoadScene(def.sceneName);
    }

    /// <summary>
    /// BattleSceneController ��� ��Ƽ���� ���� ��ȸ�� �� ���
    /// </summary>
    public List<CharacterModel> GetParty() => party;
    public List<CharacterModel> GetEnemies() => enemies;

    /// <summary>
    /// �� ��(��������) �Ϸ� �� ȣ��
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