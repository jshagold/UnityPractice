using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

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
             MinNodeCountByDepth: 2,
             MaxNodeCountByDepth: 3,
             randomSeed: 1,
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
            int seed = UnityEngine.Random.Range(1, 100000);
            
                         var stageData = new StageData(
                 stageId: 100 + i,
                 stageName: $"랜덤 테스트 {i + 1}",
                 stageDescription: $"랜덤 시드 {seed} 테스트",
                 stageLength: 5,
                 MinNodeCountByDepth: 1,
                 MaxNodeCountByDepth: 3,
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
                 stageId: UnityEngine.Random.Range(200, 999),
                 stageName: testCase.name,
                 stageDescription: $"{testCase.name} 테스트",
                 stageLength: testCase.length,
                 MinNodeCountByDepth: 1,
                 MaxNodeCountByDepth: testCase.choices,
                 randomSeed: UnityEngine.Random.Range(1, 100000),
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
            var typeCounts = new Dictionary<StageRoomType, int>();
            var stateCounts = new Dictionary<StageStateType, int>();
            var successNodes = new List<StageNodeData>();
            
            foreach (var node in stageData.allNodes)
            {
                if (!typeCounts.ContainsKey(node.type))
                    typeCounts[node.type] = 0;
                typeCounts[node.type]++;
                
                if (!stateCounts.ContainsKey(node.state))
                    stateCounts[node.state] = 0;
                stateCounts[node.state]++;
                
                if (node.state == StageStateType.SUCCESS)
                    successNodes.Add(node);
            }
            
            Debug.Log("노드 타입별 개수:");
            foreach (var kvp in typeCounts)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value}개");
            }
            
            Debug.Log("노드 상태별 개수:");
            foreach (var kvp in stateCounts)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value}개");
            }
            
            // 새로운 규칙 검증
            var failNodes = new List<StageNodeData>();
            var neutralNodes = new List<StageNodeData>();
            var bossNodes = new List<StageNodeData>();
            
            foreach (var node in stageData.allNodes)
            {
                if (node.state == StageStateType.FAIL)
                    failNodes.Add(node);
                else if (node.state == StageStateType.NEUTRAL)
                    neutralNodes.Add(node);
                
                if (node.type == StageRoomType.Boss)
                    bossNodes.Add(node);
            }
            
            // 성공 노드는 1개만 있어야 함
            if (successNodes.Count == 1)
            {
                Debug.Log($"✅ 성공 노드 개수 검증 성공: {successNodes.Count}개");
            }
            else
            {
                Debug.LogError($"❌ 성공 노드 개수 불일치! 예상: 1개, 실제: {successNodes.Count}개");
            }
            
            // 실패 노드는 lastRoomCount - 1개 있어야 함
            if (failNodes.Count == stageData.lastRoomCount - 1)
            {
                Debug.Log($"✅ 실패 노드 개수 검증 성공: {failNodes.Count}개");
            }
            else
            {
                Debug.LogError($"❌ 실패 노드 개수 불일치! 예상: {stageData.lastRoomCount - 1}개, 실제: {failNodes.Count}개");
            }
            
            // 보스 방은 1개만 있어야 함
            if (bossNodes.Count == 1)
            {
                Debug.Log($"✅ 보스 방 개수 검증 성공: {bossNodes.Count}개");
                // 보스방의 위치 정보 출력
                var bossNode = bossNodes[0];
                Debug.Log($"📍 보스방 위치: 깊이 {bossNode.depth}, 인덱스 {bossNode.index}");
            }
            else
            {
                Debug.LogError($"❌ 보스 방 개수 불일치! 예상: 1개, 실제: {bossNodes.Count}개");
            }
            
            // 맵 구조 출력
            if (stageData.rootNode != null)
            {
                Debug.Log("맵 구조:");
                PrintStageMapStructure(stageData);
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
         
         // 연결되지 않은 노드 검증
         ValidateNodeConnections(stageData);
         
         // 간선 수 제한 검증
         ValidateEdgeCountLimits(stageData);
    }
    
    private void PrintStageMapStructure(StageData stageData)
    {
        // 스테이지 설정 정보 출력
        Debug.Log($"설정: 길이={stageData.stageLength}, 노드수={stageData.MinNodeCountByDepth}~{stageData.MaxNodeCountByDepth}, 마지막방={stageData.lastRoomCount}");
        
        // 깊이별로 노드들을 정리
        var nodesByDepth = new Dictionary<int, List<StageNodeData>>();
        
        foreach (var node in stageData.allNodes)
        {
            if (!nodesByDepth.ContainsKey(node.depth))
                nodesByDepth[node.depth] = new List<StageNodeData>();
            nodesByDepth[node.depth].Add(node);
        }
        
        // 깊이별로 노드 출력
        for (int depth = 0; depth <= stageData.stageLength - 1; depth++)
        {
            if (nodesByDepth.ContainsKey(depth))
            {
                Debug.Log($"깊이 {depth}: [{string.Join(", ", nodesByDepth[depth].Select(n => GetNodeDisplayInfo(n, stageData)))}]");
            }
        }
        
        // 간선 정보 출력
        Debug.Log("\n=== 간선 정보 ===");
        foreach (var node in stageData.allNodes)
        {
            if (node.childNodeIds.Count > 0)
            {
                var childInfos = node.childNodeIds
                    .Select(childId => stageData.GetNodeById(childId))
                    .Where(child => child != null)
                    .Select(child => GetNodeDisplayInfo(child, stageData));
                
                Debug.Log($"{GetNodeDisplayInfo(node, stageData)} → [{string.Join(", ", childInfos)}]");
            }
        }
    }
    
    private string GetNodeDisplayInfo(StageNodeData node, StageData stageData)
    {
        // 간단한 노드 식별자
        string nodeId = $"N{node.nodeId}";
        
        // 노드 타입에 따른 간단한 표시
        string typeMark = node.type switch
        {
            StageRoomType.Start => "S",
            StageRoomType.Boss => "B",
            StageRoomType.Event => "E",
            StageRoomType.Battle => "F",
            _ => "?"
        };
        
        // 상태 표시
        string stateMark = node.state switch
        {
            StageStateType.SUCCESS => "✓",
            StageStateType.FAIL => "✗",
            StageStateType.NEUTRAL => "-",
            _ => "?"
        };
        
        return $"{nodeId}({typeMark}{stateMark})";
    }
    
    private string GetNodeStatus(StageNodeData node, StageData stageData)
    {
        if (stageData.visitedNodeIds.Contains(node.nodeId))
            return " [방문됨]";
        else if (stageData.availableNodeIds.Contains(node.nodeId))
            return " [접근가능]";
        else
            return " [잠김]";
    }
    
         private string GetNodeStateMark(StageNodeData node)
     {
         return node.state switch
         {
             StageStateType.SUCCESS => " [성공]",
             StageStateType.FAIL => " [실패]",
             StageStateType.NEUTRAL => " [중립]",
             _ => ""
         };
     }
     
     /// <summary>
     /// 노드 연결 상태를 검증합니다.
     /// </summary>
     private void ValidateNodeConnections(StageData stageData)
     {
         Debug.Log("=== 노드 연결 검증 ===");
         
         var isolatedNodes = new List<StageNodeData>();
         var unreachableNodes = new List<StageNodeData>();
         
         // 시작 노드에서 도달할 수 없는 노드들 찾기
         var reachableNodes = new HashSet<int>();
         CollectReachableNodes(stageData.rootNode.nodeId, stageData, reachableNodes);
         
         foreach (var node in stageData.allNodes)
         {
             // 시작 노드에서 도달할 수 없는 노드
             if (!reachableNodes.Contains(node.nodeId))
             {
                 unreachableNodes.Add(node);
             }
             
             // 자식이 없는 노드 (마지막 깊이 제외)
             if (node.childNodeIds.Count == 0 && node.depth < stageData.stageLength - 1)
             {
                 isolatedNodes.Add(node);
             }
         }
         
         // 결과 출력
         if (isolatedNodes.Count == 0)
         {
             Debug.Log("✅ 모든 노드가 자식 노드를 가지고 있습니다.");
         }
         else
         {
             Debug.LogError($"❌ 자식이 없는 노드 {isolatedNodes.Count}개 발견:");
             foreach (var node in isolatedNodes)
             {
                 Debug.LogError($"  - {GetNodeDisplayInfo(node, stageData)} (깊이: {node.depth})");
             }
         }
         
         if (unreachableNodes.Count == 0)
         {
             Debug.Log("✅ 모든 노드가 시작 노드에서 도달 가능합니다.");
         }
         else
         {
             Debug.LogError($"❌ 도달할 수 없는 노드 {unreachableNodes.Count}개 발견:");
             foreach (var node in unreachableNodes)
             {
                 Debug.LogError($"  - {GetNodeDisplayInfo(node, stageData)} (깊이: {node.depth})");
             }
         }
     }
     
     /// <summary>
     /// 시작 노드에서 도달할 수 있는 모든 노드를 수집합니다.
     /// </summary>
     private void CollectReachableNodes(int nodeId, StageData stageData, HashSet<int> reachableNodes)
     {
         if (reachableNodes.Contains(nodeId))
             return;
             
         reachableNodes.Add(nodeId);
         
         var node = stageData.GetNodeById(nodeId);
         if (node != null)
         {
             foreach (var childId in node.childNodeIds)
             {
                 CollectReachableNodes(childId, stageData, reachableNodes);
             }
         }
     }
     
     /// <summary>
     /// 간선 수 제한을 검증합니다.
     /// </summary>
     private void ValidateEdgeCountLimits(StageData stageData)
     {
         Debug.Log("=== 간선 수 제한 검증 ===");
         
         var violations = new List<string>();
         
         // 깊이별로 노드들을 정리
         var nodesByDepth = new Dictionary<int, List<StageNodeData>>();
         foreach (var node in stageData.allNodes)
         {
             if (!nodesByDepth.ContainsKey(node.depth))
                 nodesByDepth[node.depth] = new List<StageNodeData>();
             nodesByDepth[node.depth].Add(node);
         }
         
         // 각 깊이의 노드들에 대해 간선 수 검증
         for (int depth = 0; depth < stageData.stageLength - 1; depth++)
         {
             if (!nodesByDepth.ContainsKey(depth) || !nodesByDepth.ContainsKey(depth + 1))
                 continue;
                 
             var currentNodes = nodesByDepth[depth];
             var nextNodes = nodesByDepth[depth + 1];
             
                           // 현재 깊이의 모든 노드가 가진 간선 수의 총합
              int totalEdgeCount = currentNodes.Sum(n => n.childNodeIds.Count);
              int maxAllowedTotalEdges = nextNodes.Count + 1;
              
              if (totalEdgeCount > maxAllowedTotalEdges)
              {
                  violations.Add($"깊이 {depth}: 총 간선 수 {totalEdgeCount}개 (최대 허용: {maxAllowedTotalEdges}개)");
              }
              
              // 각 노드별 간선 수 정보 출력
              foreach (var currentNode in currentNodes)
              {
                  int edgeCount = currentNode.childNodeIds.Count;
                  Debug.Log($"노드 {GetNodeDisplayInfo(currentNode, stageData)}: 간선 수 {edgeCount}개");
              }
              
              // 깊이별 총 간선 수 정보 출력
              Debug.Log($"깊이 {depth}: 총 간선 수 {totalEdgeCount}개 (최대 허용: {maxAllowedTotalEdges}개)");
         }
         
         // 결과 출력
         if (violations.Count == 0)
         {
             Debug.Log("✅ 모든 노드가 간선 수 제한을 준수합니다.");
         }
         else
         {
             Debug.LogError($"❌ 간선 수 제한 위반 {violations.Count}개 발견:");
             foreach (var violation in violations)
             {
                 Debug.LogError($"  - {violation}");
             }
         }
     }
}
