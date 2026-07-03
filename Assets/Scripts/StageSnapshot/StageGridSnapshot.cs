#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageGridSnapshot
{
    public int snapshotIndex;
    public StageGridSnapshotStep step;
    public List<RoomGrid> rooms = new();
    public int targetRoomId = -1;

    public Vector2Int focusPosition;
    public string description;

    public List<Vector2Int> candidatePositions = new();

    public int roomCount { get; }

    public StageGridSnapshot(int snapshotIndex, StageGridSnapshotStep step, List<RoomGrid> sourceRooms, int targetRoomId = -1, 
        Vector2Int focusPosition = default, string description = "",
        List<Vector2Int> candidates = null)
    {
        this.snapshotIndex = snapshotIndex;
        this.step = step;
        this.targetRoomId = targetRoomId;
        this.focusPosition = focusPosition;
        this.description = description;

        roomCount = sourceRooms.Count;

        rooms.Capacity = roomCount;

        for (int i = 0; i < roomCount; i++)
            rooms.Add(Clone(sourceRooms[i]));

        if (candidates != null)
            candidatePositions = new List<Vector2Int>(candidates);
    }

    private static RoomGrid Clone(RoomGrid room)
    {
        if (room is BossRoomGrid bossRoom)
            return new BossRoomGrid(bossRoom);

        return new RoomGrid(room);
    }

}

public enum StageGridSnapshotStep
{
    None, Normal, NormalCandidate, SpecialRoom, SpecialCandidate, BossRoom, EliteRoom, Gate, Complete
}

#endif