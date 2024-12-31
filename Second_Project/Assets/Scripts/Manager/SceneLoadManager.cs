using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : ManagerBase
{
    public SCENE_TYPE AfterSceneType { get; set; }


    private void Awake()
    {
        DontDestroy<SceneLoadManager>();
    }

    public void SetInit()
    {
    }

    public void SceneLoad(SCENE_TYPE sceneType)
    {
        this.AfterSceneType = sceneType;
        SceneManager.LoadScene(SCENE_TYPE.Loading.ToString());
    }
}
