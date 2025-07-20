using System;
using Unity.VisualScripting;

public class StatPlusEffect : ISkillEffect
{
    private CharacterStats _stat;


    public StatPlusEffect(CharacterStats stat, CharacterStatType targetStat, float value)
    {
        _stat = stat;

        switch (targetStat)
        {
            case CharacterStatType.Level:
                {
                    int n = Convert.ToInt32(value);
                    int convertLevel = stat.level + n;

                    _stat.level = convertLevel;
                    break;
                }

            case CharacterStatType.Health:
                {
                    int n = Convert.ToInt32(value);
                    int convertHealth = stat.health + n;

                    _stat.health = convertHealth;
                    break;
                }

            case CharacterStatType.Exp:
                {
                    int n = Convert.ToInt32(value);
                    int convertExp = stat.currentExp + n;

                    _stat.health = convertExp;
                    break;
                }

            case CharacterStatType.Strength:
                {
                    int n = Convert.ToInt32(value);
                    int convertStrength = stat.strength + n;

                    _stat.strength = convertStrength;
                    break;
                }

            case CharacterStatType.Toughness:
                {
                    int n = Convert.ToInt32(value);
                    int convertToughness = stat.toughness + n;

                    _stat.health = convertToughness;
                    break;
                }

            case CharacterStatType.Agility:
                {
                    float convertAgility = stat.agility + value;

                    _stat.agility = convertAgility;
                    break;
                }

            case CharacterStatType.EvasionRate:
                {
                    float convertEvasionRate = stat.evasionRate + value;

                    _stat.agility = convertEvasionRate;
                    break;
                }

            case CharacterStatType.CriticalRate:
                {
                    float convertCriticalRate = stat.criticalRate + value;

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