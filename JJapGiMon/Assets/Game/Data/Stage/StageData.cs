using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class StageData
{

    // 🆕 실제 맵 데이터
    public StageNodeData rootNode;          // 시작 
    public List<StageNodeData> allNodes;    // 모든 노드 데이터
    [NonSerialized]
    [Newtonsoft.Json.JsonIgnore]
    public Dictionary<int, StageNodeData> nodeMap; // 빠른 접근용

    // 🆕 스테이지 진행 상태
    public int currentNodeId;               // 현재 위치한 노드 ID
    public List<int> visitedNodeIds;        // 방문한 노드 ID 목록
    public List<int> availableNodeIds;      // 접근 가능한 노드 ID 목록
    public bool isCompleted;                // 스테이지 완료 여부
    public bool isFailed;                   // 스테이지 실패 여부   

    public StageData()
    {
        allNodes = new List<StageNodeData>();
        visitedNodeIds = new List<int>();
        availableNodeIds = new List<int>();
        isCompleted = false;
        isFailed = false;
    }

    /// <summary>
    /// 노드 맵 초기화
    /// </summary>
    public void InitializeNodeMap()
    {
        if (allNodes != null && allNodes.Count > 0)
        {
            nodeMap = allNodes.ToDictionary(n => n.nodeId);
        }
    }

    /// <summary>
    /// 특정 노드 ID로 노드 찾기
    /// </summary>
    public StageNodeData GetNodeById(int nodeId)
    {
        if (nodeMap != null && nodeMap.ContainsKey(nodeId))
        {
            return nodeMap[nodeId];
        }
        return null;
    }

    /// <summary>
    /// 현재 노드 가져오기
    /// </summary>
    public StageNodeData GetCurrentNode()
    {
        return GetNodeById(currentNodeId);
    }

    /// <summary>
    /// 현재 노드에서 접근 가능한 자식 노드들 가져오기
    /// </summary>
    public List<StageNodeData> GetAvailableChildren()
    {
        var currentNode = GetCurrentNode();
        if (currentNode == null) return new List<StageNodeData>();

        var availableChildren = new List<StageNodeData>();
        foreach (var childId in currentNode.childNodeIds)
        {
            var childNode = GetNodeById(childId);
            if (childNode != null && availableNodeIds.Contains(childId))
            {
                availableChildren.Add(childNode);
            }
        }
        return availableChildren;
    }

    /// <summary>
    /// 노드 방문 처리
    /// </summary>
    public void VisitNode(int nodeId)
    {
        if (!visitedNodeIds.Contains(nodeId))
        {
            visitedNodeIds.Add(nodeId);
        }
        
        if (availableNodeIds.Contains(nodeId))
        {
            availableNodeIds.Remove(nodeId);
        }
    }

    /// <summary>
    /// 노드를 접근 가능하게 만들기
    /// </summary>
    public void MakeNodeAvailable(int nodeId)
    {
        if (!availableNodeIds.Contains(nodeId))
        {
            availableNodeIds.Add(nodeId);
        }
    }

    /// <summary>
    /// 현재 노드에서 특정 자식 노드로 이동
    /// </summary>
    public bool MoveToNode(int childIndex)
    {
        var currentNode = GetCurrentNode();
        if (currentNode == null) return false;

        if (childIndex < 0 || childIndex >= currentNode.childNodeIds.Count)
        {
            return false;
        }

        var targetNodeId = currentNode.childNodeIds[childIndex];
        var targetNode = GetNodeById(targetNodeId);
        
        if (targetNode == null || !availableNodeIds.Contains(targetNodeId))
        {
            return false;
        }

        // 현재 노드 방문 처리
        VisitNode(currentNodeId);
        
        // 다음 노드로 이동
        currentNodeId = targetNodeId;
        
        // 자식 노드들을 접근 가능하게 만들기
        foreach (var childId in targetNode.childNodeIds)
        {
            MakeNodeAvailable(childId);
        }
        
        return true;
    }

    /// <summary>
    /// 스테이지 진행률 계산
    /// </summary>
    public float GetProgressPercentage()
    {
        if (allNodes == null || allNodes.Count == 0) return 0f;
        
        return (float)visitedNodeIds.Count / allNodes.Count * 100f;
    }

    /// <summary>
    /// 스테이지 완료 여부 확인
    /// </summary>
    public bool IsCompleted()
    {
        return isCompleted || allNodes?.Any(n => n.state == StageStateType.SUCCESS && visitedNodeIds.Contains(n.nodeId)) == true;
    }

    /// <summary>
    /// 스테이지 실패 여부 확인
    /// </summary>
    public bool IsFailed()
    {
        return isFailed;
    }

    public override string ToString()
    {
        return $"Stage: {stageName} (ID: {stageId}, Progress: {GetProgressPercentage():F1}%)"  ;
    }
}