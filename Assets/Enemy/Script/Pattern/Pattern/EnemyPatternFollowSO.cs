using System.Collections;
using UnityEngine;

[CreateAssetMenu(
    fileName = "E999_FollowSO", 
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Follow")]
public class EnemyPatternFollowSO : EnemyPatternSO
{
    [SerializeField] public float speed;
    [SerializeField] private float dis;
    [SerializeField] private bool ignoreWall;

    private static float followInitDelay = 0.2f;
    private static readonly WaitForSeconds followWait = new(followInitDelay);


    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        EnemyController enemy = aiContext.enemy;

        Transform playerTf = aiContext.player.transform;
        Transform enemyTf = enemy.transform;

        Vector2 aPos;
        Vector2 bPos;
        Vector2 dir;
        float currentDis;
        bool existWall;

        int wallMask = aiContext.wallLayer;

        yield return new WaitForSeconds(startDelay);

        enemy.SetMoveSpeed(speed);

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
                existWall = Physics2D.Linecast(bPos, aPos, wallMask);
            

            if (currentDis <= dis && !existWall)
                break;

            enemy.SetNavDir(playerTf);
            yield return followWait;
        }

        aiContext.targetPos = playerTf.position;

        enemy.SetMoveSpeed(0);
        enemy.EndNav();

        yield return new WaitForSeconds(endDelay);
    }


#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        return new PatternPreviewElement("Following",
            new PatternPreviewDetail($"Speed: {speed}", Color.cyan),
            new PatternPreviewDetail($"Dis: {dis}", Color.red),
            new PatternPreviewDetail($"IgnoreWall: {ignoreWall}"),
            new PatternPreviewDescription(ignoreWall ? 
            $"Chase the player to {dis}m at the speed of {speed} ignoring the wall." :
            $"Chase the player to {dis}m at the speed of {speed} without ignoring the wall."));
    }

#endif
}
