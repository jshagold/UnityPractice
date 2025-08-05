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

    // --- Domain Events ---
    public event Action<int, int> OnHpChanged;          // current, max
    public event Action<int, int> OnExpChanged;         // current, max
    public event Action<int> OnLevelUp;                 // new level
    public event Action OnDeath;
    public event Action<CharacterStats> OnStatChanged;
    public event Action<ActiveSkill> OnMainSkillChanged;
    public event Action<ActiveSkill?> OnSub1SkillChanged;
    public event Action<ActiveSkill?> OnSub2SkillChanged;

    // --- Properties --- //
    // 이름
    public string DisplayName => string.IsNullOrEmpty(SaveData.Name) ? TemplateData.CharacterName : SaveData.Name;  // 화면에 보여지는 이름
    public int Level => SaveData.Level; // 레벨
    public CharacterType CharacterType => SaveData.CharacterType;   // 캐릭터 유형
    public int EvolutionStage => SaveData.EvolutionStage;   // 캐릭터 진화 단계
    public CharacterFaction Faction => SaveData.Faction;    // 캐릭터 진영
    public List<CharacterKeyword> KeywordList => SaveData.KeywordList;  // 캐릭터 키워드


    // HP and Exp
    public int MaxHp => _statCalc.GetDefaultStatByLevel(Level).health;  // 현재 레벨에 따른 최대 체력
    public int CurrentHp => SaveData.CurrentHealth; // 현재 체력
    public int MaxExp => _statCalc.CalcMaxExp(Level);
    public int CurrentExp => SaveData.CurrentExp;   // 현재 경험치
    public bool IsDead => CurrentHp <= 0;   // 캐릭터 사망 상태

    // --- Stats
    private CharacterStats _defaultStat;
    private CharacterStats _phase0Stat;
    private CharacterStats _phase5Stat;
    private CharacterStats _currentStat;
    public CharacterStats CurrentStat => _currentStat;

    // Skill List
    public ActiveSkill MainSkill => SaveData.MainSkill;
    public ActiveSkill? Sub1Skill => SaveData.Sub1Skill;
    public ActiveSkill? Sub2Skill => SaveData.Sub2Skill;
    public List<PassiveSkill?> PassiveList => SaveData.PassiveList;


    // --- Constructor --- //
    public CharacterModel(CharacterData characterData, CharacterSaveData characterSaveData)
    {
        TemplateData = characterData;
        SaveData = characterSaveData;
        _statCalc = new StatCalculator(this);

        Initialize();
    }

    private void Initialize()
    {
        ApplyDefaultStat();
        OnExpChanged?.Invoke(SaveData.CurrentExp, _statCalc.CalcMaxExp(Level));

        SetInitialSkills();
    }

    // --- 도메인 동작(비즈니스 규칙) --- ///

    // Damage
    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        SaveData.CurrentHealth = Mathf.Max(0, CurrentHp - amount);
        OnHpChanged?.Invoke(SaveData.CurrentHealth, MaxHp);

        if (SaveData.CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if(IsDead) return;
        SaveData.CurrentHealth = Mathf.Min(MaxHp, CurrentHp + amount);
        OnHpChanged?.Invoke(SaveData.CurrentHealth, MaxHp);
    }

    // 최대경험치 초과분은 삭제된다.
    public void GainExp(int amount)
    {
        SaveData.CurrentExp += amount;
        if(SaveData.CurrentExp >= MaxExp)
        {
            SaveData.CurrentExp = 0;
            SaveData.Level++;
            OnLevelUp?.Invoke(SaveData.Level);

            // Recalculate stats on level up
            var newStats = _statCalc.GetDefaultStatByLevel(SaveData.Level);
            _currentStat = newStats;
            SaveData.CurrentHealth = Mathf.Min(CurrentHp, newStats.health);
            OnStatChanged?.Invoke(_currentStat);
            OnHpChanged?.Invoke(SaveData.CurrentHealth, newStats.health);
        }

        OnExpChanged?.Invoke(SaveData.CurrentExp, MaxExp);
    }

    // Stat
    public void ApplyDefaultStat()
    {
        _defaultStat = _statCalc.GetDefaultStatByLevel(Level);
        _currentStat = _defaultStat;
        OnStatChanged?.Invoke(_currentStat);
        OnHpChanged?.Invoke(CurrentHp, _currentStat.health);
    }
    public void ApplyPhase0()
    {
        // TODO Phase0 스탯 계산해야함
        _phase0Stat = _defaultStat.Clone();
        _currentStat = _phase0Stat;
        OnStatChanged?.Invoke(_currentStat);
        OnHpChanged?.Invoke(CurrentHp, _currentStat.health);
    }
    public void ApplyPhase5()
    {
        // TODO Phase5 스탯 계산해야함
        _phase5Stat = _phase0Stat.Clone();
        _currentStat = _phase5Stat;
        OnStatChanged?.Invoke(_currentStat);
        OnHpChanged?.Invoke(CurrentHp, _currentStat.health);
    }
    public void ApplyToCurrentStat(SkillEffect skillEffect)
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


    // --- Skill --- //
    private void SetInitialSkills()
    {
        OnMainSkillChanged?.Invoke(SaveData.MainSkill);
        OnSub1SkillChanged?.Invoke(SaveData.Sub1Skill);
        OnSub2SkillChanged?.Invoke(SaveData.Sub2Skill);
    }
    // --- Active Skill
    public void SetMainSkill(ActiveSkill skill)
    {
        if (skill != null)
        {
            SaveData.MainSkill = skill;
        }
    }
    public void SetSub1Skill(ActiveSkill skill)
    {
        SaveData.Sub1Skill = skill;
    }
    public void SetSub2Skill(ActiveSkill skill)
    {
        SaveData.Sub2Skill = skill;
    }

    // --- Passive Skill
    public void AddPassiveSkill(PassiveSkill skill)
    {
        SaveData.PassiveList.Add(skill);
    }
    public void RemovePassiveSkill(PassiveSkill skill)
    {
        SaveData.PassiveList.Remove(skill);
    }
}
