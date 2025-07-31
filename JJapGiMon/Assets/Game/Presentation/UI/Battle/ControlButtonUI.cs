using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlButtonsUI : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button startButton;

    public void SetCancel(Action onClick)
    {
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.gameObject.SetActive(onClick != null);
        if (onClick != null)
            cancelButton.onClick.AddListener(() => onClick());
    }

    public void SetStart(Action onClick)
    {
        var btn = Instantiate(startButton, buttonContainer);
        btn.onClick.RemoveAllListeners();
        var label = btn.GetComponentInChildren<TextMeshProUGUI>(true);
        label.text = "Battle Start!!";
        btn.gameObject.SetActive(onClick != null);
        if (onClick != null)
        {
            btn.onClick.AddListener(() => onClick());
        }
    }
}