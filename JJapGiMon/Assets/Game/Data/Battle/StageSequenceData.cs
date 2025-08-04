using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stage Sequence")]
public class StageSequenceData : ScriptableObject
{
    // 순서대로 로드할 스테이지 목록
    public List<StageDefinition> stages = new List<StageDefinition>();
}

[System.Serializable]
public class StageDefinition
{
    public string sceneName;               // ex) "BattleScene", "EventScene", "BossScene"
    public bool isBattle;                // 전투 맵 여부
    public bool isBoss;                  // 보스 맵 여부
    public float battleProbability = 1f;  // 이벤트 맵일 때 전투 확률(1f = 항상 전투)
    // 필요하다면 적 생성용 데이터나, 이벤트 파라미터를 추가로 정의할 수 있습니다.
}