using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "E999_ChargeRepositionSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Charge Reposition")]
public class EnemyPatternChargeRepositionSO : EnemyPatternRepositionSO
{
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
    {
        CapsuleCollider2D capsule = aiContext.mapJugeCol;

        return Mathf.Max(
            capsule.size.x,
            capsule.size.y) * 0.5f;
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        return new PatternPreviewElement("Reposition",
            new PatternPreviewDetail($"Speed: {followSO.speed}", Color.cyan),
            new PatternPreviewDetail($"Dis: Enemy's bigger x or y capsule Collider. * 3 + 0.1f", Color.red),
            new PatternPreviewDetail($"Radius: Enemy's bigger x or y capsule Collider.", Color.green),
            new PatternPreviewDescription("Move to the possible location attacked."));

    }
#endif
}
