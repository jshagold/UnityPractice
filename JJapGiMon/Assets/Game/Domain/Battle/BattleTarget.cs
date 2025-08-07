using System.Collections.Generic;

public class BattleTarget
{
    public CharacterModel Caster { get; }
    public CharacterFaction CasterFaction { get; }
    public ActiveSkill Skill { get; }
    public List<CharacterModel> Targets { get; }

    public List<(DamageEffect dmgEffect, bool qte)> DmgQtePair { get; private set; }

    public BattleTarget(CharacterModel caster, CharacterFaction characterFaction, ActiveSkill skill, List<CharacterModel> targets)
    {
        Caster = caster;
        CasterFaction = characterFaction;
        Skill = skill;
        Targets = targets;
    }

    public void SetDmgQtePair(List<(DamageEffect dmgEffect, bool qte)> pair)
    {
        DmgQtePair = pair;
    }

}