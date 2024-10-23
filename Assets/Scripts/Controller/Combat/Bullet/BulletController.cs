using System.Collections.Generic;
using UnityEngine;

public class BulletController : HaveShadowThingMovable
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Bullet Controller")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public BulletState BulletState;
    [SerializeField] protected float CurrentAliveTime = 0;

    [Space(10)]
    [Header("=== Physics")]
    [SerializeField] protected Rigidbody2D ThisRb;

    [Space(10)]
    [Header("=== Judg")]
    [SerializeField] protected List<string> DestroyTagList;

    #endregion

    #region State

    public void ResetState()
    {
        BulletState.ResetState();
        
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        CurrentAliveTime = 0;
        ThisRb.simulated = false;
    }

    public virtual void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, float _TargetRange)
    {
        CurrentAliveTime = 0;

        this.transform.position = _SpawnVec;

        this.BulletState = _BulletState;

        if(BulletState.IsCritical)
        {
            this.BulletState.BaseDamage *= BulletState.CD;
        }

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
        if (ThisRb != null)
        { ThisRb.velocity = ((BulletState.MuzzleSpeed * 500f) * this.transform.up) * Time.deltaTime; }
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
    [SerializeField] public float KnockbackPower;
    [SerializeField] public float KnockbackTime;

    public BulletState(
        eDamageType _eDamageType,
        float _BaseDamage,
        float _MuzzleSpeed, 
        float _AliveTime, 
        bool _IsCritical, 
        float _CD,
        float knockbackPower,
        float knockbackTime)
    {
        DamageType = _eDamageType;
        BaseDamage = _BaseDamage;
        MuzzleSpeed = _MuzzleSpeed;
        AliveTime = _AliveTime;
        IsCritical = _IsCritical;
        CD = _CD;
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
        KnockbackPower = 0;
    }
}


