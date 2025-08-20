using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Stage Background Table")]
public class StageBackgroundTable : ScriptableObject
{
    [Serializable]
    public struct Entry { public int stageId; public Sprite sprite; }

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Entry[] entries;

    private Dictionary<int, Sprite> _map;

    private void OnEnable()
    {
        _map = entries?.ToDictionary(e => e.stageId, e => e.sprite)
               ?? new Dictionary<int, Sprite>();
    }

    public Sprite Get(int stageId)
        => _map.TryGetValue(stageId, out var s) ? s : defaultSprite;
}