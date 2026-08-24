#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyPatternSequenceSO))]
public class EnemyPatternSequencePreviewEditor : Editor
{
    private static Color mainPreviewNameClr = Color.black;
    private static Color subPreviewNameClr = new Color(0.1f, 0.1f, 0.1f, 1f);
    private static Color elementNameClr = Color.white;
    private static Color descriptionClr = new Color(0.5f, 0.5f, 0.5f, 1f);

    private static float detailIntervalX = 80;

    private Dictionary<Object, bool> foldouts = new();

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

        if (SO.ConditionGroupRoot == null)
            return;

        TextCondition(SO.ConditionGroupRoot);
    }

    private void TextCondition(EnemyPatternConditionSO conditionSO)
    {
        if (conditionSO == null)
            return;

        if (conditionSO is EnemyPatternGroupConditionSO groupSO)
        {
            TextConditionGroup(groupSO);
            return;
        }

        TextNormalCondition(conditionSO);
    }

    private void TextConditionGroup(EnemyPatternGroupConditionSO groupSO)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Group Operator
        string operatorText = groupSO.BoolOper == BoolOperator.And
            ? "[ AND ]"
            : "[ OR ]";

        EditorGUILayout.LabelField(
            operatorText,
            EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        var conditions = groupSO.Conditions;

        if (conditions != null)
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                EnemyPatternConditionSO condition = conditions[i];

                if (condition == null)
                    continue;

                TextCondition(condition);
            }
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();
    }

    private void TextNormalCondition(EnemyPatternConditionSO conditionSO)
    {
        var preview = conditionSO.GetPreview();

        TextElement(preview);
        FoldOutDescription(conditionSO, preview.desc.desc);
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

        if (SO.Patterns == null || SO.Patterns.Count == 0)
            return;

        foreach (var pattern in SO.Patterns)
            TextPattern(pattern);
    }

    private void TextPattern(EnemyPatternSO patternSO)
    {
        if (patternSO == null)
            return;

        Rect rect = EditorGUILayout.GetControlRect(false, 0f);
        var preview = patternSO.GetPreview();
        TextElement(preview);
        FoldOutDescription(patternSO, preview.desc.desc);
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


    // Foldout Description
    private void FoldOutDescription(Object SO, string desc)
    {
        GUI.contentColor = descriptionClr;

        bool state = GetFoldoutState(SO);
        state = EditorGUILayout.Foldout(state, "Description");

        foldouts[SO] = state;
        
        if (state)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(20);
                GUILayout.Label(desc);
            }
        }
    }

    private bool GetFoldoutState(Object SO)
    {
        if (!foldouts.TryGetValue(SO, out bool state))
        {
            state = false;
            foldouts.Add(SO, state);
        }

        return state;
    }
}

#endif