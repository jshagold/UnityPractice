[System.Serializable]
public class EffectDto
{
    public string type;     // "statMul", "damage" ��
    public string stat;     // "MoveSpeed", "AttackPower" ��
    public double value;    // ��/���� ��

    public DamageDTO damage; // null ����
}