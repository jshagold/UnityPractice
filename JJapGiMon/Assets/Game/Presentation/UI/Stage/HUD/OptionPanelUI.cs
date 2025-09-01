using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class OptionPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject optionButtonContainer;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    public Action OnClickSetting;
    public Action OnClickSaveAndExit;
    public Action OnClickGiveUp;


    public void Show()
    {

        optionButtonContainer.SetActive(true);
        Clear();
        
        var optionButton = Instantiate(buttonPrefab, buttonContainer);
        var optionlabel = optionButton.GetComponentInChildren<TextMeshProUGUI>(true);
        optionlabel.text = "설정";
        optionButton.onClick.AddListener(() => OnClickSetting?.Invoke());
        optionButton.gameObject.SetActive(true);

        var saveAndExitButton = Instantiate(buttonPrefab, buttonContainer);
        var saveAndExitLabel = saveAndExitButton.GetComponentInChildren<TextMeshProUGUI>(true);
        saveAndExitLabel.text = "저장 후 메인화면으로";
        saveAndExitButton.onClick.AddListener(() => OnClickSaveAndExit?.Invoke());
        saveAndExitButton.gameObject.SetActive(true);

        var giveUpButton = Instantiate(buttonPrefab, buttonContainer);
        var giveUpLabel = giveUpButton.GetComponentInChildren<TextMeshProUGUI>(true);
        giveUpLabel.text = "포기";
        giveUpButton.onClick.AddListener(() => OnClickGiveUp?.Invoke());
        giveUpButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        optionButtonContainer.SetActive(false);
        Clear();
    }


    private void Clear()
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}