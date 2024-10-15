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

    public override void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, float _FireMinDisLimit, bool _IsCritical, float _CD)
    {
        Vector2 targetPos = InputManager.Instance.MousePosByWorld;

        if (_FireMinDisLimit > Vector3.Magnitude(InputManager.Instance.DirFromPlayerPos))
        {
            targetPos = (Vector2)PlayerManager.Instance.PlayerController.transform.position +
                InputManager.Instance.DirFromPlayerPos.normalized * _FireMinDisLimit;
        }

        Vector2 dir = (targetPos - _SpawnVec).normalized;
        Quaternion targetQuat = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, dir));

        this.transform.localRotation = targetQuat;

        base.SetState(_SpawnVec, _SpreadAngle, _BulletState, _FireMinDisLimit, _IsCritical, _CD);

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
