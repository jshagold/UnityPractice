using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stage Sequence")]
public class StageSequenceData : ScriptableObject
{
    // ������� �ε��� �������� ���
    public List<StageDefinition> stages = new List<StageDefinition>();
}

[System.Serializable]
public class StageDefinition
{
    public string sceneName;               // ex) "BattleScene", "EventScene", "BossScene"
    public bool isBattle;                // ���� �� ����
    public bool isBoss;                  // ���� �� ����
    public float battleProbability = 1f;  // �̺�Ʈ ���� �� ���� Ȯ��(1f = �׻� ����)
    // �ʿ��ϴٸ� �� ������ �����ͳ�, �̺�Ʈ �Ķ���͸� �߰��� ������ �� �ֽ��ϴ�.
}