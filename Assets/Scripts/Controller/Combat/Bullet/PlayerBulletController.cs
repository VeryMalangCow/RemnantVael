using UnityEngine;

public class PlayerBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] public PlayerVisual<Sprite> BulletSprite;

    #endregion

    #region State

    public override void Set_State_Base(BulletState _BulletState, float _TargetRange)
    {
        base.Set_State_Base(_BulletState, _TargetRange);

        // 알맞는 이미지
        ThisSR.sprite = BulletSprite.Get_CorrectType(_BulletState.DmgState.DmgType).Get_Special(_BulletState.IsCritical);
    }

    #endregion

    #region Collision

    protected virtual void OnTriggerEnter2D(Collider2D _Col)
    {
        if (!this.gameObject.activeSelf)
        { return; }

        Try_Hit_Enemy(_Col);
        Try_Hit_DestructibleObject(_Col);

        Try_Remove(_Col.tag);
    }

    protected void Try_Hit_Enemy(Collider2D _Col)
    {
        if (_Col.tag == "Enemy")
        {
            if (_Col.transform.parent.TryGetComponent(out EnemyController EC))
            {
                UnitManager.Instance.OnceTime_AnimGenerator.Anim_Attacked_Circle(
                    TargetObject.transform.position, transform.rotation);
                UnitManager.Instance.OnceTime_AnimGenerator.Anim_Attacked_Slice(
                    TargetObject.transform.position, State.IsCritical, transform.rotation);
                PlayerManager.Instance.CameraController.Play_HitEnemyAnim();
                EC.Take_Damaged(State, DevTool.Get_DirFromAngle(transform.eulerAngles.z));
            }
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