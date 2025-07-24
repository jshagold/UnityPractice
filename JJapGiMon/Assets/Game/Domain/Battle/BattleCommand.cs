using System.Collections.Generic;

public abstract class BattleCommand
{
    public StageDifficulty Difficulty { get; }
    public BattleTarget BattlePair { get; }
    public ActiveSkill ActiveSkill { get; }
    public bool QteSucceeded { get; }

    protected BattleCommand(StageDifficulty difficulty, BattleTarget battlePair, ActiveSkill activeSkill, bool qte) {
        Difficulty = difficulty;
        BattlePair = battlePair;
        ActiveSkill = activeSkill;
        QteSucceeded = qte;
    }

    public abstract void Execute();
}