public sealed record CharacterFaction()
{
	// �÷��̾�
	public static readonly CharacterFaction Player = new();

	// �Ʊ�
	public static readonly CharacterFaction Ally = new();

	// ����
	public static readonly CharacterFaction Enemy = new();

	// �߸�
	public static readonly CharacterFaction Neutral = new();

}