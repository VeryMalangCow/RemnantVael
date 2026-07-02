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


    public int roomCount { get; }

    public StageGridSnapshot(int snapshotIndex, StageGridSnapshotStep step, List<RoomGrid> sourceRooms, int targetRoomId = -1, 
        Vector2Int focusPosition = default, string description = "")
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
    None, Normal, SpecialCandidate, BossRoom, SpecialRoom, EliteRoom, Gate, Complete
}

#endif