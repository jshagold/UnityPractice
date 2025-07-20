[System.Serializable]
public class EffectDto
{
    public string type;     // "statMul", "damage" …
    public string stat;     // "MoveSpeed", "AttackPower" …
    public double value;    // 곱/가감 등

    public DamageDTO damage; // null 가능
}