using System;
using TMPro;
using UnityEngine;


public enum POPUP_MESSAGE_TYPE
{
    ONE_BUTTON,
    TWO_BUTTON,
}

public class PopupMessageInfo
{
    public readonly POPUP_MESSAGE_TYPE type;
    public readonly string title;
    public readonly string contents;

    public PopupMessageInfo(POPUP_MESSAGE_TYPE type, string title, string contents)
    {
        this.type = type;
        this.title = title;
        this.contents = contents;
    }
}


public class PopupMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentsText;

    [SerializeField] private GameObject oneButton;
    [SerializeField] private GameObject twoButton;

    private PopupMessageInfo popupMessageInfo; // cache
    private Action cancelAction; // cache
    private Action okAction; // cache
    
    public void OpenMessage(PopupMessageInfo popupMessageInfo, Action cancelAction, Action okAction)
    {
        this.popupMessageInfo = popupMessageInfo;
        this.cancelAction = cancelAction;
        this.okAction = okAction;   

        // set UI
        this.titleText.text = popupMessageInfo.title;
        this.contentsText.text = popupMessageInfo.contents;
        
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
        okAction?.Invoke();
        Destroy(this.gameObject);
    }

    public void OnClick_OnlyOk()
    {
        okAction?.Invoke();
        Destroy(this.gameObject);
    }
}
