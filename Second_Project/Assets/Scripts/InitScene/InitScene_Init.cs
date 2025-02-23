using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitScene_Init : MonoBehaviour
{
    [SerializeField] private GameObject prefabPopupMessage;
    [SerializeField] private GameObject prefabPopupInputMessage;
    [SerializeField] private Transform parentPopupMessage;


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

        DevelopmentIdPopup();
    }

    private void DevelopmentIdPopup()
    {
        GameObject objPopupInputMessage = Instantiate(prefabPopupInputMessage, parentPopupMessage);
        PopupMessageInfo popupMessageInfo = new PopupMessageInfo(POPUP_MESSAGE_TYPE.TWO_BUTTON, "Development Id", "put id for test");
        PopupInputMessage popupInputMessage = objPopupInputMessage.GetComponent<PopupInputMessage>();
        popupInputMessage.OpenMessage(popupMessageInfo, SystemManager.Instance.DevelopmentId,
            () =>
            {
                StartCoroutine(C_Manager());
            },
            (string inputFieldValue) =>
            {
                SystemManager.Instance.DevelopmentId = inputFieldValue;
                StartCoroutine(C_Manager());
            }
        );
    }

    private IEnumerator C_Manager()
    {
        NetworkManagerInit();

        IEnumerator enumerator = NetworkManagerInit();
        yield return StartCoroutine(enumerator);
        bool isNetworkManagerSuccess = (bool)enumerator.Current;
        if (!isNetworkManagerSuccess)
        {
            Debug.Log("��Ʈ��ũ ����, popup open");
            GameObject objPopupMessage = Instantiate(prefabPopupMessage, parentPopupMessage);

            PopupMessageInfo popupMessageInfo = new PopupMessageInfo(POPUP_MESSAGE_TYPE.ONE_BUTTON, "��������", "�������� �߻�");
            PopupMessage popupMessage = objPopupMessage.GetComponent<PopupMessage>();
            popupMessage.OpenMessage(popupMessageInfo, null, () =>
            {
                // finish app
                Application.Quit();
            });

            yield break;
        }

        if(SystemManager.Instance.dEVELOPMENT_ID_AUTHORITY == DEVELOPMENT_ID_AUTHORITY.None)
        {
            // ����
            // �ش� ��Ŷ���� ������ �߻��ص�, ��� �����ϵ��� ó���ؾ� �Ѵ�.
            IEnumerator eMaintenance = MaintenancePacket();
            yield return StartCoroutine(eMaintenance);
            MaintenanceReceivePacket maintenanceReceivePacket = eMaintenance.Current as MaintenanceReceivePacket;
            if (maintenanceReceivePacket != null && maintenanceReceivePacket.IsMaintenance)
            {
                GameObject objPopupMessage = Instantiate(prefabPopupMessage, parentPopupMessage);

                PopupMessageInfo popupMessageInfo = new PopupMessageInfo(POPUP_MESSAGE_TYPE.ONE_BUTTON, maintenanceReceivePacket.Title, maintenanceReceivePacket.Contents);
                PopupMessage popupMessage = objPopupMessage.GetComponent<PopupMessage>();
                popupMessage.OpenMessage(popupMessageInfo, null, () =>
                {
                    // finish app
                    Debug.Log("�������� ���� �� ����");
                    Application.Quit();
                });

                yield break;

            }

        }

        yield return StartCoroutine(EtcManager());
    }

    private IEnumerator MaintenancePacket()
    {
        MaintenanceSendPacket sendPacket = new MaintenanceSendPacket(
                SystemManager.Instance.ApiUrl,
                PACKET_NAME_TYPE.Maintenance,
                Config.E_ENVIRONMENT_TYPE,
                Config.E_OS_TYPE,
                Config.APP_VERSION,
                Application.systemLanguage
                );


        IEnumerator enumerator = networkManager.C_SendPacket<MaintenanceReceivePacket>(sendPacket);
        yield return StartCoroutine(enumerator);
        yield return enumerator.Current;
        
    }

    private IEnumerator EtcManager()
    {
        List<Action> actions = new List<Action>
        {
            SystemManagerInit,
            ObjectPoolManagerInit,
            EffectManagerInit,
            SoundManagerInit,
            WindowManagerInit,
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

    private IEnumerator NetworkManagerInit()
    {
        networkManager.SetInit();

        ApplicationConfigSendPacket applicationConfigSendPacket
            = new ApplicationConfigSendPacket(
                Config.SERVER_APP_CONFIG_URL,
                PACKET_NAME_TYPE.ApplicationConfig,
                Config.E_ENVIRONMENT_TYPE,
                Config.E_OS_TYPE,
                Config.APP_VERSION,
                SystemManager.Instance.DevelopmentId
                );


        //networkManager.C_SendPacket<ApplicationConfigReceivePacket>(applicationConfigSendPacket, AppConfig);

        IEnumerator enumerator = networkManager.C_SendPacket<ApplicationConfigReceivePacket>(applicationConfigSendPacket);
        yield return StartCoroutine(enumerator);
        ApplicationConfigReceivePacket receivePacket = enumerator.Current as ApplicationConfigReceivePacket;
        if (receivePacket != null && receivePacket.ReturnCode == (int)RETURN_CODE.Success)
        {
            SystemManager.Instance.ApiUrl = receivePacket.ApiUrl;
            SystemManager.Instance.dEVELOPMENT_ID_AUTHORITY = (DEVELOPMENT_ID_AUTHORITY)receivePacket.DevelopmentIdAuthority;
            yield return true;
        }
        else
        {
            yield return false;
        }
    }

    //private void AppConfig(ReceivePacketBase receivePacketBase)
    //{
    //    ApplicationConfigReceivePacket receivePacket = receivePacketBase as ApplicationConfigReceivePacket;
    //    if (receivePacket != null && receivePacket.ReturnCode == (int)RETURN_CODE.Success)
    //    {
    //        SystemManager.Instance.ApiUrl = receivePacket.ApiUrl;
    //        Debug.Log("����"); // �״��� ������ ���⼭ ����
    //        StartCoroutine(EtcManager());
    //    }
    //    else
    //    {
    //        Debug.Log("����"); // �����˾� ���� ����
    //    }
    //}

    private void SceneLoadManagerInit()
    {
        SceneLoadManager.Instance.SetInit();
    }

    private void LoadScene()
    {
        SceneLoadManager.Instance.SceneLoad(SceneLoadManager.Instance.InitSceneType);
    }
}







