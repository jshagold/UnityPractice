using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬 선택 UI 컴포넌트. 버튼 클릭 시 선택된 스킬을 콜백.
/// </summary>
public class SkillSelectorUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    private Action<ActiveSkill> onSkillPicked;

    public void Show(List<ActiveSkill> skills, Action<ActiveSkill> callback)
    {
        onSkillPicked = callback;
        panel.SetActive(true);
        Clear();
        foreach (var skill in skills)
        {
            var btn = Instantiate(buttonPrefab, buttonContainer);
            btn.GetComponentInChildren<Text>().text = skill.skillName;
            btn.onClick.AddListener(() => HandlePick(skill));
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
        Clear();
    }

    private void HandlePick(ActiveSkill skill)
    {
        onSkillPicked?.Invoke(skill);
    }

    private void Clear()
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}