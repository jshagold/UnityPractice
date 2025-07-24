
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkill", menuName = "Scriptable Objects/ActiveSkill")]
public class ActiveSkill : SkillData
{
    private int _targetCount;
    private SkillCost _skillCost;
    
    public int TargetCount => _targetCount;
    public SkillCost SkillCost => _skillCost;


    public void SetTargetCount(int targetCount)
    {
        _targetCount = targetCount;
    }

    public void SetSkillCost(SkillCost skillCost)
    {
        _skillCost = skillCost;
    }
} 