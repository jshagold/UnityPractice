using UnityEngine;

public class StatCalculator
{
    private readonly CharacterModel _characterModel;

    public StatCalculator(CharacterModel m) => _characterModel = m;


    public CharacterStats GetDefaultStatByLevel(int level)
    {
        CharacterStats Stat = level switch
        {
            0 => new CharacterStats(
                    health: 100,
                    strength: 1,
                    toughness: 1,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            1 => new CharacterStats(
                    health: 110,
                    strength: 10,
                    toughness: 10,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            2 => new CharacterStats(
                    health: 120,
                    strength: 11,
                    toughness: 11,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            3 => new CharacterStats(
                    health: 130,
                    strength: 12,
                    toughness: 12,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            4 => new CharacterStats(
                    health: 140,
                    strength: 13,
                    toughness: 13,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            5 => new CharacterStats(
                    health: 150,
                    strength: 14,
                    toughness: 14,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            6 => new CharacterStats(
                    health: 160,
                    strength: 15,
                    toughness: 15,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            7 => new CharacterStats(
                    health: 170,
                    strength: 16,
                    toughness: 16,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            8 => new CharacterStats(
                    health: 180,
                    strength: 17,
                    toughness: 17,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            9 => new CharacterStats(
                    health: 190,
                    strength: 18,
                    toughness: 18,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            10 => new CharacterStats(
                    health: 200,
                    strength: 19,
                    toughness: 19,
                    agility: 1.0f,
                    evasionRate: 0.0f,
                    criticalRate: 0.0f
                ),
            _ => throw new System.NotImplementedException(),
        };

        return Stat;
    }

    public int CalcDefense(int level)
    {
        int dfs = level switch
        {
            0 => 1,
            1 => 10,
            2 => 11,
            3 => 12,
            4 => 13,
            5 => 14,
            6 => 15,
            7 => 16,
            8 => 17,
            9 => 18,
            10 => 19,
            _ => 0,
        };

        return dfs;
    }

    public double CalcSpeed(int level)
    {
        return 1.0;
    }

    public int CalcMaxExp(int level)
    {
        int exp = level switch
        {
            0 => 999,
            1 => 100,
            2 => 120,
            3 => 156,
            4 => 218,
            5 => 328,
            6 => 524,
            7 => 891,
            8 => 1604,
            9 => 3047,
            10 => 6095,
            _ => 0,
        };

        return exp;
    }
}