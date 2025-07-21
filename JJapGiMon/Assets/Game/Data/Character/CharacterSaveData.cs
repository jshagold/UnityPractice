using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterSaveData
{
    public string id;

    public string name;

    public CharacterType CharacterType;
    public int EvolutionStage;
    public CharacterFaction Faction;
    public List<CharacterKeyword> KeywordList;

    // stat
    public int level;
    public int currentExp;
    public int currentHealth;

    public SkillData MainSkill;
    public SkillData Sub1Skill;
    public SkillData Sub2Skill;
    public List<SkillData> PassiveList;
}
