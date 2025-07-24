using System;
using System.Collections.Generic;

public static class ListExtensions
{
    public static List<T> PickRandom<T>(this IList<T> source, int k)
    {
        if (k < 0 || k > source.Count)
            throw new System.ArgumentOutOfRangeException(nameof(k));

        // ① 인덱스 배열 준비
        var idx = new int[source.Count];
        for (int i = 0; i < idx.Length; i++) idx[i] = i;

        // ② 앞쪽 k칸만 부분 셔플
        for (int i = 0; i < k; i++)
        {
            // Range(minInclusive, maxExclusive) — 상한은 포함되지 않음
            int j = UnityEngine.Random.Range(i, source.Count);
            (idx[i], idx[j]) = (idx[j], idx[i]);
        }

        // ③ 결과 추출
        var result = new List<T>(k);
        for (int i = 0; i < k; i++)
            result.Add(source[idx[i]]);

        return result;
    }
}