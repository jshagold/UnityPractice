using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitScene_Init : MonoBehaviour
{
    private static bool isInit = false;


    private int PROGRESS_VALUE = 5;
    private int progressAddValue = 0;


    private ObjectPoolManager objectPoolManager; // cache
    private EffectManager effectManager; // cache
    private SoundManager soundManager; // cache
    private WindowManager windowManager; // cache
    private NetworkManager networkManager; // cache

    private InitScene_UI initSceneUI; // cache


    private void Awake()
    {
        initSceneUI = FindAnyObjectByType<InitScene_UI>();
     
        if (!isInit)
        {
            isInit = true;
            objectPoolManager = new GameObject("ObjectPoolManager").AddComponent<ObjectPoolManager>();
            effectManager = new GameObject("EffectManager").AddComponent<EffectManager>();
            soundManager = new GameObject("SoundManager").AddComponent<SoundManager>();
            windowManager = new GameObject("WindowManager").AddComponent<WindowManager>();
            networkManager = new GameObject("NetworkManager").AddComponent<NetworkManager>();
        }
        else
        {
            objectPoolManager = FindAnyObjectByType<ObjectPoolManager>();
            effectManager = FindAnyObjectByType<EffectManager>();
            soundManager = FindAnyObjectByType<SoundManager>();
            windowManager = FindAnyObjectByType<WindowManager>();
            networkManager = FindAnyObjectByType<NetworkManager>();
        }
    }

    private IEnumerator Start()
    {
        yield return null;

        StartCoroutine(C_Manager());
    }

    private IEnumerator C_Manager()
    {
        List<Action> actions = new List<Action>
        {
            SystemManagerInit,
            ObjectPoolManagerInit,
            EffectManagerInit,
            SoundManagerInit,
            WindowManagerInit,
            NetworkManagerInit,
            SceneLoadManagerInit,
            LoadScene,
        };
        PROGRESS_VALUE = actions.Count;

        foreach (var action in actions)
        {
            yield return new WaitForSeconds(0.1f);
            action?.Invoke();
            SetProgress();
        }
    }

    private void SetProgress()
    {
        initSceneUI.SetPercent((float)++progressAddValue / (float)PROGRESS_VALUE);
    }

    private void SystemManagerInit()
    {
        SystemManager.Instance.SetInit();
    }

    private void ObjectPoolManagerInit()
    {
        objectPoolManager.SetInit();

    }

    private void EffectManagerInit()
    {
        effectManager.SetInit();
    }

    private void SoundManagerInit()
    {
        soundManager.SetInit();
    }

    private void WindowManagerInit()
    {
        windowManager.SetInit();
    }

    private void NetworkManagerInit()
    {
        networkManager.SetInit(apiUrl: Config.SERVER_API_URL);

        ApplicationConfigSendPacket applicationConfigSendPacket
            = new ApplicationConfigSendPacket(
                PacketName.ApplicationConfig,
                Config.E_ENVIRONMENT_TYPE,
                ApplicationConfigSendPacket.E_OS_TYPE,
                Config.APP_VERSION
                );

        networkManager.SendPacket(applicationConfigSendPacket);
    }

    private void SceneLoadManagerInit()
    {
        SceneLoadManager.Instance.SetInit();
    }

    private void LoadScene()
    {
        SceneLoadManager.Instance.SceneLoad(SceneLoadManager.Instance.InitSceneType);
    }
}







