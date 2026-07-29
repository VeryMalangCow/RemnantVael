using UnityEngine;

[CreateAssetMenu(fileName = "DistanceConditionSO", 
    menuName = "ScriptableObject/EnemyPatternSO/Condition/Distance")]
public class EnemyPatternDistanceConditionSO : EnemyPatternConditionSO
{
    [Header("Distance")]
    [SerializeField] private bool farMode;
    [SerializeField] private float targetRange;

    public override bool CanPlayPattern(EnemyAIContext aiContext)
    {
        float currentDis = Vector2.Distance(aiContext.player.transform.position, aiContext.enemy.transform.position);

        return farMode ? 
            currentDis >= targetRange : 
            currentDis <= targetRange;
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        string name = "Distance";
        PatternPreviewDetail left = new PatternPreviewDetail("Dis", new Color(1f, 0.6f, 0f, 1f));
        Color operClr = new Color(1f, 0.3f, 0f, 1f);
        Color valueClr = Color.red;

        return farMode
            ? new PatternPreviewElement(name, left, 
            new PatternPreviewDetail(">=", operClr), 
            new PatternPreviewDetail($"{targetRange}m (farther)", valueClr))
            : new PatternPreviewElement(name, left, 
            new PatternPreviewDetail("<=", operClr), 
            new PatternPreviewDetail($"{targetRange}m (closer)", valueClr));
    }
#endif
}
