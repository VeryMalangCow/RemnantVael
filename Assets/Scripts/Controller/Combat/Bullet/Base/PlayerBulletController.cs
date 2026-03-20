using UnityEngine;

public class PlayerBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private bool lightIsApplyPlayerState = true;
    [SerializeField] private bool trailIsApplyPlayerState = true;

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] public PlayerVisual<Sprite> bulletSprite;

    [Space(10)]
    [Header("=== Light")]
    [SerializeField] private float intensity;

    [Space(10)]
    [Header("=== Trail")]
    [SerializeField] private float trailTime;
    [SerializeField] private float trailStartWidth;


    #endregion

    #region Light

    protected override void SetOn_Light()
    {
        base.SetOn_Light();

        light2d.lightCookieSprite = ThisSR.sprite;
        if (lightIsApplyPlayerState)
        {
            light2d.color = PlayerManager.instance.playerController.Get_CorrectColor(this.state.dmgState.dmgType, this.state.isCritical);
            if (this.state.dmgState.dmgType == eDamageType.Physics)
                light2d.intensity = intensity;
            else
                light2d.intensity = intensity * 0.5f;
        }
    }

    #endregion

    #region Trail

    protected override void SetOn_Trail()
    {
        base.SetOn_Trail();

        if (trailIsApplyPlayerState)
        {
            trail.time = trailTime;
            trail.startWidth = trailStartWidth;
            trail.colorGradient = PlayerManager.instance.playerController.Get_CorrectGradient(this.state.dmgState.dmgType, this.state.isCritical);
        }
    }

    #endregion

    #region State

    public override void Set_State_Base(BulletState bulletState, float targetRange)
    {
        base.Set_State_Base(bulletState, targetRange);

        // 알맞는 이미지
        ThisSR.sprite = bulletSprite.Get_CorrectType(bulletState.dmgState.dmgType).Get_Special(bulletState.isCritical);
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_Enemy(col);

        base.OnTriggerEnter2D(col);
    }

    protected void Try_Hit_Enemy(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "Enemy", out EnemyController ec))
        {
            UnitManager.instance.onceTime_AnimGenerator.Anim_Attacked_Circle(
                TargetObject.transform.position, transform.rotation);
            UnitManager.instance.onceTime_AnimGenerator.Anim_Attacked_Slice(
                TargetObject.transform.position, state.isCritical, transform.rotation);

            PlayerManager.instance.cameraController.Play_HitEnemyAnim();
            ec.Try_Hitted(this);
        }
    }

    #endregion

    #region Effect

    protected override void ExtraEffect()
    {
        switch (poolingString)
        {
            case "PlayerBullet": // 기본탄
                UnitManager.instance.onceTime_AnimGenerator.Anim_AttackSuccess(
                    TargetObject.transform.position, state.dmgState.dmgType, state.isCritical, 1.0f);
                UnitManager.instance.player_ExplImgGenerator.Expl_Player_ObjectDestroy(
                    PlayerManager.instance.playerController.Get_ID(), TargetObject.transform.position, state.dmgState.dmgType, state.isCritical);
                break;

            case "MI_000_Bullet": // 에너지 유도탄
                break;

            case "MI_001_Bullet": // 물리 유도탄
                break;

            default:
                break;
        }
    }

    #endregion

    #region Pooling

    protected override void PoolingSet()
    {
        switch (poolingString)
        {
            case "PlayerBullet": // 기본탄
                PoolingManager.instance.playerBullet.Enqueue(this);
                break;

            case "MI_000_Bullet": // 에너지 유도탄
                PoolingManager.instance.moduleItem_000_Bullets.Enqueue(this);
                break;

            case "MI_001_Bullet": // 물리 유도탄
                PoolingManager.instance.moduleItem_001_Bullets.Enqueue(this);
                break;

            default:
                break;
        }
    }

    #endregion
}