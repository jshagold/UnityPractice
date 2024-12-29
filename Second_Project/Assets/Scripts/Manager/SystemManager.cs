using UnityEngine;

public class SystemManager : MonoBehaviour
{

    public bool IsInit { get; set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
    public void SetInit()
    {
        IsInit = true;
    }
}
