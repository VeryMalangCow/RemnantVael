#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageGridReplayWindow : EditorWindow
{
    private StageManager stageManager;
    private StageGridSnapshotRecorder recorder;


    private float zoom = 2f;
    private int currentIndex;
    
    [MenuItem("Tools/Stage/Grid Replay")]
    private static void Open()
    {
        GetWindow<StageGridReplayWindow>("Grid Replay");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        stageManager = (StageManager)EditorGUILayout.ObjectField(
            "Stage Manager",
            stageManager,
            typeof(StageManager),
            true);

        if (stageManager == null)
            return;

        recorder = stageManager.StageGridGenerator.SnapshotRecorder;

        if (recorder == null || recorder.IsEmpty)
        {
            EditorGUILayout.HelpBox(
                "Snapshot이 존재하지 않습니다.\n먼저 Grid를 생성하세요.",
                MessageType.Info);
            return;
        }

        currentIndex = EditorGUILayout.IntSlider(
            "Snapshot",
            currentIndex,
            0,
            recorder.Count - 1);

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        GUI.enabled = currentIndex > 0;
        if (GUILayout.Button("Previous"))
            currentIndex--;

        GUI.enabled = currentIndex < recorder.Count - 1;
        if (GUILayout.Button("Next"))
            currentIndex++;

        GUI.enabled = true;

        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        DrawSnapshotInfo();

        GUILayout.Space(20);

        DrawGrid();
    }

    private void DrawSnapshotInfo()
    {
        StageGridSnapshot snapshot = recorder.Get(currentIndex);

        if (snapshot == null)
            return;

        EditorGUILayout.LabelField("Step", snapshot.step.ToString());

        EditorGUILayout.LabelField("Snapshot", $"{snapshot.snapshotIndex}");

        EditorGUILayout.LabelField("Room Count", $"{snapshot.roomCount}");

        EditorGUILayout.LabelField("Target Room", $"{snapshot.targetRoomId}");

        EditorGUILayout.LabelField("Focus", snapshot.focusPosition.ToString());
    }

    private void DrawGrid()
    {
        StageGridSnapshot snapshot = recorder.Get(currentIndex);

        if (snapshot == null)
            return;

        Rect rect = GUILayoutUtility.GetRect(300, 400);

        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        DrawRooms(rect, snapshot);
    }

    private void DrawRooms(Rect rect, StageGridSnapshot snapshot)
    {
        float cellSize = 28f * zoom;
        Vector2 center = rect.center;

        foreach (RoomGrid room in snapshot.rooms)
        {
            HashSet<Vector2Int> cells = new HashSet<Vector2Int>(room.roomPos);

            Color color = GetRoomColor(room);

            foreach (Vector2Int pos in room.roomPos)
            {
                Vector2Int left = pos + Vector2Int.left;
                Vector2Int right = pos + Vector2Int.right;
                Vector2Int up = pos + Vector2Int.up;
                Vector2Int down = pos + Vector2Int.down;

                bool hasLeft = cells.Contains(left);
                bool hasRight = cells.Contains(right);
                bool hasUp = cells.Contains(up);
                bool hasDown = cells.Contains(down);

                Rect cell = new Rect(
                    center.x + pos.x * cellSize,
                    center.y - pos.y * cellSize,
                    cellSize,
                    cellSize);

                DrawOutline(cell, color, hasLeft, hasRight, hasUp, hasDown);
            }
        }
    }

    private void DrawOutline(Rect rect, Color fill, bool left, bool right, bool up, bool down)
    {
        EditorGUI.DrawRect(rect, fill);

        Color outline = Color.black;

        float t = 1f; // thickness

        // LEFT
        if (!left)
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, t, rect.height), outline);

        // RIGHT
        if (!right)
            EditorGUI.DrawRect(new Rect(rect.xMax - t, rect.y, t, rect.height), outline);

        // TOP
        if (!up)
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, t), outline);

        // BOTTOM
        if (!down)
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - t, rect.width, t), outline);
    }

    private Color GetRoomColor(RoomGrid room)
    {
        switch (room.roomType)
        {
            case RoomGridType.start:
                return Color.green;

            case RoomGridType.normal:
                return Color.gray;

            case RoomGridType.elite:
                return Color.yellow;

            case RoomGridType.boss:
                return Color.red;

            default:
                return Color.cyan;
        }
    }
}

#endif