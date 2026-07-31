#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyPatternCollectionSO))]
public class EnemyPatternCollectionPreviewEditor : Editor
{
    private static Color mainPreviewNameClr = Color.black;
    private static Color elementNameClr = Color.white;
    private static Color arrowClr = Color.gray;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemyPatternCollectionSO SO = (EnemyPatternCollectionSO)target;

        EditorGUILayout.LabelField("Must Apply before Play");
        if (GUILayout.Button("Apply (Sort)"))
        {
            SO.SortPatterns();
        }

        EditorGUILayout.Space(20); 
        GUI.contentColor = mainPreviewNameClr;
        EditorGUILayout.LabelField("[ Pattern Preview ]", EditorStyles.boldLabel);

        for (int i = 0; i < SO.patterns.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            TextPatternSequence(SO.patterns[i].so);
            EditorGUILayout.EndHorizontal();
        }

    }

    public void TextPatternSequence(EnemyPatternSequenceSO sequenceSO)
    {
        var patterns = sequenceSO.Patterns;
        for (int i = 0; i < patterns.Count; i++)
        {
            TextPatternElement(patterns[i]);
            if (i != patterns.Count - 1)
                TextArrow();
        }
    }

    public void TextPatternElement(EnemyPatternSO pattern)
    {
        GUI.contentColor = elementNameClr;

        PatternPreviewElement element = pattern.GetPreview();
        string name = element.name;
        float width = EditorStyles.label.CalcSize(new GUIContent(name)).x;
        GUILayout.Label(name, GUILayout.Width(width + 6));
    }

    public void TextArrow()
    {
        GUI.contentColor = arrowClr;

        string name = "¢º";
        float width = EditorStyles.label.CalcSize(new GUIContent(name)).x;
        GUILayout.Label(name, GUILayout.Width(width + 6));
    }
}
#endif