using UnityEngine;

/// <summary>
/// 새로운 스테이지 시스템 사용 예시
/// </summary>
public class StageSystemExample : MonoBehaviour
{
    [Header("스테이지 설정")]
    [SerializeField] private StageData stageData;
    
    [Header("테스트 설정")]
    [SerializeField] private bool createTestStageOnStart = true;
    [SerializeField] private int testSeed = 12345;

    private StageManager stageManager;
    private StageSceneController sceneController;

    void Start()
    {
        // 컴포넌트 참조 가져오기
        stageManager = GetComponent<StageManager>();
        sceneController = GetComponent<StageSceneController>();

        if (createTestStageOnStart)
        {
            CreateAndStartTestStage();
        }
    }

    /// <summary>
    /// 테스트 스테이지 생성 및 시작
    /// </summary>
    [ContextMenu("테스트 스테이지 생성")]
    public void CreateAndStartTestStage()
    {
        // 1. StageData 생성
        var testStageData = new StageData(
            stageId: 1,
            stageName: "테스트 스테이지",
            stageDescription: "새로운 스테이지 시스템 테스트용",
            stageLength: 5,
            choicesPerStep: 3,
            randomSeed: testSeed,
            lastRoomCount: 3
        );

        // 2. StageSceneController를 통해 스테이지 시작
        if (sceneController != null)
        {
            sceneController.StartStageWithData(testStageData);
        }
        else if (stageManager != null)
        {
            stageManager.StartStage(testStageData);
        }

        Debug.Log("테스트 스테이지가 시작되었습니다.");
    }

    /// <summary>
    /// 현재 스테이지 정보 출력
    /// </summary>
    [ContextMenu("스테이지 정보 출력")]
    public void PrintStageInfo()
    {
        if (stageManager == null) return;

        var progress = stageManager.GetProgress();
        var currentNode = stageManager.CurrentNode;
        var availableNodes = stageManager.GetAvailableChildren();

        Debug.Log("=== 현재 스테이지 정보 ===");
        Debug.Log($"현재 노드: {currentNode?.roomName ?? "없음"}");
        Debug.Log($"진행률: {progress.ProgressPercentage:F1}%");
        Debug.Log($"접근 가능한 노드: {availableNodes.Count}개");

        foreach (var node in availableNodes)
        {
            Debug.Log($"  - {node.roomName} ({node.type})");
        }
    }

    /// <summary>
    /// 첫 번째 접근 가능한 노드로 이동
    /// </summary>
    [ContextMenu("첫 번째 노드로 이동")]
    public void MoveToFirstAvailableNode()
    {
        if (sceneController != null)
        {
            bool success = sceneController.MoveToNode(0);
            if (success)
            {
                Debug.Log("첫 번째 노드로 이동했습니다.");
                PrintStageInfo();
            }
            else
            {
                Debug.LogWarning("노드 이동에 실패했습니다.");
            }
        }
    }

    /// <summary>
    /// 새로운 시드로 스테이지 재생성
    /// </summary>
    [ContextMenu("새로운 시드로 재생성")]
    public void RegenerateWithNewSeed()
    {
        int newSeed = Random.Range(1, 10000);
        testSeed = newSeed;
        
        if (sceneController != null)
        {
            sceneController.RegenerateMap(newSeed);
            Debug.Log($"새로운 시드({newSeed})로 스테이지가 재생성되었습니다.");
        }
    }

    /// <summary>
    /// 스테이지 맵 전체 출력
    /// </summary>
    [ContextMenu("스테이지 맵 출력")]
    public void PrintFullStageMap()
    {
        if (stageManager != null)
        {
            stageManager.PrintStageMap();
        }
    }

    /// <summary>
    /// 이벤트 방 처리 예시
    /// </summary>
    public void HandleEventRoomExample()
    {
        var currentNode = stageManager?.CurrentNode;
        if (currentNode?.type == StageRoomType.Event)
        {
            Debug.Log($"이벤트 방 처리: {currentNode.roomName}");
            
            switch (currentNode.eventType)
            {
                case EventRoomType.Rest:
                    Debug.Log("휴식 이벤트 - 체력 회복");
                    break;
                case EventRoomType.Shop:
                    Debug.Log("상점 이벤트 - 아이템 구매");
                    break;
                case EventRoomType.Maintenance:
                    Debug.Log("정비 이벤트 - 장비 강화");
                    break;
                case EventRoomType.Event:
                    Debug.Log("특별 이벤트 - 랜덤 이벤트");
                    break;
            }
        }
    }

    /// <summary>
    /// 전투 방 처리 예시
    /// </summary>
    public void HandleBattleRoomExample()
    {
        var currentNode = stageManager?.CurrentNode;
        if (currentNode?.type == StageRoomType.Battle)
        {
            Debug.Log($"전투 방 처리: {currentNode.roomName}");
            
            switch (currentNode.battleType)
            {
                case BattleRoomType.NormalBattle:
                    Debug.Log("일반 전투 시작");
                    break;
                case BattleRoomType.EliteBattle:
                    Debug.Log("정예 전투 시작");
                    break;
                case BattleRoomType.AmbushBattle:
                    Debug.Log("매복 전투 시작");
                    break;
                case BattleRoomType.ArenaBattle:
                    Debug.Log("아레나 전투 시작");
                    break;
            }
        }
    }
}
