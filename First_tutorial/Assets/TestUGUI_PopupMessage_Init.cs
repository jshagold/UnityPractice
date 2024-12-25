using UnityEngine;

public class TestUGUI_PopupMessage_Init : MonoBehaviour
{
    [SerializeField] GameObject prefabPopupMessage;
    [SerializeField] Transform parentPopupMessage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdatePopup();
    }

    private void UpdatePopup()
    {
        GameObject gameObject = Instantiate(prefabPopupMessage, parentPopupMessage);
        PopupMessage popupMessage = gameObject.GetComponent<PopupMessage>();
        PopupMessageInfo popupMessageInfo = new PopupMessageInfo(type: POPUP_MESSAGE_TYPE.TWO_BUTTON, title: "Notification Update", contents: "Could you update?");
        popupMessage.OpenMessage(popupMessageInfo, CancelUpdate, GoUpdate);
    }

    private void MaintenanacePopup()
    {
        GameObject gameObject = Instantiate(prefabPopupMessage, parentPopupMessage);
        PopupMessage popupMessage = gameObject.GetComponent<PopupMessage>();
        PopupMessageInfo popupMessageInfo = new PopupMessageInfo(type: POPUP_MESSAGE_TYPE.ONE_BUTTON, title: "Notification Maintenance", contents: "Fixing");
        popupMessage.OpenMessage(popupMessageInfo, null, GoMaintenance);
    }

    private void GoUpdate()
    {
        Debug.Log("GoUpdate");

    }

    private void CancelUpdate()
    {
        Debug.Log("CancelUpdate");
        MaintenanacePopup();
    }

    private void GoMaintenance()
    {
        Debug.Log("GoUpdate");

    }

    private void CancelMaintenance()
    {
        Debug.Log("CancelUpdate");
    }

}
