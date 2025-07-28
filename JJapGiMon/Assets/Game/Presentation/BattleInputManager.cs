using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Phase2 전용 입력 처리: 플레이어 캐릭터 순차 선택 및 스킬·타겟 획득.
/// </summary>
public sealed class BattleInputManager : MonoBehaviour
{
    /// <summary>
    /// players.Count 만큼 캐스터-스킬-타겟을 선택하여 outTargets에 추가.
    /// </summary>
    public IEnumerator CollectPlayerTargets(
        List<CharacterModel> players,
        List<CharacterModel> enemies,
        List<BattleTarget> outTargets)
    {
        outTargets.Clear();

        while (outTargets.Count < players.Count)
        {
            // 1) 캐스터 선택
            CharacterModel caster = null;
            bool chosen = false;
            var remaining = players.Except(outTargets.Select(t => t.Caster)).ToList();
            BattleUI.Instance.ShowAllySelector(remaining, c => { caster = c; chosen = true; });
            yield return new WaitUntil(() => chosen);
            BattleUI.Instance.HideAllySelector();

            // 2) 스킬 선택
            ActiveSkill skill = null;
            chosen = false;
            BattleUI.Instance.ShowSkillPanel(caster, s => { skill = s; chosen = true; });
            yield return new WaitUntil(() => chosen);
            BattleUI.Instance.HideSkillPanel();

            // 3) 타겟 선택 (SingleEnemy만 UI로)
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