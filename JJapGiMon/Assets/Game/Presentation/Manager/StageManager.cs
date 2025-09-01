using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class StageManager : MonoBehaviour
{
    [Header("스테이지 설정")]
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private StageGraph stageGraph;
    [SerializeField] private StageState stageState;
    [SerializeField] private StageNode rootNode;
    [SerializeField] private StageNode currentNode;

    [Header("현재 위치 depth")]
    [SerializeField] private int currentDepth = 0;


    // 이벤트
    public event Action<StageNode> OnStageGenerated;
    public event Action<StageNode> OnEventRoomEntered;
    public event Action<StageNode> OnBattleRoomEntered;
    public event Action<StageNode> OnBossRoomEntered;

    
    void Awake()
    {
        // Awake는 최소셋업만 유지하거나 비우는걸 권장!
    }


    // 초기화
    public void Initialize(StageConfig stageConfig, StageGraph stageGraph, StageState stageState)
    {
        rootNode = stageGraph.rootNode;

        // 현재 노드 설정
        currentNode = GetNodeById(stageState.currentNodeId);
        if (currentNode == null)
        {
            currentNode = rootNode; // 시작 노드로 설정
            stageState.currentNodeId = currentNode.nodeId;
        }
        
        currentDepth = currentNode.depth;
  
        OnStageGenerated?.Invoke(rootNode);
    }

    

    /// <summary>
    /// 노드 ID로 노드 찾기
    /// </summary>
    private StageNode GetNodeById(int nodeId)
    {
        return FindNodeRecursive(rootNode, nodeId);
    }

    private StageNode FindNodeRecursive(StageNode node, int targetId)
    {
        if (node.nodeId == targetId) return node;
        
        foreach (var child in node.children)
        {
            var result = FindNodeRecursive(child, targetId);
            if (result != null) return result;
        }
        
        return null;
    }

    /// <summary>
    /// 현재 노드에서 특정 자식 노드로 이동합니다.
    /// </summary>
    public bool MoveToNode(int childNodeId)
    {
        if (currentNode == null || currentNode.children.Count <= 0)
        {
            Debug.LogWarning($"자식노드 존재하지 않음.");
            return false;
        }

        var targetNode = stageGraph.GetNodeById(childNodeId);
        if(!currentNode.children.Contains(targetNode))
        {
            Debug.LogWarning($"이동할 수 없는 노드임.");
            return false;
        }
        
        // StageData를 통한 이동 처리
        stageState.visitedNodeIds.Add(currentNode.nodeId);
        stageState.SetAvailableNodeIds(currentNode.children.Select(c => c.nodeId).ToList());

        currentNode = targetNode;
        currentDepth = currentNode.depth;

        Debug.Log($"노드 이동: {currentNode.roomName} (깊이: {currentDepth})");
        
        return true;
    }

    public bool CanMoveToNode(int childNodeId)
    {
        var childNode = GetNodeById(childNodeId);
        return currentNode.children.Contains(childNode);
    }



    /// <summary>
    /// 현재 노드의 타입에 따라 적절한 처리를 수행합니다.
    /// </summary>
    public void OnEnterRoom(int nodeId)
    {
        var targetNode = GetNodeById(nodeId);

        if (targetNode == null) return;

        switch (targetNode.type)
        {
            case StageRoomType.Start:
                Debug.Log("시작 지점에 도착했습니다.");
                break;
                
            case StageRoomType.Event:
                ProcessEventRoom(targetNode);
                break;
                
            case StageRoomType.Battle:
                ProcessBattleRoom(targetNode);
                break;
                
            case StageRoomType.Boss:
                ProcessBossRoom(targetNode);
                break;
        }
    }

    private void ProcessEventRoom(StageNode node)
    {
        Debug.Log($"이벤트 방 처리: {node.roomName}");
        
        switch (node.eventType)
        {
            case EventRoomType.Rest:
                Debug.Log("휴식 공간에 입장했습니다. 체력을 회복할 수 있습니다.");
                break;
                
            case EventRoomType.Story:
                Debug.Log("스토리 이벤트가 발생했습니다.");
                break;
                
            case EventRoomType.Maintenance:
                Debug.Log("정비소에 입장했습니다. 장비를 강화할 수 있습니다.");
                break;
                
            case EventRoomType.Event:
                Debug.Log("특별한 이벤트가 발생했습니다.");
                break;
        }
        
        OnEventRoomEntered?.Invoke(node);
    }

    private void ProcessBattleRoom(StageNode node)
    {
        Debug.Log($"전투 방 처리: {node.roomName}");
        
        switch (node.battleType)
        {
            case BattleRoomType.Normal:
                Debug.Log("일반 전투를 시작합니다.");
                break;
        }
        
        OnBattleRoomEntered?.Invoke(node);
    }

    private void ProcessBossRoom(StageNode node)
    {
        Debug.Log($"보스 방 처리: {node.roomName}");
        
        OnBossRoomEntered?.Invoke(node);
    }

}