using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class SpriteNameReplaceTool : EditorWindow
{
    private Texture2D texture;

    private string findText = "";
    private string replaceText = "";

    [MenuItem("Tools/Sprite Name Replace")]
    static void Open()
    {
        GetWindow<SpriteNameReplaceTool>("Sprite Name Replace");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Multiple Name Replace", EditorStyles.boldLabel);

        texture = (Texture2D)EditorGUILayout.ObjectField(
            "Texture",
            texture,
            typeof(Texture2D),
            false);

        findText = EditorGUILayout.TextField("Find", findText);
        replaceText = EditorGUILayout.TextField("Replace", replaceText);

        GUI.enabled = texture != null && !string.IsNullOrEmpty(findText);

        if (GUILayout.Button("Replace"))
        {
            ReplaceNames();
        }

        GUI.enabled = true;
    }
    private void ReplaceNames()
    {
        string path = AssetDatabase.GetAssetPath(texture);

        TextureImporter importer =
            AssetImporter.GetAtPath(path) as TextureImporter;

        var factory = new SpriteDataProviderFactories();
        factory.Init();

        var dataProvider =
            factory.GetSpriteEditorDataProviderFromObject(importer);

        dataProvider.InitSpriteEditorDataProvider();

        var rects = dataProvider.GetSpriteRects();

        int changed = 0;

        for (int i = 0; i < rects.Length; i++)
        {
            string oldName = rects[i].name;
            string newName = oldName.Replace(findText, replaceText);

            if (oldName != newName)
            {
                rects[i].name = newName;
                changed++;
            }
        }

        dataProvider.SetSpriteRects(rects);
        dataProvider.Apply();

        importer.SaveAndReimport();

        Debug.Log($"{changed}개의 Sprite 이름을 변경했습니다.");
    }
}