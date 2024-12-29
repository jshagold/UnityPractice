using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScene_Init : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SCENE_TYPE.Init.ToString());
    }

    
}
