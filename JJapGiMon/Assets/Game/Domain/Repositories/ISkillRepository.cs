public interface ISkillRepository
{
    void Save(SkillData skillData);

    ActiveSkill LoadActiveSkill(string skillId);
    PassiveSkill LoadPassiveSkill(string skillId);
}