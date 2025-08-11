
using System;
using System.Collections.Generic;
using Unity.Profiling;

[Serializable]
public class StageNode
{
    public int depth;
    public int index;
    public StageRoomType type;
    public bool isGoal; // 마지막 방에서 목표 여부 (true = 목표, false = 실패)
    public List<StageNode> children = new();

    public StageNode(int depth, int index, StageRoomType type)
    {
        this.depth = depth;
        this.index = index;
        this.type = type;
    }

    public override string ToString() => $"{type} depth: {depth} index: {index}";
}