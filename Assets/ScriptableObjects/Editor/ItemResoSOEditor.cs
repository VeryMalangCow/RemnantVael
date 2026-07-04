#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemResoSO))]
public class ItemResoSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12);

        ItemResoSO iconSO = (ItemResoSO)target;

        if (GUILayout.Button("Collect Item Sprites"))
        {
            Undo.RecordObject(iconSO, "Collect Item Sprites");

            CollectModuleItemSprites(iconSO);
            CollectModuleSynhronySprites(iconSO);

            EditorUtility.SetDirty(iconSO);
            AssetDatabase.SaveAssets();
        }
    }

    private void CollectModuleItemSprites(ItemResoSO iconSO)
    {
        if (CollectSprites(iconSO.moduleItemTextures, iconSO.moduleItemSprites, out string wariningLog_ModuleItem))
            Debug.Log($"Module Item Sprites 수집 완료: {iconSO.moduleItemSprites.Count}개");
        else
            Debug.LogWarning(wariningLog_ModuleItem, iconSO);
    }

    private void CollectModuleSynhronySprites(ItemResoSO iconSO)
    {
        if (CollectSprites(iconSO.moduleSynhronyTextures, iconSO.moduleSynhronySprites, out string wariningLog_ModuleSynhrony))
            Debug.Log($"Module Synhrony Sprites 수집 완료: {iconSO.moduleItemSprites.Count}개");
        else
            Debug.LogWarning(wariningLog_ModuleSynhrony, iconSO);
    }

    private bool CollectSprites(Texture2D[] textureArray, List<Sprite> spriteList, out string warningLog)
    {
        warningLog = "";
        if (textureArray == null || textureArray.Length == 0)
        {
            warningLog = "Module Item Textures가 비어있습니다.";
            return false;
        }

        if (spriteList == null) spriteList = new List<Sprite>();
        else spriteList.Clear();

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