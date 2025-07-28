using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// 아군 선택 UI 컴포넌트. 버튼 클릭 시 선택된 캐릭터를 콜백.
/// </summary>
public sealed class PlayerSelector
{
    private readonly VisualElement container;

    public PlayerSelector(VisualElement container)
    {
        this.container = container;
    }

    public void Open(List<CharacterModel> players, Action<CharacterModel> onPick)
    {
        container.Clear();
        foreach (var player in players)
        {
            var btn = new Button(() => onPick(player)) { text = player.DisplayName };
            container.Add(btn);
        }
        container.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        container.Clear();
        container.style.display = DisplayStyle.None;
    }
}