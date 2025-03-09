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
                EC.Gen_HittedPointEffect(
                    this.TargetObject.transform.position,
                    State.DmgState.DmgType,
                    State.IsCritical,
                    transform.rotation);
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
                Gen_AttackPointEffect(TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical);
                Gen_ExplosionEffect(TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical);
                PoolingManager.Instance.PlayerBullet.Queue.Enqueue(this);
                Debug.Log("PlayerBullet!");
                break;

            case "MI_000_Bullet": // 에너지 유도탄
                PoolingManager.Instance.MI_000_Bullets.Queue.Enqueue(this);
                Debug.Log("MI_000_Bullet!");
                break;

            case "MI_001_Bullet": // 물리 유도탄
                PoolingManager.Instance.MI_001_Bullets.Queue.Enqueue(this);
                Debug.Log("MI_001_Bullet!");
                break;

            default:
                break;
        }
    }

    #endregion

    #region Effect

    private void Gen_ExplosionEffect(Vector2 _SpawndPos, eDamageType _DamageType, bool _IsCritical)
    {
        int index = DevTool.Get_IndexOfDmgTypeAndCritical(_DamageType, _IsCritical);

        PlayerManager.Instance.PlayerController.PlayerMEI.Gen_ExplosionImgs(
            _SpawndPos,
            4, 0.3f, 0.4f,
            0.6f, 0.05f, 0.1f,
            0.3f, 0.5f, 1.0f,
            index, PlayerManager.Instance.PlayerController.ThisPlayerMaterialList[0]);
    }

    private void Gen_AttackPointEffect(Vector2 _SpanwedPos, eDamageType _DamageType, bool _IsCritical)
    {
        OnceTimeAnimController oota = PoolingManager.Instance.Get_OP_OnlyOnceAnimator();
        oota.Start_Anim(
            PlayerManager.Instance.PlayerController.Get_AnimClip_CorrectHitted(_DamageType, _IsCritical),
            _SpanwedPos, 
            PlayerManager.Instance.PlayerController.ThisPlayerMaterialList[0], 
            2.0f, 1.0f);
    }

    #endregion
}