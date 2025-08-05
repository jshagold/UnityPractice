using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    public string skillId;
    public string skillName;
    public string skillDescription;

    public SkillType skillType;

    [JsonProperty("effects", ItemConverterType = typeof(SkillEffectConverter))]
    public List<SkillEffect> effects { get; set; } = new();

    // todo serialize 할수있어야함
    //[Header("UI")]
    //public Sprite IconSprite;          // 스킬 아이콘
    //public Color IconTint = Color.white;  // 필요하면 색상 보정
}