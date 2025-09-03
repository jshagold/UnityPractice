using System;
using UnityEngine;

public class StageHUDController : MonoBehaviour
{
    [SerializeField] private StageTitleUI stageTitleUI;
    [SerializeField] private OptionButtonUI optionButtonUI;

    [Header("Modal Panels")]
    [SerializeField] private ModalManager modalManager;
    [SerializeField] private OptionPanelUI optionPanelUI;
    [SerializeField] private SettingPanelUI settingPanelUI;

    
    public event Action OpenOptionRequested; // 옵션창 열림 알림
    public event Action OnSaveAndExitStage; // 저장 후 메인화면으로
    public event Action OnGiveUpStage; // 스테이지 포기
    public event Action OpenSettingRequested; // 게임 설정 창 열림 알림

    private void Awake()
    {
        if(modalManager == null)
            modalManager = GetComponent<ModalManager>();
    }

    private void OnEnable()
    {
        optionButtonUI.OnClicked += HandleOpenOptionPanel;
        optionPanelUI.OnClickSaveAndExit += HandleSaveAndExitStage;
        optionPanelUI.OnClickGiveUp += HandleGiveUpStage;
        optionPanelUI.OnClickSetting += HandleOpenSettingPanel;
    }
    
    private void OnDisable()
    {
        optionButtonUI.OnClicked -= HandleOpenOptionPanel;
        optionPanelUI.OnClickSaveAndExit -= HandleSaveAndExitStage;
        optionPanelUI.OnClickGiveUp -= HandleGiveUpStage;
        optionPanelUI.OnClickSetting -= HandleOpenSettingPanel;
    }

    private void Start()
    {
        optionPanelUI.Hide();
    }

    public void SetStageTitle(string title) => stageTitleUI.SetTitle(title);


    // --- Handler
    private void HandleOpenOptionPanel()
    {
        modalManager.Show(optionPanelUI);
        OpenOptionRequested?.Invoke();
    }
    
    private void HandleSaveAndExitStage()
    {
        modalManager.Hide(optionPanelUI);
        OnSaveAndExitStage?.Invoke();
    }
    
    private void HandleGiveUpStage()
    {
        modalManager.Hide(optionPanelUI);
        OnGiveUpStage?.Invoke();
    }
    
    private void HandleOpenSettingPanel()
    {
        // todo setting panel 구성하고 열기
        // settingPanelUI.Show();
        OpenSettingRequested?.Invoke();
    }
    
    
}
