using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// QTE 입력 UI 컴포넌트. Toggle과 완료 버튼으로 구성.
/// </summary>
public class QTEPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform toggleContainer;
    [SerializeField] private Button qteButtonPrefab;

    private Action<List<bool>> onComplete;

    public void Show(int hitCount, Action<List<bool>> callback)
    {
        onComplete = callback;
        panel.SetActive(true);
        Clear();
        var results = Enumerable.Repeat(false, hitCount).ToList();
        for (int i = 0; i < hitCount; i++)
        {
            var qteButton = Instantiate(qteButtonPrefab, toggleContainer);
            qteButton.onClick.AddListener(() => { TabQteButton(); results[i] = true; });
            qteButton.gameObject.SetActive(true);
            //yield return new WaitForSeconds(1.0f);
        }
        Complete(results);
    }

    public void Hide()
    {
        panel.SetActive(false);
        Clear();
    }

    private void Complete(List<bool> results)
    {
        onComplete?.Invoke(results);
    }

    private void Clear()
    {
        for (int i = toggleContainer.childCount - 1; i >= 0; i--)
            Destroy(toggleContainer.GetChild(i).gameObject);
    }

    private void TabQteButton()
    {
        Debug.Log("QTE Tab");
    }
}