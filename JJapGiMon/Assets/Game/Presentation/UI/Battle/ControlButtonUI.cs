using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlButtonsUI : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button battleStartButton;




    public void SetCancel(Action onClick)
    {
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.gameObject.SetActive(onClick != null);
        if (onClick != null)
            cancelButton.onClick.AddListener(() => onClick());
    }


    public void ShowBattleStartPanel(Action onClick)
    {
        battleStartButton.gameObject.SetActive(true);
        Clear();

        var btn = Instantiate(battleStartButton, buttonContainer);
        btn.onClick.AddListener(() => onClick());
        var label = btn.GetComponentInChildren<TextMeshProUGUI>(true);
        label.text = "Battle Start!!";
        btn.gameObject.SetActive(true);
    }

    public void HideBattleStartPanel()
    {
        battleStartButton.gameObject.SetActive(false);
        Clear();
    }

    private void Clear()
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}