using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene_UI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text textPercent;

    public void SetPercent(float factor) // 0~1
    {
        slider.value = factor;
        textPercent.text = $"{factor * 100}%";
    }
}
