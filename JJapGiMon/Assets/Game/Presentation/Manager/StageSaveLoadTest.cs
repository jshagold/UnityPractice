using UnityEngine;

/// <summary>
/// StageManager의 저장/로드 기능을 테스트하는 스크립트 (단일 저장 방식)
/// </summary>
public class StageSaveLoadTest : MonoBehaviour
{
    [Header("테스트 설정")]
    [SerializeField] private bool autoTestOnStart = false;
    
    private StageManager stageManager;
    
    void Start()
    {
        stageManager = FindObjectOfType<StageManager>();
        
        if (stageManager == null)
        {
            Debug.LogError("StageManager를 찾을 수 없습니다.");
            return;
        }
        
        if (autoTestOnStart)
        {
            RunTest();
        }
    }
    
    /// <summary>
    /// 저장/로드 테스트 실행
    /// </summary>
    [ContextMenu("저장/로드 테스트 실행")]
    public void RunTest()
    {
        if (stageManager == null)
        {
            Debug.LogError("StageManager가 없습니다.");
            return;
        }
        
        Debug.Log("=== 스테이지 저장/로드 테스트 시작 ===");
        
        // 1. 저장 데이터 존재 여부 확인
        CheckSaveData();
        
        // 2. 테스트용 스테이지 생성
        CreateTestStage();
        
        // 3. 스테이지 저장
        TestSave();
        
        // 4. 스테이지 로드
        TestLoad();
        
        Debug.Log("=== 스테이지 저장/로드 테스트 완료 ===");
    }
    
    /// <summary>
    /// 저장 데이터 존재 여부 확인
    /// </summary>
    [ContextMenu("저장 데이터 확인")]
    public void CheckSaveData()
    {
        if (stageManager == null) return;
        
        Debug.Log("--- 저장 데이터 확인 ---");
        //bool hasSaveData = stageManager.HasSaveData();
        //Debug.Log($"저장된 데이터 존재: {hasSaveData}");
    }
    
    /// <summary>
    /// 테스트용 스테이지 생성
    /// </summary>
    private void CreateTestStage()
    {
        var testStageData = new StageData(
            stageId: 1,
            stageName: "테스트 스테이지",
            stageDescription: "저장/로드 테스트용 스테이지",
            stageLength: 5,
            MaxNodeCountByDepth: 3,
            randomSeed: 12345,
            lastRoomCount: 2
        );
        
        //stageManager.StartStage(testStageData);
        Debug.Log("테스트용 스테이지 생성 완료");
    }
    
    /// <summary>
    /// 저장 테스트
    /// </summary>
    [ContextMenu("저장 테스트")]
    public void TestSave()
    {
        if (stageManager == null) return;
        
        Debug.Log("--- 저장 테스트 ---");
        stageManager.SaveStage();
    }
    
    /// <summary>
    /// 로드 테스트
    /// </summary>
    [ContextMenu("로드 테스트")]
    public void TestLoad()
    {
        if (stageManager == null) return;
        
        Debug.Log("--- 로드 테스트 ---");
        //bool success = stageManager.LoadStage();
        bool success = false;
        
        if (success)
        {
            Debug.Log("스테이지 로드 성공");
            stageManager.PrintStageMap();
        }
        else
        {
            Debug.Log("스테이지 로드 실패");
        }
    }
    
    /// <summary>
    /// 삭제 테스트
    /// </summary>
    [ContextMenu("삭제 테스트")]
    public void TestDelete()
    {
        if (stageManager == null) return;
        
        Debug.Log("--- 저장 데이터 삭제 테스트 ---");
        //stageManager.DeleteSaveData();
    }
    
    /// <summary>
    /// 현재 스테이지 정보 출력
    /// </summary>
    [ContextMenu("현재 스테이지 정보 출력")]
    public void PrintCurrentStageInfo()
    {
        if (stageManager == null) return;
        
        Debug.Log("--- 현재 스테이지 정보 ---");
        
        var currentData = stageManager.CurrentStageData;
        if (currentData != null)
        {
            Debug.Log($"스테이지 이름: {currentData.stageName}");
            Debug.Log($"스테이지 ID: {currentData.stageId}");
            Debug.Log($"현재 노드 ID: {currentData.currentNodeId}");
            Debug.Log($"방문한 노드 수: {currentData.visitedNodeIds.Count}");
            Debug.Log($"접근 가능한 노드 수: {currentData.availableNodeIds.Count}");
            Debug.Log($"완료 여부: {currentData.isCompleted}");
            Debug.Log($"실패 여부: {currentData.isFailed}");
        }
        else
        {
            Debug.Log("현재 스테이지 데이터가 없습니다.");
        }
    }
    
    /// <summary>
    /// 자동 저장 테스트
    /// </summary>
    [ContextMenu("자동 저장 테스트")]
    public void TestAutoSave()
    {
        if (stageManager == null) return;
        
        Debug.Log("--- 자동 저장 테스트 ---");
        stageManager.AutoSave();
    }
    
    
}
