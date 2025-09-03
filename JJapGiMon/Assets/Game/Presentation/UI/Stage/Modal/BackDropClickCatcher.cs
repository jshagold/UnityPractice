using UnityEngine;
using UnityEngine.EventSystems;

public sealed class BackDropClickCatcher : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ModalManager modalManager;

    private void Awake()
    {
        if(modalManager == null)
            Debug.LogError("BackDropClickCatcher modalManager is null");
    }

    public void OnPointerClick(PointerEventData eventData) => modalManager?.HideTopIfAllowed();
    
}