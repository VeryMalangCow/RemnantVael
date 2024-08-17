using UnityEngine;

public class BulletController : HaveShadowThing
{
    #region Value
    [Space(20)]
    [Header("<><><><><> Bullet Controller")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] protected BulletState BulletState;
    [SerializeField] public bool IsCritical = false;
    [SerializeField] protected float CurrentAliveTime = 0;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private Rigidbody2D ThisRb;

    #endregion

    #region State

    public virtual void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, float _fireMinDisLimit, bool _IsCritical, float _CD)
    {
        this.transform.position = _SpawnVec;

        this.BulletState = _BulletState;

        IsCritical = _IsCritical;
        if(IsCritical)
        {
            this.BulletState.BaseDamage *= _CD;
            Debug.Log("Å©¸® ºÒ·¿!");
        }

        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z += _SpreadAngle;
        transform.eulerAngles = currentRotation;

        CurrentAliveTime = 0;
    }

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();
        ThisRb.velocity = this.transform.up * BulletState.MuzzleSpeed * 1000f * Time.deltaTime;
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
}


