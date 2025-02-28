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
    [SerializeField] public BUState<float> BaseDamage;
    [SerializeField] public float AliveTime;
    [SerializeField] public BUState<float> MuzzleSpeed;
    [SerializeField] public BUState<float> ROF;
    [SerializeField] public BUState<float> CC;
    [SerializeField] public BUState<float> CD;
    [SerializeField] public BUState<float> AccuracyRate;
    [SerializeField] public BUState<float> KnockbackPower;

    [SerializeField] public float CurrentDelayROF = 0;

    [Space(10)]
    [Header("=== GunPos")]
    [SerializeField] protected List<Transform> BulletSpawnTFs;

    [Space(10)]
    [Header("=== Player")]
    [SerializeField] private float fireMinDisLimit = 4;

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
            PlayerManager.Instance.CameraController.PlayShotAnim(1/ROF.ActualState.Value, PBClist[0].BulletState.BaseDamage);

            ModuleItemManager.Instance.Active_Fire();
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
            InputManager.Instance.AimController.SetBaseAttack(false);
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
        //randomAngle = 0f;
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
            /*Vector2 targetPos = InputManager.Instance.MousePosByWorld;
            if (fireMinDisLimit > Vector3.Magnitude(InputManager.Instance.DirFromPlayerPos))
            {
                targetPos = (Vector2)PlayerManager.Instance.PlayerController.transform.position +
                    InputManager.Instance.DirFromPlayerPos.normalized * fireMinDisLimit;
            }
            Vector2 dir = (targetPos - (Vector2)BulletSpawnTFs[i].transform.position).normalized;
*/
            Vector2 dir = GetDir((Vector2)BulletSpawnTFs[i].transform.position);

            // Base State 
            BulletState bulletState = new BulletState(
                DamageType, 
                BaseDamage.BuffedState,
                MuzzleSpeed.ActualState.Value, 
                AliveTime, 
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
            if (BulletSpawnTFs[i].TryGetComponent(out HaveShadowThing posHst))
            {
                ExplosionEffect_Fan((Vector2)posHst.TargetObject.transform.position + (dir * 0.3f),
                    DamageType, isCritical, i, dir);
            }
        }
        InputManager.Instance.AimController.SetBaseAttack(true);
        CurrentDelayROF = 0;

        // Tween
        this.transform.DOShakePosition(1f / ROF.ActualState.Value, 0.05f, 20, 90, false, true);
        
    }

    public Vector2 GetDir(Vector2 _SpawnPos)
    {
        Vector2 targetPos = InputManager.Instance.MousePosByWorld;
        if (fireMinDisLimit > Vector3.Magnitude(InputManager.Instance.DirFromPlayerPos))
        {
            targetPos = (Vector2)PlayerManager.Instance.PlayerController.transform.position +
                InputManager.Instance.DirFromPlayerPos.normalized * fireMinDisLimit;
        }
        
        return (targetPos - _SpawnPos).normalized;
    }


    #endregion

    #region Effect

    private void ExplosionEffect_Fan(Vector2 _SpawndPos, eDamageType _DamageType, bool _IsCritical,
        int _Index, Vector2 _Dir)
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

        PlayerController.PlayerMEI.GenExplosionImgs_Fan(
            _SpawndPos,
            _Dir, 45f,
            3, 0.2f, 1f,
            0.8f, 0.05f, 0.1f,
            0.4f, 0.5f, 1.0f,
            index, PlayerController.ThisPlayerMaterial_000);
    }

    #endregion


}

