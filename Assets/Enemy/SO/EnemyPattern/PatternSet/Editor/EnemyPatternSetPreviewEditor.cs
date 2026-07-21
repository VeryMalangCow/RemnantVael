#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyPatternSetSO))]
public class EnemyPatternSetPreviewEditor : Editor
{
    private static Color mainPreviewNameClr = Color.black;
    private static Color subPreviewNameClr = new Color(0.1f, 0.1f, 0.1f, 1f);
    private static Color elementNameClr = new Color(1f, 1f, 1f, 1f);

    private static float detailIntervalX = 80;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemyPatternSetSO SO = (EnemyPatternSetSO)target;

        EditorGUILayout.Space(20); 
        GUI.contentColor = mainPreviewNameClr;
        EditorGUILayout.LabelField("[ Pattern Preview ]", EditorStyles.boldLabel);

        TextConditions(SO);

        TextPatterns(SO);
    }

    // Condition
    private void TextConditions(EnemyPatternSetSO SO)
    {
        EditorGUILayout.Space(16);
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, subPreviewNameClr);

        GUI.contentColor = subPreviewNameClr;
        EditorGUILayout.LabelField("( Condition )", EditorStyles.boldLabel);

        Rect rect2 = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect2, subPreviewNameClr);

        foreach (var condition in SO.conditions)
            TextCondition(condition);
    }

    private void TextCondition(EnemyPatternConditionSO conditionSO)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 0f);

        var preview = conditionSO.GetPreview();
        TextElement(preview);
    }


    // Pattern
    private void TextPatterns(EnemyPatternSetSO SO)
    {
        EditorGUILayout.Space(16);
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, subPreviewNameClr);
        
        GUI.contentColor = subPreviewNameClr;
        EditorGUILayout.LabelField("( Pattern )", EditorStyles.boldLabel);

        Rect rect2 = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect2, subPreviewNameClr);

        foreach (var pattern in SO.patterns)
            TextPattern(pattern);
    }

    private void TextPattern(EnemyPatternSO patternSO)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 0f);
        var preview = patternSO.GetPreview();
        TextElement(preview);
    }


    // Element & Detail
    private void TextElement(PatternPreviewElement element)
    {
        GUI.contentColor = elementNameClr;
        GUILayout.Label(element.name, EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(detailIntervalX);
            TextDetail(element.left, 100f);
            TextDetail(element.oper, 100f);
            TextDetail(element.right, 100f);
        }
    }

    private void TextDetail(PatternPreviewDetail detail, float width = 0f)
    {
        GUI.contentColor = detail.clr;
        GUILayout.Label(detail.name, GUILayout.Width(width));
    }
}

#endif