using System.Collections.Generic;

public abstract class BattleCommand
{
    public StageDifficulty Difficulty { get; }
    public BattleTarget BattlePair { get; }
    public SkillData Skill { get; }
    public bool QteSucceeded { get; }

    protected BattleCommand(StageDifficulty difficulty, BattleTarget battlePair, SkillData skill, bool qte) {
        Difficulty = difficulty;
        BattlePair = battlePair;
        Skill = skill;
        QteSucceeded = qte;
    }

    public abstract void Execute();
}