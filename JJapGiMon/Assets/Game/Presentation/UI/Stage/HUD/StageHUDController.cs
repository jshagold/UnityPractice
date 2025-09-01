using System;
using UnityEngine;

public class StageHUDController : MonoBehaviour
{
    [SerializeField] private StageTitleUI stageTitleUI;
    [SerializeField] private OptionButtonUI optionButtonUI;
    [SerializeField] private OptionPanelUI optionPanelUI;

    
    public Action OnOpenOption; // 옵션 창
    public Action OnSaveAndExitStage; // 저장 후 메인화면으로
    public Action OnGiveUpStage; // 스테이지 포기
    public Action OnClickSetting; // 게임 설정 창

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        optionButtonUI.OnClicked += OnOpenOption;
        optionPanelUI.OnClickSaveAndExit += () => OnSaveAndExitStage?.Invoke();
        optionPanelUI.OnClickGiveUp += () => OnGiveUpStage?.Invoke();
        optionPanelUI.OnClickSetting += () => OnClickSetting?.Invoke();
    }
    
    private void OnDisable()
    {
        optionButtonUI.OnClicked -= OnOpenOption;
        optionPanelUI.OnClickSaveAndExit -= () => OnSaveAndExitStage?.Invoke();
        optionPanelUI.OnClickGiveUp -= () => OnGiveUpStage?.Invoke();
        optionPanelUI.OnClickSetting -= () => OnClickSetting?.Invoke();
    }

    public void SetStageTitle(string title) => stageTitleUI.SetTitle(title);
}
