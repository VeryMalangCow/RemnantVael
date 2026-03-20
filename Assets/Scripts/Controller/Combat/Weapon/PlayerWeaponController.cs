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
    [SerializeField] public eDamageType dmgType;
    [SerializeField] public float aliveTime;
    [HideInInspector] public float currentDelayROF = 0;
    [HideInInspector] public bool isInputed = false;
    [HideInInspector] public bool isShooting = false;

    [Space(10)]
    [Header("=== BUState")]
    [SerializeField] public BUState<float> baseDamage;
    [SerializeField] public BUState<float> muzzleSpeed;
    [SerializeField] public BUState<float> rof;
    [SerializeField] public BUState<float> cc;
    [SerializeField] public BUState<float> cd;
    [SerializeField] public BUState<float> accRate;
    [SerializeField] public BUState<float> kbPower;

    [Space(10)]
    [Header("=== GunPos")]
    [SerializeField] protected List<Transform> bulletSpawnTfList;

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
    private void Caculate_ROF(float deltaTime)
    {
        if (currentDelayROF < 1f)
        {
            currentDelayROF += deltaTime * rof.buffedState;
            isShooting = true;
        }
        else
        {
            InputManager.instance.aimController.Set_ActivingAttack(false);
            isShooting = false;
        }
    }

    #endregion

    #region Fire

    // 사격 시도
    private void Try_Fire()
    {
        if (Check_Fire())
        {
            Play_Fire(PoolingManager.instance.Get_OP_PlayerBullet(bulletSpawnTfList.Count));
            PlayerManager.instance.cameraController.Play_ShotAnim(1 / rof.buffedState, PlayerController.BaseWeapon.baseDamage.buffedState);
            ModuleItemManager.instance.Active_Fire();
        }
    }

    // 사격을 해야 하는가 + 할 수 있는가
    private bool Check_Fire()
    {
        if (isInputed &&
           currentDelayROF >= 1 &&
           PlayerController.MovementState == eMovementState.IdleOrWalk)
        {
            return true;
        }
        return false;
    }

    // 사격 (발사)
    protected void Play_Fire(List<PlayerBulletController> bulletList)
    {
        float randomAngle = DevTool.Get_RandomValueBaseZero(100 - accRate.actualState.Value);
        
        for (int i = 0; i < bulletSpawnTfList.Count; i++)
        {
            Play_Fire(bulletList[i], DevTool.Get_ComponentTType<DepthController>(bulletSpawnTfList[i].gameObject), randomAngle);
        }

        InputManager.instance.aimController.Set_ActivingAttack(true);
        currentDelayROF -= 1;

        // 모듈 싱크 효과 => 사격 후
        ModuleItemManager.instance.ActiveSync_AfterFire();

        // Tween
        this.transform.DOShakePosition(1f / rof.buffedState, 0.05f, 20, 90, false, true);

        // Audio

        // Sound
        SoundManager.instance.Play_2D_SFX_Player_Random(
            PlayerController.Get_AS(), PlayerController.Get_ID(), "Shot", 2);
    }

    // 사격 (한발마다)
    private void Play_Fire(PlayerBulletController bullet, DepthController targetSpawnDepth, float spreadAngle)
    {
        Vector2 dir = DevTool.Get_MinFireDir(targetSpawnDepth.transform.position);

        // 총알 스탯과 SortingOrder 설정
        bullet.Set_State(
            Get_CurrentBulletState(),
            state_PosAndRot: new BulletState_PosAndRot(targetSpawnDepth.transform.position, dir, spreadAngle),
            state_Size: null,
            state_Anim: null,
            state_Effect: null,
            targetSpawnDepth.TargetRange);

        ModuleItemManager.instance.ActiveSync_Fire(bullet);

        // 폭발 이펙트   
        UnitManager.instance.player_ExplImgGenerator.Expl_Player_ShootBaseBullet(
            PlayerController.Get_ID(),
            (Vector2)targetSpawnDepth.TargetObject.transform.position + (dir * 0.1f),
            dir,
            dmgType,
            bullet.state.isCritical);
    }

    #endregion

    #region Get

    // 플레이어의 현재 총알 스탯을 가져오기
    private BulletState Get_CurrentBulletState()
    {
        return new BulletState(
            new CombatState(
                new CombatOwner(eCombatOwner.Player),
                new DmgState(dmgType, PlayerController.BaseWeapon.baseDamage.buffedState),
                new CriticalState(PlayerController.BaseWeapon.cc.actualState.Value, PlayerController.BaseWeapon.cd.buffedState),
                new KnockbackState(dmgType == eDamageType.Physics ? true : false, PlayerController.BaseWeapon.kbPower.actualState.Value, 0.2f)),
            checkIsCritical: true,
            muzzleSpeed: muzzleSpeed.actualState.Value,
            aliveTime);
    }

    #endregion    
}

