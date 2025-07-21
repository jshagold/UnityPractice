using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Overlays;
using UnityEngine;

public class CharacterModel
{
    public CharacterData TemplateData { get; }
    public CharacterSaveData SaveData { get; private set; }

    private readonly StatCalculator _statCalc; // 스탯 전용 계산기

    public CharacterModel(CharacterData characterData, CharacterSaveData characterSaveData)
    {
        TemplateData = characterData;
        SaveData = characterSaveData;
        _statCalc = new StatCalculator(this);
    }

    // --- 읽기 전용 Property --- //

    public string DisplayName => string.IsNullOrEmpty(SaveData.name) ? TemplateData.CharacterName : SaveData.name;
    public int Level => SaveData.level;
    public int CurrentExp => SaveData.currentExp;
    public CharacterType CharacterType => SaveData.CharacterType;
    public int EvolutionStage => SaveData.EvolutionStage;
    public CharacterFaction Faction => SaveData.Faction;
    public List<CharacterKeyword> KeywordList => SaveData.KeywordList;


    // Current Stats Strength, toughness, agility, evasionRate, criticalRate
    public int CurrentHp => SaveData.currentHealth;
    public CharacterStats DefaultStat => _statCalc.GetDefaultStatByLevel(Level);

    // Skill List

    public SkillData? _MainSkill => SaveData.MainSkill;

    
    public SkillData? Sub1Skill => SaveData.Sub1Skill;
    public SkillData? Sub2Skill => SaveData.Sub2Skill;





    // --- 도메인 동작(비즈니스 규칙) --- ///



    // Skill
    public void AddMainSkill(SkillData Skill)
    {
        if (MainSkill == null)
        {
            MainSkill = Skill;
        }
    }

    public void RemoveMainSkill(SkillData skill)
    {
        MainSkill = null;
    }

}
