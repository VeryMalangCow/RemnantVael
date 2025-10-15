using UnityEngine;

public class PlayerBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]
    [SerializeField] private int PlayerID;

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
            ThisLight.color = PlayerManager.Instance.PlayerController.Get_CorrectColor(this.State.DmgState.DmgType, this.State.IsCritical);
            if (this.State.DmgState.DmgType == eDamageType.Physics)
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
            ThisTrail.colorGradient = PlayerManager.Instance.PlayerController.Get_CorrectGradient(this.State.DmgState.DmgType, this.State.IsCritical);
        }
    }

    #endregion

    #region State

    public override void Set_State_Base(BulletState _BulletState, float _TargetRange)
    {
        base.Set_State_Base(_BulletState, _TargetRange);

        // 알맞는 이미지
        ThisSR.sprite = BulletSprite.Get_CorrectType(_BulletState.DmgState.DmgType).Get_Special(_BulletState.IsCritical);
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
        switch (PoolingString)
        {
            case "PlayerBullet": // 기본탄

                UnitManager.Instance.OnceTime_AnimGenerator.Anim_AttackSuccess(
                    TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical, 1.0f);
                UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_ObjectDestroy(
                    PlayerManager.Instance.PlayerController.Get_ID(), TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical);

                SoundManager.Instance.Play_2D_SFX($"Player{DevTool.Get_LengthString(PlayerID, 2)}_BreakBullet");

                PoolingManager.Instance.PlayerBullet.Queue.Enqueue(this);
                break;

            case "MI_000_Bullet": // 에너지 유도탄
                PoolingManager.Instance.MI_000_Bullets.Queue.Enqueue(this);
                break;

            case "MI_001_Bullet": // 물리 유도탄
                PoolingManager.Instance.MI_001_Bullets.Queue.Enqueue(this);
                break;

            default:
                break;
        }
    }

    #endregion
}