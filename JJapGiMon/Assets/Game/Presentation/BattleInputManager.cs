using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Phase2 ���� �Է� ó��: �÷��̾� ĳ���� ���� ���� �� ��ų��Ÿ�� ȹ��.
/// </summary>
public sealed class BattleInputManager : MonoBehaviour
{
    /// <summary>
    /// players.Count ��ŭ ĳ����-��ų-Ÿ���� �����Ͽ� outTargets�� �߰�.
    /// </summary>
    public IEnumerator CollectPlayerTargets(
        List<CharacterModel> players,
        List<CharacterModel> enemies,
        List<BattleTarget> outTargets)
    {
        outTargets.Clear();

        while (outTargets.Count < players.Count)
        {
            // 1) ĳ���� ����
            CharacterModel caster = null;
            bool chosen = false;
            var remaining = players.Except(outTargets.Select(t => t.Caster)).ToList();
            BattleUI.Instance.ShowAllySelector(remaining, c => { caster = c; chosen = true; });
            yield return new WaitUntil(() => chosen);
            BattleUI.Instance.HideAllySelector();

            // 2) ��ų ����
            ActiveSkill skill = null;
            chosen = false;
            BattleUI.Instance.ShowSkillPanel(caster, s => { skill = s; chosen = true; });
            yield return new WaitUntil(() => chosen);
            BattleUI.Instance.HideSkillPanel();

            // 3) Ÿ�� ���� (SingleEnemy�� UI��)
            List<CharacterModel> targets;
            if (skill.TargetType == SkillTargeting.SingleEnemy)
            {
                targets = null;
                chosen = false;
                BattleUI.Instance.ShowTargetSelector(skill, enemies, t => { targets = t; chosen = true; });
                yield return new WaitUntil(() => chosen);
                BattleUI.Instance.HideTargetSelector();
            }
            //else if (skill.TargetType == SkillTargeting.Self)
            //{
            //    targets = new() { caster };
            //}
            else if(skill.TargetType == SkillTargeting.WholeEnemy) // AllEnemies
            {
                targets = enemies.Where(e => !e.IsDead).ToList();
            }
            else
            {
                targets = null;
            }

            outTargets.Add(new BattleTarget(caster, skill, targets));
        }
    }
}