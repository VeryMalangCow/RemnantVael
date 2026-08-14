using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPatternRepositionSO : EnemyPatternSO
{
    [Header("=== Move")]
    [SerializeField] protected float arriveDistance = 0.1f;
    [SerializeField] protected RepositionType repositionType;
    // Dir
    public static readonly int candidateCount = 8;

    [Header("=== Connector")]
    [SerializeField] protected EnemyPatternFollowSO followSO;

    // 이동할 위치 (이동 가능 -> 공격 가능 순)
    protected bool TryFindRepositionTarget(EnemyAIContext aiContext, Vector2 origin, out Vector2 target)
    {
        target = default;

        float radius = GetRadius(aiContext);
        /*
        if (IsAttackPosition(aiContext, origin, radius + 0.1f))
            return false;
        */
        float angleStep = 360f / candidateCount;
        float moveDis = (radius * 3f) + 0.1f;

        List<RepositionCandidate> candidates = new List<RepositionCandidate>(candidateCount);
        bool hasAttackableCandidate = false;

        for (int i = 0; i < candidateCount; i++)
        {
            float angle = angleStep * i;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad));

            Vector2 candidate = origin + (dir * moveDis);

            if (!IsValidCandidate(aiContext, origin, candidate))
                continue;

            bool isAttackPos = IsAttackPosition(aiContext, candidate, radius);

            if (isAttackPos)
            {
                if (!hasAttackableCandidate)
                {
                    candidates.Clear();
                    hasAttackableCandidate = true;
                }

                candidates.Add(new RepositionCandidate
                {
                    position = candidate
                });
            }
            else if (!hasAttackableCandidate)
            {
                candidates.Add(new RepositionCandidate
                {
                    position = candidate
                });
            }
        }

        if (candidates.Count == 0)
        {
            Debug.Log("후보 없음!!!");
            return false;
        }
        Debug.Log("후보: " + candidates.Count);

        // 나열 코드 추가
        SortCandidatesByDistance(candidates, aiContext.targetPos);
        switch (repositionType)
        {
            case RepositionType.Closest:
                target = candidates[0].position;
                break;

            case RepositionType.Median:
                target = candidates[(candidates.Count - 1) / 2].position;
                break;

            case RepositionType.Farthest:
                target = candidates[candidates.Count - 1].position;
                break;
        }

        return true;
    }

    // 해당 위치에 갈 수 있는가?
    protected bool IsValidCandidate(EnemyAIContext aiContext, Vector2 origin, Vector2 candidate)
    {
        CapsuleCollider2D capsule = aiContext.mapJugeCol;

        // Capsule의 크기 중 가장 긴 축을 기준으로 Circle 반지름 계산
        float radius = Mathf.Max(
            capsule.size.x,
            capsule.size.y) * 0.5f;

        // 후보 위치에 Enemy 몸체가 들어갈 수 있는지
        if (Physics2D.OverlapCircle(
            candidate,
            radius,
            aiContext.wallLayer))
        {
            return false;
        }

        // 현재 위치 → 후보 위치
        Vector2 dir = candidate - origin;
        float distance = dir.magnitude;

        if (distance <= Mathf.Epsilon)
            return false;

        Vector2 normalizedDir = dir / distance;

        // 시작과 끝에서 Circle 반지름만큼 제외
        float castDistance = distance - radius * 2f;

        // 두 Circle이 이미 겹칠 정도로 가까운 경우
        if (castDistance <= 0f)
            return true;

        Vector2 castStart =
            origin + normalizedDir * radius;

        RaycastHit2D hit = Physics2D.CircleCast(
            castStart,
            radius,
            normalizedDir,
            castDistance,
            aiContext.wallLayer);

#if UNITY_EDITOR
        Vector2 castEnd =
            candidate - normalizedDir * radius;

        PatternDraw.DrawCircleCast(
            castStart,
            castEnd,
            radius,
            Color.yellow);
#endif

        return hit.collider == null;
    }

    // 해당 위치가 공격 가능한가?
    protected bool IsAttackPosition(EnemyAIContext aiContext, Vector2 position, float radius)
    {
        Vector2 target = aiContext.targetPos;

        Vector2 dir = target - position;
        float distance = dir.magnitude;

        if (distance <= Mathf.Epsilon)
            return false;

        Vector2 normalizedDir = dir / distance;

        // CircleCast 시작점과 끝점을 반지름만큼 안쪽으로 조정
        float castDistance = distance - radius * 2f;

        // 두 지점 사이가 너무 짧다면 별도의 벽 판정이 필요하지 않음
        if (castDistance <= 0f)
            return true;

        Vector2 castStart = position + normalizedDir * radius;

        bool result = Physics2D.CircleCast(
            castStart,
            radius,
            normalizedDir,
            castDistance,
            aiContext.wallLayer).collider == null;

#if UNITY_EDITOR
        Vector2 castEnd = target - normalizedDir * radius;

        PatternDraw.DrawCircleCast(
            castStart,
            castEnd,
            radius,
            result ? Color.green : Color.red);
#endif

        return result;
    }

    // 위치로 이동 코루틴
    protected IEnumerator MoveToTarget(EnemyController enemy, Vector2 target)
    {
        Vector2 start = enemy.transform.position;

        float distance = Vector2.Distance(start, target);
        float speed = followSO.speed;

        if (speed <= Mathf.Epsilon)
            yield break;

        float maxMoveTime = distance / speed;
        float elapsedTime = 0f;

        enemy.SetNavDir(target);
        enemy.SetMoveSpeed(speed);

        while (elapsedTime < maxMoveTime)
        {
            Vector2 current = enemy.transform.position;

            // 정상적으로 목표에 도착
            if ((current - target).sqrMagnitude <=
                arriveDistance * arriveDistance)
            {
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        enemy.SetMoveSpeed(0);
        enemy.EndNav();
    }

    // 거리 정렬
    protected void SortCandidatesByDistance(List<RepositionCandidate> candidates, Vector2 targetPos)
    {
        for (int i = 0; i < candidates.Count - 1; i++)
        {
            int minIndex = i;

            float minDistance =
                (candidates[i].position - targetPos).sqrMagnitude;

            for (int j = i + 1; j < candidates.Count; j++)
            {
                float distance =
                    (candidates[j].position - targetPos).sqrMagnitude;

                if (distance < minDistance)
                {
                    minIndex = j;
                    minDistance = distance;
                }
            }

            if (minIndex == i)
                continue;

            RepositionCandidate temp = candidates[i];
            candidates[i] = candidates[minIndex];
            candidates[minIndex] = temp;
        }
    }

    // 반지름
    protected abstract float GetRadius(EnemyAIContext aiContext);

}
public enum RepositionType
{
    Closest, Farthest, Median
}
public struct RepositionCandidate // 추후 확장 고려
{
    public Vector2 position;
}
