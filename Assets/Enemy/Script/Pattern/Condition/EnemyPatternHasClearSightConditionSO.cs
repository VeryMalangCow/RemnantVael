using UnityEngine;

[CreateAssetMenu(
    fileName = "E999_HasClearSightSO",
    menuName = "ScriptableObject/EnemyPatternSO/Condition/HasClearSight")]
public class EnemyPatternHasClearSightConditionSO : EnemyPatternConditionSO
{
    [Header("=== HasClearSight")]
    [SerializeField] private bool Interference;
    [SerializeField] private float radius;

    public override bool IsSatisfied(EnemyAIContext aiContext)
    {
        Vector2 aPos = aiContext.enemy.transform.position;
        Vector2 bPos = aiContext.player.transform.position;

        Vector2 dir = bPos - aPos;
        float distance = dir.magnitude;

        if (distance <= Mathf.Epsilon)
            return false;

        bool result;

#if UNITY_EDITOR
        Color drawColor;
#endif

        if (radius <= Mathf.Epsilon)
        {
            result = Physics2D.Linecast(
                aPos, 
                bPos, 
                aiContext.wallLayer).collider == null;

#if UNITY_EDITOR
            drawColor = result ? Color.green : Color.red;
            PatternDraw.DrawLineCast(aPos, bPos, drawColor);
#endif
        }
        else
        {
            result = Physics2D.CircleCast(
                aPos,
                radius,
                dir / distance,
                distance,
                aiContext.wallLayer).collider == null;

#if UNITY_EDITOR
            drawColor = result ? Color.green : Color.red;
            PatternDraw.DrawCircleCast(aPos, bPos, radius, drawColor);
#endif
        }

        return Interference ? !result : result;
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        return new PatternPreviewElement("HasClearSight",
            new PatternPreviewDetail("Wall"),
            new PatternPreviewDetail(Interference ? "Need Obstacle" : "No Obstacle", Color.yellow),
            new PatternPreviewDetail($"Radius: {radius}m", Color.red),
            new PatternPreviewDescription(Interference ?
            $"There must be obstacle of {radius * 2}m thickness between the player and the opponent." :
            $"There must be no obstacle of {radius * 2}m thickness between the player and the opponent."));

    }
#endif

}
