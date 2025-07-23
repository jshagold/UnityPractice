using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Overlays;
using UnityEngine;

public class CharacterModel
{
    public CharacterData TemplateData { get; }
    public CharacterSaveData SaveData { get; private set; }

    private readonly StatCalculator _statCalc; // 스탯 전용 계산기

    // --- 도메인 이벤트 --- ///
    public event Action<int> OnHpChanged;
    public event Action<int> OnLevelUp;
    public event Action OnDeath;


    public CharacterModel(CharacterData characterData, CharacterSaveData characterSaveData)
    {
        TemplateData = characterData;
        SaveData = characterSaveData;
        _statCalc = new StatCalculator(this);

        Phase0Stat = DefaultStat;
    }

    // --- 읽기 전용 Property --- //

    public string DisplayName => string.IsNullOrEmpty(SaveData.name) ? TemplateData.CharacterName : SaveData.name;
    public int Level => SaveData.level;
    public int CurrentExp => SaveData.currentExp;
    public CharacterType CharacterType => SaveData.CharacterType;
    public int EvolutionStage => SaveData.EvolutionStage;
    public CharacterFaction Faction => SaveData.Faction;
    public List<CharacterKeyword> KeywordList => SaveData.KeywordList;
    public bool IsDead => CurrentHp <= 0;


    // Current Stats Strength, toughness, agility, evasionRate, criticalRate
    public int CurrentHp => SaveData.currentHealth;
    private CharacterStats DefaultStat => _statCalc.GetDefaultStatByLevel(Level);

    private CharacterStats Phase0Stat;
    private CharacterStats Phase5Stat;

    private CharacterStats _currentStat;
    public CharacterStats CurrentStat => _currentStat;

    // Skill List
    public SkillData MainSkill
    {
        get => SaveData.MainSkill;
        private set => SaveData.MainSkill = value;
    }
    public SkillData? Sub1Skill
    {
        get => SaveData.Sub1Skill;
        private set => SaveData.Sub1Skill = value;
    }
    public SkillData? Sub2Skill
    {
        get => SaveData.Sub2Skill;
        private set => SaveData.Sub2Skill = value;
    }

    public List<SkillData?> PassiveList => SaveData.PassiveList;





    // --- 도메인 동작(비즈니스 규칙) --- ///

    // Damage
    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        SaveData.currentHealth = Mathf.Max(0, CurrentHp - amount);
        OnHpChanged?.Invoke(SaveData.currentHealth);

        if (SaveData.currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    // Stat
    public void ApplyDefaultStat()
    {
        _currentStat = DefaultStat;
    }

    public void ApplyPhase0()
    {
        Phase0Stat = DefaultStat.Clone();
        _currentStat = Phase0Stat;
    }

    public void ApplyPhase5()
    {
        Phase5Stat = Phase0Stat.Clone();
        _currentStat = Phase5Stat;
    }

    public void ApplyStat(SkillEffect skillEffect)
    {
        CharacterStatType? targetStat = skillEffect.statType;
        float value = skillEffect.value;

        switch (skillEffect)
        {
            case StatMulEffect:
                switch (targetStat)
                {
                    case CharacterStatType.Health:
                        {
                            int n = Convert.ToInt32(value);
                            int convertHealth = _currentStat.health * n;

                            _currentStat.health = convertHealth;
                            break;
                        }

                    case CharacterStatType.Strength:
                        {
                            int n = Convert.ToInt32(value);
                            int convertStrength = _currentStat.strength * n;

                            _currentStat.strength = convertStrength;
                            break;
                        }

                    case CharacterStatType.Toughness:
                        {
                            int n = Convert.ToInt32(value);
                            int convertToughness = _currentStat.toughness * n;

                            _currentStat.health = convertToughness;
                            break;
                        }

                    case CharacterStatType.Agility:
                        {
                            float convertAgility = _currentStat.agility * value;

                            _currentStat.agility = convertAgility;
                            break;
                        }

                    case CharacterStatType.EvasionRate:
                        {
                            float convertEvasionRate = _currentStat.evasionRate * value;

                            _currentStat.agility = convertEvasionRate;
                            break;
                        }

                    case CharacterStatType.CriticalRate:
                        {
                            float convertCriticalRate = _currentStat.criticalRate * value;

                            _currentStat.agility = convertCriticalRate;
                            break;
                        }
                }
                break;

            case StatPlusEffect:
                switch (targetStat)
                {
                    case CharacterStatType.Health:
                        {
                            int n = Convert.ToInt32(value);
                            int convertHealth = _currentStat.health + n;

                            _currentStat.health = convertHealth;
                            break;
                        }

                    case CharacterStatType.Strength:
                        {
                            int n = Convert.ToInt32(value);
                            int convertStrength = _currentStat.strength + n;

                            _currentStat.strength = convertStrength;
                            break;
                        }

                    case CharacterStatType.Toughness:
                        {
                            int n = Convert.ToInt32(value);
                            int convertToughness = _currentStat.toughness + n;

                            _currentStat.health = convertToughness;
                            break;
                        }

                    case CharacterStatType.Agility:
                        {
                            float convertAgility = _currentStat.agility + value;

                            _currentStat.agility = convertAgility;
                            break;
                        }

                    case CharacterStatType.EvasionRate:
                        {
                            float convertEvasionRate = _currentStat.evasionRate + value;

                            _currentStat.agility = convertEvasionRate;
                            break;
                        }

                    case CharacterStatType.CriticalRate:
                        {
                            float convertCriticalRate = _currentStat.criticalRate + value;

                            _currentStat.agility = convertCriticalRate;
                            break;
                        }
                }
                break;
        }
    }

    // Skill
    public void AddMainSkill(SkillData skill)
    {
        MainSkill = skill;
    }
    public void RemoveMainSkill()
    {
        MainSkill = null;
    }

    public void AddSub1Skill(SkillData skill)
    {
        MainSkill = skill;
    }
    public void RemoveSub1Skill()
    {
        MainSkill = null;
    }
    public void AddSub2Skill(SkillData skill)
    {
        MainSkill = skill;
    }
    public void RemoveSub2Skill()
    {
        MainSkill = null;
    }

    public void AddPassiveSkill(SkillData skill)
    {
        PassiveList.Add(skill);
    }
    public void RemovePassiveSkill(SkillData skill)
    {
        PassiveList.Remove(skill);
    }
}
