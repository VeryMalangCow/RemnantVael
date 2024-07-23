using UnityEngine;

public class BulletController : MonoBehaviour
{
    #region Value

    [Header("=== State")]
    [SerializeField] protected float baseDamage;
    [SerializeField] protected float muzzleSpeed;
    [SerializeField] protected float aliveTime;
    [SerializeField] protected float currentAliveTime = 0;

    [Header("=== Component")]
    [SerializeField] private Rigidbody2D ThisRb;

    #endregion

    #region State

    public virtual void SetState(Vector2 SpawnVec, float _baseDamage, float _muzzleSpeed, float _aliveTime)
    {
        this.transform.position = SpawnVec;
        baseDamage = _baseDamage;
        muzzleSpeed = _muzzleSpeed;
        aliveTime = _aliveTime;
        currentAliveTime = 0;
    }

    #endregion

    #region Framework

    protected virtual void FixedUpdate()
    {
        ThisRb.velocity = this.transform.up * 1000f * Time.deltaTime;
    }

    #endregion

}
