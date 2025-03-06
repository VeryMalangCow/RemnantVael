using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : SolarSystemController
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
        PitchTF.transform.localRotation = Get_RotationSmooth(InputManager.Instance.DirFromPlayerPos.normalized, PitchTF, rotateSpeed);

        foreach (SatelliteController hand in Hands)
        {
            hand.SetPos(PlayerSR.sortingOrder);
        }

        Caculate_ROF();

        if (Can_Fire())
        {
            List<PlayerBulletController> PBClist = new List<PlayerBulletController>();
            foreach (Transform TF in BulletSpawnTFs)
            { PBClist.Add(PoolingManager.Instance.Get_OP_PlayerBullet()); }

            Play_Fire(PBClist);
            PlayerManager.Instance.CameraController.Play_ShotAnim(1/ROF.ActualState.Value, PBClist[0].State.DmgState.Dmg);

            ModuleItemManager.Instance.Active_Fire();
        }
    }


    #endregion

    #region ROF

    private void Caculate_ROF()
    {
        CurrentDelayROF += Time.deltaTime * ROF.ActualState.Value;

        if (CurrentDelayROF > 1)
        {
            CurrentDelayROF = 1;
            InputManager.Instance.AimController.Set_AttackState(false);
        }
    }

    #endregion

    #region Judg Can Fire

    private bool Can_Fire()
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

    protected void Play_Fire<T>(List<T> _Ts)
    {
        float spreadMaxLimit = 100 - AccuracyRate.ActualState.Value;
        float randomAngle = UnityEngine.Random.Range(-spreadMaxLimit, spreadMaxLimit);
        //randomAngle = 0f;
        for (int i = 0; i < BulletSpawnTFs.Count; i++)
        {
            PlayerBulletController PBC = StaticCaculator.Get_CastingTType<PlayerBulletController>(_Ts[i]);
            //PlayerBulletController PBC = GameManager.Get_CastIfPossible<PlayerBulletController>(_Ts[i]);

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
            if (BulletSpawnTFs[i].TryGetComponent(out DepthController HST))
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
            Vector2 dir = Get_Dir((Vector2)BulletSpawnTFs[i].transform.position);

            // Base State 

            DmgState dmgState = new DmgState(DamageType, PlayerController.BaseWeapon.BaseDamage.BuffedState);
            CriticalState criticalState = new CriticalState(PlayerController.BaseWeapon.CC.ActualState.Value, PlayerController.BaseWeapon.CD.ActualState.Value);
            KnockbackState knockbackState = new KnockbackState(ableKnockback, PlayerController.BaseWeapon.KnockbackPower.ActualState.Value, 0.2f);

            BulletState bulletState = new BulletState(new CombatState(dmgState, criticalState, knockbackState), true, MuzzleSpeed.ActualState.Value, AliveTime);

            PBC.Set_State(BulletSpawnTFs[i].position, randomAngle, bulletState, dir, targetShadow);

            // Sorting Layer
            if (BulletSpawnTFs[i].gameObject.TryGetComponent(out DepthController hst))
            { PBC.ThisSR.sortingOrder = hst.ThisSR.sortingOrder - 1; }

            // Effect
            if (BulletSpawnTFs[i].TryGetComponent(out DepthController posHst))
            {
                Gen_ExplosionEffect_Fan((Vector2)posHst.TargetObject.transform.position + (dir * 0.3f),
                    DamageType, isCritical, i, dir);
            }
        }
        InputManager.Instance.AimController.Set_AttackState(true);
        CurrentDelayROF = 0;

        // Tween
        this.transform.DOShakePosition(1f / ROF.ActualState.Value, 0.05f, 20, 90, false, true);
        
    }

    public Vector2 Get_Dir(Vector2 _SpawnPos)
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

    private void Gen_ExplosionEffect_Fan(Vector2 _SpawndPos, eDamageType _DamageType, bool _IsCritical,
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

        PlayerController.PlayerMEI.Gen_ExplosionImgs_Fan(
            _SpawndPos,
            _Dir, 45f,
            3, 0.2f, 1f,
            0.8f, 0.05f, 0.1f,
            0.4f, 0.5f, 1.0f,
            index, PlayerController.ThisPlayerMaterial_000);
    }

    #endregion


}

