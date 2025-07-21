public class DamageEffect : ISkillEffect
{
    private float skillPhaseDamage = 0f;


    public DamageEffect(float statPhase, Damage dmgValue)
    {
       skillPhaseDamage = statPhase * dmgValue.scalingDamage + dmgValue.fixedDamage;

    }


    public float GetDamage()
    {
        return skillPhaseDamage;
    }

}