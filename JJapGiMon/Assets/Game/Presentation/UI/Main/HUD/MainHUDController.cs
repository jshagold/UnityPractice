

using System;
using UnityEngine;
using UnityEngine.UI;

public class MainHUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button optionButton;
    [SerializeField] private Button actionButton;
    [SerializeField] private ActionCountHUD actionCountHUD;

    [Header("Modal Panels")]
    [SerializeField] private ModalManager modalManager;
    [SerializeField] private MainOptionPanel optionPanelUI;

    private event Action OpenOptionRequested;
    private event Action OnActionRequested;
    
    private void Awake()
    {
        if(modalManager == null)
            modalManager = GetComponent<ModalManager>();

        optionButton.onClick.AddListener(HandleOpenOptionPanel);
        actionButton.onClick.AddListener(HandleOpenActionPanel);
    }

    private void Start()
    {
        optionPanelUI.Hide();


    }



    private void HandleOpenOptionPanel()
    {
        modalManager.Show(optionPanelUI);
        OpenOptionRequested?.Invoke();
    }

    private void HandleOpenActionPanel()
    {
        OnActionRequested?.Invoke();
    }


}