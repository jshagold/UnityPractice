using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] private GameObject playerContainer;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;


    private void Awake()
    {
    }

    public void Start()
    {
        Show();
    }

    public void Show()
    {
        playerContainer.SetActive(true);
        Clear();

        var dailyArgs = new StageLaunchArgs
        {
            StageId = 1,
            ContentId = "daily_stage_1",
            Seed = 1000,
            PartyCharacterIds = new string[] { "1", "2", "3" },
        };
        var questArgs = new StageLaunchArgs
        {
            StageId = 2,
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