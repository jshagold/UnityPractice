using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class ResultDialogUi : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI resultText;

    public void Show()
    {
        panel.SetActive(true);

        resultText.text = "!!! Battle End !!!";
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}