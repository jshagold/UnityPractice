using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectedPlayersUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    private Action<CharacterModel> onResetRequest;

    public void Show(List<BattleTarget> selections, Action<CharacterModel> callback)
    {
        onResetRequest = callback;
        panel.SetActive(true);
        Clear();

        foreach (var bt in selections)
        {
            var btn = Instantiate(buttonPrefab, buttonContainer);
            btn.GetComponentInChildren<Text>().text = bt.Caster.DisplayName;
            btn.onClick.AddListener(() => onResetRequest?.Invoke(bt.Caster));
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
        Clear();
    }

    private void Clear()
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}