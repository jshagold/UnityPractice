using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    // --- 데이터 보관 --- //
    private StageMapModel stageMapModel;
    private StageNode currentNode;
    private List<CharacterModel> party;    // 선택된 아군

    // --- 상태값 --- //
    private bool stageRunning = true;

    // --- Events --- //
    public event Action<StageNode> OnCurrentNodeChanged; // 현재 노드 변경 이벤트
    public event Action<StageNode> OnNodeSelected; // 노드 선택 이벤트
    public event Action<StageRoomType> OnRoomEntered; // 방 진입 이벤트

    private void Awake()
    {
        Instance = this;
    }

    // --- 1. 초기화 --- //
    public void SetupStage(int stageLength, int choicesPerStep, int? seed, List<CharacterModel> selectedParty)
    {
        party = selectedParty;
        stageMapModel = new StageMapModel(stageLength, choicesPerStep, seed);
        currentNode = stageMapModel.levels[0][0]; // 시작 노드
        stageRunning = true;

        // 이벤트 발생
        OnCurrentNodeChanged?.Invoke(currentNode);
    }

    // --- 2. 노드 선택 로직 --- //
    public bool CanSelectNode(StageNode targetNode)
    {
        return currentNode.children.Contains(targetNode);
    }

    public void SelectNode(StageNode selectedNode)
    {
        if (!CanSelectNode(selectedNode))
        {
            Debug.LogWarning($"Cannot select node: {selectedNode} - not a valid child of current node");
            return;
        }

        currentNode = selectedNode;
        
        // 이벤트 발생
        OnCurrentNodeChanged?.Invoke(currentNode);
        OnNodeSelected?.Invoke(currentNode);

        // 노드 타입에 따른 처리
        HandleNodeType(currentNode);
    }

    // --- 3. 노드 타입 처리 --- //
    private void HandleNodeType(StageNode node)
    {
        OnRoomEntered?.Invoke(node.type);
    }

    // --- 4. 스테이지 완료 처리 --- //
    public void OnStageComplete(bool cleared)
    {
        stageRunning = false;
        
        if (!cleared)
        {
            // 게임 오버 처리
            Debug.Log("Stage Failed - Game Over");
        }
        else
        {
            // 스테이지 성공 처리
            Debug.Log("Stage Completed Successfully");
        }
    }

    // --- 5. 데이터 조회 --- //
    public StageNode GetCurrentNode() => currentNode;
    public StageMapModel GetStageMapModel() => stageMapModel;
    public List<CharacterModel> GetParty() => party;
    public bool IsLastRoom() => currentNode.depth == stageMapModel.length;
    public bool IsGoalRoom() => currentNode.isGoal;
    public bool IsStageRunning() => stageRunning;

    // --- 6. 스테이지 맵 재생성 --- //
    public void RegenerateMap(int stageLength, int choicesPerStep, int? newSeed)
    {
        stageMapModel = new StageMapModel(stageLength, choicesPerStep, newSeed);
        currentNode = stageMapModel.levels[0][0];
        
        // 이벤트 발생
        OnCurrentNodeChanged?.Invoke(currentNode);
    }
}
