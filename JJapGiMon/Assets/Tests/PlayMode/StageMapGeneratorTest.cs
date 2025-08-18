using UnityEngine;
using System.Collections.Generic;

public class StageMapGeneratorTest : MonoBehaviour
{
    [Header("테스트 설정")]
    public int testSeed = 12345;
    public int stageLength = 5;
    public int choicesPerStep = 3;
    public int lastRoomCount = 3;
    
    [Header("테스트 결과")]
    [SerializeField] private string testResult = "";
    
    [ContextMenu("스테이지 맵 생성 테스트")]
    public void TestStageMapGeneration()
    {
        Debug.Log("=== StageMapGenerator 테스트 시작 ===");
        
        // 1. 기본 StageData 생성
        var stageData = new StageData(
            stageId: 1,
            stageName: "테스트 스테이지",
            stageDescription: "StageMapGenerator 테스트용 스테이지",
            stageLength: stageLength,
            choicesPerStep: choicesPerStep,
            randomSeed: testSeed,
            lastRoomCount: lastRoomCount
        );
        
        // 2. StageMapGenerator 생성
        var generator = new StageMapGenerator(stageData);
        
        // 3. 완전한 스테이지 데이터 생성
        var completeStageData = generator.GenerateCompleteStageData();
        
        // 4. 결과 검증
        ValidateStageData(completeStageData);
        
        // 5. 맵 구조 출력
        PrintStageMapStructure(completeStageData);
        
        Debug.Log("=== StageMapGenerator 테스트 완료 ===");
    }
    
    private void ValidateStageData(StageData stageData)
    {
        Debug.Log($"스테이지 검증 시작: {stageData.stageName}");
        
        // 기본 정보 검증
        Debug.Log($"- 스테이지 ID: {stageData.stageId}");
        Debug.Log($"- 스테이지 길이: {stageData.stageLength}");
        Debug.Log($"- 선택 가능한 노드 수: {stageData.choicesPerStep}");
        Debug.Log($"- 보스 방 개수: {stageData.lastRoomCount}");
        
        // 노드 데이터 검증
        if (stageData.allNodes != null)
        {
            Debug.Log($"- 총 노드 수: {stageData.allNodes.Count}");
            
            // 시작 노드 확인
            if (stageData.rootNode != null)
            {
                Debug.Log($"- 시작 노드 ID: {stageData.rootNode.nodeId}");
                Debug.Log($"- 시작 노드 타입: {stageData.rootNode.type}");
                Debug.Log($"- 시작 노드 자식 수: {stageData.rootNode.childNodeIds.Count}");
            }
            
            // 노드 타입별 개수 확인
            var typeCounts = new Dictionary<StageRoomType, int>();
            var goalNodes = new List<StageNodeData>();
            
            foreach (var node in stageData.allNodes)
            {
                if (!typeCounts.ContainsKey(node.type))
                    typeCounts[node.type] = 0;
                typeCounts[node.type]++;
                
                if (node.isGoal)
                    goalNodes.Add(node);
            }
            
            Debug.Log("노드 타입별 개수:");
            foreach (var kvp in typeCounts)
            {
                Debug.Log($"  - {kvp.Key}: {kvp.Value}개");
            }
            
            Debug.Log($"- 목표 노드 수: {goalNodes.Count}");
            
            // 보스 방 개수 검증
            var bossNodes = stageData.allNodes.FindAll(n => n.type == StageRoomType.Boss);
            if (bossNodes.Count != stageData.lastRoomCount)
            {
                Debug.LogError($"보스 방 개수 불일치! 예상: {stageData.lastRoomCount}, 실제: {bossNodes.Count}");
            }
            else
            {
                Debug.Log($"보스 방 개수 검증 성공: {bossNodes.Count}개");
            }
        }
        
        // 접근 가능한 노드 검증
        Debug.Log($"- 현재 접근 가능한 노드 수: {stageData.availableNodeIds.Count}");
        Debug.Log($"- 현재 노드 ID: {stageData.currentNodeId}");
        
        // 노드 맵 초기화 확인
        if (stageData.nodeMap != null)
        {
            Debug.Log($"- 노드 맵 초기화 성공: {stageData.nodeMap.Count}개 노드");
        }
        else
        {
            Debug.LogError("노드 맵이 초기화되지 않았습니다!");
        }
    }
    
    private void PrintStageMapStructure(StageData stageData)
    {
        Debug.Log("=== 스테이지 맵 구조 ===");
        
        if (stageData.rootNode == null)
        {
            Debug.LogError("루트 노드가 없습니다!");
            return;
        }
        
        PrintNodeRecursive(stageData.rootNode, stageData, "");
    }
    
    private void PrintNodeRecursive(StageNodeData node, StageData stageData, string indent)
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
                PrintNodeRecursive(childNode, stageData, indent + "  ");
            }
        }
    }
    
    [ContextMenu("랜덤 시드로 테스트")]
    public void TestWithRandomSeed()
    {
        testSeed = Random.Range(1, 100000);
        Debug.Log($"랜덤 시드로 테스트: {testSeed}");
        TestStageMapGeneration();
    }
    
    [ContextMenu("다양한 설정으로 테스트")]
    public void TestWithDifferentSettings()
    {
        Debug.Log("=== 다양한 설정으로 테스트 ===");
        
        // 테스트 케이스들
        var testCases = new[]
        {
            new { length = 3, choices = 2, rooms = 1, name = "짧은 스테이지" },
            new { length = 5, choices = 3, rooms = 2, name = "보통 스테이지" },
            new { length = 7, choices = 4, rooms = 3, name = "긴 스테이지" },
            new { length = 10, choices = 5, rooms = 5, name = "매우 긴 스테이지" }
        };
        
        foreach (var testCase in testCases)
        {
            Debug.Log($"\n--- {testCase.name} 테스트 ---");
            
            var stageData = new StageData(
                stageId: Random.Range(100, 999),
                stageName: testCase.name,
                stageDescription: $"{testCase.name} 테스트",
                stageLength: testCase.length,
                choicesPerStep: testCase.choices,
                randomSeed: Random.Range(1, 100000),
                lastRoomCount: testCase.rooms
            );
            
            var generator = new StageMapGenerator(stageData);
            var completeStageData = generator.GenerateCompleteStageData();
            
            Debug.Log($"생성된 노드 수: {completeStageData.allNodes.Count}");
            Debug.Log($"보스 방 수: {completeStageData.allNodes.FindAll(n => n.type == StageRoomType.Boss).Count}");
        }
    }
}
