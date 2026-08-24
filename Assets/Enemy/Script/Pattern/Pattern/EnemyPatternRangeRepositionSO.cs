using System.Collections;
using UnityEngine;

[CreateAssetMenu(
    fileName = "E999_RangeRepositionSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Range Reposition")]
public class EnemyPatternRangeRepositionSO : EnemyPatternRepositionSO
{
    [Header("=== Connector (Range)")]
    [SerializeField] private EnemyPatternRangeSO rangeSO;

    // Pattern PLAY
    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        EnemyController enemy = aiContext.enemy;

        if (aiContext.targetPos == Vector2.zero)
            aiContext.targetPos = aiContext.player.transform.position;

        enemy.SetLookAtPos(aiContext.targetPos);
        yield return new WaitForSeconds(startDelay);

        Vector2 enemyPos = enemy.transform.position;

        // 1. 후보 위치 생성
        // 2. 후보 위치 검증
        // 3. 공격 가능한 후보만 수집
        // 4. 가장 적합한 후보 선택

        Vector2 targetPos;

        if (!TryFindRepositionTarget(aiContext, enemyPos, out targetPos))
        {
            yield break;
        }

        // 5. 선택된 위치로 이동
        yield return MoveToTarget(enemy, targetPos);

        yield return new WaitForSeconds(endDelay);
        enemy.SetLookAtTarget();
    }

    // 크기 반지름 계산
    protected override float GetRadius(EnemyAIContext aiContext)
        => rangeSO.size.x * rangeSO.bulletShadowScale.x * 0.5f;
    

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        float radius = GetRadius(null);
        float moveDis = (radius * 3f) + 0.1f;

        return new PatternPreviewElement("Reposition",
            new PatternPreviewDetail($"Speed: {followSO.speed}", Color.cyan),
            new PatternPreviewDetail($"Dis: {moveDis}", Color.red),
            new PatternPreviewDetail($"Radius: {radius}", Color.green),
            new PatternPreviewDescription("Move to the possible location attacked."));

    }
#endif
}


