using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "E999_SequenceSO_000", 
    menuName = "ScriptableObject/EnemyPatternSO/Sequence")]
public class EnemyPatternSequenceSO : ScriptableObject
{
    [SerializeField] private EnemyPatternGroupConditionSO conditionGroupRoot;
    [SerializeField] private EnemyPatternSO[] patterns;

    public EnemyPatternGroupConditionSO ConditionGroupRoot => conditionGroupRoot;
    public IReadOnlyList<EnemyPatternSO> Patterns => patterns;


    public bool IsSatisfied(EnemyAIContext context)
    {
        if (conditionGroupRoot == null)
            return true;

        return conditionGroupRoot.IsSatisfied(context);
    }
}
