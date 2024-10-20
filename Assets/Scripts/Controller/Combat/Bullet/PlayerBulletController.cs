using UnityEngine;

public class PlayerBulletController : BulletController
{
    #region Framework

    protected override void Update()
    {
        base.Update();

        // Time
        CurrentAliveTime += Time.deltaTime;
        if (CurrentAliveTime >= BulletState.AliveTime)
        {
            DeleteThis();
        }
    }

    #endregion

    #region Set State

    public void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, Vector2 _Dir, bool _IsCritical, float _CD, float _TargetRange)
    {
        Quaternion targetQuat = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, _Dir));

        this.transform.localRotation = targetQuat;

        base.SetState(_SpawnVec, _SpreadAngle, _BulletState, _IsCritical, _CD, _TargetRange);

        ThisRb.simulated = true;
        gameObject.SetActive(true);
    }

    #endregion

    #region Delete

    private void DeleteThis()
    {
        ResetState();

        this.gameObject.SetActive(false);

        PoolingManager.Instance.PlayerBullet.Queue.Enqueue(this);
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
                EC.TakeDamage(BulletState.DamageType, BulletState.BaseDamage);
            }
        }

        if (DestroyTagList.Contains(_Col.tag))
        {
            DeleteThis();
        }
    }

    #endregion
}
