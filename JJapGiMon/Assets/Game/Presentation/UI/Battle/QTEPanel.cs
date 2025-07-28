using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// QTE 입력 UI 컴포넌트. Toggle과 완료 버튼으로 구성.
/// </summary>
public sealed class QTEPanel
{
    private readonly VisualElement container;
    private readonly Button completeButton;
    private List<bool> results;

    public QTEPanel(VisualElement container)
    {
        this.container = container;
        this.completeButton = container.Q<Button>("QTECompleteButton");
    }

    public void Open(int hitCount, Action<List<bool>> onComplete)
    {
        container.Clear();
        results = new List<bool>(new bool[hitCount]);

        for (int i = 0; i < hitCount; i++)
        {
            var toggle = new Toggle($"Hit {i + 1}") { value = false };
            int idx = i;
            toggle.RegisterValueChangedCallback(evt => results[idx] = evt.newValue);
            container.Add(toggle);
        }
        completeButton.clicked -= null;
        completeButton.clicked += () => onComplete(new List<bool>(results));
        container.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        completeButton.clicked -= null;
        container.Clear();
        container.style.display = DisplayStyle.None;
    }
}