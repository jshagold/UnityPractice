
using System.Collections.Generic;

[System.Serializable]
public class SkillDto
{
    public int id;
    public string skillName;
    public string description;

    public SkillType skillType;      // "active", "passive"
    public CostDto cost;          // null °¡´É
    public List<EffectDto> effects;
}