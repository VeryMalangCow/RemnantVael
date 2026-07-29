using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FollowPatternSO", 
    menuName = "ScriptableObject/EnemyPatternSO/PatternElement/Follow")]
public class EnemyPatternFollowSO : EnemyPatternSO
{
    [SerializeField] private float speed;
    [SerializeField] private float dis;
    [SerializeField] private bool ignoreWall;
    [SerializeField] private float rayRadius;

    private static float followInitDelay = 0.2f;
    private static readonly WaitForSeconds followWait = new(followInitDelay);


    protected override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        EnemyController enemy = aiContext.enemy;

        Transform playerTf = aiContext.player.transform;
        Transform enemyTf = enemy.transform;

        Vector2 aPos;
        Vector2 bPos;
        Vector2 dir;
        float currentDis;
        bool existWall;

        yield return new WaitForSeconds(startDelay);

        enemy.Set_MoveSpeed(speed);

        while (true)
        {
            aPos = playerTf.position;
            bPos = enemyTf.position;
            dir = bPos - aPos;
            currentDis = dir.magnitude;

            if (currentDis <= Mathf.Epsilon)
                break;

            existWall = false;
            if (!ignoreWall)
            {
                existWall = Physics2D.CircleCast(
                    aPos, rayRadius, dir.normalized, currentDis,
                    LayerMask.GetMask("Wall")).collider != null;
            }

            if (currentDis <= dis && !existWall)
                break;

            enemy.SetNavDir(playerTf);
            yield return followWait;
        }
        enemy.EndNav();

        yield return new WaitForSeconds(endDelay);
    }


#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        return new PatternPreviewElement("Following",
            new PatternPreviewDetail($"Speed: {speed}", Color.cyan),
            new PatternPreviewDetail($"Dis: {dis}", Color.red),
            new PatternPreviewDetail($"IgnoreWall: {ignoreWall}"));
    }

#endif
}
