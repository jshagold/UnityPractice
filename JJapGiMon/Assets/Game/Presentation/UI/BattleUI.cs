using System.Collections.Generic;
using UnityEngine;

public sealed class BattleUI : MonoBehaviour
{
    public static BattleUI Instance { get; private set; } // 싱글턴

    private void Awake() => Instance = this;

    [SerializeField] PlayerSelector playerSelector;
    [SerializeField] SkillPanel skillPanel;
    [SerializeField] TargetSelector targetSelector;


    // --- Player 선택 --- //
    public void OpenPlayerSelector(List<CharacterModel> players) => playerSelector.OpenPlayer(players);
    public CharacterModel player => playerSelector.SelectPlayer();

    
    // --- Skill 선택 --- //
    public void OpenSkillSelector(CharacterModel caster) => skillPanel.OpenSkill(caster);
    public SkillData skillData => skillPanel.SkillData;


    // --- Target 선택 --- //
    public void OpenTargetSelector(List<CharacterModel> characters) => targetSelector.OpenTarget(characters); 
    //List<CharacterModel> targets = BattleUI.Instance.PopTargetSelection();



    // --- 선택 종료 --- //
    public bool IsFinishSelect;
    public BattleTarget

}