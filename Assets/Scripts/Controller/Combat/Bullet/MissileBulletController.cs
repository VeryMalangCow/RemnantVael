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

    #endregion

    #region Framework

    protected override void Update()
    {
        if (!IsGuided)
        {
            if (SpreadTime <= CurrentAliveTime)
            {
                IsGuided = true;
            }
        }
        
        base.Update();
    }

    #endregion

    #region Delete

    protected override void DeleteThis()
    {
        AttackPointEffect(TargetObject.transform.position, BulletState.DamageType, BulletState.IsCritical);
        ExplosionEffect(TargetObject.transform.position, BulletState.DamageType, BulletState.IsCritical);

        base.DeleteThis();
    }

    #endregion

    #region Extra State

    public void SetState_forMissile(Vector2 _SpawnVec, BulletState _BulletState, Vector2 _Dir, float _TargetRange)
    {
        // Offset
        IsGuided = false;

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
                EC.HittedPointEffect(
                    this.TargetObject.transform.position, 
                    BulletState.DamageType, 
                    BulletState.IsCritical, 
                    transform.rotation);
                EC.TakeDamaged(BulletState, GetDirByAngle(transform.eulerAngles.z));
            }
        }
        else if (_Col.tag == "DestructibleObject")
        {
            if (_Col.transform.parent.TryGetComponent(out DestructibleBuildController DBC))
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

        PlayerManager.Instance.PlayerController.PlayerMEI.GenExplosionImgs(
                     _SpawndPos,
                     4, 0.3f, 0.4f,
                     1.9f, 0.05f, 0.1f,
                     0.8f, 0.5f, 1.0f,
                     index, PlayerManager.Instance.PlayerController.ThisPlayerMaterial_000);
    }

    private void AttackPointEffect(Vector2 _SpanwedPos, eDamageType _DamageType, bool _IsCritical)
    {
        OnlyOnceTimeAnimation oota = PoolingManager.Instance.GetOP_OnlyOnceAnimator();
        oota.StartAnim(
            PlayerManager.Instance.PlayerController.GetCorrectHitted_AC(_DamageType, _IsCritical),
            _SpanwedPos,
            PlayerManager.Instance.PlayerController.ThisPlayerMaterial_000,
            2f, 1.8f);

    }


    #endregion
}
