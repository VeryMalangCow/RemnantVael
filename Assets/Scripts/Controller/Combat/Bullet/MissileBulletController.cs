using DG.Tweening;
using UnityEngine;

public class MissileBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Missile Controller")]

    [Space(10)]
    [Header("=== Extra State")]
    [SerializeField] private GameObject Missile_Prefab;
    [SerializeField] private float ShadowRangeTarget = 0.4f;
    [SerializeField] private float SpreadTime = 1f;

    #endregion

    #region Framework

    protected override void Update()
    {
        Set_Guide();
        base.Update();
    }

    #endregion

    #region Guided On

    private void Set_Guide()
    {
        if (!IsGuided)
        {
            if (SpreadTime <= CurrentAliveTime)
            {
                IsGuided = true;
            }
        }
    }

    #endregion

    #region Extra State

    public override void Set_State_Base(BulletState _BulletState, float _TargetRange)
    {
        base.Set_State_Base(_BulletState, _TargetRange);

        float targetSpeed = _BulletState.MuzzleSpeed;
        base.State.MuzzleSpeed *= 0.3f;

        DOTween.To(() => State.MuzzleSpeed, x => State.MuzzleSpeed = x, targetSpeed, SpreadTime)
            .SetEase(Ease.Linear);
    }

    public override void Set_State_Extra()
    {
        base.Set_State_Extra();

        IsGuided = false;
        TargetEnemyController = null;

        DOTween.To(() => TargetRange, y => TargetRange = y, ShadowRangeTarget, SpreadTime)
            .SetEase(Ease.Linear);
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
                EC.Take_Damaged(State, DevTool.Get_DirFromAngle(transform.eulerAngles.z));
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
            1.9f, 0.05f, 0.1f,
            0.8f, 0.5f, 1.0f,
            index, PlayerManager.Instance.PlayerController.ThisPlayerMaterialList[0]);
    }

    private void Gen_AttackPointEffect(Vector2 _SpanwedPos, eDamageType _DamageType, bool _IsCritical)
    {
        OnceTimeAnimController oota = PoolingManager.Instance.Get_OP_OnlyOnceAnimator();
        oota.Start_Anim(
            PlayerManager.Instance.PlayerController.Get_AnimClip_CorrectHitted(_DamageType, _IsCritical),
            _SpanwedPos,
            PlayerManager.Instance.PlayerController.ThisPlayerMaterialList[0],
            2f, 1.8f);

    }


    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        switch (PoolingString)
        {
            case "MissileBullet":
                Gen_AttackPointEffect(TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical);
                Gen_ExplosionEffect(TargetObject.transform.position, State.DmgState.DmgType, State.IsCritical);
                PoolingManager.Instance.MissileBullet.Queue.Enqueue(this);
                break;

            default:
                break;
        }
    }

    #endregion
}
