using System;
using UnityEngine;
using UnityEngine.UI;

public class ControlButtonsUI : MonoBehaviour
{
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
        startButton.onClick.RemoveAllListeners();
        startButton.gameObject.SetActive(onClick != null);
        if (onClick != null)
            startButton.onClick.AddListener(() => onClick());
    }
}