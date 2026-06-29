using System.Linq;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StagePassageThemeSO))]
public class StagePassageThemeSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Collect Stage Sprites"))
        {
            BuildSpriteList((StagePassageThemeSO)target);
        }
    }

    private static void BuildSpriteList(StagePassageThemeSO so)
    {
        if (so.passageTexture == null)
        {
            Debug.LogError("Passage Texture가 없습니다.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(so.passageTexture);

        Sprite[] sprites = AssetDatabase
            .LoadAllAssetsAtPath(path)
            .OfType<Sprite>()
            .OrderBy(GetSpriteId)
            .ToArray();

        // ID 연속성 검사
        for (int i = 0; i < sprites.Length; i++)
        {
            int id = GetSpriteId(sprites[i]);

            if (id != i)
            {
                Debug.LogError(
                    $"Sprite ID가 올바르지 않습니다.\n" +
                    $"Expected : {i:000}\n" +
                    $"Found : {sprites[i].name}");
                return;
            }
        }

        so.sprites.Clear();
        so.sprites.AddRange(sprites);

        EditorUtility.SetDirty(so);
        AssetDatabase.SaveAssets();

        Debug.Log($"Build Complete ({sprites.Length} Sprites)");
    }

    private static int GetSpriteId(Sprite sprite)
    {
        string name = sprite.name;

        int index = name.LastIndexOf('_');

        if (index < 0)
            throw new Exception($"잘못된 Sprite 이름 : {name}");

        return int.Parse(name[(index + 1)..]);
    }
}
