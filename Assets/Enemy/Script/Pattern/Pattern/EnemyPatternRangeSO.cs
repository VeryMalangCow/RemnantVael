using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "E999_RangeSO",
    menuName = "ScriptableObject/EnemyPatternSO/Pattern/Range")]
public class EnemyPatternRangeSO : EnemyPatternSO
{
    [Header("=== Visual")]
    [SerializeField] private AnimationClip animation;
    [Space(10)]
    [SerializeField] private Gradient trailGradient;
    [SerializeField] private float trailStartWidth;
    [SerializeField] private float trailTime;
    [Space(10)]
    [SerializeField] public Vector2 bulletShadowScale;
    [Space(10)]
    [SerializeField] private int shootExplAmount = 3;
    [SerializeField] private int explAmount;

    [Space(10)]
    [SerializeField] private float lightIntensity;

    [Header("=== State")]
    [SerializeField] private BulletState bulletState;
    [SerializeField] private float baseAngle = 0f;
    [SerializeField] public Vector2 size;

    public override IEnumerator PlayPattern(EnemyAIContext aiContext)
    {
        // Pre
        var module = aiContext.GetModule<EnemyRangeModule>();
        if (module == null)
            yield break;

        var enemy = aiContext.enemy;
        var enemyTf = enemy.transform;
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
            if (depth == null)
                continue;
            Vector2 shootDir = DevTool.GetDirFromAngle(DevTool.GetAngleFromDir(dir) + baseAngle);
            PlayAttack(enemy, depth, shootDir);
        }
        SoundManager.instance.PlayEnemyAttackSfxRandom(enemyTf.position, "Bullet");

        // End
        module.weaponScalePresenter.PlayVfxOff(endDelay);
        aiContext.targetPos = Vector2.zero;
        yield return new WaitForSeconds(endDelay);
        enemy.SetLookable(true);
    }

    private void PlayAttack(EnemyController enemy, DepthController depth, Vector2 targetDir)
    {
        EnemyBulletController bullet = BulletManager.instance.SpawnEnemyBullet();
        bullet.ownEnemy = enemy;
        float targetShadow = depth.targetRange;
        bullet.SetState(
            this.bulletState,
            new BulletState_PosAndRot(depth.transform.position, targetDir, 0, size.x * 0.2f),
            new BulletState_Size(bulletShadowScale, size),
            new State_Anim(animation, 1),
            new BulletState_Effect(explAmount, 1f),
            targetShadow);

        bullet.SetOnLightIntensity(lightIntensity);
        bullet.SetOnTrailState(trailTime, trailStartWidth, trailGradient);

        // Effect
        VfxManager.instance.enemy_ExplImgGenerator.Expl_Enemy_Shoot(
            (Vector2)depth.targetObject.transform.position + (targetDir * 0.3f),
            targetDir, shootExplAmount);
    }

#if UNITY_EDITOR
    public override PatternPreviewElement GetPreview()
    {
        var dmgState = bulletState.dmgState;
        var critState = bulletState.criticalState;
        var kbState = bulletState.knockbackState;

        return new PatternPreviewElement($"Range ({animation.name})",
            new PatternPreviewDetail($"Dmg: {bulletState.dmgState.dmg}", Color.red),
            new PatternPreviewDetail($"Size: {size}", Color.green),
            new PatternPreviewDetail($"Speed: {bulletState.muzzleSpeed}", Color.cyan),
            new PatternPreviewDescription("Attacks the player with a ranged projectile.\n" +
            $"Dmg: {dmgState.dmg}\nDmg Type: {dmgState.dmgType}\n" +
            $"Critical Chance: {critState.criticalChance}\nCritical Dmg: {critState.criticalDmg}\n" +
            (kbState.canKB ? $"Knockback Power: {kbState.kbPower}\nKnockback Time: {kbState.kbTime}\n" : "No Knockback\n") +
            $"Muzzle Speed: {bulletState.muzzleSpeed}\n" +
            (bulletState.isStatus ? $"Status Effect: {bulletState.statusType.ToString()}" : "No Status") +
            $"Size: {size} (Thickness:x)"));
    }
#endif

}
