using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionCountHUD : MonoBehaviour
{
    [Header("Samsara - Level")]
    [SerializeField] private TextMeshProUGUI samsaraText;

    [Header("Next Day - Progress(Remaining / Max)")]
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Actions (아이콘 배열)")]
    [SerializeField] private RectTransform actionIconContainer;
    [SerializeField] private Image actionIconPrefab;
    [SerializeField] private Sprite remainingSprite;
    [SerializeField] private Sprite usedSprite;

    private readonly List<Image> actionIcons = new();

    // --- 외부에서 호출할 API ---
    public void SetSamsaraLevel(int samsaraLevel) {
        if(samsaraLevel <= 0) samsaraLevel = 0;
        if(samsaraText != null) samsaraText.text = $"{samsaraLevel}";
    }

    public void SetProgress(int remaining, int max) {
        if(remaining <= 0) remaining = 0;
        if(max <= 0) max = 0;

        if(progressText != null) progressText.text = $"{remaining}/{max}";
    }

    /// <summary>남은 행동 수 아이콘 갱신. 예: max=3, remaining=2 → ○○●</summary>
    public void SetActionCount(int remaining, int max) {
        if(remaining <= 0) remaining = 0;
        if(max <= 0) max = 0;
        if(remaining > max) remaining = max;

        EnsureIconCount(max);

        for (int i = 0; i < actionIcons.Count; i++) {
            bool active = i < max;
            var img = actionIcons[i];
            if(img.gameObject.activeSelf != active) img.gameObject.SetActive(active);
            if(!active) continue;

            img.sprite = i < remaining ? remainingSprite : usedSprite;
        }
    }


    // --- 내부 유틸 ---
    private void EnsureIconCount(int count) {
        if(actionIconContainer == null || actionIconPrefab == null) return;

        while(actionIcons.Count < count) {
            var img = Instantiate(actionIconPrefab, actionIconContainer);
            img.gameObject.SetActive(true);
            actionIcons.Add(img);
        }

        for(int i = 0; i < actionIcons.Count; i++) {
            if(i >= count && actionIcons[i].gameObject.activeSelf) actionIcons[i].gameObject.SetActive(false);
        }
    }
}