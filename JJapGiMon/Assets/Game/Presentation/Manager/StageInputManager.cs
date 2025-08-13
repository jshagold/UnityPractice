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

    // 이벤트 정의
    public System.Action<StageNode> OnNodeClicked;
    public System.Action<StageNode> OnNodeHovered;
    public System.Action<StageNode> OnNodeUnhovered;

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
                // 이벤트 발생
                OnNodeClicked?.Invoke(clickedNode);
            }
            else
            {
                Debug.LogWarning($"클릭된 노드가 접근 가능하지 않습니다: {clickedNode.roomName}");
            }
        }
        else
        {
            Debug.LogWarning("스테이지가 활성화되지 않았습니다.");
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
        else
        {
            Debug.LogError("StageMapUI가 할당되지 않았습니다!");
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
        else
        {
            Debug.LogError("StageMapUI가 할당되지 않았습니다!");
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
        else
        {
            Debug.LogError("StageManager 또는 StageMapUI가 할당되지 않았습니다!");
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
        else
        {
            Debug.LogError("StageMapUI가 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// 특정 노드 강조 표시
    /// </summary>
    public void HighlightNode(StageNode node, bool highlight = true)
    {
        if (stageMapUI != null)
        {
            // TODO: StageMapUI에 노드 강조 기능 추가
            Debug.Log($"노드 강조: {node?.roomName} - {highlight}");
        }
    }

    /// <summary>
    /// 모든 노드 강조 해제
    /// </summary>
    public void ClearHighlights()
    {
        if (stageMapUI != null)
        {
            // TODO: StageMapUI에 모든 강조 해제 기능 추가
            Debug.Log("모든 노드 강조 해제");
        }
    }

    /// <summary>
    /// 접근 가능한 노드들만 강조
    /// </summary>
    public void HighlightAvailableNodes()
    {
        if (stageManager != null && stageMapUI != null)
        {
            var availableNodes = stageManager.GetAvailableChildren();
            foreach (var node in availableNodes)
            {
                HighlightNode(node, true);
            }
        }
    }

    /// <summary>
    /// 입력 처리 활성화/비활성화
    /// </summary>
    public void SetInputEnabled(bool enabled)
    {
        if (stageMapUI != null)
        {
            // StageMapUI의 모든 버튼들의 interactable 상태 변경
            var buttons = stageMapUI.GetComponentsInChildren<UnityEngine.UI.Button>();
            foreach (var button in buttons)
            {
                button.interactable = enabled;
            }
        }
    }

    /// <summary>
    /// 디버그 정보 출력
    /// </summary>
    [ContextMenu("Print Debug Info")]
    public void PrintDebugInfo()
    {
        Debug.Log("=== StageInputManager 디버그 정보 ===");
        
        if (stageManager != null)
        {
            Debug.Log($"StageManager Active: {stageManager.IsStageActive}");
            Debug.Log($"Current Node: {stageManager.CurrentNode?.roomName}");
            Debug.Log($"Available Children: {stageManager.GetAvailableChildren().Count}");
        }
        else
        {
            Debug.LogError("StageManager가 할당되지 않았습니다!");
        }
        
        if (stageMapUI != null)
        {
            Debug.Log($"StageMapUI Active: {stageMapUI.gameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("StageMapUI가 할당되지 않았습니다!");
        }

        // 이벤트 구독 상태 확인
        Debug.Log($"OnNodeClicked 이벤트 구독자 수: {OnNodeClicked?.GetInvocationList().Length ?? 0}");
    }
}
