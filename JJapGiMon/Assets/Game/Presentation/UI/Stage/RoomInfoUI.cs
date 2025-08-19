using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class RoomInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject roomInfoContainer;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private TextMeshProUGUI roomInfoText;


    public void Show(Action OnClickStart, Action OnClickClose, string roomInfo)
    {
        roomInfoContainer.SetActive(true);
        Clear();
        
        var startButton = Instantiate(buttonPrefab, buttonContainer);
        var startlabel = startButton.GetComponentInChildren<TextMeshProUGUI>(true);
        startlabel.text = "시작";
        startButton.onClick.AddListener(() => OnClickStart?.Invoke());
        startButton.gameObject.SetActive(true);

        var closeButton = Instantiate(buttonPrefab, buttonContainer);
        var closeLabel = closeButton.GetComponentInChildren<TextMeshProUGUI>(true);
        closeLabel.text = "닫기";
        closeButton.onClick.AddListener(() => OnClickClose?.Invoke());
        closeButton.gameObject.SetActive(true);

        var roomInfoLabel = Instantiate(roomInfoText, buttonContainer);
        roomInfoLabel.text = roomInfo;
        roomInfoLabel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        roomInfoContainer.SetActive(false);
        Clear();
    }


    private void Clear()
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}