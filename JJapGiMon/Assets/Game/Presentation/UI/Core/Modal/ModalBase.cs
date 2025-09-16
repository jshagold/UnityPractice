using UnityEngine;

public abstract class ModalBase : MonoBehaviour
{

    [Header("ModalBase")]
    [SerializeField] private GameObject panel; // 실제 패널
    [SerializeField] private bool closeOnBackdrop = true; // 백드롭 터치/클릭 시 닫을지 여부

    public bool CloseOnBackdrop => closeOnBackdrop;

    protected virtual void OnShown() { }
    protected virtual void OnHidden() { }

    public void Show()
    {
        if(!gameObject.activeSelf) gameObject.SetActive(true);
        if(panel != null && !panel.activeSelf) panel.SetActive(true);
        transform.SetAsLastSibling();

        OnShown();
    }

    public void Hide()
    {
        if(gameObject.activeSelf) gameObject.SetActive(false);
        if(panel != null && panel.activeSelf) panel.SetActive(false);

        OnHidden();
    }
}