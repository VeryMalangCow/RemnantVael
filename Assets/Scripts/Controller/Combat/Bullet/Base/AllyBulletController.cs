using UnityEngine;

public class AllyBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private CapsuleCollider2D ThisCol;

    #endregion

    #region State

    public override void Set_State_Size(BulletState_Size? _State_Size)
    {
        if (_State_Size.HasValue)
        {
            base.Set_State_Size(_State_Size);

            TargetObject.transform.localScale = _State_Size.Value.ObjSize;
            ThisCol.transform.localScale = _State_Size.Value.ColSize;
        }
    }

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
            UnitManager.Instance.onceTime_AnimGenerator.Anim_Attacked_Circle(
                TargetObject.transform.position, transform.rotation);
            UnitManager.Instance.onceTime_AnimGenerator.Anim_Attacked_Slice(
                TargetObject.transform.position, State.IsCritical, transform.rotation);

            PlayerManager.Instance.cameraController.Play_HitEnemyAnim();
            ec.Try_Hitted(this);
        }
    }

    #endregion

    #region Effect

    protected override void ExtraEffect()
    {

    }

    #endregion

    #region Pooling

    protected override void PoolingSet()
    {
        PoolingManager.Instance.baseAllyBullet.Enqueue(this);
    }

    #endregion

    #region Light

    public void SetOn_LightIntensity(float _Intensity)
    {
        ThisLight.intensity = _Intensity;
        ThisLight.lightCookieSprite = ThisSR.sprite;
    }

    #endregion

    #region Trail

    public void SetOn_TrailState(float _Time, float _StartWidth, Gradient _Gradient)
    {
        ThisTrail.time = _Time;
        ThisTrail.startWidth = _StartWidth;
        ThisTrail.colorGradient = _Gradient;
    }

    #endregion
}
