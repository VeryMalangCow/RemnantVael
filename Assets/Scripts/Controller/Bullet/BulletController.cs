using System;
using UnityEngine;

public class BulletController : HaveShadowThing
{
    #region Value

    [Header("=== State")]
    [SerializeField] protected BulletState BulletState;
    [SerializeField] protected float currentAliveTime = 0;

    [Header("=== Component")]
    [SerializeField] private Rigidbody2D ThisRb;

    #endregion

    #region State

    public virtual void SetState(Vector2 SpawnVec, eDamageType damageType, float _baseDamage, float _muzzleSpeed, float _aliveTime)
    {
        this.transform.position = SpawnVec;
        BulletState.thisDamageType = damageType;
        BulletState.baseDamage = _baseDamage;
        BulletState.muzzleSpeed = _muzzleSpeed;
        BulletState.aliveTime = _aliveTime;
        currentAliveTime = 0;
    }

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();
        ThisRb.velocity = this.transform.up * BulletState.muzzleSpeed * 1000f * Time.deltaTime;
    }

    #endregion
}

[System.Serializable]
public class BulletState
{
    [SerializeField] public eDamageType thisDamageType;
    [SerializeField] public float baseDamage;
    [SerializeField] public float muzzleSpeed;
    [SerializeField] public float aliveTime;
}


