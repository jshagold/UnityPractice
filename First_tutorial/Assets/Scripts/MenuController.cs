using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Transform parentWindow;
    [SerializeField] private GameObject prefabUpgradeA; 
    [SerializeField] private GameObject prefabUpgradeB; 
    [SerializeField] private GameObject prefabShop;

    private WindowManager windowManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        windowManager = FindAnyObjectByType<WindowManager>();
    }

    public void OnClick_UpgradeA()
    {
        windowManager.ChangeWindow(prefabUpgradeA, parentWindow);
    }

    public void OnClick_UpgradeB()
    {
        windowManager.ChangeWindow(prefabUpgradeB, parentWindow);
    }

    public void OnClick_Shop()
    {
        windowManager.ChangeWindow(prefabShop, parentWindow);
    }
}
