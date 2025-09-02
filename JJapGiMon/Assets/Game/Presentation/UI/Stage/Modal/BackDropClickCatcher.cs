using UnityEngine;
using UnityEngine.EventSystems;

public sealed class BackDropClickCatcher : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ModalManager modalManager;
    public void OnPointerClick(PointerEventData eventData) => modalManager?.HideTopIfAllowed();
}