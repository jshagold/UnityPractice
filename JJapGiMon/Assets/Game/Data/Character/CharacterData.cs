using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string CharacterId;
    public string CharacterName;

    public string sprite;
    public string prefab;

    public CharacterType type;
    public int evolutionStage;
    public CharacterFaction faction;
    public List<CharacterKeyword> KeywordList;

    public int level;
    public int currentExp;
    public CharacterStats CharacterStat;

    public ActiveSkill MainSkill;
    public ActiveSkill Sub1Skill;
    public ActiveSkill Sub2Skill;
    public List<PassiveSkill> PassiveList;

}
