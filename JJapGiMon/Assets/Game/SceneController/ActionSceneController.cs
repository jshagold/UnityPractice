using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionSceneController : MonoBehaviour
{
    [SerializeField] private GameObject playerContainer;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    private bool hasBook = false;
    private bool hasQuest = false;

    private void Start()
    {
        ShowActionPanel();
    }

    public void ShowActionPanel()
    {
        playerContainer.SetActive(true);
        Clear();

        var searchBtn = Instantiate(buttonPrefab, buttonContainer);
        var searchLabel = searchBtn.GetComponentInChildren<TextMeshProUGUI>(true);
        searchLabel.text = "Search";
        searchBtn.onClick.AddListener(() => OnSearch());
        searchBtn.gameObject.SetActive(true);

        var trainingBtn = Instantiate(buttonPrefab, buttonContainer);
        var trainingLabel = trainingBtn.GetComponentInChildren<TextMeshProUGUI>(true);
        trainingLabel.text = "Training";
        trainingBtn.onClick.AddListener(() => OnTraining());
        trainingBtn.gameObject.SetActive(true);

        var restBtn = Instantiate(buttonPrefab, buttonContainer);
        var restLabel = restBtn.GetComponentInChildren<TextMeshProUGUI>(true);
        restLabel.text = "Rest";
        restBtn.onClick.AddListener(() => OnRest());
        restBtn.gameObject.SetActive(true);

        if(hasQuest) {
            var questBtn = Instantiate(buttonPrefab, buttonContainer);
            var questLabel = questBtn.GetComponentInChildren<TextMeshProUGUI>(true);
            questLabel.text = "Quest";
            questBtn.onClick.AddListener(() => OnQuest());
            questBtn.gameObject.SetActive(true);
        }

        if(hasBook) {
            var readBookBtn = Instantiate(buttonPrefab, buttonContainer);
            var readBookLabel = readBookBtn.GetComponentInChildren<TextMeshProUGUI>(true);
            readBookLabel.text = "Read Book";
            readBookBtn.onClick.AddListener(() => OnReadBook());
            readBookBtn.gameObject.SetActive(true);
        }  
    }

    // 탐사
    public void OnSearch() {
        Debug.Log("탐사");
    }

    // 훈련
    public void OnTraining() {
        Debug.Log("훈련");
    }

    // 휴식
    public void OnRest() {
        Debug.Log("휴식");
    }

    // 퀘스트
    public void OnQuest() {
        Debug.Log("퀘스트");
    }

    // 독서
    public void OnReadBook() {
        Debug.Log("독서");
    }


    private void Clear()
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
            Destroy(buttonContainer.GetChild(i).gameObject);
    }
}