public class StageLaunchArgs
{
    public int StageId { get; init; }
    public string ContentId { get; init; }
    public int? Seed { get; init; }
    public string[] PartyCharacterIds { get; init; }
    public int? Difficulty { get; init; }
}