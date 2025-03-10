using DG.Tweening;
using UnityEngine;

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
    }

    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        switch (PoolingString)
        {
            case "MissileBullet":

                UnitManager.Instance.OnceTime_AnimGenerator.Anim_AttackSuccess(
                    TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical, 1.8f);
                UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_BigObjectDestroy(
                    PlayerManager.Instance.PlayerController.Get_ID(), TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical);

                PoolingManager.Instance.MissileBullet.Queue.Enqueue(this);

                break;

            default:
                break;
        }

        base.Remove_Condition();
    }

    #endregion
}