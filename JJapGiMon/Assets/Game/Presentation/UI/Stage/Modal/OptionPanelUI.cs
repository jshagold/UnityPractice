using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class OptionPanelUI : ModalBase
{
    [SerializeField] private GameObject optionButtonContainer;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Button BtnClose;

    [Header("Manager Hook")]
    [SerializeField] private ModalManager modalManager;


    public event Action OnClickSetting;
    public event Action OnClickSaveAndExit;
    public event Action OnClickGiveUp;


    private void OnEnable()
    {
        if (BtnClose != null) BtnClose.onClick.AddListener(HandleCloseClicked);
    }

    private void OnDisable()
    {
        if (BtnClose != null) BtnClose.onClick.RemoveListener(HandleCloseClicked);
        Clear();
    }

    protected override void OnShown()
    {
        if(optionButtonContainer != null) optionButtonContainer.SetActive(true);
        RebuildButtons();
    }

    protected override void OnHidden()
    {
        Clear();
        if(optionButtonContainer != null) optionButtonContainer.SetActive(false);
    }

    private void RebuildButtons()
    {
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

        var closeButton = Instantiate(BtnClose, buttonContainer);
        closeButton.onClick.AddListener(() => HandleCloseClicked());
        closeButton.gameObject.SetActive(true);
    }

    private void HandleCloseClicked()
    {
        if(modalManager != null) modalManager.Hide(this);
        else base.Hide();
    }

    private void Clear()
    {
        if (buttonContainer == null) return;
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}