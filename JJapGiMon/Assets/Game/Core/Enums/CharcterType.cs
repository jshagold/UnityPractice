public sealed record CharacterType(int Id, string Name)
{
    // �ΰ�
    public static readonly CharacterType Human = new(1, "�ΰ�");
    
    // ����
    public static readonly CharacterType Animal = new(2, "����");
    
    // ���
    public static readonly CharacterType Machine = new(3, "���");
}