using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpriteSheetReplaceTool : EditorWindow
{
    private GameObject prefab;

    private Texture2D sourceTexture;
    private Texture2D targetTexture;

    [MenuItem("Tools/Sprite Sheet Replace")]
    private static void Open()
    {
        GetWindow<SpriteSheetReplaceTool>("Sprite Sheet Replace");
    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField(
            "Prefab",
            prefab,
            typeof(GameObject),
            false);

        sourceTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Source Texture",
            sourceTexture,
            typeof(Texture2D),
            false);

        targetTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Target Texture",
            targetTexture,
            typeof(Texture2D),
            false);

        GUI.enabled = prefab && sourceTexture && targetTexture;

        if (GUILayout.Button("Replace"))
        {
            Replace();
        }

        GUI.enabled = true;
    }

    private void Replace()
    {
        string prefabPath = AssetDatabase.GetAssetPath(prefab);

        GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);

        SpriteRenderer[] renderers =
            root.GetComponentsInChildren<SpriteRenderer>(true);

        //----------------------------------------
        // Target Sprite Dictionary
        //----------------------------------------

        string targetPath = AssetDatabase.GetAssetPath(targetTexture);

        Sprite[] targetSprites = AssetDatabase
            .LoadAllAssetRepresentationsAtPath(targetPath)
            .OfType<Sprite>()
            .ToArray();

        Dictionary<string, Sprite> targetMap = new();

        foreach (Sprite sprite in targetSprites)
        {
            targetMap[sprite.name] = sprite;
        }

        //----------------------------------------
        // Prefix 자동 추출
        //----------------------------------------

        string sourcePrefix = GetPrefix(sourceTexture);
        string targetPrefix = GetPrefix(targetTexture);

        Debug.Log($"Source Prefix : {sourcePrefix}");
        Debug.Log($"Target Prefix : {targetPrefix}");

        //----------------------------------------
        // Replace
        //----------------------------------------

        int changed = 0;

        foreach (SpriteRenderer sr in renderers)
        {
            if (sr.sprite == null)
                continue;

            string currentName = sr.sprite.name;

            if (!currentName.StartsWith(sourcePrefix))
                continue;

            string targetName =
                targetPrefix + currentName.Substring(sourcePrefix.Length);

            if (targetMap.TryGetValue(targetName, out Sprite newSprite))
            {
                sr.sprite = newSprite;
                changed++;

                Debug.Log($"{currentName} -> {targetName}");
            }
            else
            {
                Debug.LogWarning($"못찾음 : {targetName}");
            }
        }

        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        PrefabUtility.UnloadPrefabContents(root);

        Debug.Log($"완료 : {changed}개 변경");
    }

    private string GetPrefix(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);

        Sprite firstSprite = AssetDatabase
            .LoadAllAssetRepresentationsAtPath(path)
            .OfType<Sprite>()
            .FirstOrDefault();

        if (firstSprite == null)
            return "";

        string name = firstSprite.name;

        int index = name.IndexOf('_');

        if (index < 0)
            return "";

        return name.Substring(0, index + 1);
    }
}