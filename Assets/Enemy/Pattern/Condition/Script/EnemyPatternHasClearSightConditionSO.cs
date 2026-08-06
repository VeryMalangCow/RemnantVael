using UnityEngine;

[CreateAssetMenu(fileName = "HasClearSightConditionSO",
    menuName = "ScriptableObject/EnemyPatternSO/Condition/HasClearSight")]
public class EnemyPatternHasClearSightConditionSO : EnemyPatternConditionSO
{
    [Header("=== HasClearSight")]
    [SerializeField] private float radius;

    public override bool IsSatisfied(EnemyAIContext aiContext)
    {
        Vector2 aPos = aiContext.enemy.transform.position;
        Vector2 bPos = aiContext.player.transform.position;
        Vector2 dir = bPos - aPos;
        float distance = dir.magnitude;

        if (distance <= Mathf.Epsilon)
            return false;

        return Physics2D.CircleCast(
            aPos,
            radius,
            dir / distance,
            distance,
            aiContext.wallLayer).collider == null;
    }
    
#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        return new PatternPreviewElement("HasClearSight", 
            new PatternPreviewDetail("Wall"),
            new PatternPreviewDetail("NoObstacle", Color.yellow),
            new PatternPreviewDetail($"Radius: {radius}m", Color.red),
            new PatternPreviewDescription($"There must be no obstacle of {radius * 2}m thickness between the player and the opponent."));

    }
#endif
}
