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

    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        EnemyController enemy = aiContext.enemy;
        Transform enemyTf = enemy.transform;
        Transform playerTf = aiContext.player.transform;
        Vector2 dir = playerTf.transform.position - enemyTf.transform.position;

        enemy.TryGetComponent(out EnemyPatternMeleeProvider provider);
        
        if (provider == null)
            yield break;

        BeforeVfx(startDelay);
        yield return new WaitForSeconds(startDelay);

        // Actual Attacl Line
        SoundManager.instance.PlayEnemyAttackSfxRandom(enemyTf.transform.position, "Sword");

        AfterVfx(endDelay);
        yield return new WaitForSeconds(endDelay);
    }

    private void PlayAttack(EnemyController enemy, DepthController depth, Vector2 targetDir)
    {
        EnemyAttackerController attacker 
            = AttackerManager.instance.SpawnEnemyAttacker();
        attacker.enemy = enemy;
        float targetShadow 
            = isShadowRangeByDepthController ? depth.targetRange : 0.6f;

        /*attacker.Set_State(
            _as,
            State_Juge(),
            State_Anim(),
            State_StartTF(depth.transform.position, targetDir),
            State_EndTF(depth.transform.position, targetDir),
            targetShadow);
*/
        if (lightOn)
            attacker.Set_Light(
                lightSize, jugeAndTweenTime);
    }

    private void BeforeVfx(float startDelay)
    {

    }

    private void AfterVfx(float endDelay)
    {

    }

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
