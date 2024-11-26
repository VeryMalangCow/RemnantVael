using UnityEngine;

public class PlayerBulletController : BulletController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] public Sprite BasePhysics_Sprite;
    [SerializeField] public Sprite CriticalPhysics_Sprite;
    [SerializeField] public Sprite BaseEnergy_Sprite;
    [SerializeField] public Sprite CriticalEnergy_Sprite;

    #endregion

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

        if (BulletState.IsCritical)
        {
            this.BulletState.BaseDamage *= BulletState.CD;

            if (_BulletState.DamageType == eDamageType.Physics)
            { ThisSR.sprite = CriticalPhysics_Sprite; }
            else
            { ThisSR.sprite = CriticalEnergy_Sprite; }
        }
        else
        {
            if (_BulletState.DamageType == eDamageType.Physics)
            { ThisSR.sprite = BasePhysics_Sprite; }
            else
            { ThisSR.sprite = BaseEnergy_Sprite; }
        }

        base.SetState(_SpawnVec, _SpreadAngle, _BulletState, _TargetRange);

        ThisRb.simulated = true;
        gameObject.SetActive(true);
    }

    #endregion

    #region Delete

    private void DeleteThis()
    {
        AttackPointEffect(TargetObject.transform.position, BulletState.DamageType, BulletState.IsCritical);
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
                   index, PlayerManager.Instance.PlayerController.ThisPlayerMaterial_000);
    }

    private void AttackPointEffect(Vector2 _SpanwedPos, eDamageType _DamageType, bool _IsCritical)
    {
        OnlyOnceTimeAnimation oota = PoolingManager.Instance.GetOP_OnlyOnceAnimator();
        oota.StartAnim(
            PlayerManager.Instance.PlayerController.GetCorrectHitted_AC(_DamageType, _IsCritical),
            _SpanwedPos, 
            PlayerManager.Instance.PlayerController.ThisPlayerMaterial_000, 
            2.0f, 1.0f);
    }

    #endregion
}
