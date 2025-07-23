using System;

public class DamageCalculator
{
    // 힘 스탯
    private int Str;
    // 강인함 스탯
    private int Def;

    // 공격 밸런스 계수
    private float StrValue;
    // 수비 밸런스 계수
    private float DefValue;

    // 스킬 데미지
    private Damage Damage;

    // QTE 성공여부
    private bool QteState;
    // QTE 계수
    private float QteValue;

    // 랜덤
    private Random rand = new();


    public DamageCalculator(StageDifficulty difficulty)
    {
        switch (difficulty)
        {
            case StageDifficulty.Normal:
                StrValue = 1f;
                DefValue = 1f;
                break;

            case StageDifficulty.Hard:
                StrValue = 0.5f;
                DefValue = 0.5f;
                break;
        }

        
    }

    // --- 계산식 --- ///
    // 1. 스탯 계산
    // 2. 스킬 계산
    // 3. QTE 성공/실패 계산
    // 4. 마지막 확률 변수 계산
    public void SetState(CharacterStats stat, Damage dmg, bool qte)
    {
        Str = stat.strength;
        Def = stat.toughness;

        Damage = dmg;

        QteState = qte;

        // todo QteValue 지정해야함
        QteValue = 1.2f;
    }

    public float StatCalculatePhase() {
        float raw = (Str * StrValue) - (Def * DefValue);

        if(raw <= 0) {
            return 0f;
        }
        else {
             return raw;
        }
    }

    public float SkillCalculatePhase(float statPhase)
    {
        return (statPhase * Damage.scalingDamage) + Damage.fixedDamage;
    }

    public float QteCalculatePhase(float skillPhase)
    {
        if (QteState)
        {
            return skillPhase * QteValue;
        }
        else
        {
            return skillPhase;
        }
    }

    public int RandomCalculatePhase(float qtePhase)
    {
        float RandValue = rand.Next(95, 105) / 100;

        return Convert.ToInt32(qtePhase * RandValue);
    }

    public int CalculateDamage()
    {
        float statPhase = StatCalculatePhase();
        float skillPhase = SkillCalculatePhase(statPhase);
        float qtePhase = QteCalculatePhase(skillPhase);
        int randomPhase = RandomCalculatePhase(qtePhase);

        return randomPhase;
    }
}