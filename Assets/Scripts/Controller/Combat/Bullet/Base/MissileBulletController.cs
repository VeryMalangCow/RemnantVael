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

        float targetSpeed = _BulletState.MuzzleSpeed;
        base.State.MuzzleSpeed *= 0.3f;

        DOTween.To(() => State.MuzzleSpeed, x => State.MuzzleSpeed = x, targetSpeed, SpreadTime)
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
                UnitManager.Instance.OnceTime_AnimGenerator.Anim_AttackSuccess(
                    TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical, 1.8f);
                UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_BigObjectDestroy(
                    PlayerManager.Instance.PlayerController.Get_ID(), TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical);
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
                PoolingManager.Instance.MissileBullet.Enqueue(this);

                break;

            default:
                break;
        }

    }

    #endregion

    #region Explosion

    private void Play_ExplosionAttack()
    {
        PlayerExplosionController pec = PoolingManager.Instance.Get_OP_PlayerExplosion();
        pec.Set_State(
            Get_ExlposionState(),
            _AC: UnitManager.Instance.ExplosionAC,
            Get_SpawnTF(),
            this.TargetRange);
    }

    private ExplosionState Get_ExlposionState()
    {
        return new ExplosionState(
            new CombatState(
                new CombatOwner(eCombatOwner.Player),
                new DmgState(eDamageType.Physics, State.DmgState.Dmg * 2),
                new CriticalState(State.CriticalState),
                new KnockbackState(true, State.KnockbackState.KBPower * 2, State.KnockbackState.KBTime)),
            new AttackSizeState(1f),
            new List<bool> { false, true, false, false }); // Fire, Cold, Electricity, Corrosion
    }

    private State_TF2D Get_SpawnTF()
    {
        return new State_TF2D(transform.position, Quaternion.identity, Vector2.one);
    }

    #endregion
}