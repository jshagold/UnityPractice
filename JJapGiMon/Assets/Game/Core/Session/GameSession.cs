using UnityEngine;
using System.Collections.Generic;

public class GameSession : MonoBehaviour
{
    private static GameSession _instance;
    public static GameSession I => _instance ??= Create();

    // 넘길 값 DTO 등
    public StageLaunchArgs PendingStageLaunch { get; private set; }
    public void SetStageLaunchArgs(StageLaunchArgs args) => PendingStageLaunch = args;
    public StageLaunchArgs ConsumeStageLaunchArgs() { 
        var x = PendingStageLaunch; 
        PendingStageLaunch = null; 
        return x; 
    }

    private readonly Dictionary<System.Type, object> _payloads = new();

    public void Set<T>(T value) where T : class => _payloads[typeof(T)] = value;

    public bool TryConsume<T>(out T value) where T : class
    {
        if (_payloads.TryGetValue(typeof(T), out var obj) && obj is T v)
        {
            value = v; _payloads.Remove(typeof(T));
            return true;
        }
        value = null; return false;
    }


    // ────────────────────────── 싱글톤/수명 보장 ──────────────────────────

    private static GameSession Create()
    {
        var go = new GameObject("[GameSession]");
        var s = go.AddComponent<GameSession>();
        DontDestroyOnLoad(go);
        return _instance = s;
    }

    //  '플레이 시작 시' 안전하게 초기화/재설정
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

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

}