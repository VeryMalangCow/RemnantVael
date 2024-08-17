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
            this.gameObject.SetActive(false);
            PoolingManager.Instance.PlayerBullet.Queue.Enqueue(this);
        }
    }

    #endregion

    #region Set State

    public override void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, float _fireMinDisLimit, bool _IsCritical, float _CD)
    {
        Vector2 targetPos = InputManager.Instance.MousePosByWorld;
        if(_fireMinDisLimit > Vector3.Magnitude(InputManager.Instance.DirFromPlayerPos))
        {
            targetPos = (Vector2)PlayerManager.Instance.PlayerController.transform.position + 
                InputManager.Instance.DirFromPlayerPos.normalized * _fireMinDisLimit;
        }

        Vector2 dir = (targetPos - _SpawnVec).normalized;
        Quaternion targetQuat = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, dir));

        this.transform.localRotation = targetQuat;


        base.SetState(_SpawnVec, _SpreadAngle, _BulletState, _fireMinDisLimit, _IsCritical, _CD);
    }

    #endregion

    #region Collision

    private void OnTriggerEnter2D(Collider2D _Collision)
    {
        // Hit Enemy
        if (_Collision.tag == "Enemy")
        {
            if(_Collision.transform.parent.TryGetComponent(out EnemyController EC))
            {
                EC.TakeDamage(BulletState.DamageType, BulletState.BaseDamage);
            }
            PoolingManager.Instance.PlayerBullet.Queue.Enqueue(this);
            this.gameObject.SetActive(false);
        }
    }

    #endregion
}
