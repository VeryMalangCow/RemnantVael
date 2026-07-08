using UnityEditor;
using UnityEngine;

public class PrefabSpriteRendererReplaceTool : EditorWindow
{
    private GameObject prefab;

    [Header("Sprite")]
    private Sprite fromSprite;
    private Sprite toSprite;

    [Header("Material")]
    private Material fromMaterial;
    private Material toMaterial;

    [MenuItem("Tools/Prefab/Sprite Renderer Replace")]
    private static void Open()
    {
        GetWindow<PrefabSpriteRendererReplaceTool>("Sprite Replace");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField(
            "Prefab",
            prefab,
            typeof(GameObject),
            false);

        EditorGUILayout.Space(10);

        GUILayout.Label("Sprite Replace", EditorStyles.boldLabel);

        fromSprite = (Sprite)EditorGUILayout.ObjectField(
            "From",
            fromSprite,
            typeof(Sprite),
            false);

        toSprite = (Sprite)EditorGUILayout.ObjectField(
            "To",
            toSprite,
            typeof(Sprite),
            false);

        EditorGUILayout.Space(10);

        GUILayout.Label("Material Replace", EditorStyles.boldLabel);

        fromMaterial = (Material)EditorGUILayout.ObjectField(
            "From",
            fromMaterial,
            typeof(Material),
            false);

        toMaterial = (Material)EditorGUILayout.ObjectField(
            "To",
            toMaterial,
            typeof(Material),
            false);

        EditorGUILayout.Space(20);

        GUI.enabled = prefab != null;

        if (GUILayout.Button("Replace", GUILayout.Height(35)))
        {
            Replace();
        }

        GUI.enabled = true;
    }

    private void Replace()
    {
        string path = AssetDatabase.GetAssetPath(prefab);

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Prefab이 아닙니다.");
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(path);

        SpriteRenderer[] renderers =
            root.GetComponentsInChildren<SpriteRenderer>(true);

        int spriteCount = 0;
        int materialCount = 0;

        foreach (SpriteRenderer sr in renderers)
        {
            if (fromSprite != null &&
                toSprite != null &&
                sr.sprite == fromSprite)
            {
                sr.sprite = toSprite;
                spriteCount++;
            }

            if (fromMaterial != null &&
                toMaterial != null &&
                sr.sharedMaterial == fromMaterial)
            {
                sr.sharedMaterial = toMaterial;
                materialCount++;
            }
        }

        PrefabUtility.SaveAsPrefabAsset(root, path);
        PrefabUtility.UnloadPrefabContents(root);

        Debug.Log(
            $"완료!\n" +
            $"Sprite 변경 : {spriteCount}\n" +
            $"Material 변경 : {materialCount}");
    }
}