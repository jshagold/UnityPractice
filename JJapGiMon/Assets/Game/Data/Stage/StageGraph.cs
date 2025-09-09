using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class StageGraph
{
    public StageNodeData rootNodeData;           // 시작 노드 데이터
    public List<StageNodeData> allNodesData;    // 모든 노드 데이터

    public StageNode rootNode => RestoreStageMap(rootNodeData, allNodesData); // 시작 노드

    [NonSerialized]
    [Newtonsoft.Json.JsonIgnore]
    public Dictionary<int, StageNodeData> nodeMap; // 빠른 접근용
    

    public StageGraph()
    {
        allNodesData = new List<StageNodeData>();
    }

    /// <summary>
    /// 노드 맵 초기화
    /// </summary>
    public void InitializeNodeMap()
    {
        if (allNodesData != null && allNodesData.Count > 0)
        {
            nodeMap = allNodesData.ToDictionary(n => n.nodeId);
        }
    }

    public StageNode GetNodeById(int nodeId)
    {
        nodeMap.TryGetValue(nodeId, out var nodeData);

        if (nodeData == null) return null;
        var node = new StageNode(
            nodeId: nodeData.nodeId, 
            depth: nodeData.depth, 
            index: nodeData.index, 
            type: nodeData.type, 
            eventRoomType: nodeData.eventType, 
            battleRoomType: nodeData.battleType, 
            children: nodeData.childNodeIds.Select(id => GetNodeById(id)).ToList(), 
            state: nodeData.state,
            seed: nodeData.seed
        );

        return node;
    }


    /// 저장된 맵 데이터로 StageNode 구조 복원
    private StageNode RestoreStageMap(StageNodeData rootData, List<StageNodeData> allNodes)
    {
        Debug.Log($"RootNode RestoreStageMap: {rootData.nodeId}");

        var nodeMap = allNodes.ToDictionary(n => n.nodeId);
        var restoredNodes = new Dictionary<int, StageNode>(); // 노드 캐싱
        return RestoreNodeRecursive(rootData, nodeMap, restoredNodes);
    }

    private StageNode RestoreNodeRecursive(StageNodeData nodeData, Dictionary<int, StageNodeData> nodeMap, Dictionary<int, StageNode> restoredNodes)
    {
        // 이미 복원된 노드가 있으면 반환
        if (restoredNodes.ContainsKey(nodeData.nodeId))
        {
            return restoredNodes[nodeData.nodeId];
        }

        // StageNode 생성 (런타임 정보는 생성자에서 자동으로 설정됨)
        var node = new StageNode(
            nodeId: nodeData.nodeId, 
            depth: nodeData.depth, 
            index: nodeData.index, 
            type: nodeData.type, 
            eventRoomType: nodeData.eventType, 
            battleRoomType: nodeData.battleType, 
            children: new(), 
            state: nodeData.state,
            seed: nodeData.seed
        );

        // 캐시에 추가
        restoredNodes[nodeData.nodeId] = node;

        // 자식 노드들 복원
        foreach (var childId in nodeData.childNodeIds)
        {
            if (nodeMap.ContainsKey(childId))
            {
                var childNode = RestoreNodeRecursive(nodeMap[childId], nodeMap, restoredNodes);
                node.AddChild(childNode);
            }
        }

        return node;
    }
}