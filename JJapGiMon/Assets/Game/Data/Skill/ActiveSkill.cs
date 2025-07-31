
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkill", menuName = "Scriptable Objects/ActiveSkill")]
public class ActiveSkill : SkillData
{
    private SkillTargeting _targetType;
    private SkillCost _skillCost;
    
    public SkillTargeting TargetType => _targetType;
    public SkillCost SkillCost => _skillCost;

    public ActiveSkill() { }

    private ActiveSkill(string id)
    {
        this.skillId = id;
        this.skillName = "";
        this.skillDescription = "";
        this.skillType = SkillType.Active;

        this._targetType = SkillTargeting.None;
    }

    public static ActiveSkill New(string characterId)
    {
        return new ActiveSkill(characterId);
    }

    public void SetTargetType(SkillTargeting targetType)
    {
        _targetType = targetType;
    }

    public void SetSkillCost(SkillCost skillCost)
    {
        _skillCost = skillCost;
    }
} 