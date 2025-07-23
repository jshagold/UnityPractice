using System.Collections.Generic;

public sealed class AttackCommand : BattleCommand
{
    private DamageCalculator dmgCalculator;

    public AttackCommand(StageDifficulty difficulty, BattleTarget battlePair, SkillData skill, bool qte) : base(difficulty, battlePair, skill, qte) 
    {
        dmgCalculator = new DamageCalculator(Difficulty);
    }

    public override void Execute()
    {
        

        foreach (var target in BattlePair.Targets)
        {
            dmgCalculator.SetState
        }
    }
}