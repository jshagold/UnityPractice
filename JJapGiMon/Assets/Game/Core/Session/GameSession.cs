using UnityEngine;

public class GameSession : MonoBehaviour
{
    private static GameSession _instance;
    public static GameSession I => _instance ??= Create();

    // �������� �ѱ� ����
    public StageLaunchArgs PendingStageLaunch { get; private set; }
    public void SetStageLaunchArgs(StageLaunchArgs args) => PendingStageLaunch = args;
    public StageLaunchArgs ConsumeStageLaunchArgs() { 
        var x = PendingStageLaunch; 
        PendingStageLaunch = null; 
        return x; 
    }

    private static GameSession Create()
    {
        var go = new GameObject("[GameSession]");
        var s = go.AddComponent<GameSession>();
        DontDestroyOnLoad(go);
        return _instance = s;
    }

    // ���� '�÷��� ���� ��' �����ϰ� �ʱ�ȭ/�缳�� ����
    // 1) ������ ���ε� ���� ȣ���: ���� �ʵ� ���¿�
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _instance = null;
    }

    // 2) ù �� �ε� ���������� ����: �ν��Ͻ� ����
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureBeforeFirstScene()
    {
        if (_instance == null) Create();
    }

}