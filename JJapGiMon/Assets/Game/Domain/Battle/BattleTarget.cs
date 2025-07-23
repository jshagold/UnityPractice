using System.Collections.Generic;

public class BattleTarget
{
    public CharacterModel Caster { get; }
    public IReadOnlyList<CharacterModel> Targets { get; }

    public BattleTarget(CharacterModel caster, SkillData skill, IReadOnlyList<CharacterModel> targets)
    {
        Caster = caster;
        Targets = targets;
    }
}