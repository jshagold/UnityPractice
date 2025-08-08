using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StageMapModel
{
    public int length;                 // ��: 5 (����, �߰�3, ������)
    public int choicesPerStep;         // ��: 3
    public List<List<StageNode>> levels = new();  // depth�� ��� ��� [depth][index]

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

        // depth 0: Start 1��
        var start = new StageNode(0, 0, StageRoomType.Start);
        levels.Add(new List<StageNode> { start });

        // depth 1..length-2: �߰�(��Ʋ/�̺�Ʈ) ����
        for (int d = 1; d < length - 1; d++)
        {
            var list = new List<StageNode>();
            for (int i = 0; i < choicesPerStep; i++)
            {
                // ������ Ȯ��: 70% ��Ʋ, 30% �̺�Ʈ
                bool isBattle = rng.NextDouble() < 0.7;
                list.Add(new StageNode(d, i, isBattle ? StageRoomType.Battle : StageRoomType.Event));
            }
            levels.Add(list);
        }

        // depth (length-1): Boss 1��
        var boss = new StageNode(length - 1, 0, StageRoomType.Boss);
        levels.Add(new List<StageNode> { boss });

        // ����(�θ���ڽ�) ����: �� depth�� ��� ��尡 ���� depth�� ��� ���� ����
        // (Slay the Spire �ʱ�����ó�� ����-�ڽ� ����: �� depth���� �ִ� n�� ������ ����)
        for (int d = 0; d < length - 1; d++)
        {
            foreach (var node in levels[d])
                node.children.AddRange(levels[d + 1]);
        }
    }
}