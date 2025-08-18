using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class StageMapGeneratorTest : MonoBehaviour
{
    [Header("테스트 설정")]
    public int testSeed = 12345;
    public int stageLength = 5;
    public int MinNodeCountByDepth = 1;
    public int MaxNodeCountByDepth = 3;
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
            MinNodeCountByDepth: MinNodeCountByDepth,
            MaxNodeCountByDepth: MaxNodeCountByDepth,
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
        Debug.Log($"- 최소 노드 수: {stageData.MinNodeCountByDepth}");
        Debug.Log($"- 최대 노드 수: {stageData.MaxNodeCountByDepth}");
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
                Debug.Log($"  - {kvp.Key}: {kvp.Value}개");
            }
            
            Debug.Log("노드 상태별 개수:");
            foreach (var kvp in stateCounts)
            {
                Debug.Log($"  - {kvp.Key}: {kvp.Value}개");
            }
            
            Debug.Log($"- 성공 노드 수: {successNodes.Count}");
            
            // 새로운 규칙 검증
            var failNodes = stageData.allNodes.FindAll(n => n.state == StageStateType.FAIL);
            var neutralNodes = stageData.allNodes.FindAll(n => n.state == StageStateType.NEUTRAL);
            var bossNodes = stageData.allNodes.FindAll(n => n.type == StageRoomType.Boss);
            
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
        
                 // 연결되지 않은 노드 검증
         ValidateNodeConnections(stageData);
         
         // 간선 수 제한 검증
         ValidateEdgeCountLimits(stageData);
    }
    
    private void PrintStageMapStructure(StageData stageData)
    {
        Debug.Log("=== 스테이지 맵 구조 ===");
        
        // 스테이지 설정 정보 출력
        Debug.Log($"설정: 길이={stageData.stageLength}, 노드수={stageData.MinNodeCountByDepth}~{stageData.MaxNodeCountByDepth}, 마지막방={stageData.lastRoomCount}");
        
        if (stageData.rootNode == null)
        {
            Debug.LogError("루트 노드가 없습니다!");
            return;
        }
        
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
    
    private void PrintNodeRecursive(StageNodeData node, StageData stageData, string indent, bool isLast = true)
    {
        // 간결한 노드 정보 표시
        string nodeInfo = GetNodeDisplayInfo(node, stageData);
        Debug.Log($"{indent}{nodeInfo}");
        
        var children = new List<StageNodeData>();
        foreach (var childId in node.childNodeIds)
        {
            var childNode = stageData.GetNodeById(childId);
            if (childNode != null)
            {
                children.Add(childNode);
            }
        }
        
        for (int i = 0; i < children.Count; i++)
        {
            bool isLastChild = (i == children.Count - 1);
            string childIndent = indent + (isLast ? "    " : "│   ");
            string connector = isLastChild ? "└── " : "├── ";
            
            string childInfo = GetNodeDisplayInfo(children[i], stageData);
            Debug.Log($"{childIndent}{connector}{childInfo}");
            
            if (children[i].childNodeIds.Count > 0)
            {
                PrintNodeRecursive(children[i], stageData, childIndent + (isLastChild ? "    " : "│   "), isLastChild);
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
    
    [ContextMenu("랜덤 시드로 테스트")]
    public void TestWithRandomSeed()
    {
        testSeed = UnityEngine.Random.Range(1, 100000);
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
                 stageId: UnityEngine.Random.Range(100, 999),
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
            
            Debug.Log($"생성된 노드 수: {completeStageData.allNodes.Count}");
            Debug.Log($"보스 방 수: {completeStageData.allNodes.FindAll(n => n.type == StageRoomType.Boss).Count}");
            Debug.Log($"성공 노드 수: {completeStageData.allNodes.FindAll(n => n.state == StageStateType.SUCCESS).Count}");
            Debug.Log($"실패 노드 수: {completeStageData.allNodes.FindAll(n => n.state == StageStateType.FAIL).Count}");
            Debug.Log($"중립 노드 수: {completeStageData.allNodes.FindAll(n => n.state == StageStateType.NEUTRAL).Count}");
        }
    }
}
