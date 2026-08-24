using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "E999_SequenceSO_000", 
    menuName = "ScriptableObject/EnemyPatternSO/Sequence")]
public class EnemyPatternSequenceSO : ScriptableObject
{
    [SerializeField] private EnemyPatternConditionSO[] conditions;
    [SerializeField] private EnemyPatternSO[] patterns;

    public IReadOnlyList<EnemyPatternSO> Patterns => patterns;
    public IReadOnlyList<EnemyPatternConditionSO> Conditions => conditions;


    public bool IsSatisfied(EnemyAIContext context)
    {
        if (conditions == null || conditions.Length == 0)
            return true;

        for (int i = 0; i < conditions.Length; i++)
        {
            if (conditions[i] == null)
                continue;

            if (!conditions[i].IsSatisfied(context))
                return false;
        }

        return true;
    }
}
