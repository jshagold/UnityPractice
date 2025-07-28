
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkill", menuName = "Scriptable Objects/ActiveSkill")]
public class ActiveSkill : SkillData
{
    private SkillTargeting _targetType;
    private SkillCost _skillCost;
    
    public SkillTargeting TargetType => _targetType;
    public SkillCost SkillCost => _skillCost;


    public void SetTargetType(SkillTargeting targetType)
    {
        _targetType = targetType;
    }

    public void SetSkillCost(SkillCost skillCost)
    {
        _skillCost = skillCost;
    }
} 