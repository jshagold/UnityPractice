using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text textBtn;

    [Header("Slider")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text textSlider;

    [Header("Toggle")]
    [SerializeField] private Toggle toggleA;
    [SerializeField] private Toggle toggleB;

    [Header("InputField")]
    [SerializeField] private TMP_InputField inputField;

    [Header("Dropdown")]
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteB;
    [SerializeField] private Sprite spriteC;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textBtn.text = "Go";

        slider.value = 0.5f;
        slider.interactable = false;
        textSlider.text = "50%";

        Debug.Log(toggleA.isOn);
        //toggleA.isOn = false;

        Debug.Log(inputField.text);

        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData() { text = "a", image = spriteA });
        dropdown.options.Add(new TMP_Dropdown.OptionData() { text = "b", image = spriteB });
        dropdown.options.Add(new TMP_Dropdown.OptionData() { text = "c", image = spriteC });

        dropdown.value = 1; // dropdown 선택하기
    }

    public void OnClick_Button()
    {
        Debug.Log("시작");
    }

    public void OnValueChange_ToggleA(bool isOn)
    {
        Debug.Log("OnValueChange_ToggleA " + isOn);
    }
    public void OnValueChange_ToggleB(bool isOn)
    {
        Debug.Log("OnValueChange_ToggleB " + isOn);
    }

    public void OnValueChange_Dropdown()
    {
        Debug.Log(dropdown.value);
        TMP_Dropdown.OptionData optionData = dropdown.options[dropdown.value];
        Debug.Log(optionData.text);
    }
}
