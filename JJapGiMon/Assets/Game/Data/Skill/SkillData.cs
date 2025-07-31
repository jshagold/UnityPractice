using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    public string skillId;
    public string skillName;
    public string skillDescription;

    public SkillType skillType;

    public readonly List<SkillEffect> effects;

    // todo serialize 할수있어야함
    //[Header("UI")]
    //public Sprite IconSprite;          // 스킬 아이콘
    //public Color IconTint = Color.white;  // 필요하면 색상 보정
}