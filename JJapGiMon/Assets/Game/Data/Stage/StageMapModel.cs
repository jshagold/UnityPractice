using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StageMapModel
{
    public int length;                 // 예: 5 (시작, 중간3, 마지막)
    public int choicesPerStep;         // 예: 3
    public List<List<StageNode>> levels = new();  // depth별 노드 목록 [depth][index]

    private System.Random randomRange;

    public StageMapModel(int length = 5, int choicesPerStep = 3, int? seed = null)
    {
        this.length = Mathf.Max(2, length);
        this.choicesPerStep = choicesPerStep;
        randomRange = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        Build();
    }

    void Build()
    {
        levels.Clear();

        // depth 0: Start 1개
        var start = new StageNode(0, 0, StageRoomType.Start);
        levels.Add(new List<StageNode> { start });

        // depth 1..length-1: 중간(배틀/이벤트) 노드들 - 1~3개 무작위
        for (int d = 1; d < length; d++)
        {
            var list = new List<StageNode>();
            // 각 depth마다 1~3개 사이의 무작위 개수 생성
            int roomCount = randomRange.Next(1, choicesPerStep + 1); // 1, 2, 3 중 하나
            
            for (int i = 0; i < roomCount; i++)
            {
                // 간단한 확률: 70% 배틀, 30% 이벤트
                bool isBattle = randomRange.NextDouble() < 0.7;
                list.Add(new StageNode(d, i, isBattle ? StageRoomType.Battle : StageRoomType.Event));
            }
            levels.Add(list);
        }

        // depth (length): 마지막 방 3개 (목표 1개, 실패 2개) - Boss/Event 중 하나만 사용하고 무작위 순서로 섞기
        var lastDepth = length;
        
        // Boss와 Event 중 하나를 랜덤 선택
        var baseRoomType = randomRange.NextDouble() < 0.5 ? StageRoomType.Boss : StageRoomType.Event;
        
        // 목표/실패 상태를 나타내는 리스트 (true = 목표 1개, false = 실패 2개)
        var isGoalList = new List<bool> { true, false, false };
        
        // Fisher-Yates 셔플 알고리즘으로 목표/실패 순서를 섞기
        for (int i = isGoalList.Count - 1; i > 0; i--)
        {
            int j = randomRange.Next(i + 1);
            var temp = isGoalList[i];
            isGoalList[i] = isGoalList[j];
            isGoalList[j] = temp;
        }
        
        var lastList = new List<StageNode>();
        for (int i = 0; i < isGoalList.Count; i++)
        {
            var node = new StageNode(lastDepth, i, baseRoomType);
            node.isGoal = isGoalList[i]; // 목표 여부 설정
            lastList.Add(node);
        }
        levels.Add(lastList);

        // 엣지(부모→자식) 연결: 각 depth의 모든 노드가 다음 depth의 모든 노드로 연결
        // (Slay the Spire 초기형식처럼 공유-자식 구조: 매 depth에서 최대 n개 선택지 보장)
        for (int d = 0; d < levels.Count - 1; d++)
        {
            foreach (var node in levels[d])
                node.children.AddRange(levels[d + 1]);
        }
    }
}