using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoPanelUI : ModalBase
{
    [SerializeField] private GameObject roomInfoContainer;
    [SerializeField] private Transform contentsContainer;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Button BtnClose;
    [SerializeField] private TextMeshProUGUI roomNameTextPrefab;
    [SerializeField] private TextMeshProUGUI roomDescriptionTextPrefab;

    [Header("Manager Hook")]
    [SerializeField] private ModalManager modalManager;


    public event Action OnClickEnter;


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
        if(roomInfoContainer != null) roomInfoContainer.SetActive(true);
        RebuildButtons();
    }

    protected override void OnHidden()
    {
        Clear();
        if(roomInfoContainer != null) roomInfoContainer.SetActive(false);
    }

    private void RebuildButtons()
    {
        Clear();

        var roomNameText = Instantiate(roomNameTextPrefab, contentsContainer);
        roomNameText.text = "방 이름";
        roomNameText.gameObject.SetActive(true);
        
        var roomDescriptionText = Instantiate(roomDescriptionTextPrefab, contentsContainer);
        roomDescriptionText.text = "방 설명";
        roomDescriptionText.gameObject.SetActive(true);

        var enterButton = Instantiate(buttonPrefab, contentsContainer);
        var enterlabel = enterButton.GetComponentInChildren<TextMeshProUGUI>(true);
        enterlabel.text = "입장";
        enterButton.onClick.AddListener(() => OnClickEnter?.Invoke());
        enterButton.gameObject.SetActive(true);

        var closeButton = Instantiate(BtnClose, contentsContainer);
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
        if (contentsContainer == null) return;
        for (int i = contentsContainer.childCount - 1; i >= 0; i--)
            Destroy(contentsContainer.GetChild(i).gameObject);
    }

}