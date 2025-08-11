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

    private void OnEnable()
    {
        // StageManager 이벤트 구독
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnCurrentNodeChanged += HandleCurrentNodeChanged;
            StageManager.Instance.OnNodeSelected += HandleNodeSelected;
            StageManager.Instance.OnRoomEntered += HandleRoomEntered;
        }
    }

    private void OnDisable()
    {
        // StageManager 이벤트 구독 해제
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnCurrentNodeChanged -= HandleCurrentNodeChanged;
            StageManager.Instance.OnNodeSelected -= HandleNodeSelected;
            StageManager.Instance.OnRoomEntered -= HandleRoomEntered;
        }
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
        if (StageManager.Instance != null)
        {
            StageManager.Instance.SelectNode(clickedNode);
        }
    }

    /// <summary>
    /// 현재 노드 변경 처리
    /// </summary>
    private void HandleCurrentNodeChanged(StageNode newNode)
    {
        if (stageMapUI != null)
        {
            stageMapUI.UpdateCurrentNode(newNode);
        }
    }

    /// <summary>
    /// 노드 선택 처리
    /// </summary>
    private void HandleNodeSelected(StageNode selectedNode)
    {
        // 노드 선택 시 추가 UI 업데이트나 효과 처리
        Debug.Log($"Node selected: {selectedNode.type} at depth {selectedNode.depth}");
    }

    /// <summary>
    /// 방 진입 처리
    /// </summary>
    private void HandleRoomEntered(StageRoomType roomType)
    {
        Debug.Log($"Entered room type: {roomType}");
        
        // 방 타입에 따른 추가 처리
        switch (roomType)
        {
            case StageRoomType.Start:
                // 시작 방 - 아무것도 하지 않음
                break;
            case StageRoomType.Battle:
                // 전투 방 - 전투 씬으로 이동
                LoadBattleScene();
                break;
            case StageRoomType.Event:
                // 이벤트 방 - 이벤트 씬으로 이동
                LoadEventScene();
                break;
            case StageRoomType.Boss:
                // 보스 방 - 보스 전투 씬으로 이동
                LoadBossScene();
                break;
        }
    }

    /// <summary>
    /// 전투 씬 로드
    /// </summary>
    private void LoadBattleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }

    /// <summary>
    /// 이벤트 씬 로드
    /// </summary>
    private void LoadEventScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EventScene");
    }

    /// <summary>
    /// 보스 전투 씬 로드
    /// </summary>
    private void LoadBossScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BossScene");
    }

    /// <summary>
    /// 스테이지 맵 초기화
    /// </summary>
    public void InitializeStageMap(StageMapModel model, StageNode currentNode)
    {
        if (stageMapUI != null)
        {
            stageMapUI.UpdateMap(model, currentNode);
        }
    }
}
