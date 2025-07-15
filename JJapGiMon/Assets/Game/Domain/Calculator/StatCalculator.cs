using UnityEngine;

public class StatCalculator
{
    private readonly CharacterModel _m;

    public StatCalculator(CharacterModel m) => _m = m;

    public int CalcAttack(int level)
    {
        int atk = level switch
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

        return atk;
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