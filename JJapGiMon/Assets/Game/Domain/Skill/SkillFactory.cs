public class SkillFactory
{
    private readonly ISkillRepository _skillRepository;

    public SkillFactory(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public ActiveSkill CreateActiveSkill(string skillId)
    {
        ActiveSkill Skill = _skillRepository.LoadActiveSkill(skillId);

        return Skill;
    }

    public PassiveSkill CreatePassiveSkill(string skillId)
    {
        PassiveSkill Skill = _skillRepository.LoadPassiveSkill(skillId);

        return Skill;
    }
}