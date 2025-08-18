using UnityEngine;
using UnityEditor;

public class StageMapGeneratorTestRunner : EditorWindow
{
    [MenuItem("Tools/StageMapGenerator Test")]
    public static void ShowWindow()
    {
        GetWindow<StageMapGeneratorTestRunner>("StageMapGenerator Test");
    }

    private void OnGUI()
    {
        GUILayout.Label("StageMapGenerator 테스트", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("기본 테스트 실행"))
        {
            RunBasicTest();
        }
        
        if (GUILayout.Button("랜덤 시드 테스트"))
        {
            RunRandomSeedTest();
        }
        
        if (GUILayout.Button("다양한 설정 테스트"))
        {
            RunMultipleSettingsTest();
        }
        
        if (GUILayout.Button("모든 테스트 실행"))
        {
            RunAllTests();
        }
    }
    
    private void RunBasicTest()
    {
        Debug.Log("=== 기본 테스트 시작 ===");
        
        var stageData = new StageData(
            stageId: 1,
            stageName: "기본 테스트 스테이지",
            stageDescription: "StageMapGenerator 기본 테스트",
            stageLength: 5,
            choicesPerStep: 3,
            randomSeed: 12345,
            lastRoomCount: 3
        );
        
        var generator = new StageMapGenerator(stageData);
        var completeStageData = generator.GenerateCompleteStageData();
        
        ValidateAndPrintResults(completeStageData, "기본 테스트");
    }
    
    private void RunRandomSeedTest()
    {
        Debug.Log("=== 랜덤 시드 테스트 시작 ===");
        
        for (int i = 0; i < 5; i++)
        {
            int seed = Random.Range(1, 100000);
            
            var stageData = new StageData(
                stageId: 100 + i,
                stageName: $"랜덤 테스트 {i + 1}",
                stageDescription: $"랜덤 시드 {seed} 테스트",
                stageLength: 5,
                choicesPerStep: 3,
                randomSeed: seed,
                lastRoomCount: 3
            );
            
            var generator = new StageMapGenerator(stageData);
            var completeStageData = generator.GenerateCompleteStageData();
            
            ValidateAndPrintResults(completeStageData, $"랜덤 테스트 {i + 1} (시드: {seed})");
        }
    }
    
    private void RunMultipleSettingsTest()
    {
        Debug.Log("=== 다양한 설정 테스트 시작 ===");
        
        var testCases = new[]
        {
            new { length = 3, choices = 2, rooms = 1, name = "짧은 스테이지" },
            new { length = 5, choices = 3, rooms = 2, name = "보통 스테이지" },
            new { length = 7, choices = 4, rooms = 3, name = "긴 스테이지" },
            new { length = 10, choices = 5, rooms = 5, name = "매우 긴 스테이지" }
        };
        
        foreach (var testCase in testCases)
        {
            var stageData = new StageData(
                stageId: Random.Range(200, 999),
                stageName: testCase.name,
                stageDescription: $"{testCase.name} 테스트",
                stageLength: testCase.length,
                choicesPerStep: testCase.choices,
                randomSeed: Random.Range(1, 100000),
                lastRoomCount: testCase.rooms
            );
            
            var generator = new StageMapGenerator(stageData);
            var completeStageData = generator.GenerateCompleteStageData();
            
            ValidateAndPrintResults(completeStageData, testCase.name);
        }
    }
    
    private void RunAllTests()
    {
        RunBasicTest();
        RunRandomSeedTest();
        RunMultipleSettingsTest();
        
        Debug.Log("=== 모든 테스트 완료 ===");
    }
    
    private void ValidateAndPrintResults(StageData stageData, string testName)
    {
        Debug.Log($"\n--- {testName} 결과 ---");
        
        // 기본 정보 출력
        Debug.Log($"스테이지: {stageData.stageName}");
        Debug.Log($"총 노드 수: {stageData.allNodes?.Count ?? 0}");
        
        // 노드 타입별 개수
        if (stageData.allNodes != null)
        {
            var typeCounts = new System.Collections.Generic.Dictionary<StageRoomType, int>();
            var bossNodes = new System.Collections.Generic.List<StageNodeData>();
            
            foreach (var node in stageData.allNodes)
            {
                if (!typeCounts.ContainsKey(node.type))
                    typeCounts[node.type] = 0;
                typeCounts[node.type]++;
                
                if (node.isGoal)
                    bossNodes.Add(node);
            }
            
            Debug.Log("노드 타입별 개수:");
            foreach (var kvp in typeCounts)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value}개");
            }
            
            // 보스 방 검증
            if (bossNodes.Count == stageData.lastRoomCount)
            {
                Debug.Log($"✅ 보스 방 개수 검증 성공: {bossNodes.Count}개");
            }
            else
            {
                Debug.LogError($"❌ 보스 방 개수 불일치! 예상: {stageData.lastRoomCount}, 실제: {bossNodes.Count}");
            }
            
            // 맵 구조 출력
            if (stageData.rootNode != null)
            {
                Debug.Log("맵 구조:");
                PrintNodeStructure(stageData.rootNode, stageData, "");
            }
        }
        
        // 노드 맵 초기화 확인
        if (stageData.nodeMap != null)
        {
            Debug.Log($"✅ 노드 맵 초기화 성공: {stageData.nodeMap.Count}개 노드");
        }
        else
        {
            Debug.LogError("❌ 노드 맵이 초기화되지 않았습니다!");
        }
    }
    
    private void PrintNodeStructure(StageNodeData node, StageData stageData, string indent)
    {
        string status = "";
        if (stageData.visitedNodeIds.Contains(node.nodeId))
            status = " [방문됨]";
        else if (stageData.availableNodeIds.Contains(node.nodeId))
            status = " [접근가능]";
        else
            status = " [잠김]";
            
        string goalMark = node.isGoal ? " [목표]" : "";
        
        Debug.Log($"{indent}{node.type} (ID: {node.nodeId}, 깊이: {node.depth}){status}{goalMark}");
        
        foreach (var childId in node.childNodeIds)
        {
            var childNode = stageData.GetNodeById(childId);
            if (childNode != null)
            {
                PrintNodeStructure(childNode, stageData, indent + "  ");
            }
        }
    }
}
