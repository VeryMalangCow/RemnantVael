using UnityEngine;

public class PlayerBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private bool LightIsApplyPlayerState = true;
    [SerializeField] private bool TrailIsApplyPlayerState = true;

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] public PlayerVisual<Sprite> BulletSprite;

    [Space(10)]
    [Header("=== Light")]
    [SerializeField] private float Intensity;

    [Space(10)]
    [Header("=== Trail")]
    [SerializeField] private float TrailTime;
    [SerializeField] private float TrailStartWidth;


    #endregion

    #region Light

    protected override void SetOn_Light()
    {
        base.SetOn_Light();

        ThisLight.lightCookieSprite = ThisSR.sprite;
        if (LightIsApplyPlayerState)
        {
            ThisLight.color = PlayerManager.instance.playerController.Get_CorrectColor(this.State.dmgState.dmgType, this.State.isCritical);
            if (this.State.dmgState.dmgType == eDamageType.Physics)
                ThisLight.intensity = Intensity;
            else
                ThisLight.intensity = Intensity * 0.5f;
        }
    }

    #endregion

    #region Trail

    protected override void SetOn_Trail()
    {
        base.SetOn_Trail();

        if (TrailIsApplyPlayerState)
        {
            ThisTrail.time = TrailTime;
            ThisTrail.startWidth = TrailStartWidth;
            ThisTrail.colorGradient = PlayerManager.instance.playerController.Get_CorrectGradient(this.State.dmgState.dmgType, this.State.isCritical);
        }
    }

    #endregion

    #region State

    public override void Set_State_Base(BulletState _BulletState, float _TargetRange)
    {
        base.Set_State_Base(_BulletState, _TargetRange);

        // 알맞는 이미지
        ThisSR.sprite = BulletSprite.Get_CorrectType(_BulletState.dmgState.dmgType).Get_Special(_BulletState.isCritical);
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
            UnitManager.instance.onceTime_AnimGenerator.Anim_Attacked_Circle(
                TargetObject.transform.position, transform.rotation);
            UnitManager.instance.onceTime_AnimGenerator.Anim_Attacked_Slice(
                TargetObject.transform.position, State.isCritical, transform.rotation);

            PlayerManager.instance.cameraController.Play_HitEnemyAnim();
            ec.Try_Hitted(this);
        }
    }

    #endregion

    #region Effect

    protected override void ExtraEffect()
    {
        switch (PoolingString)
        {
            case "PlayerBullet": // 기본탄
                UnitManager.instance.onceTime_AnimGenerator.Anim_AttackSuccess(
                    TargetObject.transform.position, State.dmgState.dmgType, State.isCritical, 1.0f);
                UnitManager.instance.player_ExplImgGenerator.Expl_Player_ObjectDestroy(
                    PlayerManager.instance.playerController.Get_ID(), TargetObject.transform.position, State.dmgState.dmgType, State.isCritical);
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
        switch (PoolingString)
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