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

    public void SetState(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, Vector2 _Dir, float _TargetRange)
    {
        this.transform.localRotation = GetRotByVec2(_Dir);

        base.SetState(_SpawnVec, _SpreadAngle, _BulletState, _TargetRange);

        ThisRb.simulated = true;
        gameObject.SetActive(true);
    }

    #endregion

    #region Delete

    private void DeleteThis()
    {
        ExplosionEffect(TargetObject.transform.position, ThisSR.sortingOrder + 1, BulletState.DamageType, BulletState.IsCritical);
        

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
                PointEffect(TargetObject.transform.position, BulletState.DamageType, BulletState.IsCritical);
                EC.TakeDamage(BulletState, GetDirByAngle(transform.eulerAngles.z));
            }
        }
        else if (_Col.tag == "DestructibleObject")
        {
            if (_Col.transform.parent.TryGetComponent(out DestructibleBuildingController DBC))
            {
                DBC.TakeDamage(true);
            }
        }

        if (DestroyTagList.Contains(_Col.tag))
        {
            DeleteThis();
        }
    }

    #endregion

    #region Effect

    private void ExplosionEffect(Vector2 _SpawndPos, int _SortLayer, eDamageType _DamageType, bool _IsCritical)
    {
        int index = 0;
        if (_DamageType == eDamageType.Physics)
        {
            if (!_IsCritical)
            { index = 0; }
            else
            { index = 1; }
        }
        else
        {
            if (!_IsCritical)
            { index = 2; }
            else
            { index = 3; }
        }

        PlayerManager.Instance.PlayerController.PlayerMEI.GenExplosionImgs(
                   _SpawndPos,
                   4, 0.3f, 0.4f,
                   0.6f, 0.05f, 0.1f,
                   0.3f, 0.5f, 1.0f,
                   index, PlayerManager.Instance.PlayerController.ThisPlayerSmokeMaterial);
    }

    private void PointEffect(Vector2 _SpanwedPos, eDamageType _DamageType, bool _IsCritical)
    {
        OnlyOnceTimeAnimation oota = PoolingManager.Instance.GetOP_OnlyOnceAnimator();
        if (_DamageType == eDamageType.Physics)
        {
            if (!_IsCritical)
            { oota.StartAnim(PlayerManager.Instance.PlayerController.PhysicsHittedPointAC, _SpanwedPos, 2f, 1f); }
            else
            { oota.StartAnim(PlayerManager.Instance.PlayerController.PhysicsCriticalHittedPointAC, _SpanwedPos, 2f, 1f); }
        }
        else
        {
            if (!_IsCritical)
            { oota.StartAnim(PlayerManager.Instance.PlayerController.EnergyHittedPointAC, _SpanwedPos, 2f, 1f); }
            else
            { oota.StartAnim(PlayerManager.Instance.PlayerController.EnergyCriticalHittedPointAC, _SpanwedPos, 2f, 1f); }
        }
    }


    #endregion
}
