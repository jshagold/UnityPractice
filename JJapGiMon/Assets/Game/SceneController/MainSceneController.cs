using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] private GameObject playerContainer;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private MainHUDController mainHUDController;


    private void Awake()
    {
    }

    private void OnEnable()
    {
        mainHUDController.OnActionRequested += HandleOpenActionPanel;
        mainHUDController.OpenOptionRequested += HandleOpenOptionPanel;
    }

    private void OnDisable()
    {
        mainHUDController.OnActionRequested -= HandleOpenActionPanel;
        mainHUDController.OpenOptionRequested -= HandleOpenOptionPanel;
    }

    public void Start()
    {
        Show();
    }

    // --- Handler ---
    private void HandleOpenActionPanel()
    {
        throw new NotImplementedException();
    }

    private void HandleOpenOptionPanel()
    {
        throw new NotImplementedException();
    }

    //



    public void Show()
    {
        playerContainer.SetActive(true);
        Clear();

        var dailyArgs = new StageLaunchArgs
        {
            StageId = 1,
            StageName = "Daily Stage",
            ContentId = "daily_stage_1",
            Seed = null,
            PartyCharacterIds = new string[] { "1", "2", "3" },
        };
        var questArgs = new StageLaunchArgs
        {
            StageId = 2,
            StageName = "Quest Stage",
            ContentId = "quest_stage_1",
            Seed = 1001,
            PartyCharacterIds = new string[] { "1", "2", "3" },
        };

        var dailyBtn = Instantiate(buttonPrefab, buttonContainer);
        var dailyLabel = dailyBtn.GetComponentInChildren<TextMeshProUGUI>(true);
        dailyLabel.text = "Daily Stage";
        dailyBtn.onClick.AddListener(() => SendDataToStageScene(dailyArgs));
        dailyBtn.gameObject.SetActive(true);

        var questBtn = Instantiate(buttonPrefab, buttonContainer);
        var questLabel = questBtn.GetComponentInChildren<TextMeshProUGUI>(true);
        questLabel.text = "Quest Stage";
        questBtn.onClick.AddListener(() => SendDataToStageScene(questArgs));
        questBtn.gameObject.SetActive(true);
    }

    public void SendDataToStageScene(StageLaunchArgs args)
    {

        GameSession.I.Set<StageLaunchArgs>(args);

        SceneManager.LoadScene("StageScene");
    }

    private void Clear()
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}