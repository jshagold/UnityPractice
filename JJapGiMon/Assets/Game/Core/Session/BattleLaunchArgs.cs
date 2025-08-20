/// <summary>
/// 배틀 씬 런치 인자
/// </summary>
public class BattleLaunchArgs : ISceneLaunchArgs
{
    public string[] PlayerCharacterIds { get; init; }
    public string[] EnemyCharacterIds { get; init; }
    public int BattleId { get; init; }
    public string BattleType { get; init; } // "normal", "boss", "elite" 등
    public int? Difficulty { get; init; }
    public bool IsTutorial { get; init; }
}
