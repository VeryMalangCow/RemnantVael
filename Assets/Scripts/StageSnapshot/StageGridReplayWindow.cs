#if UNITY_EDITOR

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

        Rect rect = GUILayoutUtility.GetRect(400, 400);

        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        DrawRooms(rect, snapshot);
    }

    private void DrawRooms(Rect rect, StageGridSnapshot snapshot)
    {
        const float baseCellSize = 28f;
        float cellSize = baseCellSize * zoom;

        Vector2 center = rect.center;

        foreach (RoomGrid room in snapshot.rooms)
        {
            Color color = GetRoomColor(room);

            foreach (Vector2Int pos in room.roomPos)
            {
                Rect cell = new Rect(
                    center.x + pos.x * cellSize,
                    center.y - pos.y * cellSize,
                    cellSize - 2,
                    cellSize - 2);

                EditorGUI.DrawRect(cell, color);
            }
        }
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