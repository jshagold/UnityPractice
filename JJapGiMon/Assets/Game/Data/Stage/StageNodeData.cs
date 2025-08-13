using System;
using System.Collections.Generic;

[Serializable]
public class StageNodeData
{
    public int nodeId;                       // 노드 고유 ID
    public int depth;                        // 깊이
    public int index;                        // 인덱스
    public StageRoomType type;               // 방 타입
    public int seed;                         // 시드값
    
    // 세부 타입
    public EventRoomType? eventType;
    public BattleRoomType? battleType;
    public int? roomContentsId; // 방 정보 Id (저장된 데이터 풀에서 가져옴. 예를들어, 기본적 3마리가 들어있는 방 id / 강한 적 1마리가 들어있는 방 id 등등에서 가져와서 저장.)

    // 구조 정보
    public List<int> childNodeIds;           // 자식 노드 ID 목록
    public int? parentNodeId;                // 부모 노드 ID
    
    // 목표 여부 (스테이지 구조의 일부이므로 저장 필요)
    public bool isGoal;                      

    public StageNodeData()
    {
        childNodeIds = new List<int>();
    }

    public override string ToString()
    {
        string subType = type switch
        {
            StageRoomType.Event => $" ({eventType})",
            StageRoomType.Battle => $" ({battleType})",
            _ => ""
        };
        
        return $"{type}{subType} depth: {depth} index: {index}";
    }
}