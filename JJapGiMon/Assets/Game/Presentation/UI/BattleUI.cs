using System.Collections.Generic;
using UnityEngine;

public sealed class BattleUI : MonoBehaviour
{
    public static BattleUI Instance { get; private set; } // �̱���

    private void Awake() => Instance = this;

    [SerializeField] PlayerSelector playerSelector;
    [SerializeField] SkillPanel skillPanel;
    [SerializeField] TargetSelector targetSelector;


    // --- Player ���� --- //
    public void OpenPlayerSelector(List<CharacterModel> players) => playerSelector.OpenPlayer(players);
    public CharacterModel player => playerSelector.SelectPlayer();

    
    // --- Skill ���� --- //
    public void OpenSkillSelector(CharacterModel caster) => skillPanel.OpenSkill(caster);
    public SkillData skillData => skillPanel.SkillData;


    // --- Target ���� --- //
    public void OpenTargetSelector(List<CharacterModel> characters) => targetSelector.OpenTarget(characters); 
    //List<CharacterModel> targets = BattleUI.Instance.PopTargetSelection();



    // --- ���� ���� --- //
    public bool IsFinishSelect;
    public BattleTarget

}