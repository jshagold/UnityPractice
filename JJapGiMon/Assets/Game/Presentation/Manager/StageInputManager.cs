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
                // StageSceneController를 통해 노드 이동
                var sceneController = FindObjectOfType<StageSceneController>();
                if (sceneController != null)
                {
                    sceneController.MoveToNode(childIndex);
                }
                else
                {
                    // 직접 StageManager로 이동
                    stageManager.MoveToNode(childIndex);
                }
            }
            else
            {
                Debug.LogWarning($"클릭된 노드가 접근 가능하지 않습니다: {clickedNode.roomName}");
            }
        }
    }

    /// <summary>
    /// 스테이지 맵 초기화 (기존 호환성)
    /// </summary>
    public void InitializeStageMap(StageMapModel model, StageNode currentNode)
    {
        if (stageMapUI != null)
        {
            stageMapUI.UpdateMap(model, currentNode);
        }
    }

    /// <summary>
    /// 새로운 스테이지 시스템용 맵 초기화
    /// </summary>
    public void InitializeStageMap(StageNode rootNode, StageNode currentNode)
    {
        if (stageMapUI != null)
        {
            // StageMapUI가 새로운 구조를 지원하도록 수정 필요
            // stageMapUI.UpdateMapNew(rootNode, currentNode);
            
            // 임시로 기존 방식 사용
            Debug.Log($"새로운 스테이지 맵 초기화: {rootNode?.roomName} -> {currentNode?.roomName}");
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
    /// 사용 가능한 노드들 업데이트
    /// </summary>
    public void UpdateAvailableNodes(List<StageNode> availableNodes)
    {
        if (stageMapUI != null)
        {
            // StageMapUI가 새로운 구조를 지원하도록 수정 필요
            // stageMapUI.UpdateAvailableNodes(availableNodes);
            
            Debug.Log($"접근 가능한 노드 업데이트: {availableNodes.Count}개");
        }
    }

    /// <summary>
    /// 스테이지 진행 상황 업데이트
    /// </summary>
    public void UpdateProgress(StageProgress progress)
    {
        if (stageMapUI != null)
        {
            // StageMapUI가 진행 상황을 표시하도록 수정 필요
            // stageMapUI.UpdateProgress(progress);
            
            Debug.Log($"진행률 업데이트: {progress.ProgressPercentage:F1}%");
        }
    }

    /// <summary>
    /// 이벤트 방 UI 표시
    /// </summary>
    public void ShowEventRoomUI(StageNode eventNode)
    {
        if (eventNode?.eventType != null)
        {
            switch (eventNode.eventType)
            {
                case EventRoomType.Rest:
                    ShowRestEventUI();
                    break;
                case EventRoomType.Shop:
                    ShowShopUI();
                    break;
                case EventRoomType.Maintenance:
                    ShowMaintenanceUI();
                    break;
                case EventRoomType.Event:
                    ShowSpecialEventUI();
                    break;
            }
        }
    }

    /// <summary>
    /// 휴식 이벤트 UI 표시
    /// </summary>
    private void ShowRestEventUI()
    {
        Debug.Log("휴식 이벤트 UI 표시");
        // TODO: 실제 UI 구현
        // EventUIManager.Instance.ShowRestEvent();
    }

    /// <summary>
    /// 상점 UI 표시
    /// </summary>
    private void ShowShopUI()
    {
        Debug.Log("상점 UI 표시");
        // TODO: 실제 UI 구현
        // ShopUIManager.Instance.ShowShop();
    }

    /// <summary>
    /// 정비 UI 표시
    /// </summary>
    private void ShowMaintenanceUI()
    {
        Debug.Log("정비 UI 표시");
        // TODO: 실제 UI 구현
        // MaintenanceUIManager.Instance.ShowMaintenance();
    }

    /// <summary>
    /// 특별 이벤트 UI 표시
    /// </summary>
    private void ShowSpecialEventUI()
    {
        Debug.Log("특별 이벤트 UI 표시");
        // TODO: 실제 UI 구현
        // SpecialEventUIManager.Instance.ShowSpecialEvent();
    }

    /// <summary>
    /// 전투 씬 로드
    /// </summary>
    public void LoadBattleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }

    /// <summary>
    /// 보스 전투 씬 로드
    /// </summary>
    public void LoadBossScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BossScene");
    }

    /// <summary>
    /// 이벤트 완료 처리
    /// </summary>
    public void CompleteEvent()
    {
        Debug.Log("이벤트 완료");
        // 이벤트 UI 닫기 및 다음 단계로 진행
        // EventUIManager.Instance.HideEventUI();
    }
}
