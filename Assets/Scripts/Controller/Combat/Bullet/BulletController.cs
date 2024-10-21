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
    [SerializeField] public bool IsCritical = false;
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
        IsCritical = false;
        CurrentAliveTime = 0;
        ThisRb.simulated = false;
    }

    public virtual void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, bool _IsCritical, float _CD, float _TargetRange)
    {
        CurrentAliveTime = 0;

        this.transform.position = _SpawnVec;

        this.BulletState = _BulletState;

        IsCritical = _IsCritical;
        if(IsCritical)
        {
            this.BulletState.BaseDamage *= _CD;
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
        { ThisRb.velocity = ((BulletState.MuzzleSpeed * 250f) * this.transform.up) * Time.deltaTime; }
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

    public BulletState(eDamageType _eDamageType, float _BaseDamage, float _MuzzleSpeed, float _AliveTime)
    {
        DamageType = _eDamageType;
        BaseDamage = _BaseDamage;
        MuzzleSpeed = _MuzzleSpeed;
        AliveTime = _AliveTime;
    }

    public void ResetState()
    {
        DamageType = eDamageType.Physics;
        BaseDamage = 0;
        MuzzleSpeed = 0;
        AliveTime = 0;
    }
}


