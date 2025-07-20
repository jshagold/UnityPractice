using UnityEditor.Overlays;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class CharacterModel
{
    public CharacterData Template { get; }
    public CharacterSaveData SaveData { get; private set; }

    private readonly StatCalculator _statCalc; // 스탯 전용 계산기

    public CharacterModel(CharacterData characterData, CharacterSaveData characterSaveData)
    {

        int level = characterSaveData.level;

        this.Template = characterData;
        this.SaveData = characterSaveData;
    }

    public string DisplayName => SaveData.name;
    public int level => SaveData.level;
    public double MaxHp => 100.0;
    //public double CurrentHp => SaveData.currentHp;
    public int AttackPower => _statCalc.CalcAttack(level);
    public int DefensePower => _statCalc.CalcDefense(level);
    public double CurrentSpeed => _statCalc.CalcSpeed(level);
    public int CurrentExp => SaveData.currentExp;
}
