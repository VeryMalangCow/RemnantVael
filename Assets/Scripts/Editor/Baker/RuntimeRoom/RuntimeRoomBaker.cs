using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

/*
Tools/Bake Runtime Room 메뉴를 통해 선택한 Room 프리팹을 Bake
Tools/Bake Runtime All Rooms 메뉴를 통해 Editor 폴더의 모든 Room 프리팹을 Bake
전체 Bake 과정을 조율하는 오케스트레이터 역할 수행
*/

/// <summary> Room Prefab을 Runtime용 Prefab으로 Bake </summary>
public static class RuntimeRoomBaker
{
    private const string EditorRoot = "Assets/Stages/Rooms/Editor";
    private const string PrefabFilter = "t:Prefab";

    [MenuItem("Tools/Baker/Room/Bake Runtime Room")]
    private static void BakeSelectedRoom()
    {
        GameObject prefab = Selection.activeObject as GameObject;

        if (prefab == null)
        {
            UnityEngine.Debug.LogWarning("Please select a Room prefab.");
            return;
        }

        BakeResult result = BakeRoom(prefab);
        BakeLogger.Log(result);
    }

    [MenuItem("Tools/Baker/Room/Bake Runtime All Rooms")]
    private static void BakeAllRooms()
    {
        string[] guids = AssetDatabase.FindAssets(PrefabFilter, new[] { EditorRoot });

        List<BakeResult> results = new();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                continue;

            results.Add(BakeRoom(prefab));
        }

        BakeLogger.LogSummary(results);
    }

    private static BakeResult BakeRoom(GameObject sourcePrefab)
    {
        BakeResult result = new();

        string editorPath = AssetDatabase.GetAssetPath(sourcePrefab);

        result.TargetName = sourcePrefab.name;

        if (!BakeUtility.IsEditorPath(editorPath))
        {
            result.AddWarning("Prefab is not located inside an Editor folder.");
            return result;
        }

        string runtimePath = BakeUtility.ConvertToRuntimePath(editorPath);
        result.RuntimePath = runtimePath;

        Stopwatch stopwatch = Stopwatch.StartNew();

        GameObject instance = PrefabUtility.InstantiatePrefab(sourcePrefab) as GameObject;

        RuntimeRoomProcessor.Process(instance, result);

        BakeUtility.EnsureFolderExists(runtimePath);

        PrefabUtility.SaveAsPrefabAsset(instance, runtimePath);

        Object.DestroyImmediate(instance);

        stopwatch.Stop();

        result.ElapsedTime = stopwatch.Elapsed;

        return result;
    }
}
