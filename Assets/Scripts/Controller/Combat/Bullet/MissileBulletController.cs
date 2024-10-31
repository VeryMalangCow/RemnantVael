using DG.Tweening;
using UnityEngine;

public class MissileBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Missile Controller")]

    [Space(10)]
    [Header("=== Extra State")]
    [SerializeField] private GameObject Missile_Prefab;
    [SerializeField] private float ShadowRangeTarget = 0.4f;
    [SerializeField] private float SpreadTime = 1f;
    [SerializeField] private float SpreadAngleLimit = 20f;
    [SerializeField] private float RotateSpeed = 1f;

    [Header("=== Target")]
    [SerializeField] private EnemyController TargetEnemyController;
    [SerializeField] private bool CanHit = false;

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        CurrentAliveTime += Time.deltaTime;

        if (!CanHit) 
        {
            if (SpreadTime <= CurrentAliveTime)
            {
                CanHit = true;
                SetTarget();
            }
        }
        else if (TargetEnemyController == null || !TargetEnemyController.gameObject.activeSelf)
        {
            SetTarget();
        }

        if (CurrentAliveTime >= BulletState.AliveTime)
        {
            DeleteThis();
        }
        else
        {
            SetTargetDir();
        }
    }

    #endregion

    #region Delete

    private void DeleteThis()
    {
        ExplosionEffect(TargetObject.transform.position, BulletState.DamageType, BulletState.IsCritical);

        CanHit = false;
        CurrentAliveTime = 0f;
        TargetEnemyController = null;

        PoolingManager.Instance.MissileBullet.Queue.Enqueue(this);
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Target

    private void SetTarget()
    {
        TargetEnemyController = null;
        TargetEnemyController = EnemyManager.Instance.GetClosestEnemy(this.transform.position);
    }

    private void SetTargetDir()
    {
        if (TargetEnemyController != null)
        {
            this.transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, ((TargetEnemyController.transform.position - this.transform.position).normalized))),
                RotateSpeed * Time.deltaTime);
        }
    }


    #endregion

    #region Extra State

    public void SetState_forMissile(Vector2 _SpawnVec, BulletState _BulletState, Vector2 _Dir, float _TargetRange)
    {
        // Offset
        TargetEnemyController = null;
        float targetSpeed = _BulletState.MuzzleSpeed;

        // Base Dir
        this.transform.localRotation = this.transform.localRotation = GetRotByVec2(_Dir);

        // State + RandomDir
        float randomSpreadAngle = Random.Range(-SpreadAngleLimit, SpreadAngleLimit);
        base.SetState(_SpawnVec, randomSpreadAngle, _BulletState, _TargetRange);
        base.BulletState.MuzzleSpeed *= 0.3f;

        // Dotween
        DOTween.To(() => BulletState.MuzzleSpeed, x => BulletState.MuzzleSpeed = x, targetSpeed, SpreadTime)
            .SetEase(Ease.Linear);
        DOTween.To(() => TargetRange, y => TargetRange = y, ShadowRangeTarget, SpreadTime)
            .SetEase(Ease.Linear);

        ThisRb.simulated = true;
        gameObject.SetActive(true);
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

        if (DestroyTagList.Contains(_Col.tag))
        {
            DeleteThis();
        }
    }

    #endregion

    #region Effect

    private void ExplosionEffect(Vector2 _SpawndPos, eDamageType _DamageType, bool _IsCritical)
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

        PlayerManager.Instance.PlayerController.BaseWeapon.MEIs[0].GenExplosionImgs(
                     _SpawndPos,
                     8, 0.3f, 0.4f,
                     2.2f, 0.05f, 0.1f,
                     0.0f, 0.5f, 1.0f,
                     index, PlayerManager.Instance.PlayerController.ThisPlayerSmokeMaterial);
    }

    private void PointEffect(Vector2 _SpanwedPos, eDamageType _DamageType, bool _IsCritical)
    {
        OnlyOnceTimeAnimation oota = PoolingManager.Instance.GetOP_OnlyOnceAnimator();
        
        if (_DamageType == eDamageType.Physics)
        {
            if (!_IsCritical)
            { oota.StartAnim(PlayerManager.Instance.PlayerController.PhysicsHittedPointAC, _SpanwedPos, 2f, 3f); }
            else
            { oota.StartAnim(PlayerManager.Instance.PlayerController.PhysicsCriticalHittedPointAC, _SpanwedPos, 2f, 3f); }
        }
        else
        {
            if (!_IsCritical)
            { oota.StartAnim(PlayerManager.Instance.PlayerController.EnergyHittedPointAC, _SpanwedPos, 2f, 3f); }
            else
            { oota.StartAnim(PlayerManager.Instance.PlayerController.EnergyCriticalHittedPointAC, _SpanwedPos, 2f, 3f); }
        }
    }


    #endregion
}
