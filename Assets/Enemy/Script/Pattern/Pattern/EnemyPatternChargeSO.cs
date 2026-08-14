using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "E999_ChargeSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Charge")]
public class EnemyPatternChargeSO : EnemyPatternSO
{
    [Header("=== Visual")]
    [SerializeField] private AnimationClip animation;
    [Space(10)]
    [SerializeField] private bool lightOn;
    [SerializeField] private float lightSize;

    [Header("=== State")]
    [SerializeField] private float dur;
    [Space(10)]
    [SerializeField] private float jugeAndTweenTime;
    [SerializeField] private float animSpeed;
    [SerializeField] private float speed;

    [Space(10)]
    [SerializeField] private Vector2 size;

    [Space(10)]
    [SerializeField] private AttackerState attackerState;

    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    { 
        // Pre
        var module = aiContext.GetModule<EnemyChargeModule>();
        if (module == null)
            yield break;

        var enemy = aiContext.enemy;
        var enemyTf = enemy.transform;
        var enemyPos = enemyTf.position;
        var playerTf = aiContext.player.transform;
        Vector2 dir;
        if (aiContext.targetPos != Vector2.zero)
            dir = (aiContext.targetPos - (Vector2)enemyTf.position).normalized;
        else
            dir = (aiContext.player.transform.position - enemyTf.position).normalized;

        // Start
        module.weaponScalePresenter.PlayVfxOn(startDelay);
        enemy.SetLookable(false);
        yield return new WaitForSeconds(startDelay);

        // Actual Attack Line
        var depths = module.attackPointer.AttackDepths;
        for (int i = 0; i < depths.Length; i++)
        {
            var depth = depths[i];
            PlayAttack(enemy, depth, dir);
        }
        if (DevTool.TryGetDirNavMeshEnd(enemyPos, dir, out Vector2 endPoint))
        {
            enemy.SetNavDir(endPoint);
            enemy.SetMoveSpeed(speed);
        }
        SoundManager.instance.PlayEnemyAttackSfxRandom(enemyPos, "Thrust");
        yield return new WaitForSeconds(dur);
        enemy.SetNavDir(Vector2.zero);
        enemy.SetMoveSpeed(0);

        // End
        module.weaponScalePresenter.PlayVfxOff(endDelay);
        aiContext.targetPos = Vector2.zero;
        yield return new WaitForSeconds(endDelay);
        enemy.SetLookable(true);
    }

    private void PlayAttack(EnemyController enemy, DepthController depth, Vector2 targetDir)
    {
        EnemyAttackerController attacker = AttackerManager.instance.SpawnEnemyAttacker();
        attacker.enemy = enemy;
        float targetShadow = depth.targetRange;

        Quaternion q = DevTool.GetRotFromDir(targetDir);

        attacker.SetState(
            attackerState,
            new AttackerState_Juge<CircleCollider2D>(size),
            new State_Anim(animation, animSpeed),
            new State_TF2D(Vector2.zero, q, Vector2.one),
            new AttackerState_EndTF(Vector2.zero, q, Vector2.one, jugeAndTweenTime),
            targetShadow,
            parent: depth.transform,
            isLocal: true);

        if (lightOn)
            attacker.Set_Light(lightSize, jugeAndTweenTime);
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        var dmgState = attackerState.dmgState;
        var critState = attackerState.criticalState;
        var kbState = attackerState.knockbackState;
        return new PatternPreviewElement($"Charge ({animation.name})",
            new PatternPreviewDetail($"Dmg: {attackerState.dmgState.dmg}", Color.red),
            new PatternPreviewDetail($"Size: {size}", Color.green),
            new PatternPreviewDetail($"Speed: {speed}", Color.cyan),
            /*수정 부분*/
            new PatternPreviewDescription("Dash toward the player and attack.\n" +
            $"Dmg: {dmgState.dmg}\nDmg Type: {dmgState.dmgType}\n" +
            $"Critical Chance: {critState.criticalChance}\nCritical Dmg: {critState.criticalDmg}\n" +
            (kbState.canKB ? $"Knockback Power: {kbState.kbPower}\nKnockback Time: {kbState.kbTime}\n" :
            "No Knockback\n") +
            $"Size: {size}\n" +
            $"Speed: {speed}"));
    }
#endif
}
