using TMPro;
using UnityEngine;

public class StageTitleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageTitle;
    public void SetTitle(string title) => stageTitle.text = title;
}
