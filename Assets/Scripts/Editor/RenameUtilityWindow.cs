// Assets/Editor/RenameUtilityWindow.cs
// Unity 2021.2+ 호환 / 서브 스프라이트 이름 변경은 ISpriteEditorDataProvider 경로 사용

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Sprites;   // SpriteDataProviderFactories, ISpriteEditorDataProvider, SpriteRect
using UnityEngine;
using UnityEngine.SceneManagement;

public class RenameUtilityWindow : EditorWindow
{
    [MenuItem("Tools/Rename Utility")]
    public static void Open() => GetWindow<RenameUtilityWindow>("Rename Utility");

    string findText = "Map00";
    string replaceText = "Map01";

    bool searchInProject = true;   // Project 창의 에셋 이름
    bool searchInHierarchy = false;  // 현재 씬의 GameObject 이름
    bool onlyInSelection = true;   // 선택 항목만

    void OnGUI()
    {
        EditorGUILayout.LabelField("Find & Replace", EditorStyles.boldLabel);
        findText = EditorGUILayout.TextField("Find", findText);
        replaceText = EditorGUILayout.TextField("Replace", replaceText);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Scope", EditorStyles.boldLabel);
        searchInProject = EditorGUILayout.Toggle("Project Assets", searchInProject);
        searchInHierarchy = EditorGUILayout.Toggle("Hierarchy (Scene)", searchInHierarchy);
        onlyInSelection = EditorGUILayout.Toggle("Only Selection", onlyInSelection);

        EditorGUILayout.Space(10);
        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(findText)))
        {
            if (GUILayout.Button("Preview & Apply", GUILayout.Height(28)))
            {
                int changed = 0;
                try
                {
                    AssetDatabase.StartAssetEditing();

                    if (searchInProject) changed += RenameInProject(onlyInSelection, findText, replaceText);
                    if (searchInHierarchy) changed += RenameInHierarchy(onlyInSelection, findText, replaceText);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
                finally { AssetDatabase.StopAssetEditing(); }

                EditorUtility.DisplayDialog("Rename Utility", $"변경된 항목: {changed}개", "OK");
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "• Project Assets: 파일/에셋 이름을 변경합니다.\n" +
            "• 멀티 슬라이스 텍스처의 서브 스프라이트 이름도 안전하게 변경합니다.\n" +
            "• Hierarchy: 씬의 GameObject 이름을 변경합니다.\n" +
            "• Only Selection: 선택된 항목만 대상으로 제한합니다.",
            MessageType.Info);
    }

    // ---------------- Project (Assets) ----------------

    static int RenameInProject(bool selectionOnly, string findText, string replaceText)
    {
        var targetPaths = new List<string>();

        if (selectionOnly)
        {
            foreach (var obj in Selection.objects)
            {
                var p = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(p)) targetPaths.Add(p);
            }
        }
        else
        {
            foreach (var guid in AssetDatabase.FindAssets("")) // 전체 프로젝트
                targetPaths.Add(AssetDatabase.GUIDToAssetPath(guid));
        }

        int count = 0;
        foreach (var path in targetPaths)
        {
            if (string.IsNullOrEmpty(path)) continue;

            // 1) 에셋 파일명 변경
            var name = Path.GetFileNameWithoutExtension(path);
            if (!string.IsNullOrEmpty(name) && name.Contains(findText))
            {
                var err = AssetDatabase.RenameAsset(path, name.Replace(findText, replaceText));
                if (string.IsNullOrEmpty(err)) count++;
            }

            // 2) 스프라이트시트 내부 서브 스프라이트 이름 변경
            count += RenameSubSprites(path, findText, replaceText);
        }
        return count;
    }

    /// <summary>
    /// ISpriteEditorDataProvider를 이용해 스프라이트시트(멀티) 내부 서브 스프라이트 이름을 일괄 변경
    /// </summary>
    static int RenameSubSprites(string assetPath, string findText, string replaceText)
    {
        // Texture2D 로드 (Sprite로 로드하면 DataProvider를 못 얻을 수 있음)
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (tex == null) return 0;

        // ★ 버전 호환: 팩토리를 인스턴스화해서 사용 (정적 호출 불가 환경 대비)
        var factories = new SpriteDataProviderFactories();
        factories.Init();

        var providerObj = factories.GetSpriteEditorDataProviderFromObject(tex);
        var provider = providerObj as ISpriteEditorDataProvider;
        if (provider == null) return 0;

        provider.InitSpriteEditorDataProvider();

        // 멀티 여부 및 현 스프라이트 Rect들
        var rects = provider.GetSpriteRects(); // SpriteRect[]
        if (rects == null || rects.Length == 0) return 0;

        int changed = 0;
        bool any = false;

        for (int i = 0; i < rects.Length; i++)
        {
            var r = rects[i];
            if (!string.IsNullOrEmpty(r.name) && r.name.Contains(findText))
            {
                r.name = r.name.Replace(findText, replaceText);
                rects[i] = r;
                changed++;
                any = true;
            }
        }

        if (any)
        {
            provider.SetSpriteRects(rects);
            provider.Apply(); // 변경 적용
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate); // 재임포트
        }

        return changed;
    }

    // ---------------- Hierarchy (Scene) ----------------

    static int RenameInHierarchy(bool selectionOnly, string findText, string replaceText)
    {
        GameObject[] roots;
        if (selectionOnly)
            roots = Selection.gameObjects;
        else
            roots = SceneManager.GetActiveScene().GetRootGameObjects();

        int count = 0;
        foreach (var root in roots)
        {
            if (root == null) continue;
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (!string.IsNullOrEmpty(t.name) && t.name.Contains(findText))
                {
                    t.name = t.name.Replace(findText, replaceText);
                    count++;
                }
            }
        }
        return count;
    }
}