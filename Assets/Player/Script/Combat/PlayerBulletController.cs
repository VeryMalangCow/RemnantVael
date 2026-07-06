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

    protected override void RemoveObject()
    {
        BulletManager.instance.RemovePlayerBullet(this);
    }

    #region Light

    protected override void SetOn_Light()
    {
        base.SetOn_Light();

        light2d.lightCookieSprite = thisSr.sprite;
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
        thisSr.sprite = bulletSprite.Get_CorrectType(bulletState.dmgState.dmgType).Get_Special(bulletState.isCritical);
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
            VfxManager.instance.onceTime_AnimGenerator.Anim_Attacked_Circle(
                targetObject.transform.position, transform.rotation);
            VfxManager.instance.onceTime_AnimGenerator.Anim_Attacked_Slice(
                targetObject.transform.position, state.isCritical, transform.rotation);

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
                VfxManager.instance.onceTime_AnimGenerator.Anim_AttackSuccess(
                    targetObject.transform.position, state.dmgState.dmgType, state.isCritical, 1.0f);
                VfxManager.instance.player_ExplImgGenerator.Expl_Player_ObjectDestroy(
                    PlayerManager.instance.playerController.Get_ID(), targetObject.transform.position, state.dmgState.dmgType, state.isCritical);
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
}