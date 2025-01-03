using UnityEngine;

public class SystemManager : ManagerBase
{
    private static SystemManager instance;

    public static SystemManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("SystemManager").AddComponent<SystemManager>();
            }

            return instance;
        }
    }

    public bool IsInit { get; set; } = false;

    public string ApiUrl { get; set; } = string.Empty;

    private void Awake()
    {
        DontDestroy<SystemManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
    public void SetInit()
    {
        IsInit = true;
    }
}
