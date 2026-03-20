using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class MissileBulletController : PlayerBulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Missile Controller")]

    [Space(10)]
    [Header("=== Extra State")]
    [SerializeField] private GameObject missile_Prefab;
    [SerializeField] private float shadowRangeTarget = 0.4f;
    [SerializeField] private float spreadTime = 1f;
    [SerializeField] private float baseRotatePower = 6f;

    #endregion

    #region Framework

    protected override void Update()
    {
        Set_Guide();
        base.Update();
    }

    #endregion

    #region Guided On

    private void Set_Guide()
    {
        if (!isGuided)
        {
            if (spreadTime <= currentAliveTime)
            {
                isGuided = true;
            }
        }
    }

    #endregion

    #region State

    public override void Set_State_Base(BulletState bulletState, float targetRange)
    {
        base.Set_State_Base(bulletState, targetRange);

        float targetSpeed = bulletState.muzzleSpeed;
        base.state.muzzleSpeed *= 0.3f;

        DOTween.To(() => state.muzzleSpeed, x => state.muzzleSpeed = x, targetSpeed, spreadTime)
            .SetEase(Ease.Linear);
    }

    public override void Set_State_Extra()
    {
        base.Set_State_Extra();

        isGuided = false;
        enemy = null;

        DOTween.To(() => TargetRange, y => TargetRange = y, shadowRangeTarget, spreadTime)
            .SetEase(Ease.Linear);

        rotSpeed += baseRotatePower;
    }

    #endregion

    #region Effect

    protected override void ExtraEffect()
    {
        //base.ExtraEffect();

        Play_ExplosionAttack();
        switch (poolingString)
        {
            case "MissileBullet":
                UnitManager.instance.onceTime_AnimGenerator.Anim_AttackSuccess(
                    TargetObject.transform.position, state.dmgState.dmgType, state.isCritical, 1.8f);
                UnitManager.instance.player_ExplImgGenerator.Expl_Player_BigObjectDestroy(
                    PlayerManager.instance.playerController.Get_ID(), TargetObject.transform.position, state.dmgState.dmgType, state.isCritical);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Pooling

    protected override void PoolingSet()
    {
        //base.PoolingSet();

        switch (poolingString)
        {
            case "MissileBullet":
                PoolingManager.instance.missileBullet.Enqueue(this);

                break;

            default:
                break;
        }

    }

    #endregion

    #region Explosion

    private void Play_ExplosionAttack()
    {
        PlayerExplosionController pec = PoolingManager.instance.Get_OP_PlayerExplosion();
        pec.Set_State(
            Get_ExlposionState(),
            ac: ResourceManager.instance.explosionAC,
            Get_SpawnTF(),
            this.TargetRange);
    }

    private ExplosionState Get_ExlposionState()
    {
        return new ExplosionState(
            new CombatState(
                new CombatOwner(eCombatOwner.Player),
                new DmgState(eDamageType.Physics, state.dmgState.dmg * 2),
                new CriticalState(state.criticalState),
                new KnockbackState(true, state.knockbackState.kbPower * 2, state.knockbackState.kbTime)),
            new AttackSizeState(1f),
            new List<bool> { false, true, false, false }); // Fire, Cold, Electricity, Corrosion
    }

    private State_TF2D Get_SpawnTF()
    {
        return new State_TF2D(transform.position, Quaternion.identity, Vector2.one);
    }

    #endregion
}