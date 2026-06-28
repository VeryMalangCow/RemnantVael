#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageThemeSO))]
public class StageThemeSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12);

        StageThemeSO themeSO = (StageThemeSO)target;

        if (GUILayout.Button("Collect Stage Sprites"))
        {
            Undo.RecordObject(themeSO, "Collect Stage Sprites");

            CollectStageSprites(themeSO);
            CollectStageFieldObjSprites(themeSO);

            EditorUtility.SetDirty(themeSO);
            AssetDatabase.SaveAssets();
        }
    }

    private void CollectStageSprites(StageThemeSO themeSO)
    {
        if (themeSO.stageTextures == null || themeSO.stageTextures.Length == 0)
        {
            Debug.LogWarning("stageTextures가 비어있습니다.", themeSO);
            return;
        }

        if (themeSO.stageAllSprites == null)
            themeSO.stageAllSprites = new List<Sprite>();
        else
            themeSO.stageAllSprites.Clear();

        string prefix = $"Map{themeSO.stageId:00}_";

        // 예: Map00_000, Map00_001 만 허용
        // Map00A_000, Map00F_000 등은 무시
        Regex regex = new Regex("^" + Regex.Escape(prefix) + @"(\d+)$");

        List<SpriteEntry> spriteEntries = new List<SpriteEntry>();

        CollectSubSprites(themeSO, (sprite) =>
        {
            Match match = regex.Match(sprite.name);

            if (!match.Success)
                return;

            if (!int.TryParse(match.Groups[1].Value, out int id))
                return;

            spriteEntries.Add(new SpriteEntry(id, sprite));
        });

        spriteEntries.Sort((a, b) => a.id.CompareTo(b.id));

        HashSet<int> usedIds = new HashSet<int>();

        for (int i = 0; i < spriteEntries.Count; i++)
        {
            SpriteEntry entry = spriteEntries[i];

            if (!usedIds.Add(entry.id))
            {
                Debug.LogWarning($"중복 stage sprite ID 발견: {entry.id} / {entry.sprite.name}", themeSO);
                continue;
            }

            themeSO.stageAllSprites.Add(entry.sprite);
        }

        Debug.Log($"stageAllSprites 수집 완료: {themeSO.stageAllSprites.Count}개 / Prefix: {prefix}", themeSO);
    }

    private void CollectStageFieldObjSprites(StageThemeSO themeSO)
    {
        if (themeSO.stageTextures == null || themeSO.stageTextures.Length == 0)
        {
            Debug.LogWarning("stageTextures가 비어있습니다.", themeSO);
            return;
        }

        string prefix = $"Map{themeSO.stageId:00}F_";

        // 예:
        // Map00F_00_000
        // Map00F_02_001
        //
        // Group 1 = fieldObj index
        // Group 2 = sprite index
        Regex regex = new Regex("^" + Regex.Escape(prefix) + @"(\d+)_(\d+)$");

        Dictionary<int, List<FieldObjSpriteEntry>> groupedEntries =
            new Dictionary<int, List<FieldObjSpriteEntry>>();

        int maxFieldObjIndex = -1;

        CollectSubSprites(themeSO, (sprite) =>
        {
            Match match = regex.Match(sprite.name);

            if (!match.Success)
                return;

            if (!int.TryParse(match.Groups[1].Value, out int fieldObjIndex))
                return;

            if (!int.TryParse(match.Groups[2].Value, out int spriteIndex))
                return;

            if (!groupedEntries.TryGetValue(fieldObjIndex, out List<FieldObjSpriteEntry> list))
            {
                list = new List<FieldObjSpriteEntry>();
                groupedEntries.Add(fieldObjIndex, list);
            }

            list.Add(new FieldObjSpriteEntry(spriteIndex, sprite));

            if (fieldObjIndex > maxFieldObjIndex)
                maxFieldObjIndex = fieldObjIndex;
        });

        if (maxFieldObjIndex < 0)
        {
            themeSO.fieldObjSprites = new SerializableArray<Sprite>[0];
            Debug.LogWarning($"FieldObj Sprite를 찾지 못했습니다. Prefix: {prefix}", themeSO);
            return;
        }

        themeSO.fieldObjSprites = new SerializableArray<Sprite>[maxFieldObjIndex + 1];

        for (int fieldObjIndex = 0; fieldObjIndex <= maxFieldObjIndex; fieldObjIndex++)
        {
            if (!groupedEntries.TryGetValue(fieldObjIndex, out List<FieldObjSpriteEntry> list))
            {
                themeSO.fieldObjSprites[fieldObjIndex] = CreateSpriteArray(new Sprite[0]);
                continue;
            }

            list.Sort((a, b) => a.spriteIndex.CompareTo(b.spriteIndex));

            int maxSpriteIndex = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].spriteIndex > maxSpriteIndex)
                    maxSpriteIndex = list[i].spriteIndex;
            }

            Sprite[] sprites = new Sprite[maxSpriteIndex + 1];
            HashSet<int> usedSpriteIndices = new HashSet<int>();

            for (int i = 0; i < list.Count; i++)
            {
                FieldObjSpriteEntry entry = list[i];

                if (!usedSpriteIndices.Add(entry.spriteIndex))
                {
                    Debug.LogWarning(
                        $"중복 FieldObj Sprite index 발견: FieldObj {fieldObjIndex}, SpriteIndex {entry.spriteIndex}, Sprite: {entry.sprite.name}",
                        themeSO
                    );
                    continue;
                }

                sprites[entry.spriteIndex] = entry.sprite;
            }

            themeSO.fieldObjSprites[fieldObjIndex] = CreateSpriteArray(sprites);
        }

        Debug.Log($"fieldObjSprites 수집 완료: {themeSO.fieldObjSprites.Length}종 / Prefix: {prefix}", themeSO);
    }

    private void CollectSubSprites(StageThemeSO themeSO, System.Action<Sprite> onSpriteFound)
    {
        for (int i = 0; i < themeSO.stageTextures.Length; i++)
        {
            Texture2D texture = themeSO.stageTextures[i];

            if (texture == null)
                continue;

            string path = AssetDatabase.GetAssetPath(texture);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning($"Texture 경로를 찾을 수 없습니다: {texture.name}", themeSO);
                continue;
            }

            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

            for (int j = 0; j < subAssets.Length; j++)
            {
                if (!(subAssets[j] is Sprite sprite))
                    continue;

                onSpriteFound?.Invoke(sprite);
            }
        }
    }

    private SerializableArray<Sprite> CreateSpriteArray(Sprite[] sprites)
    {
        SerializableArray<Sprite> result = new SerializableArray<Sprite>();
        result.array = sprites;
        return result;
    }

    private readonly struct SpriteEntry
    {
        public readonly int id;
        public readonly Sprite sprite;

        public SpriteEntry(int id, Sprite sprite)
        {
            this.id = id;
            this.sprite = sprite;
        }
    }

    private readonly struct FieldObjSpriteEntry
    {
        public readonly int spriteIndex;
        public readonly Sprite sprite;

        public FieldObjSpriteEntry(int spriteIndex, Sprite sprite)
        {
            this.spriteIndex = spriteIndex;
            this.sprite = sprite;
        }
    }
}
#endif