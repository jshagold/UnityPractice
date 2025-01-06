using System;
using TMPro;
using UnityEngine;

public class PopupInputMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentsText;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private GameObject oneButton;
    [SerializeField] private GameObject twoButton;

    private PopupMessageInfo popupMessageInfo; // cache
    private Action cancelAction; // cache
    private Action<string> okAction; // cache

    public void OpenMessage(PopupMessageInfo popupMessageInfo, string value, Action cancelAction, Action<string> okAction)
    {
        this.popupMessageInfo = popupMessageInfo;
        this.cancelAction = cancelAction;
        this.okAction = okAction;

        // set UI
        this.titleText.text = popupMessageInfo.title;
        this.contentsText.text = popupMessageInfo.contents;
        this.inputField.text = value;

        oneButton.SetActive(popupMessageInfo.type == POPUP_MESSAGE_TYPE.ONE_BUTTON);
        twoButton.SetActive(popupMessageInfo.type == POPUP_MESSAGE_TYPE.TWO_BUTTON);
    }

    public void OnClick_Cancel()
    {
        cancelAction?.Invoke();
        Destroy(this.gameObject);
    }

    public void OnClick_Ok()
    {
        okAction?.Invoke(inputField.text);
        Destroy(this.gameObject);
    }

    public void OnClick_OnlyOk()
    {
        okAction?.Invoke(inputField.text);
        Destroy(this.gameObject);
    }
}
