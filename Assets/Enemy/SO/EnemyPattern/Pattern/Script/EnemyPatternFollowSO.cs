using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FollowPatternSO", 
    menuName = "ScriptableObject/EnemyPatternSO/PatternElement/Follow")]
public class EnemyPatternFollowSO : EnemyPatternSO
{
    public float speed;
    public float dis;
    public bool ignoreWall;

    public static float followInitDelay = 0.2f;

    protected override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        Transform playerTf = aiContext.player.transform;
        EnemyController enemy = aiContext.enemy;
        Transform enemyTf = enemy.transform;

        yield return new WaitForSeconds(startDelay);

        enemy.Set_MoveSpeed(speed);

        while (true)
        {
            if (Vector2.Distance(playerTf.position, enemyTf.position) <= dis)
            {
                enemy.Set_NavDir(playerTf);
                yield return new WaitForSeconds(followInitDelay);
            }
            else
            {
                break;
            }

        }
        enemy.End_Nav();

        yield return new WaitForSeconds(endDelay);
    }


#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        return new PatternPreviewElement("Following",
            new PatternPreviewDetail($"Speed: {speed}", Color.cyan),
            new PatternPreviewDetail($"dis: {dis}", Color.red),
            new PatternPreviewDetail($"IgnoreWall: {ignoreWall}"));
    }

#endif
}
