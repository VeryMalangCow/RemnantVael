#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageManager))]
public class StageGeneratePreviewEditor : Editor
{
    private const float PreviewHeight = 240f;
    private const float Padding = 12f;

    private static Dictionary<StageGridGenerator.RoomGridType, Color> clrDict = new Dictionary<StageGridGenerator.RoomGridType, Color> 
    {
        { StageGridGenerator.RoomGridType.normal, new Color(0.8f, 0.8f, 0.8f) },
        { StageGridGenerator.RoomGridType.boss, new Color(0.5f, 0.0f, 1.0f) },
        { StageGridGenerator.RoomGridType.vault, new Color(1.0f, 0.5f, 0.0f) },
        { StageGridGenerator.RoomGridType.baseShop, new Color(0.0f, 1.0f, 0.0f) },
        { StageGridGenerator.RoomGridType.allyShop, new Color(0.0f, 0.5f, 0.0f) },
        { StageGridGenerator.RoomGridType.stPrison, new Color(1.0f, 0.0f, 0.0f) },
        { StageGridGenerator.RoomGridType.utPrison, new Color(1.0f, 1.0f, 0.0f) },
        { StageGridGenerator.RoomGridType.ntPrison, new Color(0.0f, 1.0f, 1.0f) },
    };

    private static Color startClr = new Color(1f, 1f, 1f);

    private static Color gateClr = new Color(0.6f, 0.6f, 0.6f);

    public override void OnInspectorGUI()
    {

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("Room Preview", EditorStyles.boldLabel);

        DrawRoomLegend();

        EditorGUILayout.Space(6);

        StageManager preview = (StageManager)target;

        DrawRoomPreview(preview.StageGridGenerator.allRoomGrids);

        DrawDefaultInspector();
    }



    private void DrawRoomLegend()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Room Type", EditorStyles.boldLabel);

        DrawLegendItem(startClr, "Start Room");
        DrawLegendItem(clrDict[StageGridGenerator.RoomGridType.normal], "Normal Room");
        DrawLegendItem(clrDict[StageGridGenerator.RoomGridType.boss], "Boss Room");
        DrawLegendItem(clrDict[StageGridGenerator.RoomGridType.vault], "Vault Room");
        DrawLegendItem(clrDict[StageGridGenerator.RoomGridType.baseShop], "BaseShop Room");
        DrawLegendItem(clrDict[StageGridGenerator.RoomGridType.allyShop], "AllyShop Room");
        DrawLegendItem(clrDict[StageGridGenerator.RoomGridType.stPrison], "ST Room");
        DrawLegendItem(clrDict[StageGridGenerator.RoomGridType.utPrison], "UT Room");
        DrawLegendItem(clrDict[StageGridGenerator.RoomGridType.ntPrison], "NT Room");

        EditorGUILayout.EndVertical();


        EditorGUILayout.Space(3);


        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Gate Type", EditorStyles.boldLabel);

        DrawLegendItem(gateClr, "Gate");

        EditorGUILayout.EndVertical();
    }

    private void DrawLegendItem(Color color, string label)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 18f);

        Rect colorRect = new Rect(
            rect.x,
            rect.y + 3f,
            12f,
            12f
        );

        Rect labelRect = new Rect(
            rect.x + 18f,
            rect.y,
            rect.width - 18f,
            rect.height
        );

        EditorGUI.DrawRect(colorRect, color);
        DrawRectOutline(colorRect, Color.black, 1f);

        EditorGUI.LabelField(labelRect, label);
    }





    private void DrawRoomPreview(IReadOnlyList<StageGridGenerator.RoomGrid> rooms)
    {
        Rect previewRect = EditorGUILayout.GetControlRect(
            false,
            PreviewHeight,
            GUILayout.ExpandWidth(true)
        );

        EditorGUI.DrawRect(previewRect, new Color(0.12f, 0.12f, 0.12f));

        if (rooms == null || rooms.Count == 0)
        {
            EditorGUI.LabelField(previewRect, "No Room Data", GetCenteredLabelStyle());
            return;
        }

        GetBounds(rooms, out int minX, out int maxX, out int minY, out int maxY);

        int widthCount = maxX - minX + 1;
        int heightCount = maxY - minY + 1;

        float availableWidth = previewRect.width - Padding * 2f;
        float availableHeight = previewRect.height - Padding * 2f;

        float cellSize = Mathf.Min(
            availableWidth / widthCount,
            availableHeight / heightCount
        );

        float totalWidth = cellSize * widthCount;
        float totalHeight = cellSize * heightCount;

        float startX = previewRect.x + (previewRect.width - totalWidth) * 0.5f;
        float startY = previewRect.y + (previewRect.height - totalHeight) * 0.5f;

        // 기존 전체 그리드 라인을 그리고 싶지 않다면 이 함수 호출은 제거
        // DrawGrid(startX, startY, widthCount, heightCount, cellSize);

        for (int i = 0; i < rooms.Count; i++)
        {
            DrawRoomBlock(
                rooms[i],
                minX,
                maxY,
                startX,
                startY,
                cellSize
            );
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            DrawRoomGates(
                rooms[i],
                minX,
                maxY,
                startX,
                startY,
                cellSize
            );
        }
    }



    private void DrawRoomBlock(StageGridGenerator.RoomGrid roomData, int minX, int maxY, float startX, float startY, float cellSize)
    {
        if (roomData == null || roomData.roomPos == null)
            return;

        HashSet<Vector2Int> localSet = new HashSet<Vector2Int>(roomData.roomPos);

        Color fillColor = roomData.instanceId == 0
            ? startClr
            : clrDict[roomData.roomType];

        // 1. 방 내부 칸 채우기
        for (int i = 0; i < roomData.roomPos.Length; i++)
        {
            Vector2Int pos = roomData.roomPos[i];

            Rect cellRect = GetCellRect(
                pos,
                minX,
                maxY,
                startX,
                startY,
                cellSize
            );

            EditorGUI.DrawRect(cellRect, fillColor);
        }

        // 2. 같은 방 내부 선은 제외하고 외곽선만 그리기
        for (int i = 0; i < roomData.roomPos.Length; i++)
        {
            Vector2Int pos = roomData.roomPos[i];

            Rect cellRect = GetCellRect(
                pos,
                minX,
                maxY,
                startX,
                startY,
                cellSize
            );

            DrawCellOuterEdges(cellRect, pos, localSet, Color.black, 2f);
        }
    }

    private Rect GetCellRect(Vector2Int pos, int minX, int maxY, float startX, float startY, float cellSize)
    {
        int xIndex = pos.x - minX;
        int yIndex = maxY - pos.y;

        return new Rect(
            startX + xIndex * cellSize,
            startY + yIndex * cellSize,
            cellSize,
            cellSize
        );
    }

    private void DrawCellOuterEdges(Rect rect, Vector2Int pos, HashSet<Vector2Int> sameRoomSet, Color color, float thickness)
    {
        // 위쪽
        if (!sameRoomSet.Contains(pos + Vector2Int.up))
        {
            EditorGUI.DrawRect(
                new Rect(rect.x, rect.y, rect.width, thickness),
                color
            );
        }

        // 아래쪽
        if (!sameRoomSet.Contains(pos + Vector2Int.down))
        {
            EditorGUI.DrawRect(
                new Rect(rect.x, rect.yMax - thickness, rect.width, thickness),
                color
            );
        }

        // 왼쪽
        if (!sameRoomSet.Contains(pos + Vector2Int.left))
        {
            EditorGUI.DrawRect(
                new Rect(rect.x, rect.y, thickness, rect.height),
                color
            );
        }

        // 오른쪽
        if (!sameRoomSet.Contains(pos + Vector2Int.right))
        {
            EditorGUI.DrawRect(
                new Rect(rect.xMax - thickness, rect.y, thickness, rect.height),
                color
            );
        }
    }

    private void GetBounds(IReadOnlyList<StageGridGenerator.RoomGrid> rooms, out int minX, out int maxX, out int minY, out int maxY)
    {
        minX = int.MaxValue;
        maxX = int.MinValue;
        minY = int.MaxValue;
        maxY = int.MinValue;

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i] == null || rooms[i].roomPos == null)
                continue;

            for (int j = 0; j < rooms[i].roomPos.Length; j++)
            {
                Vector2Int pos = rooms[i].roomPos[j];

                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.y > maxY) maxY = pos.y;
            }
        }

        if (minX == int.MaxValue)
        {
            minX = maxX = minY = maxY = 0;
        }
    }

    private void DrawRectOutline(Rect rect, Color color, float thickness)
    {
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
        EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
        EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
    }

    private GUIStyle GetCenteredLabelStyle()
    {
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.gray;
        return style;
    }


    private void DrawRoomGates(
        StageGridGenerator.RoomGrid roomData,
        int minX,
        int maxY,
        float startX,
        float startY,
        float cellSize)
    {
        if (roomData == null || roomData.gates == null)
            return;

        for (int i = 0; i < roomData.gates.Count; i++)
        {
            StageGridGenerator.GateGrid gate = roomData.gates[i];

            Rect cellRect = GetCellRect(
                gate.pos,
                minX,
                maxY,
                startX,
                startY,
                cellSize
            );

            DrawGateInsideCell(cellRect, gate.dir, cellSize);
        }
    }

    private void DrawGateInsideCell(Rect cellRect, Vector2Int dir, float cellSize)
    {
        float gateLength = cellSize * 0.4f;
        float gateThickness = Mathf.Max(3f, cellSize * 0.12f);

        // 방 외곽선과 겹치지 않게 안쪽으로 조금 넣기
        float inset = Mathf.Max(3f, cellSize * 0.04f);

        Rect gateRect;
        if (dir == Vector2Int.up)
        {
            gateRect = new Rect(
                cellRect.center.x - gateLength * 0.5f,
                cellRect.y + inset,
                gateLength,
                gateThickness
            );
        }
        else if (dir == Vector2Int.down)
        {
            gateRect = new Rect(
                cellRect.center.x - gateLength * 0.5f,
                cellRect.yMax - inset - gateThickness,
                gateLength,
                gateThickness
            );
        }
        else if (dir == Vector2Int.left)
        {
            gateRect = new Rect(
                cellRect.x + inset,
                cellRect.center.y - gateLength * 0.5f,
                gateThickness,
                gateLength
            );
        }
        else if (dir == Vector2Int.right)
        {
            gateRect = new Rect(
                cellRect.xMax - inset - gateThickness,
                cellRect.center.y - gateLength * 0.5f,
                gateThickness,
                gateLength
            );
        }
        else
        {
            return;
        }

        EditorGUI.DrawRect(gateRect, gateClr);
        DrawRectOutline(gateRect, Color.black, 1f);
    }
}
#endif