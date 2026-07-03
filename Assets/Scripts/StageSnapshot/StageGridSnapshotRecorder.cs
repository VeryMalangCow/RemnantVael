#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

public class StageGridSnapshotRecorder
{
    private readonly List<StageGridSnapshot> snapshots = new();

    public IReadOnlyList<StageGridSnapshot> Snapshots => snapshots;

    public int Count => snapshots.Count;

    public bool IsEmpty => snapshots.Count == 0;

    public void Clear()
    {
        snapshots.Clear();
    }

    public void Record(StageGridSnapshotStep step, List<RoomGrid> rooms, int targetRoomId = -1,
        Vector2Int focusPosition = default, string description = "",
        List<Vector2Int> candidates = null)
    {
        snapshots.Add(new StageGridSnapshot(snapshots.Count, step, rooms, targetRoomId,
            focusPosition, description,
            candidates));
    }

    public StageGridSnapshot Get(int index)
    {
        if (index < 0 || index >= snapshots.Count)
            return null;

        return snapshots[index];
    }

    public StageGridSnapshot GetLast()
    {
        if (snapshots.Count == 0)
            return null;

        return snapshots[snapshots.Count - 1];
    }
}

#endif