using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SerializationRenameTool
{
    [MenuItem("Tools/Serialization/Apply FormerlySerializedAs (FULL PROJECT)")]
    public static void ApplyAll()
    {
        bool confirmed = EditorUtility.DisplayDialog(
            "Apply FormerlySerializedAs (FULL)",
            "프로젝트 전체 씬 + 프리팹을 다시 저장합니다.\n\n" +
            "시간이 오래 걸릴 수 있습니다.\n" +
            "반드시 백업 또는 Git 커밋 후 진행하세요.\n\n" +
            "계속하시겠습니까?",
            "실행",
            "취소");

        if (!confirmed)
            return;

        string originalScenePath = SceneManager.GetActiveScene().path;

        int sceneCount = 0;
        int prefabCount = 0;

        try
        {
            Debug.Log("=== FULL Serialization Apply Start ===");

            // -----------------------------
            // 1. 모든 씬 처리
            // -----------------------------
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });

            for (int i = 0; i < sceneGuids.Length; i++)
            {
                float progress = (float)i / sceneGuids.Length;
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);

                if (!scenePath.StartsWith("Assets/"))
                    continue;

                bool cancel = EditorUtility.DisplayCancelableProgressBar(
                    "Processing Scenes",
                    scenePath,
                    progress);

                if (cancel)
                    break;

                try
                {
                    var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                    if (!scene.IsValid() || !scene.isLoaded)
                        continue;

                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);

                    sceneCount++;
                    Debug.Log($"[Scene] Saved: {scenePath}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Scene] Failed: {scenePath}\n{ex}");
                }
            }

            // -----------------------------
            // 2. 모든 프리팹 처리
            // -----------------------------
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

            for (int i = 0; i < prefabGuids.Length; i++)
            {
                float progress = (float)i / prefabGuids.Length;
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);

                if (!prefabPath.StartsWith("Assets/"))
                    continue;

                bool cancel = EditorUtility.DisplayCancelableProgressBar(
                    "Processing Prefabs",
                    prefabPath,
                    progress);

                if (cancel)
                    break;

                GameObject root = null;

                try
                {
                    root = PrefabUtility.LoadPrefabContents(prefabPath);
                    if (root == null)
                        continue;

#if UNITY_2021_3_OR_NEWER
                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath, out bool success);
#else
                    var saved = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                    bool success = saved != null;
#endif

                    if (success)
                    {
                        prefabCount++;
                        Debug.Log($"[Prefab] Saved: {prefabPath}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Prefab] Failed: {prefabPath}\n{ex}");
                }
                finally
                {
                    if (root != null)
                        PrefabUtility.UnloadPrefabContents(root);
                }
            }

            // -----------------------------
            // 3. 저장
            // -----------------------------
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"=== DONE === Scenes:{sceneCount}, Prefabs:{prefabCount}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Process failed.\n{ex}");
        }
        finally
        {
            EditorUtility.ClearProgressBar();

            // 원래 씬 복구
            try
            {
                if (!string.IsNullOrEmpty(originalScenePath) &&
                    originalScenePath.StartsWith("Assets/"))
                {
                    EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
                }
            }
            catch { }
        }
    }
}