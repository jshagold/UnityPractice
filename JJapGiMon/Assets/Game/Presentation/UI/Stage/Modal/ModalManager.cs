using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공용 Backdrop 1개 + 모달 스택 관리.
/// Show: push + 최상단 정렬 + Backdrop ON
/// Hide: pop  + 비었으면 Backdrop OFF
/// ESC/Back/Backdrop: top만 닫기
/// </summary>
public class ModalManager : MonoBehaviour
{
    [Header("ModalManager")]
    [SerializeField] private GameObject backdrop;

    private readonly Stack<ModalBase> _modalStack = new();

    private void Awake()
    {
        if (backdrop != null) backdrop.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) HideTopIfAllowed();
    }

    public void Show(ModalBase modal)
    {
        if (modal == null) return;
        if(!_modalStack.Contains(modal)) _modalStack.Push(modal);

        modal.transform.SetAsLastSibling();
        if(backdrop != null && !backdrop.activeSelf) backdrop.SetActive(true);

        modal.Show();
    }

    public void Hide(ModalBase modal)
    {
        if(modal == null || !_modalStack.Contains(modal)) return;

        modal.Hide();
        _modalStack.Pop();

        if(_modalStack.Count == 0)
        {
            if(backdrop != null && backdrop.activeSelf) backdrop.SetActive(false);
        }
        else 
        {
            var top = _modalStack.Peek();
            if(top != null) top.transform.SetAsLastSibling();
        }
    }

    public void HideTopIfAllowed()
    {
        if(_modalStack.Count == 0) return;

        var top = _modalStack.Peek();
        if(top.CloseOnBackdrop) Hide(top);
    }
}