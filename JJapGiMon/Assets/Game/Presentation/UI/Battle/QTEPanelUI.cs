using System;
using System.Collections;
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
    [SerializeField] private Button evasionButtonPrefab;

    [Header("Spawn Points")]
    [Tooltip("QTE 패널이 나타날 위치들 (1~3개)")]
    [SerializeField] private Transform[] qteSpawnPoints;

    private Action<List<bool>> onComplete;

    public void Show(CharacterFaction faction, int hitCount, Action<List<bool>> callback)
    {
        Debug.Log($"Show QTE Panel count: {hitCount}");
        onComplete = callback;
        panel.SetActive(true);
        Clear();


        switch (faction)
        {
            case CharacterFaction.Player:
                StartCoroutine(RunPlayerQTE(hitCount));
                break;

            case CharacterFaction.Enemy:
                StartCoroutine(RunEnemyQTE(hitCount));
                break;
        }

    }

    public void Hide()
    {
        panel.SetActive(false);
        Clear();
    }

    private IEnumerator RunPlayerQTE(int hitCount)
    {
        var results = Enumerable.Repeat(false, hitCount).ToList();

        for (int i = 0; i < hitCount; i++)
        {
            int posIndex = i % qteSpawnPoints.Length; 
            Vector3 worldPos = qteSpawnPoints[posIndex].position;
            worldPos.z = 0f;

            bool clicked = false;
            int index = i;
            var btn = Instantiate(qteButtonPrefab, toggleContainer);
            
            // World-Space Canvas 하위에 생성
            btn.transform.position = worldPos;
            btn.transform.rotation = Quaternion.identity;

            btn.gameObject.SetActive(true);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                TabQteButton();
                clicked = true;
                results[index] = true;
                btn.interactable = false;
            });

            float elapsed = 0f;
            while (elapsed < 1f && !clicked)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(btn.gameObject);
        }

        onComplete?.Invoke(results);
    }

    private IEnumerator RunEnemyQTE(int hitCount)
    {
        var results = Enumerable.Repeat(false, hitCount).ToList();

        for (int i = 0; i < hitCount; i++)
        {
            int posIndex = i % qteSpawnPoints.Length;
            Vector3 worldPos = qteSpawnPoints[posIndex].position;
            worldPos.z = 0f;

            bool clicked = false;
            int index = i;
            var btn = Instantiate(evasionButtonPrefab, toggleContainer);

            // World-Space Canvas 하위에 생성
            btn.transform.position = worldPos;
            btn.transform.rotation = Quaternion.identity;

            btn.gameObject.SetActive(true);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                TabQteButton();
                clicked = true;
                results[index] = true;
                btn.interactable = false;
            });

            float elapsed = 0f;
            while (elapsed < 1f && !clicked)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(btn.gameObject);
        }

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