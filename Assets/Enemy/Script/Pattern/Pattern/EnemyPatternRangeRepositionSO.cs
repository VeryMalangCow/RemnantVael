using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "E999_RangeRepositionSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Range Reposition")]
public class EnemyPatternRangeRepositionSO : EnemyPatternSO
{
    public enum RepositionType
    {
        Closest, Farthest, Median
    }

    private struct RepositionCandidate // 추후 확장 고려
    {
        public Vector2 position;
    }

    private static readonly int candidateCount = 8;
    [SerializeField] private float arriveDistance = 0.1f;
    [SerializeField] private EnemyPatternFollowSO followSO;
    [SerializeField] private EnemyPatternRangeSO rangeSO;
    [SerializeField] private RepositionType repositionType;

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

    private bool TryFindRepositionTarget(EnemyAIContext aiContext, Vector2 origin, out Vector2 target)
    {
        target = default;

        float radius = GetRadius();
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
    private bool IsValidCandidate(EnemyAIContext aiContext, Vector2 origin, Vector2 candidate)
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
    private bool IsAttackPosition(EnemyAIContext aiContext, Vector2 position, float radius)
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
    private IEnumerator MoveToTarget(EnemyController enemy, Vector2 target)
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

    // 위치: 가장 가까운
    private RepositionCandidate GetClosestPos(List<RepositionCandidate> candidates, Vector2 targetPos)
    {
        if (candidates.Count == 0)
            return default;

        if (candidates.Count == 1)
            return candidates[0];

        RepositionCandidate closest = candidates[0];

        float closestDistance =
            (closest.position - targetPos).sqrMagnitude;

        for (int i = 1; i < candidates.Count; i++)
        {
            float distance =
                (candidates[i].position - targetPos).sqrMagnitude;

            if (distance >= closestDistance)
                continue;

            closest = candidates[i];
            closestDistance = distance;
        }

        return closest;
    }

    // 위치: 가장 먼
    private RepositionCandidate GetFarthestPos(List<RepositionCandidate> candidates, Vector2 targetPos)
    {
        if (candidates.Count == 0)
            return default;

        if (candidates.Count == 1)
            return candidates[0];

        RepositionCandidate farthest = candidates[0];

        float farthestDistance =
            (farthest.position - targetPos).sqrMagnitude;

        for (int i = 1; i < candidates.Count; i++)
        {
            float distance =
                (candidates[i].position - targetPos).sqrMagnitude;

            if (distance <= farthestDistance)
                continue;

            farthest = candidates[i];
            farthestDistance = distance;
        }

        return farthest;
    }

    // 위치: 중간
    private RepositionCandidate GetMedianPos(List<RepositionCandidate> candidates, Vector2 targetPos)
    {
        int count = candidates.Count;

        if (count == 0)
            return default;

        if (count == 1)
            return candidates[0];

        // 후보를 복사
        RepositionCandidate[] sorted =
            new RepositionCandidate[count];

        candidates.CopyTo(sorted);

        // 거리 기준 오름차순 정렬
        for (int i = 0; i < count - 1; i++)
        {
            int minIndex = i;

            float minDistance =
                (sorted[i].position - targetPos).sqrMagnitude;

            for (int j = i + 1; j < count; j++)
            {
                float distance =
                    (sorted[j].position - targetPos).sqrMagnitude;

                if (distance < minDistance)
                {
                    minIndex = j;
                    minDistance = distance;
                }
            }

            if (minIndex == i)
                continue;

            RepositionCandidate temp = sorted[i];
            sorted[i] = sorted[minIndex];
            sorted[minIndex] = temp;
        }

        return sorted[count / 2];
    }
    
    // 거리 정렬
    private void SortCandidatesByDistance(List<RepositionCandidate> candidates, Vector2 targetPos)
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

    // 총알 크기 반지름 계산 (+a 여유)
    private float GetRadius()
    {
        return (rangeSO.size.x * rangeSO.bulletShadowScale.x * 0.5f);
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        float radius = GetRadius();
        float moveDis = (radius * 3f) + 0.1f;

        return new PatternPreviewElement("Reposition",
            new PatternPreviewDetail($"Speed: {followSO.speed}", Color.cyan),
            new PatternPreviewDetail($"Dis: {moveDis}", Color.red),
            new PatternPreviewDetail($"Radius: {radius}", Color.green),
            new PatternPreviewDescription("Move to the possible location attacked."));

    }
#endif
}


