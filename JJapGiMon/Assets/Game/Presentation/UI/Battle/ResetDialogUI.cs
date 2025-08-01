using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResetDialogUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public void Show(CharacterModel player, Action onConfirm, Action onCancel)
    {
        panel.SetActive(true);
        messageText.text = $"Reset selection for {player.DisplayName}?";
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => { onConfirm?.Invoke(); panel.SetActive(false); });
        cancelButton.onClick.AddListener(() => { onCancel?.Invoke(); panel.SetActive(false); });
    }

    public void Hide() => panel.SetActive(false);
}