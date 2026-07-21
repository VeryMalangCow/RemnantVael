using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPatternSetSO", menuName = "ScriptableObject/EnemyPatternSO/PatternSet")]
public class EnemyPatternSetSO : ScriptableObject
{
    public EnemyPatternConditionSO[] conditions;
    public EnemyPatternSO[] patterns;
}
