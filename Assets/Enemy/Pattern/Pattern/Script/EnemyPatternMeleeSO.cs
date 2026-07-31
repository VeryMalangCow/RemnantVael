using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleePatternSO",
    menuName = "ScriptableObject/EnemyPatternSO/PatternElement/Melee")]
public class EnemyPatternMeleeSO : EnemyPatternSO
{
    [Header("=== Visual")]
    [Header("-- Anim")]
    [SerializeField] private AnimationClip animation;
    [SerializeField] private bool isShadowRangeByDepthController = true;
    [SerializeField] private bool lightOn;
    [SerializeField] private float lightSize;

    [Header("=== State")]
    [Header("-- Time")]
    [SerializeField] private float jugeAndTweenTime;
    [SerializeField] private float animSpeed;

    [Header("-- Distance")]
    [SerializeField] private float spawnDis;
    [SerializeField] private float endDis;

    [Header("-- Size")]
    [SerializeField] private Vector2 size;

    [Header("-- State")]
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

        module.presenter.PlayVfxOn();
        yield return new WaitForSeconds(startDelay);

        // Actual Attacl Line
        var depths = module.pointer.AttackDepths;
        for (int i = 0; i< depths.Length; i++)
        {
            var depth = depths[i];
            if (depth == null)
                continue;
            PlayAttack(enemy, depth, dir);
        }

        SoundManager.instance.PlayEnemyAttackSfxRandom(enemyTf.transform.position, "Sword");

        module.presenter.PlayVfxOff();
        yield return new WaitForSeconds(endDelay);
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
            new PatternPreviewDetail($"JugeTime: {jugeAndTweenTime}s", Color.red),
            new PatternPreviewDetail($"Size: {size}", Color.green),
            new PatternPreviewDetail($"Dis: {spawnDis} ~ {endDis}", Color.cyan));
    }
#endif
}
