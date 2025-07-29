using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아군 선택 UI 컴포넌트. 버튼 클릭 시 선택된 캐릭터를 콜백.
/// </summary>
public class PlayerSelectorUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    private Action<CharacterModel> onPlayerPicked;

    public void Show(List<CharacterModel> players, Action<CharacterModel> callback)
    {
        onPlayerPicked = callback;
        panel.SetActive(true);
        Clear();
        foreach (var player in players)
        {
            var btn = Instantiate(buttonPrefab, buttonContainer);
            btn.GetComponentInChildren<Text>().text = player.DisplayName;
            btn.onClick.AddListener(() => HandlePick(player));
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
        Clear();
    }

    private void HandlePick(CharacterModel player)
    {
        onPlayerPicked?.Invoke(player);
    }

    private void Clear()
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}