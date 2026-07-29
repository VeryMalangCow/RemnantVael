using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPatternSetSO", 
    menuName = "ScriptableObject/EnemyPatternSO/PatternSet")]
public class EnemyPatternSequenceSO : ScriptableObject
{
    [SerializeField] private EnemyPatternConditionSO[] conditions;
    [SerializeField] private EnemyPatternSO[] patterns;

#if UNITY_EDITOR
    public EnemyPatternSO[] Patterns => patterns;
    public EnemyPatternConditionSO[] Conditions => conditions;
#endif

    public bool CanPlayPattern(EnemyAIContext context)
    {
        for (int i = 0; i < conditions.Length; i++)
        {
            if (!conditions[i].CanPlayPattern(context))
                return false;
        }

        return true;
    }
}
