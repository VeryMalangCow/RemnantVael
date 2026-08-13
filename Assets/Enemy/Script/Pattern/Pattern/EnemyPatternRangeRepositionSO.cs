using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "E999_RangeRepositionSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Range Reposition")]
public class EnemyPatternRangeRepositionSO : EnemyPatternSO
{
    private static readonly int candidateCount = 8;
    [SerializeField] private float arriveDistance = 0.1f;
    [SerializeField] private EnemyPatternFollowSO followSO;
    [SerializeField] private EnemyPatternRangeSO rangeSO;

    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        EnemyController enemy = aiContext.enemy;

        if (aiContext.targetPos == Vector2.zero)
            aiContext.targetPos = aiContext.player.transform.position;

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
    }

    private bool TryFindRepositionTarget(EnemyAIContext aiContext, Vector2 origin, out Vector2 target)
    {
        target = default;


        float radius = GetRadius();

        if (IsAttackPosition(aiContext, origin, radius))
            return false;
        
        
        float angleStep = 360f / candidateCount;
        float moveDis = radius * 3f;

        for (int i = 0; i < candidateCount; i++)
        {
            float angle = angleStep * i;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad));

            Vector2 candidate = origin + (dir * moveDis);

            if (!IsValidCandidate(aiContext, origin, candidate))
                continue;

            if (!IsAttackPosition(aiContext, candidate, radius))
                continue;

            target = candidate;
            return true;
        }

        return false;
    }

    private bool IsValidCandidate(EnemyAIContext aiContext, Vector2 origin, Vector2 candidate)
    {
        CapsuleCollider2D capsule = aiContext.mapJugeCol;
        var capsuleSize = capsule.size;
        var capsuleDir = capsule.direction;
        var capsuleAngle = capsule.transform.eulerAngles.z;

        // 후보 위치에 Enemy 몸체가 들어갈 수 있는지
        if (Physics2D.OverlapCapsule(
            candidate,
            capsuleSize,
            capsuleDir,
            capsuleAngle,
            aiContext.wallLayer))
        {
            return false;
        }

        // 현재 위치에서 후보까지 실제로 이동 가능한지
        Vector2 dir = candidate - origin;
        float distance = dir.magnitude;

        if (distance <= Mathf.Epsilon)
            return false;

        RaycastHit2D hit = Physics2D.CapsuleCast(
            origin,
            capsuleSize,
            capsuleDir,
            capsuleAngle,
            dir.normalized,
            distance,
            aiContext.wallLayer);

#if UNITY_EDITOR
        PatternDraw.DrawCircleCast(origin, candidate, 0.1f);
#endif

        return hit.collider == null;
    }

    private bool IsAttackPosition(EnemyAIContext aiContext, Vector2 position, float radius)
    {
        Vector2 target = aiContext.targetPos;

        Vector2 dir = target - position;
        float distance = dir.magnitude;

        if (distance <= Mathf.Epsilon)
            return false;

        // 탄환의 크기를 고려한 Cast
        RaycastHit2D hit = Physics2D.CircleCast(
            position,
            radius,
            dir.normalized,
            distance,
            aiContext.wallLayer);
#if UNITY_EDITOR
        PatternDraw.DrawCircleCast(position, target, radius);
#endif

        return hit.collider == null;
    }

    private IEnumerator MoveToTarget(EnemyController enemy, Vector2 target)
    {
        enemy.SetNavDir(target);
        enemy.SetMoveSpeed(followSO.speed);

        while (true)
        {
            Vector2 current = enemy.transform.position;

            if ((current - target).sqrMagnitude <= arriveDistance * arriveDistance)
                break;

            yield return null;
        }

        enemy.SetMoveSpeed(0);
        enemy.EndNav();
    }

    private float GetRadius()
    {
        return (rangeSO.size.x * rangeSO.bulletShadowScale.x * 0.5f) + 0.1f;
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        float radius = GetRadius();
        float moveDis = radius * 3f;

        return new PatternPreviewElement("Reposition",
            new PatternPreviewDetail($"Speed: {followSO.speed}", Color.cyan),
            new PatternPreviewDetail($"Dis: {moveDis}", Color.red),
            new PatternPreviewDetail($"Radius: {radius}", Color.green),
            new PatternPreviewDescription("Move to the possible location attacked."));

    }
#endif
}
