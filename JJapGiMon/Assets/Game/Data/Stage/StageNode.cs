
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
    public bool isGoal; // 마지막 방에서 목표 여부 (true = 목표, false = 실패)
    public int seed; // 시드값 (전투 방의 무작위 선택에 사용)
    public List<StageNode> children = new();

    // 세부 타입들
    public EventRoomType? eventType; // 이벤트 방일 때만 사용
    public BattleRoomType? battleType; // 전투 방일 때만 사용

    // 방 정보
    public string roomName; // 방 이름
    public string roomDescription; // 방 설명
    public bool isVisited; // 방 방문 여부
    public bool isAvailable; // 방 접근 가능 여부

    // 🆕 부모 노드 참조
    public StageNode parent;

    public StageNode(int depth, int index, StageRoomType type, int seed = 0)
    {
        this.depth = depth;
        this.index = index;
        this.type = type;
        this.seed = seed;
        this.isVisited = false;
        this.isAvailable = false;
        
        // 타입에 따라 세부 타입 초기화
        InitializeSubType();
        InitializeRoomInfo();
    }

    private void InitializeSubType()
    {
        switch (type)
        {
            case StageRoomType.Event:
                // 이벤트 방은 4가지 타입 중 하나로 무작위 선택
                var random = new System.Random(seed);
                eventType = (EventRoomType)random.Next(0, 4);
                break;
                
            case StageRoomType.Battle:
                // 전투 방은 시드값에 따라 4가지 타입 중 하나로 무작위 선택
                var battleRandom = new System.Random(seed);
                battleType = (BattleRoomType)battleRandom.Next(0, 4);
                break;
                
            default:
                // Start, Boss는 세부 타입이 없음
                eventType = null;
                battleType = null;
                break;
        }
    }

    private void InitializeRoomInfo()
    {
        switch (type)
        {
            case StageRoomType.Start:
                roomName = "시작 지점";
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
                roomName = "보스 방";
                roomDescription = "강력한 보스가 기다리고 있습니다.";
                break;
        }
    }

    private string GetEventRoomName()
    {
        return eventType switch
        {
            EventRoomType.Rest => "휴식 공간",
            EventRoomType.Shop => "상점",
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
            EventRoomType.Shop => "다양한 아이템을 구매할 수 있는 상점입니다.",
            EventRoomType.Maintenance => "장비를 정비하고 강화할 수 있는 곳입니다.",
            EventRoomType.Event => "특별한 이벤트가 일어날 수 있는 공간입니다.",
            _ => "이벤트가 발생하는 방입니다."
        };
    }

    private string GetBattleRoomName()
    {
        return battleType switch
        {
            BattleRoomType.NormalBattle => "일반 전투",
            BattleRoomType.EliteBattle => "정예 전투",
            BattleRoomType.AmbushBattle => "매복 전투",
            BattleRoomType.ArenaBattle => "아레나 전투",
            _ => "전투 방"
        };
    }

    private string GetBattleRoomDescription()
    {
        return battleType switch
        {
            BattleRoomType.NormalBattle => "일반적인 적들과의 전투입니다.",
            BattleRoomType.EliteBattle => "강력한 정예 적들과의 전투입니다.",
            BattleRoomType.AmbushBattle => "매복한 적들과의 급작스러운 전투입니다.",
            BattleRoomType.ArenaBattle => "아레나에서 벌어지는 특별한 전투입니다.",
            _ => "적들과의 전투가 벌어지는 방입니다."
        };
    }

    /// <summary>
    /// 자식 노드 추가 (부모 참조 자동 설정)
    /// </summary>
    public void AddChild(StageNode child)
    {
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
        
        return $"{roomName}{subType}{status} depth: {depth} index: {index} seed: {seed}";
    }
}