using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StageMapModel
{
    public int length;                 // 예: 5 (시작, 중간3, 마지막)
    public int choicesPerStep;         // 예: 3
    public List<List<StageNode>> levels = new();  // depth별 노드 목록 [depth][index]

    System.Random rng;

    public StageMapModel(int length = 5, int choicesPerStep = 3, int? seed = null)
    {
        this.length = Mathf.Max(2, length);
        this.choicesPerStep = Mathf.Clamp(choicesPerStep, 1, 5);
        rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        Build();
    }

    void Build()
    {
        levels.Clear();

        // depth 0: Start 1개
        var start = new StageNode(0, 0, StageRoomType.Start);
        levels.Add(new List<StageNode> { start });

        // depth 1..length-2: 중간(배틀/이벤트) 노드들
        for (int d = 1; d < length - 1; d++)
        {
            var list = new List<StageNode>();
            for (int i = 0; i < choicesPerStep; i++)
            {
                // 간단한 확률: 70% 배틀, 30% 이벤트
                bool isBattle = rng.NextDouble() < 0.7;
                list.Add(new StageNode(d, i, isBattle ? StageRoomType.Battle : StageRoomType.Event));
            }
            levels.Add(list);
        }

        // depth (length-1): Boss 1개
        var boss = new StageNode(length - 1, 0, StageRoomType.Boss);
        levels.Add(new List<StageNode> { boss });

        // 엣지(부모→자식) 연결: 각 depth의 모든 노드가 다음 depth의 모든 노드로 연결
        // (Slay the Spire 초기형식처럼 공유-자식 구조: 매 depth에서 최대 n개 선택지 보장)
        for (int d = 0; d < length - 1; d++)
        {
            foreach (var node in levels[d])
                node.children.AddRange(levels[d + 1]);
        }
    }
}