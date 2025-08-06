using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Phase2 전용 입력 처리: 플레이어 캐릭터 순차 선택 및 스킬·타겟 획득.
/// </summary>
public sealed class BattleInputManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private PlayerSelectorUI playerSelectorUI;
    [SerializeField] private SkillSelectorUI skillSelectorUI;
    [SerializeField] private TargetSelectorUI targetSelectorUI;
    [SerializeField] private SelectedPlayersUI selectedPlayersUI;
    [SerializeField] private ResetDialogUI resetDialogUI;
    [SerializeField] private ControlButtonsUI controlButtonsUI;
    [SerializeField] private QTEPanelUI qtePanelUI;
    [SerializeField] private ResultDialogUi resultDialogUI;

    private void OnEnable()
    {
        BattleManager.Instance.OnBattleEnd += HandleBattleEnd;
    }

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
            // 0) 모든 창 닫기
            CloseAllPanel();

            // 1) 이미 선택된 캐릭터 목록 표시
            CharacterModel toReset = null;
            bool resetRequested = false;
            selectedPlayersUI.Show(outTargets, p => { toReset = p; resetRequested = true; });


            Debug.Log("playerSelectorUI.Show");
            // 1) 플레이어 캐릭터 선택
            CharacterModel chosenPlayer = null;
            bool playerPicked = false;
            playerSelectorUI.Show(players.Except(outTargets.Select(t => t.Caster)).ToList(), p => { chosenPlayer = p; playerPicked = true; });
            yield return new WaitUntil(() => playerPicked || resetRequested);
            playerSelectorUI.Hide();
            selectedPlayersUI.Hide();

            // 다시 설정
            if (resetRequested)
            {
                Debug.Log("resetDialogUI.Show");
                bool confirmed = false, cancelled = false;
                resetDialogUI.Show(toReset, () => confirmed = true, () => cancelled = true);
                yield return new WaitUntil(() => confirmed || cancelled);
                if (confirmed) outTargets.RemoveAll(bt => bt.Caster == toReset);
                resetDialogUI.Hide();
                continue;
            }


            Debug.Log("skillSelectorUI.Show");
            // 2) 스킬 선택
            ActiveSkill chosenSkill = null;
            bool skillPicked = false, goBack = false;
            var skills = new[] { chosenPlayer.MainSkill, chosenPlayer.Sub1Skill, chosenPlayer.Sub2Skill }
                         .Where(s => s != null).ToList();
            controlButtonsUI.SetCancel(() => { goBack = true; skillPicked = true; });
            skillSelectorUI.Show(skills, s => { chosenSkill = s; skillPicked = true; });
            yield return new WaitUntil(() => skillPicked);
            skillSelectorUI.Hide();
            controlButtonsUI.SetCancel(null);
            if (goBack) continue;

            Debug.Log("targetSelectorUI.Show");
            // 3) 타겟 선택 (SingleEnemy만 UI로)
            List<CharacterModel> chosenTargets;
            Debug.Log($"TargetType: {chosenSkill.TargetType}");
            switch (chosenSkill.TargetType)
            {
                case SkillTargeting.SingleEnemy:
                    CharacterModel single = null;
                    bool targetPicked = false;
                    controlButtonsUI.SetCancel(() => { goBack = true; targetPicked = true; });
                    targetSelectorUI.Show(enemies.Where(e => !e.IsDead).ToList(), e => { single = e; targetPicked = true; });
                    yield return new WaitUntil(() => targetPicked);
                    targetSelectorUI.Hide();
                    controlButtonsUI.SetCancel(null);
                    if (goBack) continue;
                    chosenTargets = new List<CharacterModel> { single };
                    break;

                case SkillTargeting.WholeEnemy:
                    chosenTargets = enemies.Where(e => !e.IsDead).ToList();
                    break;

                default:
                    chosenTargets = new List<CharacterModel>();
                    break;
            }

            outTargets.Add(new BattleTarget(chosenPlayer, chosenSkill, chosenTargets));
            Debug.Log($"outTargets Count {outTargets.Count}");
        }

        // Enable start
        Debug.Log("controlButtonsUI.Show");
        bool start = false;
        controlButtonsUI.ShowBattleStartPanel(() => start = true);
        yield return new WaitUntil(() => start);
        controlButtonsUI.HideBattleStartPanel();
        selectedPlayersUI.Hide();
    }


    public IEnumerator CollectQTEResults(List<BattleTarget> battleOrderList)
    {
        foreach (var battlePair in battleOrderList)
        {
            if(battlePair.Skill.effects == null) continue;
            var dmgEffects = battlePair.Skill.effects.OfType<DamageEffect>().ToList();
            int hitCount = dmgEffects.Count;
            List<bool> results = null;

            bool done = false;
            qtePanelUI.Show(hitCount, r => { results = r; done = true; });
            yield return new WaitUntil(() => done);
            qtePanelUI.Hide();

            battlePair.SetDmgQtePair(dmgEffects.Zip(results, (d, q) => (d, q)).ToList());
        }
    }

    public void CloseAllPanel()
    {
        playerSelectorUI.Hide();
        skillSelectorUI.Hide();
        targetSelectorUI.Hide();
        selectedPlayersUI.Hide();
        resetDialogUI.Hide();
        //controlButtonsUI.SetStart(null);
        controlButtonsUI.HideBattleStartPanel();
        controlButtonsUI.SetCancel(null);
        qtePanelUI.Hide();
        resultDialogUI.Hide();
    }

    /*
     * BattleManager에서 전투가 끝났을 때
     */
    public void HandleBattleEnd()
    {
        CloseAllPanel();
        resultDialogUI.Show();
    }
}