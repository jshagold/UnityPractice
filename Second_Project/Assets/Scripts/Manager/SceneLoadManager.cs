using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : ManagerBase
{
    private static SceneLoadManager instance;
    public static SceneLoadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("SceneLoadManager").AddComponent<SceneLoadManager>();
            }

            return instance;
        }
    }


    public SCENE_TYPE InitSceneType { get; set; } = SCENE_TYPE.Lobby;
    public SCENE_TYPE AfterSceneType { get; set; }


    private void Awake()
    {
        DontDestroy<SceneLoadManager>();
    }

    public void SetInit()
    {
    }

    public void GoInitAndReturnScene(SCENE_TYPE sceneType)
    {
        this.InitSceneType = sceneType;
        SceneManager.LoadScene(SCENE_TYPE.Init.ToString());
    }

    public void SceneLoad(SCENE_TYPE sceneType)
    {
        this.AfterSceneType = sceneType;
        SceneManager.LoadScene(SCENE_TYPE.Loading.ToString());
    }
}
