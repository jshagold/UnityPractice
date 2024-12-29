using UnityEngine;

public class SystemManager : ManagerBase
{

    public bool IsInit { get; set; } = false;

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
