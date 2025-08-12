using UnityEngine;

public class StageTest : MonoBehaviour
{
    [Header("테스트 설정")]
    [SerializeField] private bool runTestOnStart = true;
    [SerializeField] private int testSeed = 12345;

    void Start()
    {
        if (runTestOnStart)
        {
            RunStageTest();
        }
    }

    [ContextMenu("스테이지 테스트 실행")]
    public void RunStageTest()
    {
        Debug.Log("=== 스테이지 시스템 테스트 시작 ===");

        // 1. StageData 생성 테스트
        var stageData = new StageData(
            stageId: 1,
            stageName: "테스트 스테이지",
            stageDescription: "스테이지 시스템 테스트용",
            stageLength: 5,
            choicesPerStep: 3,
            randomSeed: testSeed,
            lastRoomCount: 3
        );

        Debug.Log($"StageData 생성: {stageData}");

        // 2. StageMapGenerator 테스트
        var generator = new StageMapGenerator(stageData);
        var stageMap = generator.GenerateStageMap();

        Debug.Log("스테이지 맵 생성 완료");

        // 3. 노드 정보 출력
        PrintStageMap(stageMap);

        // 4. 특정 깊이의 노드들 확인
        for (int depth = 0; depth <= 6; depth++)
        {
            var nodesAtDepth = generator.GetNodesAtDepth(stageMap, depth);
            Debug.Log($"깊이 {depth}: {nodesAtDepth.Count}개 노드");
            
            foreach (var node in nodesAtDepth)
            {
                Debug.Log($"  - {node}");
            }
        }

        Debug.Log("=== 스테이지 시스템 테스트 완료 ===");
    }

    private void PrintStageMap(StageNode node, string indent = "")
    {
        Debug.Log($"{indent}{node}");
        
        foreach (var child in node.children)
        {
            PrintStageMap(child, indent + "  ");
        }
    }

    [ContextMenu("다른 시드로 테스트")]
    public void TestWithDifferentSeed()
    {
        testSeed = Random.Range(1, 10000);
        Debug.Log($"새로운 시드로 테스트: {testSeed}");
        RunStageTest();
    }
}
