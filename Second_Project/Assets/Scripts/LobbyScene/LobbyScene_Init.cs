using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScene_Init : MonoBehaviour
{
    private void Awake()
    {
        if (!SystemManager.Instance.IsInit)
        {
            SceneLoadManager.Instance.GoInitAndReturnScene(SCENE_TYPE.Lobby);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SCENE_TYPE.Init.ToString());
    }

    
}
