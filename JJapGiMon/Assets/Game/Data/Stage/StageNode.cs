
using System;
using System.Collections.Generic;
using Unity.Profiling;

[Serializable]
public class StageNode
{
    public int nodeId;                       // 🆕 노드 고유 ID
    public int depth;
    public int index;
    public StageRoomType type;
    public StageStateType state; // 방 상태 (성공/실패/중립)
    public int seed; // 시드값 (전투 방의 무작위 선택에 사용)
    public List<StageNode> children = new();

    // 세부 타입들
    public EventRoomType? eventType; // 이벤트 방일 때만 사용
    public BattleRoomType? battleType; // 전투 방일 때만 사용
    public int? roomContentsId; // 방 정보 Id (저장된 데이터 풀에서 가져옴. 예를들어, 기본적 3마리가 들어있는 방 id / 강한 적 1마리가 들어있는 방 id 등등에서 가져와서 저장.)

    // 방 정보
    public string roomName; // 방 이름
    public string roomDescription; // 방 설명
    public bool isVisited; // 방 방문 여부
    public bool isAvailable; // 방 접근 가능 여부

    // 🆕 부모 노드 참조
    public StageNode parent;

    public StageNode(int depth, int index, StageRoomType type, EventRoomType? eventRoomType, BattleRoomType? battleRoomType, int seed = 0)
    {
        this.depth = depth;
        this.index = index;
        this.type = type;
        this.seed = seed;
        this.eventType = eventRoomType;
        this.battleType = battleRoomType;
        this.state = StageStateType.NEUTRAL; // 기본값은 중립
        this.isVisited = false;
        this.isAvailable = false;
        
        // 타입에 따라 세부 타입 초기화
        InitializeRoomInfo();
    }

 

    private void InitializeRoomInfo()
    {
        switch (type)
        {
            case StageRoomType.Start:
                roomName = "Start";
                roomDescription = "스테이지의 시작점입니다.";
                isAvailable = true; // 시작 방은 항상 접근 가능
                break;
                
            case StageRoomType.Event:
                roomName = GetEventRoomName();
                roomDescription = GetEventRoomDescription();
                break;
                
            case StageRoomType.Battle:
                roomName = GetBattleRoomName();
                roomDescription = GetBattleRoomDescription();
                break;
                
            case StageRoomType.Boss:
                roomName = "Boss";
                roomDescription = "강력한 보스가 기다리고 있습니다.";
                break;
        }
    }

    private string GetEventRoomName()
    {
        return eventType switch
        {
            EventRoomType.Rest => "휴식 공간",
            EventRoomType.Story => "스토리",
            EventRoomType.Maintenance => "정비소",
            EventRoomType.Event => "이벤트 공간",
            _ => "이벤트 방"
        };
    }

    private string GetEventRoomDescription()
    {
        return eventType switch
        {
            EventRoomType.Rest => "체력을 회복할 수 있는 안전한 공간입니다.",
            EventRoomType.Story => "스토리가 진행되는 곳입니다.",
            EventRoomType.Maintenance => "장비를 정비하고 강화할 수 있는 곳입니다.",
            EventRoomType.Event => "특별한 이벤트가 일어날 수 있는 공간입니다.",
            _ => "이벤트가 발생하는 방입니다."
        };
    }

    private string GetBattleRoomName()
    {
        return battleType switch
        {
            BattleRoomType.Normal => "Normal Battle",
            _ => "전투 방"
        };
    }

    private string GetBattleRoomDescription()
    {
        return battleType switch
        {
            BattleRoomType.Normal => "일반적인 적들과의 전투입니다.",
            _ => "적들과의 전투가 벌어지는 방입니다."
        };
    }

    /// <summary>
    /// 자식 노드 추가 (부모 참조 자동 설정)
    /// </summary>
    public void AddChild(StageNode child)
    {
        // 중복 체크
        if (children.Contains(child))
        {
            return; // 이미 존재하는 자식이면 추가하지 않음
        }
        
        child.parent = this;
        children.Add(child);
    }

    /// <summary>
    /// 방을 방문 처리합니다.
    /// </summary>
    public void Visit()
    {
        isVisited = true;
    }

    /// <summary>
    /// 방을 접근 가능하게 만듭니다.
    /// </summary>
    public void MakeAvailable()
    {
        isAvailable = true;
    }

    /// <summary>
    /// 자식 노드들을 접근 가능하게 만듭니다.
    /// </summary>
    public void MakeChildrenAvailable()
    {
        foreach (var child in children)
        {
            child.MakeAvailable();
        }
    }

    public override string ToString()
    {
        string subType = type switch
        {
            StageRoomType.Event => $" ({eventType})",
            StageRoomType.Battle => $" ({battleType})",
            _ => ""
        };
        
        string status = isVisited ? " [방문됨]" : isAvailable ? " [접근가능]" : " [잠김]";
        string stateStatus = state switch
        {
            StageStateType.SUCCESS => " [성공]",
            StageStateType.FAIL => " [실패]",
            StageStateType.NEUTRAL => " [중립]",
            _ => ""
        };
        
        return $"{roomName}{subType}{status}{stateStatus} depth: {depth} index: {index} seed: {seed}";
    }
}