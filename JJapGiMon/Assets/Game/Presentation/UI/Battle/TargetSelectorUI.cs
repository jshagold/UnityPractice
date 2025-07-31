using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 적 타겟 선택 UI 컴포넌트. SingleEnemy만 UI로 처리.
/// </summary>
public class TargetSelectorUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    private Action<CharacterModel> onTargetPicked;

    public void Show(List<CharacterModel> targets, Action<CharacterModel> callback)
    {
        onTargetPicked = callback;
        panel.SetActive(true);
        Clear();
        foreach (var target in targets)
        {
            var btn = Instantiate(buttonPrefab, buttonContainer);
            var label = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            label.text = target.DisplayName;

            btn.onClick.AddListener(() => HandlePick(target));
            btn.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
        Clear();
    }

    private void HandlePick(CharacterModel target)
    {
        onTargetPicked?.Invoke(target);
    }

    private void Clear()
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}