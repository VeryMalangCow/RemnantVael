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

    #region Set State

    public void Set_State(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, Vector2 _Dir, float _TargetRange)
    {
        this.transform.localRotation = Get_RotByVec2(_Dir);

        base.Set_State(_SpawnVec, _SpreadAngle, _BulletState, _TargetRange);
        ThisSR.sprite = BulletSprite.Get_CorrectType(_BulletState.DmgState.DmgType).Get_Special(_BulletState.IsCritical);

        ThisRb.simulated = true;
        gameObject.SetActive(true);
    }

    #endregion

    #region Remove

    protected override void Remove_Object()
    {
        Gen_AttackPointEffect(TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical);
        Gen_ExplosionEffect(TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical);
       
        base.Remove_Object();
    }

    #endregion

    #region Collision

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if (!this.gameObject.activeSelf)
        { return; }

        // Hit Enemy
        if (_Col.tag == "Enemy")
        {
            if (_Col.transform.parent.TryGetComponent(out EnemyController EC))
            {
                EC.Gen_HittedPointEffect(
                    this.TargetObject.transform.position, 
                    State.DmgState.DmgType, 
                    State.IsCritical,
                    transform.rotation);
                EC.Take_Damaged(State, Get_DirByAngle(transform.eulerAngles.z));
            }
        }
        else if (_Col.tag == "DestructibleObject")
        {
            if (_Col.transform.parent.TryGetComponent(out DestructibleBuildController DBC))
            {
                DBC.Take_Damage(true);
            }
        }

        if (DestroyTagList.Contains(_Col.tag))
        {
            Remove_Object();
        }
    }

    #endregion

    #region Effect

    private void Gen_ExplosionEffect(Vector2 _SpawndPos, eDamageType _DamageType, bool _IsCritical)
    {
        int index = 0;
        if (_DamageType == eDamageType.Physics)
        {
            if (!_IsCritical)
            { index = 0; }
            else
            { index = 1; }
        }
        else
        {
            if (!_IsCritical)
            { index = 2; }
            else
            { index = 3; }
        }

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
