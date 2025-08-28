using System;
using System.Collections.Generic;
using System.Linq;

public class StageConfig
{
    // 기본 정보
    public int stageId;
    public string stageName;                // 스테이지 이름
    public string stageDescription;         // 스테이지 설명

    // 맵 생성 설정
    public int stageLength = 5;             // 스테이지의 총 길이 (시작, 보스 포함)
    public int MinNodeCountByDepth = 1;     // 깊이에서 생성되는 최소 노드 수
    public int MaxNodeCountByDepth = 5;     // 깊이에서 생성되는 최대 노드 수
    public int? randomSeed = null;          // 스테이지의 랜덤 시드
    public int lastRoomCount = 3;           // 마지막 방 개수
    
    // 난이도 설정
    public int? Difficulty { get; init; }

    // 방 타입 가중치
    public List<RoomTypeWeight> roomTypeWeights = new()
    {
        new RoomTypeWeight(StageRoomType.Event, 1),
        new RoomTypeWeight(StageRoomType.Battle, 3),
    };

    public List<EventTypeWeight> eventRoomTypeWeights = new()
    {
        new EventTypeWeight(EventRoomType.Rest, 3),
        new EventTypeWeight(EventRoomType.Story, 1),
        new EventTypeWeight(EventRoomType.Maintenance, 3),
        new EventTypeWeight(EventRoomType.Event, 2),
    };

    // 가중치에 따라 방 타입 선택 //
    public StageRoomType GetRandomRoomType(Random random)
    {
        if (roomTypeWeights == null || roomTypeWeights.Count == 0)
        {
            // 기본값 반환
            return StageRoomType.Battle;
        }

        // 총 가중치 계산
        int totalWeight = roomTypeWeights.Sum(w => w.weight);
        
        if (totalWeight <= 0)
        {
            // 가중치가 모두 0이면 첫 번째 타입 반환
            return roomTypeWeights[0].type;
        }

        // 랜덤 값 생성 (0 ~ totalWeight-1)
        int randomValue = random.Next(totalWeight);
        
        // 가중치에 따라 타입 선택
        int currentWeight = 0;
        foreach (var weight in roomTypeWeights)
        {
            currentWeight += weight.weight;
            if (randomValue < currentWeight)
            {
                return weight.type;
            }
        }
        
        // 안전장치: 마지막 타입 반환
        return roomTypeWeights[roomTypeWeights.Count - 1].type;
    }

    // 가중치에 따라 이벤트 방 타입 선택 //
    public EventRoomType GetRandomEventRoomType(Random random)
    {
        if (eventRoomTypeWeights == null || eventRoomTypeWeights.Count == 0)
        {
            // 기본값 반환
            return EventRoomType.Rest;
        }

        // 총 가중치 계산
        int totalWeight = eventRoomTypeWeights.Sum(w => w.weight);
        
        if (totalWeight <= 0)
        {
            // 가중치가 모두 0이면 첫 번째 타입 반환
            return eventRoomTypeWeights[0].type;
        }

        // 랜덤 값 생성 (0 ~ totalWeight-1)
        int randomValue = random.Next(totalWeight);
        
        // 가중치에 따라 타입 선택
        int currentWeight = 0;
        foreach (var weight in eventRoomTypeWeights)
        {
            currentWeight += weight.weight;
            if (randomValue < currentWeight)
            {
                return weight.type;
            }
        }
        
        // 안전장치: 마지막 타입 반환
        return eventRoomTypeWeights[eventRoomTypeWeights.Count - 1].type;
    }

    // 방 타입 가중치
    public struct RoomTypeWeight
    {
        public StageRoomType type;
        public int weight;
        public RoomTypeWeight(StageRoomType type, int weight)
        {
            this.type = type;
            this.weight = weight;
        }
    }

    // 이벤트 방 타입 가중치
    public struct EventTypeWeight
    {
        public EventRoomType type;
        public int weight;
        public EventTypeWeight(EventRoomType type, int weight)
        {
            this.type = type;
            this.weight = weight;
        }
    }
}