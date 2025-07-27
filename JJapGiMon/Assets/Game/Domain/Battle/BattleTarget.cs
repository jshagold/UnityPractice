using System.Collections.Generic;

public class BattleTarget
{
    public CharacterModel Caster { get; }
    public ActiveSkill Skill { get; }
    public List<CharacterModel> Targets { get; }

    public List<(DamageEffect dmgEffect, bool qte)> DmgQtePair { get; private set; }

    public BattleTarget(CharacterModel caster, ActiveSkill skill, List<CharacterModel> targets)
    {
        Caster = caster;
        Skill = skill;
        Targets = targets;
    }

    public void SetDmgQtePair(List<(DamageEffect dmgEffect, bool qte)> pair)
    {
        DmgQtePair = pair;
    }

}