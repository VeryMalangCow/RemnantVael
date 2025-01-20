using System.Collections.Generic;
using UnityEngine;

public class BulletController : HaveShadowThingMovable
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Bullet Controller")]
    [SerializeField] private string PoolingString = "";

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public BulletState BulletState;
    [SerializeField] protected float CurrentAliveTime = 0;
    [HideInInspector] private static float BaseBulletSpeed = 500f; 

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected Rigidbody2D ThisRb;

    [Space(10)]
    [Header("=== Judg")]
    [SerializeField] protected List<string> DestroyTagList;


    // Extra
    [Space(10)]
    [Header("=== Target")]
    [SerializeField] protected bool IsGuided = false;
    [SerializeField] protected EnemyController TargetEnemyController = null;
    [SerializeField] protected float RotateSpeed = 1f;

    #endregion

    #region State

    public void ResetState()
    {
        BulletState.ResetState();
        
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
        CurrentAliveTime = 0;
        ThisRb.simulated = false;

        TargetEnemyController = null;
    }

    public virtual void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, float _TargetRange)
    {
        CurrentAliveTime = 0;

        this.transform.position = _SpawnVec;

        this.BulletState = new BulletState(_BulletState);


        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z += _SpreadAngle;
        transform.eulerAngles = currentRotation;

        TargetRange = _TargetRange;
    }

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        CurrentAliveTime += Time.deltaTime;

        if (CurrentAliveTime >= BulletState.AliveTime)
        {
            DeleteThis();
            return;
        }

        if (ThisRb != null)
        {
            if (IsGuided)
            {
                if (TargetEnemyController != null && TargetEnemyController.gameObject.activeSelf)
                {
                    SetTargetDir();
                }
                else
                {
                    SetTarget();
                }
            }

            ThisRb.velocity = ((BulletState.MuzzleSpeed * BaseBulletSpeed * Time.deltaTime) * this.transform.up);
        }

    }

    #endregion

    #region Delete

    protected virtual void DeleteThis()
    {
        switch (PoolingString)
        {
            case "BaseBullet":
                if (this is PlayerBulletController pbc)
                PoolingManager.Instance.PlayerBullet.Queue.Enqueue(pbc);
                break;

            case "EnemyBullet":
                if (this is EnemyBulletController ebc)
                    PoolingManager.Instance.EnemyBullets.Queue.Enqueue(ebc);
                break;

            case "MissileBullet":
                if (this is MissileBulletController mbc)
                    PoolingManager.Instance.MissileBullet.Queue.Enqueue(mbc);
                break;

            case "MI_000_Bullet":
                if (this is PlayerBulletController MI_000_pbc)
                    PoolingManager.Instance.MI_000_Bullets.Queue.Enqueue(MI_000_pbc);
                break;

            case "MI_001_Bullet":
                if (this is PlayerBulletController MI_001_pbc)
                    PoolingManager.Instance.MI_001_Bullets.Queue.Enqueue(MI_001_pbc);
                break;

            default:
                break;
        }

        ResetState();
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Angle Vector Things

    protected Vector2 GetDirByAngle(float _Angle)
    {
        return new Vector2(
                    Mathf.Cos((_Angle + 90) * Mathf.Deg2Rad),
                    Mathf.Sin((_Angle + 90) * Mathf.Deg2Rad)).normalized;
    }

    protected Quaternion GetRotByVec2(Vector2 _Dir)
    {
        return Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, _Dir));
    }

    #endregion

    #region Induction

    protected void SetTarget()
    {
        TargetEnemyController = null;
        TargetEnemyController = EnemyManager.Instance.GetClosestEnemy(this.transform.position);
    }

    protected void SetTargetDir()
    {
        if (TargetEnemyController != null)
        {
            this.transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, ((TargetEnemyController.transform.position - this.transform.position).normalized))),
                RotateSpeed * Time.deltaTime);
        }
    }

    #endregion
}

[System.Serializable]
public class BulletState
{
    [SerializeField] public eDamageType DamageType;
    [SerializeField] public float BaseDamage;
    [SerializeField] public float MuzzleSpeed;
    [SerializeField] public float AliveTime;

    [SerializeField] public bool IsCritical;
    [SerializeField] public float CD;

    [SerializeField] public bool AbleKnockback;
    [SerializeField] public float KnockbackPower;
    [SerializeField] public float KnockbackTime;

    public BulletState(BulletState _BS)
    {
        DamageType = _BS.DamageType;
        BaseDamage = _BS.BaseDamage;
        MuzzleSpeed = _BS.MuzzleSpeed;
        AliveTime = _BS.AliveTime;

        IsCritical = _BS.IsCritical;
        CD = _BS.CD;

        AbleKnockback = _BS.AbleKnockback;
        KnockbackPower = _BS.KnockbackPower;
        KnockbackTime = _BS.KnockbackTime;
    }

    public BulletState(
        eDamageType _eDamageType,
        float _BaseDamage,
        float _MuzzleSpeed, 
        float _AliveTime, 
        bool _IsCritical, 
        float _CD,
        bool _AbleKnockback,
        float knockbackPower,
        float knockbackTime)
    {
        DamageType = _eDamageType;
        BaseDamage = _BaseDamage;
        MuzzleSpeed = _MuzzleSpeed;
        AliveTime = _AliveTime;

        IsCritical = _IsCritical;
        CD = _CD;

        AbleKnockback = _AbleKnockback;
        KnockbackPower = knockbackPower;
        KnockbackTime = knockbackTime;
    }

    public BulletState(
        float _BaseDamage, 
        float _AliveTime, 
        bool _AbleKnockback,
        float knockbackPower,
        float knockbackTime)
    {
        BaseDamage = _BaseDamage;
        AliveTime = _AliveTime;

        AbleKnockback = _AbleKnockback;
        KnockbackPower = knockbackPower;
        KnockbackTime = knockbackTime;
    }

    public void ResetState()
    {
        DamageType = eDamageType.Physics;
        BaseDamage = 0;
        MuzzleSpeed = 0;
        AliveTime = 0;

        IsCritical = false;
        CD = 0;

        AbleKnockback = false;
        KnockbackPower = 0;
        KnockbackTime = 0;
    }
}


