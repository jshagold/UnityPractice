using UnityEngine;

public class GameSession : MonoBehaviour
{
    private static GameSession _instance;
    public static GameSession I => _instance ??= Create();

    // 세션으로 넘길 값들
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

    // ── '플레이 시작 시' 안전하게 초기화/재설정 ──
    // 1) 도메인 리로드 꺼도 호출됨: 정적 필드 리셋용
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _instance = null;
    }

    // 2) 첫 씬 로드 ‘이전’에 실행: 인스턴스 보장
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureBeforeFirstScene()
    {
        if (_instance == null) Create();
    }

}