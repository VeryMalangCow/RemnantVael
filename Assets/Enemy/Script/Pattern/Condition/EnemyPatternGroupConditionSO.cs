using UnityEngine;

[CreateAssetMenu(fileName = "E999_GroupSO",
    menuName = "ScriptableObject/EnemyPatternSO/Condition/Group")]

public class EnemyPatternGroupConditionSO : EnemyPatternConditionSO
{
    [SerializeField] private BoolOperator boolOper;
    [SerializeField] private EnemyPatternConditionSO[] conditions;

    public override bool IsSatisfied(EnemyAIContext aiContext)
    {
        switch (boolOper)
        {
            case BoolOperator.And:
                return IsSatisfiedByAndCondition(aiContext);
            case BoolOperator.Or:
                return IsSatisfiedByOrCondition(aiContext);
        }

        return false;
    }

    private bool IsSatisfiedByAndCondition(EnemyAIContext context)
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

    private bool IsSatisfiedByOrCondition(EnemyAIContext context)
    {
        if (conditions == null || conditions.Length == 0)
            return true;

        for (int i = 0; i < conditions.Length; i++)
        {
            if (conditions[i] == null)
                continue;

            if (conditions[i].IsSatisfied(context))
                return true;
        }

        return false;
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        throw new System.NotImplementedException();
    }
#endif

}

public enum BoolOperator
{
    And, Or
}