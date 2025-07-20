using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillId;
    public string skillName;
    public string skillDescription;

    public SkillType skillType;

    public readonly List<ISkillEffect> effects;

}