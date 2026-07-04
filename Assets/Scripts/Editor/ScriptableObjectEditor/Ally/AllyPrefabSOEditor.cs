#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AllyPrefabSO))]
public class AllyPrefabSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12);

        AllyPrefabSO iconSO = (AllyPrefabSO)target;

        if (GUILayout.Button("Collect Card Sprites"))
        {
            Undo.RecordObject(iconSO, "Collect Card Sprites");

            CollectAllyCardSprites(iconSO);

            EditorUtility.SetDirty(iconSO);
            AssetDatabase.SaveAssets();
        }
    }

    private void CollectAllyCardSprites(AllyPrefabSO iconSO)
    {
        var leng = iconSO.allyCardTextures;
        iconSO.allyCardIcons = new SerializableArray<Sprite>[leng.Length];
        for (int i = 0; i < leng.Length; i++)
        {
            Texture2D[] texture = iconSO.allyCardTextures[i].array;
            iconSO.allyCardIcons[i] = new SerializableArray<Sprite>();
            if (CollectSprites(texture, out List<Sprite> spriteList, out string wariningLog_ModuleItem))
            {
                iconSO.allyCardIcons[i].array = spriteList.ToArray();
                Debug.Log($"Ally Card Sprites 수집 완료: {iconSO.allyCardIcons[i].array.Length}개");
            }
            else
            {
                Debug.LogWarning(wariningLog_ModuleItem, iconSO);
            }
        }

    }

    private bool CollectSprites(Texture2D[] textureArray, out List<Sprite> spriteList, out string warningLog)
    {
        spriteList = new List<Sprite>();
        warningLog = "";
        if (textureArray == null || textureArray.Length == 0)
        {
            warningLog = "Textures가 비어있습니다.";
            return false;
        }

        if (!CollectSubSprites(textureArray, spriteList, out warningLog))
            return false;

        return true;
    }

    private bool CollectSubSprites(Texture2D[] textureArray, List<Sprite> spriteList, out string warningLog)
    {
        warningLog = "";
        for (int i = 0; i < textureArray.Length; i++)
        {
            Texture2D texture = textureArray[i];

            if (texture == null)
                continue;

            string path = AssetDatabase.GetAssetPath(texture);

            if (string.IsNullOrEmpty(path))
            {
                warningLog = $"Texture 경로를 찾을 수 없습니다: {texture.name}";
                return false;
            }

            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

            for (int j = 0; j < subAssets.Length; j++)
            {
                if (!(subAssets[j] is Sprite sprite))
                {
                    warningLog = $"Sprite가 아닙니다: {subAssets[j].name}";
                    return false;
                }

                spriteList.Add(sprite);
            }
        }

        return true;
    }
}

#endif