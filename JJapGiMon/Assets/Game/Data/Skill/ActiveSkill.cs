
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkill", menuName = "Scriptable Objects/ActiveSkill")]
public class ActiveSkill : SkillData
{   
    public SkillTargeting TargetType;
    public SkillCost SkillCost;

    public ActiveSkill() { }

    private ActiveSkill(string id)
    {
        this.skillId = id;
        this.skillName = "";
        this.skillDescription = "";
        this.skillType = SkillType.Active;
    }

    public static ActiveSkill New(string characterId)
    {
        return new ActiveSkill(characterId);
    }
} 