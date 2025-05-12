using UnityEngine;

public class AllyBulletController : BulletController
{
    #region Value
/*
    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Sprite")]
*/
    #endregion

    #region State
/*
    public override void Set_State_Base(BulletState _BulletState, float _TargetRange)
    {
        base.Set_State_Base(_BulletState, _TargetRange);
    }
*/
    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D _Col)
    {
        Try_Hit_Enemy(_Col);

        base.OnTriggerEnter2D(_Col);
    }

    protected void Try_Hit_Enemy(Collider2D _Col)
    {
        if (DevTool.Can_Collding(_Col, "Enemy", out EnemyController ec))
        {
            UnitManager.Instance.OnceTime_AnimGenerator.Anim_Attacked_Circle(
                TargetObject.transform.position, transform.rotation);
            UnitManager.Instance.OnceTime_AnimGenerator.Anim_Attacked_Slice(
                TargetObject.transform.position, State.IsCritical, transform.rotation);

            PlayerManager.Instance.CameraController.Play_HitEnemyAnim();
            ec.Try_Hitted(this);
        }
    }

    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        PoolingManager.Instance.BaseAllyBullet.Queue.Enqueue(this);
    }

    #endregion
}
