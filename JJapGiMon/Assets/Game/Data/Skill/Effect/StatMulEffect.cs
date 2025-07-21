using System;

public class StatMulEffect : ISkillEffect
{
    private CharacterStats _stat;
    private int _level;
    private int _exp;

    public StatMulEffect(CharacterStats stat, CharacterStatType targetStat, float value) {
        _stat = stat;
        
        switch (targetStat)
        {

            case CharacterStatType.Health:
                {
                    int n = Convert.ToInt32(value);
                    int convertHealth = stat.health * n;

                    _stat.health = convertHealth;
                    break;
                }

            case CharacterStatType.Strength:
                {
                    int n = Convert.ToInt32(value);
                    int convertStrength = stat.strength * n;

                    _stat.strength = convertStrength;
                    break;
                }

            case CharacterStatType.Toughness:
                {
                    int n = Convert.ToInt32(value);
                    int convertToughness= stat.toughness * n;

                    _stat.health = convertToughness;
                    break;
                }

            case CharacterStatType.Agility:
                {
                    float convertAgility = stat.agility* value;

                    _stat.agility = convertAgility;
                    break;
                }

            case CharacterStatType.EvasionRate:
                {
                    float convertEvasionRate= stat.evasionRate * value;

                    _stat.agility = convertEvasionRate;
                    break;
                }

            case CharacterStatType.CriticalRate:
                {
                    float convertCriticalRate= stat.criticalRate * value;

                    _stat.agility = convertCriticalRate;
                    break;
                }

        }

    }

    public CharacterStats GetStat()
    {
        return _stat;
    }

}