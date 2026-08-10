using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PatternMeleeSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Melee")]
public class EnemyPatternMeleeSO : EnemyPatternSO
{
    [Header("=== Visual")]
    [SerializeField] private AnimationClip animation;
    [Space(10)]
    [SerializeField] private bool isShadowRangeByDepthController = true; 
    [Space(10)]
    [SerializeField] private bool lightOn;
    [SerializeField] private float lightSize;

    [Header("=== State")]
    [SerializeField] private float jugeAndTweenTime;
    [SerializeField] private float animSpeed;

    [Space(10)]
    [SerializeField] private float spawnDis;
    [SerializeField] private float endDis;

    [Space(10)]
    [SerializeField] private Vector2 size;

    [Space(10)]
    [SerializeField] private AttackerState attackerState;

    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        // Pre
        var module = aiContext.GetModule<EnemyMeleeModule>();
        if (module == null)
            yield break;

        var enemy = aiContext.enemy;
        var enemyTf = enemy.transform;
        var playerTf = aiContext.player.transform;
        Vector2 dir = (playerTf.position - enemyTf.position).normalized;

        // Start
        module.weaponScalePresenter.PlayVfxOn(startDelay);
        enemy.SetLookable(false);
        yield return new WaitForSeconds(startDelay);

        // Actual Attack Line
        var depths = module.attackPointer.AttackDepths;
        for (int i = 0; i< depths.Length; i++)
        {
            var depth = depths[i];
            if (depth == null)
                continue;
            PlayAttack(enemy, depth, dir);
        }

        SoundManager.instance.PlayEnemyAttackSfxRandom(enemyTf.position, "Sword");

        // End
        module.weaponScalePresenter.PlayVfxOff(endDelay);
        yield return new WaitForSeconds(endDelay);
        enemy.SetLookable(true);
    }

    private void PlayAttack(EnemyController enemy, DepthController depth, Vector2 targetDir)
    {
        EnemyAttackerController attacker 
            = AttackerManager.instance.SpawnEnemyAttacker();
        attacker.enemy = enemy;

        attacker.SetState(
            attackerState,
            new AttackerState_Juge<CircleCollider2D>(size),
            new State_Anim(animation, animSpeed),
            new State_TF2D(
                GetStartPos(depth, targetDir, spawnDis),
                DevTool.GetRotFromDir(targetDir)),
            new AttackerState_EndTF(
                GetStartPos(depth, targetDir, endDis),
                DevTool.GetRotFromDir(targetDir), 
                jugeAndTweenTime),
            isShadowRangeByDepthController ? depth.targetRange : 0.6f);

        if (lightOn)
            attacker.Set_Light(
                lightSize, jugeAndTweenTime);
    }

    private Vector2 GetStartPos(DepthController depth, Vector2 targetDir, float addDis)
        => (Vector2)depth.transform.position + (targetDir * addDis);

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        var dmgState = attackerState.dmgState;
        var critState = attackerState.criticalState;
        var kbState = attackerState.knockbackState;
        return new PatternPreviewElement($"Melee ({animation.name})",
            new PatternPreviewDetail($"Dmg: {attackerState.dmgState.dmg}", Color.red),
            new PatternPreviewDetail($"Size: {size}", Color.green),
            new PatternPreviewDetail($"Dis: {spawnDis} ~ {endDis}", Color.cyan),
            new PatternPreviewDescription("Conducts a melee attack on the player.\n" +
            $"Dmg: {dmgState.dmg}\nDmg Type: {dmgState.dmgType}\n" +
            $"Critical Chance: {critState.criticalChance}\nCritical Dmg: {critState.criticalDmg}\n" +
            (kbState.canKB ? $"Knockback Power: {kbState.kbPower}\nKnockback Time: {kbState.kbTime}\n" :
            "No Knockback\n") +
            $"Size: {size}"));
    }
#endif
}
