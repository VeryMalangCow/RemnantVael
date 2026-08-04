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
    [Space(10)]
    [SerializeField] private float jugeAndTweenTime;
    [SerializeField] private float animSpeed;

    [Space(10)]
    [SerializeField] private float spawnDis;
    [SerializeField] private float endDis;

    [Space(10)]
    [SerializeField] private Vector2 size;

    [Space(10)]
    [SerializeField] private AttackerState attakerState;

    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        EnemyController enemy = aiContext.enemy;
        Transform enemyTf = enemy.transform;
        Transform playerTf = aiContext.player.transform;
        Vector2 dir = playerTf.transform.position - enemyTf.transform.position;
        var module = aiContext.GetModule<EnemyMeleeModule>();
        if (module == null)
            yield break;

        module.weaponScalePresenter.PlayVfxOn(startDelay);
        enemy.SetLookable(false);
        yield return new WaitForSeconds(startDelay);

        // Actual Attacl Line
        var depths = module.attackPointer.AttackDepths;
        for (int i = 0; i< depths.Length; i++)
        {
            var depth = depths[i];
            if (depth == null)
                continue;
            PlayAttack(enemy, depth, dir);
        }

        SoundManager.instance.PlayEnemyAttackSfxRandom(enemyTf.transform.position, "Sword");

        module.weaponScalePresenter.PlayVfxOff(endDelay);
        yield return new WaitForSeconds(endDelay);
        enemy.SetLookable(true);
    }

    private void PlayAttack(EnemyController enemy, DepthController depth, Vector2 targetDir)
    {
        EnemyAttackerController attacker 
            = AttackerManager.instance.SpawnEnemyAttacker();
        attacker.enemy = enemy;

        attacker.Set_State(
            attakerState,
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
        return new PatternPreviewElement($"Melee ({animation.name})",
            new PatternPreviewDetail($"Dmg: {attakerState.dmgState.dmg}", Color.red),
            new PatternPreviewDetail($"Size: {size}", Color.green),
            new PatternPreviewDetail($"Dis: {spawnDis} ~ {endDis}", Color.cyan));
    }
#endif
}
