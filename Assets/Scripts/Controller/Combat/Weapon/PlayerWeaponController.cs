using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : SatelliteController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public eDamageType DamageType;
    [SerializeField] public BaseUpgradeState<float> BaseDamage;
    [SerializeField] public BaseUpgradeState<float> AliveTime;
    [SerializeField] public BaseUpgradeState<float> MuzzleSpeed;
    [SerializeField] public BaseUpgradeState<float> ROF;
    [SerializeField] public BaseUpgradeState<float> CC;
    [SerializeField] public BaseUpgradeState<float> CD;
    [SerializeField] public BaseUpgradeState<float> AccuracyRate;
    [SerializeField] public BaseUpgradeState<float> KnockbackPower;

    [SerializeField] public float CurrentDelayROF = 0;

    [Space(10)]
    [Header("=== GunPos")]
    [SerializeField] protected List<Transform> BulletSpawnTFs;

    [Space(10)]
    [Header("=== Player")]
    [SerializeField] private float fireMinDisLimit = 4;


    [Header("-- TargetEffect")]
    [SerializeField] private List<MakeExplosionImage> MEIs;

    #endregion

    #region Framework

    protected void Update()
    {
        PitchTF.transform.localRotation = RotateSmooth(InputManager.Instance.DirFromPlayerPos.normalized, PitchTF, rotateSpeed);

        foreach (Satellite hand in Hands)
        {
            hand.SetPos(PlayerSR.sortingOrder);
        }

        CaculateROF();

        if (CanFire())
        {
            List<PlayerBulletController> PBClist = new List<PlayerBulletController>();
            foreach (Transform TF in BulletSpawnTFs)
            { PBClist.Add(PoolingManager.Instance.GetOP_PlayerBullet()); }

            Fire(PBClist);
            PlayerManager.Instance.CameraController.PlayShotShake(1/ROF.ActualState.Value, PBClist[0].BulletState.BaseDamage);

            BoostItemManager.Instance.ActiveSkill_Fire();
        }
    }


    #endregion

    #region ROF

    private void CaculateROF()
    {
        CurrentDelayROF += Time.deltaTime * ROF.ActualState.Value;

        if (CurrentDelayROF > 1)
        {
            CurrentDelayROF = 1;
        }
    }

    #endregion

    #region Judg Can Fire

    private bool CanFire()
    {
        if(IsInputed &&
            CurrentDelayROF >= 1 &&
            !PlayerController.IsCasting &&
            PlayerController.MovementState == eMovementState.IdleOrWalk)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Fire

    protected void Fire<T>(List<T> _Ts)
    {
        float spreadMaxLimit = 100 - AccuracyRate.ActualState.Value;
        float randomAngle = UnityEngine.Random.Range(-spreadMaxLimit, spreadMaxLimit);
        for (int i = 0; i < BulletSpawnTFs.Count; i++)
        {
            PlayerBulletController PBC = GameManager.CastIfPossible<PlayerBulletController>(_Ts[i]);

            // Critical
            float rcc = UnityEngine.Random.Range(0f, 1f);
            bool isCritical = false;
            if (rcc < CC.ActualState.Value)
            {
                isCritical = true;
            }

            // Knockback

            bool ableKnockback = false;
            if (DamageType == eDamageType.Physics)
            {
                ableKnockback = true;
            }

            // Shadow
            float targetShadow = 0.4f;
            if (BulletSpawnTFs[i].TryGetComponent(out HaveShadowThing HST))
            { targetShadow = HST.TargetRange; }

            // Angle
            Vector2 targetPos = InputManager.Instance.MousePosByWorld;
            if (fireMinDisLimit > Vector3.Magnitude(InputManager.Instance.DirFromPlayerPos))
            {
                targetPos = (Vector2)PlayerManager.Instance.PlayerController.transform.position +
                    InputManager.Instance.DirFromPlayerPos.normalized * fireMinDisLimit;
            }
            Vector2 dir = (targetPos - (Vector2)MEIs[i].gameObject.transform.position).normalized;

            // Base State 
            BulletState bulletState = new BulletState(
                DamageType, 
                BaseDamage.ActualState.Value, 
                MuzzleSpeed.ActualState.Value, 
                AliveTime.ActualState.Value, 
                isCritical, 
                CD.ActualState.Value,
                ableKnockback,
                PlayerController.BaseWeapon.KnockbackPower.ActualState.Value,
                0.2f);
            PBC.SetState(BulletSpawnTFs[i].position, randomAngle, bulletState, dir, targetShadow);

            // Sorting Layer
            if (BulletSpawnTFs[i].gameObject.TryGetComponent(out HaveShadowThing hst))
            { PBC.ThisSR.sortingOrder = hst.ThisSR.sortingOrder - 1; }

            // Effect
            MEIs[i].GenExplosionImgs_Fan(
                (Vector2)MEIs[i].gameObject.transform.position + (dir * 0.3f),
                dir, 45f,
                6, 0.2f, 1f,
                0.2f, 0.05f, 0.1f,
                0.0f, 0.5f, 1.0f);
        }
        CurrentDelayROF = 0;

        // Tween
        this.transform.DOShakePosition(1f / ROF.ActualState.Value, 0.05f, 20, 90, false, true);
        
    }



    #endregion


}

