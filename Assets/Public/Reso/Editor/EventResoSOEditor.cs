#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(EventResoSO))]
public class EventResoSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12);

        EventResoSO iconSO = (EventResoSO)target;

        if (GUILayout.Button("Collect Card Sprites"))
        {
            Undo.RecordObject(iconSO, "Collect Card Sprites");

            CollectCutsceneSprites(iconSO);
            CollectDialogueSprites(iconSO);
            CollectInfoSprites(iconSO);

            EditorUtility.SetDirty(iconSO);
            AssetDatabase.SaveAssets();
        }
    }

    private void CollectDialogueSprites(EventResoSO iconSO)
    {
        if (CollectSprites(iconSO.dialogueTextures, out List<Sprite> spriteList, out string wariningLog_ModuleItem))
        {
            iconSO.dialogueSprites = spriteList.ToArray();
            Debug.Log($"Dialogue Sprites 수집 완료: {iconSO.dialogueTextures.Length}개");
        }
        else
        {
            Debug.LogWarning(wariningLog_ModuleItem, iconSO);
        }
    }

    private void CollectCutsceneSprites(EventResoSO iconSO)
    {
        if (CollectSprites(iconSO.cutsceneTextures, out List<Sprite> spriteList, out string wariningLog_ModuleItem))
        {
            iconSO.cutsceneSprites = spriteList.ToArray();
            Debug.Log($"Cutscene Sprites 수집 완료: {iconSO.cutsceneSprites.Length}개");
        }
        else
        {
            Debug.LogWarning(wariningLog_ModuleItem, iconSO);
        }
    }

    private void CollectInfoSprites(EventResoSO iconSO)
    {
        if (CollectSprites(iconSO.infoTextures, out List<Sprite> spriteList, out string wariningLog_ModuleItem))
        {
            iconSO.infoSprites = spriteList.ToArray();
            Debug.Log($"Info Sprites 수집 완료: {iconSO.cutsceneSprites.Length}개");
        }
        else
        {
            Debug.LogWarning(wariningLog_ModuleItem, iconSO);
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