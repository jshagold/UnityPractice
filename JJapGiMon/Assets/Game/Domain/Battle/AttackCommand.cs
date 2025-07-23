using System.Collections.Generic;

public sealed class AttackCommand : BattleCommand
{
    private DamageCalculator DmgCalculator;

    public AttackCommand(StageDifficulty difficulty, BattleTarget battlePair, SkillData skill, bool qte) : base(difficulty, battlePair, skill, qte) 
    {
        DmgCalculator = new DamageCalculator(Difficulty);
    }

    public override void Execute()
    {
        

        foreach (var target in BattlePair.Targets)
        {
            DmgCalculator.SetState(target.CurrentStat, Skill, QteSucceeded);
        }
    }
}