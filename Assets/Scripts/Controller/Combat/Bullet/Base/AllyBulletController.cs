using UnityEngine;

public class AllyBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private CapsuleCollider2D col;

    #endregion

    #region State

    public override void Set_State_Size(BulletState_Size? state_Size)
    {
        if (state_Size.HasValue)
        {
            base.Set_State_Size(state_Size);

            targetObject.transform.localScale = state_Size.Value.objSize;
            col.transform.localScale = state_Size.Value.colSize;
        }
    }

    #endregion

    protected override void Remove_Object()
    {
        if (currentAliveTime <= 0f) return;

        //BulletManager.instance.RemovePlayerBullet(this);

        SetOff_Trail();
        SetOff_Light();

        Reset_State();

        gameObject.SetActive(false);
    }


    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_Enemy(col);

        base.OnTriggerEnter2D(col);
    }

    protected void Try_Hit_Enemy(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "Enemy", out EnemyController enemy))
        {
            UnitManager.instance.onceTime_AnimGenerator.Anim_Attacked_Circle(
                targetObject.transform.position, transform.rotation);
            UnitManager.instance.onceTime_AnimGenerator.Anim_Attacked_Slice(
                targetObject.transform.position, state.isCritical, transform.rotation);

            PlayerManager.instance.cameraController.Play_HitEnemyAnim();
            enemy.Try_Hitted(this);
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
        PoolingManager.instance.baseAllyBullet.Enqueue(this);
    }

    #endregion

    #region Light

    public void SetOn_LightIntensity(float intensity)
    {
        light2d.intensity = intensity;
        light2d.lightCookieSprite = thisSr.sprite;
    }

    #endregion

    #region Trail

    public void SetOn_TrailState(float time, float startWidth, Gradient gradient)
    {
        trail.time = time;
        trail.startWidth = startWidth;
        trail.colorGradient = gradient;
    }

    #endregion
}
