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
    [SerializeField] private GameObject Missile_Prefab;
    [SerializeField] private float ShadowRangeTarget = 0.4f;
    [SerializeField] private float SpreadTime = 1f;
    [SerializeField] private float BaseRotatePower = 6f;

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
        if (!IsGuided)
        {
            if (SpreadTime <= CurrentAliveTime)
            {
                IsGuided = true;
            }
        }
    }

    #endregion

    #region State

    public override void Set_State_Base(BulletState _BulletState, float _TargetRange)
    {
        base.Set_State_Base(_BulletState, _TargetRange);

        float targetSpeed = _BulletState.muzzleSpeed;
        base.State.muzzleSpeed *= 0.3f;

        DOTween.To(() => State.muzzleSpeed, x => State.muzzleSpeed = x, targetSpeed, SpreadTime)
            .SetEase(Ease.Linear);
    }

    public override void Set_State_Extra()
    {
        base.Set_State_Extra();

        IsGuided = false;
        TargetEnemyController = null;

        DOTween.To(() => TargetRange, y => TargetRange = y, ShadowRangeTarget, SpreadTime)
            .SetEase(Ease.Linear);

        RotateSpeed += BaseRotatePower;
    }

    #endregion

    #region Effect

    protected override void ExtraEffect()
    {
        //base.ExtraEffect();

        Play_ExplosionAttack();
        switch (PoolingString)
        {
            case "MissileBullet":
                UnitManager.instance.onceTime_AnimGenerator.Anim_AttackSuccess(
                    TargetObject.transform.position, State.dmgState.dmgType, State.isCritical, 1.8f);
                UnitManager.instance.player_ExplImgGenerator.Expl_Player_BigObjectDestroy(
                    PlayerManager.instance.playerController.Get_ID(), TargetObject.transform.position, State.dmgState.dmgType, State.isCritical);
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

        switch (PoolingString)
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
            _AC: ResourceManager.instance.explosionAC,
            Get_SpawnTF(),
            this.TargetRange);
    }

    private ExplosionState Get_ExlposionState()
    {
        return new ExplosionState(
            new CombatState(
                new CombatOwner(eCombatOwner.Player),
                new DmgState(eDamageType.Physics, State.dmgState.dmg * 2),
                new CriticalState(State.criticalState),
                new KnockbackState(true, State.knockbackState.kbPower * 2, State.knockbackState.kbTime)),
            new AttackSizeState(1f),
            new List<bool> { false, true, false, false }); // Fire, Cold, Electricity, Corrosion
    }

    private State_TF2D Get_SpawnTF()
    {
        return new State_TF2D(transform.position, Quaternion.identity, Vector2.one);
    }

    #endregion
}