using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

/// <summary>
/// 전투 화면의 순수 뷰 레이어 (UI Toolkit 사용).
/// Ally/Skill/Target 셀렉터와 Back, Start 버튼을 관리.
/// BattleInputManager가 호출하여 입력 흐름을 제어함.
/// </summary>
public sealed class BattleUI : MonoBehaviour
{
    public static BattleUI Instance { get; private set; }
    [SerializeField] private UIDocument uiDocument;

    private VisualElement root;
    private PlayerSelector playerSelector;
    private SkillPanel skillPanel;
    private TargetSelector targetSelector;
    private Button backButton;
    private Button startBattleButton;

    private Action backCallback;
    private Action startCallback;

    void Awake()
    {
        Instance = this;
        root = uiDocument.rootVisualElement;
        // UXML 상의 요소를 이름으로 쿼리
        playerSelector = new PlayerSelector(root.Q<VisualElement>("AllySelector"));
        skillPanel = new SkillPanel(root.Q<VisualElement>("SkillPanel"));
        targetSelector = new TargetSelector(root.Q<VisualElement>("TargetSelector"));
        backButton = root.Q<Button>("BackButton");
        startBattleButton = root.Q<Button>("StartBattleButton");
    }

    // ─── Ally Selector ───
    public void ShowAllySelector(List<CharacterModel> players, Action<CharacterModel> onPick)
    {
        playerSelector.Open(players, onPick);
    }
    public void HideAllySelector()
    {
        playerSelector.Close();
    }

    // ─── Skill Panel ───
    public void ShowSkillPanel(CharacterModel caster, Action<ActiveSkill> onPick)
    {
        skillPanel.Open(caster, onPick);
    }
    public void HideSkillPanel()
    {
        skillPanel.Close();
    }

    // ─── Target Selector ───
    public void ShowTargetSelector(ActiveSkill skill, List<CharacterModel> enemies, Action<List<CharacterModel>> onPick)
    {
        targetSelector.Open(skill, enemies, onPick);
    }
    public void HideTargetSelector()
    {
        targetSelector.Close();
    }

    // ─── Back Button ───
    public void SetBackButton(bool active, Action onClick)
    {
        if (backCallback != null)
            backButton.clicked -= backCallback;

        backCallback = onClick;
        backButton.SetEnabled(active);
        if (active && backCallback != null)
            backButton.clicked += backCallback;
    }

    // ─── Start Button ───
    public void SetStartButton(bool active, Action onClick)
    {
        if (startCallback != null)
            startBattleButton.clicked -= startCallback;

        startCallback = onClick;
        startBattleButton.SetEnabled(active);
        if (active && startCallback != null)
            startBattleButton.clicked += startCallback;
    }

}