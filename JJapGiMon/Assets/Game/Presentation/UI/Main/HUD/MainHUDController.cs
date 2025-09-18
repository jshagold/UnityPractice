

using System;
using UnityEngine;
using UnityEngine.UI;

public class MainHUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button optionButton;
    [SerializeField] private Button nextDayButton;
    [SerializeField] private ActionCountHUD actionCountHUD;

    [Header("Modal Panels")]
    [SerializeField] private ModalManager modalManager;
    [SerializeField] private MainOptionPanel optionPanelUI;

    public event Action OpenOptionRequested;
    public event Action OnNextDayRequested;
    
    private void Awake()
    {
        if(modalManager == null)
            modalManager = GetComponent<ModalManager>();

        if(actionCountHUD == null)
            actionCountHUD = GetComponent<ActionCountHUD>();
            
        optionButton.onClick.AddListener(HandleOpenOptionPanel);
        nextDayButton.onClick.AddListener(HandleOpenActionPanel);
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
        OnNextDayRequested?.Invoke();
    }

    // -------
    public void SetSamsaraLevel(int level) => actionCountHUD.SetSamsaraLevel(level);
    public void SetProgress(int remaining, int max) => actionCountHUD.SetProgress(remaining, max);
    public void SetActionCount(int remaining, int max) => actionCountHUD.SetActionCount(remaining, max);
    public void SetAllActionCountHUD(int level, int progressRemaining, int progressMax, int actionRemaining, int actionMax) {
        SetSamsaraLevel(level);
        SetProgress(progressRemaining, progressMax);
        SetActionCount(actionRemaining, actionMax);
    }

}