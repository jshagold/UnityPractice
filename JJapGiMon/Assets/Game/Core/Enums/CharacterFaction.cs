public sealed record CharacterFaction()
{
	// 플레이어
	public static readonly CharacterFaction Player = new();

	// 아군
	public static readonly CharacterFaction Ally = new();

	// 적군
	public static readonly CharacterFaction Enemy = new();

	// 중립
	public static readonly CharacterFaction Neutral = new();

}