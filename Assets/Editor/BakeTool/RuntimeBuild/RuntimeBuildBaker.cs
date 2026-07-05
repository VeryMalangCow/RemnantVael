using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

/*
Assets/Prefabs/Build/Editor 내부의 Build 프리팹을 대상으로 Bake 수행
선택한 프리팹 또는 전체 프리팹 Bake 지원
Runtime 경로 생성 및 저장
RuntimeBuildProcessor에 실제 변환 작업 위임
작업 결과를 BakeLogger로 출력
Build Bake 전체 과정을 관리하는 오케스트레이터 역할 
*/

/// <summary> Build Prefab을 Runtime용 Prefab으로 Bake </summary>
public static class RuntimeBuildBaker
{
    private const string EditorRoot = "Assets/Build/Prefab/Editor";
    private const string PrefabFilter = "t:Prefab";

    [MenuItem("Tools/Baker/Build/Bake Runtime Build")]
    private static void BakeSelectedBuild()
    {
        GameObject prefab = Selection.activeObject as GameObject;

        if (prefab == null)
        {
            UnityEngine.Debug.LogWarning("Please select a Build prefab.");
            return;
        }

        BakeResult result = BakeBuild(prefab);
        BakeLogger.Log(result);
    }

    [MenuItem("Tools/Baker/Build/Bake Runtime All Build")]
    private static void BakeAllBuild()
    {
        string[] guids = AssetDatabase.FindAssets(PrefabFilter, new[] { EditorRoot });

        List<BakeResult> results = new();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                continue;

            results.Add(BakeBuild(prefab));
        }

        BakeLogger.LogSummary(results);
    }

    private static BakeResult BakeBuild(GameObject sourcePrefab)
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

        RuntimeBuildProcessor.Process(instance, result);

        BakeUtility.EnsureFolderExists(runtimePath);

        PrefabUtility.SaveAsPrefabAsset(instance, runtimePath);

        Object.DestroyImmediate(instance);

        stopwatch.Stop();

        result.ElapsedTime = stopwatch.Elapsed;

        return result;
    }
}