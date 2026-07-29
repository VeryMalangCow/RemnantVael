#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyPatternSequenceSO))]
public class EnemyPatternSequencePreviewEditor : Editor
{
    private static Color mainPreviewNameClr = Color.black;
    private static Color subPreviewNameClr = new Color(0.1f, 0.1f, 0.1f, 1f);
    private static Color elementNameClr = Color.white;

    private static float detailIntervalX = 80;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemyPatternSequenceSO SO = (EnemyPatternSequenceSO)target;

        EditorGUILayout.Space(20); 
        GUI.contentColor = mainPreviewNameClr;
        EditorGUILayout.LabelField("[ Pattern Preview ]", EditorStyles.boldLabel);

        TextConditions(SO);

        TextPatterns(SO);
    }

    // Condition
    private void TextConditions(EnemyPatternSequenceSO SO)
    {
        EditorGUILayout.Space(16);
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, subPreviewNameClr);

        GUI.contentColor = subPreviewNameClr;
        EditorGUILayout.LabelField("( Condition )", EditorStyles.boldLabel);

        Rect rect2 = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect2, subPreviewNameClr);

        foreach (var condition in SO.Conditions)
            TextCondition(condition);
    }

    private void TextCondition(EnemyPatternConditionSO conditionSO)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 0f);

        var preview = conditionSO.GetPreview();
        TextElement(preview);
    }


    // Pattern
    private void TextPatterns(EnemyPatternSequenceSO SO)
    {
        EditorGUILayout.Space(16);
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, subPreviewNameClr);
        
        GUI.contentColor = subPreviewNameClr;
        EditorGUILayout.LabelField("( Pattern )", EditorStyles.boldLabel);

        Rect rect2 = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect2, subPreviewNameClr);

        foreach (var pattern in SO.Patterns)
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
            TextDetail(element.left);
            TextDetail(element.oper);
            TextDetail(element.right);
        }
    }

    private void TextDetail(PatternPreviewDetail detail)
    {
        GUI.contentColor = detail.clr; 
        float width = EditorStyles.label.CalcSize(new GUIContent(detail.name)).x;
        GUILayout.Label(detail.name, GUILayout.Width(width + 10));
    }
}

#endif