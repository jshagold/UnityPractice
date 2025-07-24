using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    public int skillId;
    public string skillName;
    public string skillDescription;

    public SkillType skillType;

    public readonly List<SkillEffect> effects;

}