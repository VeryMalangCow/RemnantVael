using UnityEngine;

public class BulletController : HaveShadowThing
{
    #region Value
    [Header("<><><><><> Bullet Controller")]

    [Header("=== State")]
    [SerializeField] protected BulletState BulletState;
    [SerializeField] protected float CurrentAliveTime = 0;

    [Header("=== Component")]
    [SerializeField] private Rigidbody2D ThisRb;

    #endregion

    #region State

    public virtual void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState)
    {
        this.transform.position = _SpawnVec;
        BulletState = _BulletState;

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
}


