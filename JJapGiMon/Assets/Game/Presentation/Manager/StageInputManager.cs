using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지 맵 전용 입력 처리: 노드 선택 및 UI 상호작용
/// </summary>
public sealed class StageInputManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private StageMapUI stageMapUI;

    [Header("Manager References")]
    [SerializeField] private StageManager stageManager;

    private void Awake()
    {
        // StageManager 참조 확인
        if (stageManager == null)
            stageManager = GetComponent<StageManager>();
    }

    private void Start()
    {
        // UI 이벤트 구독
        if (stageMapUI != null)
        {
            stageMapUI.OnNodeClicked += HandleNodeClicked;
        }
    }

    private void OnDestroy()
    {
        // UI 이벤트 구독 해제
        if (stageMapUI != null)
        {
            stageMapUI.OnNodeClicked -= HandleNodeClicked;
        }
    }

    /// <summary>
    /// 노드 클릭 처리
    /// </summary>
    private void HandleNodeClicked(StageNode clickedNode)
    {
        if (stageManager != null && stageManager.IsStageActive)
        {
            // 클릭된 노드가 현재 노드의 자식인지 확인
            var availableChildren = stageManager.GetAvailableChildren();
            int childIndex = availableChildren.IndexOf(clickedNode);
            
            if (childIndex >= 0)
            {
                // StageManager를 통해 노드 이동
                stageManager.MoveToNode(childIndex);
            }
            else
            {
                Debug.LogWarning($"클릭된 노드가 접근 가능하지 않습니다: {clickedNode.roomName}");
            }
        }
    }

    /// <summary>
    /// 스테이지 맵 초기화
    /// </summary>
    public void InitializeStageMap(StageNode rootNode, StageNode currentNode)
    {
        if (stageMapUI != null)
        {
            stageMapUI.UpdateMap(rootNode, currentNode);
        }
    }

    /// <summary>
    /// 현재 노드 업데이트
    /// </summary>
    public void UpdateCurrentNode(StageNode newNode)
    {
        if (stageMapUI != null)
        {
            stageMapUI.UpdateCurrentNode(newNode);
        }
    }

    /// <summary>
    /// 맵 새로고침
    /// </summary>
    public void RefreshMap()
    {
        if (stageManager != null && stageMapUI != null)
        {
            stageMapUI.UpdateMap(stageManager.CurrentStageMap, stageManager.CurrentNode);
        }
    }

    /// <summary>
    /// UI 활성화/비활성화
    /// </summary>
    public void SetUIEnabled(bool enabled)
    {
        if (stageMapUI != null)
        {
            stageMapUI.gameObject.SetActive(enabled);
        }
    }

    /// <summary>
    /// 디버그 정보 출력
    /// </summary>
    [ContextMenu("Print Debug Info")]
    public void PrintDebugInfo()
    {
        if (stageManager != null)
        {
            Debug.Log($"StageManager Active: {stageManager.IsStageActive}");
            Debug.Log($"Current Node: {stageManager.CurrentNode?.roomName}");
            Debug.Log($"Available Children: {stageManager.GetAvailableChildren().Count}");
        }
        
        if (stageMapUI != null)
        {
            Debug.Log($"StageMapUI Active: {stageMapUI.gameObject.activeInHierarchy}");
        }
    }
}
