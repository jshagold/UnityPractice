
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveSkill", menuName = "Scriptable Objects/PassiveSkill")]
public class PassiveSkill : SkillData
{
    public PassiveSkill() {}
    private PassiveSkill(string id)
    {
        this.skillId = id;
        this.skillName = "";
        this.skillDescription = "";
        this.skillType = SkillType.Passive;
    }

    public static PassiveSkill New(string characterId)
    {
        return new PassiveSkill(characterId);
    }
}