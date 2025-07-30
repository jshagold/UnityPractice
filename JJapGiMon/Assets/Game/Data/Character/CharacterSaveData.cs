using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterSaveData
{
    public string Id;
    public string Name;

    public CharacterType CharacterType;
    public int EvolutionStage;
    public CharacterFaction Faction;
    public List<CharacterKeyword> KeywordList;

    // stat
    public int Level;
    public int CurrentExp;
    public int CurrentHealth;

    public ActiveSkill MainSkill;
    public ActiveSkill Sub1Skill;
    public ActiveSkill Sub2Skill;
    public List<PassiveSkill> PassiveList;

    public int SaveVersion { get; set; }

    public CharacterSaveData() { }

    private CharacterSaveData(string id)
    {
        this.Id = id;

        Level = 1;
        CurrentExp = 0;

        SaveVersion = 1;
    }

    public static CharacterSaveData New(string characterId)
    {
        return new CharacterSaveData(characterId);
    }
}
