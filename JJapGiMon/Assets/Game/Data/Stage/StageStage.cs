using System.Collections.Generic;

public class StageState
{
    // 🆕 스테이지 진행 상태
    public int currentNodeId;               // 현재 위치한 노드 ID
    public List<int> visitedNodeIds;        // 방문한 노드 ID 목록
    public List<int> availableNodeIds { get; private set; }      // 접근 가능한 노드 ID 목록
    public bool isCompleted;                // 스테이지 완료 여부
    public bool isFailed;                   // 스테이지 실패 여부   

    public StageState()
    {
        visitedNodeIds = new List<int>();
        availableNodeIds = new List<int>();
        isCompleted = false;
        isFailed = false;
    }

    public void SetAvailableNodeIds(List<int> nodeIds)
    {
        availableNodeIds.Clear();
        availableNodeIds.AddRange(nodeIds);
    }
}