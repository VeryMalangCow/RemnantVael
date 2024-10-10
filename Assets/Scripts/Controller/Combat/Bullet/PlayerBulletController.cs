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

    public override void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, float _FireMinDisLimit, bool _IsCritical, float _CD)
    {
        Vector2 targetPos = InputPlayerManager.Instance.MousePosByWorld;
        if(_FireMinDisLimit > Vector3.Magnitude(InputPlayerManager.Instance.DirFromPlayerPos))
        {
            targetPos = (Vector2)PlayerManager.Instance.PlayerController.transform.position + 
                InputPlayerManager.Instance.DirFromPlayerPos.normalized * _FireMinDisLimit;
        }

        Vector2 dir = (targetPos - _SpawnVec).normalized;
        Quaternion targetQuat = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, dir));

        this.transform.localRotation = targetQuat;


        base.SetState(_SpawnVec, _SpreadAngle, _BulletState, _FireMinDisLimit, _IsCritical, _CD);

        gameObject.SetActive(true);
    }

    #endregion

    #region Delete

    private void DeleteThis()
    {
        CurrentAliveTime = 0f;

        PoolingManager.Instance.PlayerBullet.Queue.Enqueue(this);
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Collision

    private void OnTriggerEnter2D(Collider2D _Collision)
    {
        if (_Collision.gameObject.tag == "Player" || _Collision.gameObject.tag == "PlayerThing")
        { return; }

        // Hit Enemy
        if (_Collision.tag == "Enemy")
        {
            if (_Collision.transform.parent.TryGetComponent(out EnemyController EC))
            {
                EC.TakeDamage(BulletState.DamageType, BulletState.BaseDamage);
            }
        }

        DeleteThis();
    }

    #endregion
}
