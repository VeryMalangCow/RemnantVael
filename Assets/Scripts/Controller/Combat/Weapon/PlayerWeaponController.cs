using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : PlayerSolarController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public eDamageType DamageType;
    [SerializeField] public float AliveTime;
    [HideInInspector] public float CurrentDelayROF = 0;
    [HideInInspector] public bool IsInputed = false;
    [HideInInspector] public bool IsShooting = false;

    [Space(10)]
    [Header("=== BUState")]
    [SerializeField] public BUState<float> BaseDamage;
    [SerializeField] public BUState<float> MuzzleSpeed;
    [SerializeField] public BUState<float> ROF;
    [SerializeField] public BUState<float> CC;
    [SerializeField] public BUState<float> CD;
    [SerializeField] public BUState<float> AccuracyRate;
    [SerializeField] public BUState<float> KnockbackPower;

    [Space(10)]
    [Header("=== GunPos")]
    [SerializeField] protected List<Transform> BulletSpawnTFList;

    #endregion

    #endregion

    #region Framework

    private void FixedUpdate()
    {
        Caculate_ROF(Time.fixedDeltaTime);
        Try_Fire();
    }

    #endregion

    #region ROF

    // ROF를 계산해, 연사력에 맞는 발사를 계산
    private void Caculate_ROF(float _DeltaTime)
    {
        if (CurrentDelayROF < 1f)
        {
            CurrentDelayROF += _DeltaTime * ROF.BuffedState;
            IsShooting = true;
        }
        else
        {
            InputManager.Instance.aimController.Set_ActivingAttack(false);
            IsShooting = false;
        }
    }

    #endregion

    #region Fire

    // 사격 시도
    private void Try_Fire()
    {
        if (Check_Fire())
        {
            Play_Fire(PoolingManager.Instance.Get_OP_PlayerBullet(BulletSpawnTFList.Count));
            PlayerManager.Instance.cameraController.Play_ShotAnim(1 / ROF.BuffedState, PlayerController.BaseWeapon.BaseDamage.BuffedState);
            ModuleItemManager.Instance.Active_Fire();
        }
    }

    // 사격을 해야 하는가 + 할 수 있는가
    private bool Check_Fire()
    {
        if (IsInputed &&
           CurrentDelayROF >= 1 &&
           PlayerController.MovementState == eMovementState.IdleOrWalk)
        {
            return true;
        }
        return false;
    }

    // 사격 (발사)
    protected void Play_Fire(List<PlayerBulletController> _BulletList)
    {
        float randomAngle = DevTool.Get_RandomValueBaseZero(100 - AccuracyRate.ActualState.Value);
        
        for (int i = 0; i < BulletSpawnTFList.Count; i++)
        {
            Play_Fire(_BulletList[i], DevTool.Get_ComponentTType<DepthController>(BulletSpawnTFList[i].gameObject), randomAngle);
        }

        InputManager.Instance.aimController.Set_ActivingAttack(true);
        CurrentDelayROF -= 1;

        // 모듈 싱크 효과 => 사격 후
        ModuleItemManager.Instance.ActiveSync_AfterFire();

        // Tween
        this.transform.DOShakePosition(1f / ROF.BuffedState, 0.05f, 20, 90, false, true);

        // Audio

        // Sound
        SoundManager.Instance.Play_2D_SFX_Player_Random(
            PlayerController.Get_AS(), PlayerController.Get_ID(), "Shot", 2);
    }

    // 사격 (한발마다)
    private void Play_Fire(PlayerBulletController _Bullet, DepthController _TargetSpawnDepth, float _SpreadAngle)
    {
        Vector2 dir = DevTool.Get_MinFireDir(_TargetSpawnDepth.transform.position);

        // 총알 스탯과 SortingOrder 설정
        _Bullet.Set_State(
            Get_CurrentBulletState(),
            _State_PosAndRot: new BulletState_PosAndRot(_TargetSpawnDepth.transform.position, dir, _SpreadAngle),
            _State_Size: null,
            _State_Anim: null,
            _State_Effect: null,
            _TargetSpawnDepth.TargetRange);

        ModuleItemManager.Instance.ActiveSync_Fire(_Bullet);

        // 폭발 이펙트   
        UnitManager.Instance.player_ExplImgGenerator.Expl_Player_ShootBaseBullet(
            PlayerController.Get_ID(),
            (Vector2)_TargetSpawnDepth.TargetObject.transform.position + (dir * 0.1f),
            dir,
            DamageType,
            _Bullet.State.IsCritical);
    }

    #endregion

    #region Get

    // 플레이어의 현재 총알 스탯을 가져오기
    private BulletState Get_CurrentBulletState()
    {
        return new BulletState(
            new CombatState(
                new CombatOwner(eCombatOwner.Player),
                new DmgState(DamageType, PlayerController.BaseWeapon.BaseDamage.BuffedState),
                new CriticalState(PlayerController.BaseWeapon.CC.ActualState.Value, PlayerController.BaseWeapon.CD.BuffedState),
                new KnockbackState(DamageType == eDamageType.Physics ? true : false, PlayerController.BaseWeapon.KnockbackPower.ActualState.Value, 0.2f)),
            _CheckIsCritical: true,
            _MuzzleSpeed: MuzzleSpeed.ActualState.Value,
            AliveTime);
    }

    #endregion    
}

