using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MovableObject
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]
    [SerializeField] public int ID;

    #region - Combat

    [Space(10)]
    [Header("=== Combat")]

    [Header("-- State")]
    [SerializeField] private eCombatMode CombatMode = eCombatMode.Physics;
    [SerializeField] private eCombatMode TargetCombatMode = eCombatMode.Physics;
    [SerializeField] public bool IsCasting = false;
    [SerializeField] private float CurrentCastingTime = 0;
    [SerializeField] private float TargetCastingTime = 0;
    [SerializeField] public BaseUpgradeState<float> AvoidChance;

    [Header("-- Invincible")]
    [SerializeField] private bool IsInvincible = false;
    [SerializeField] private float MaxInvincibleTime = 0.5f;
    [SerializeField] private float CurrentInvincibleTime = 0f;
    private Sequence InvincibleSeq;

    [Header("-- Energy")]
    [SerializeField] public BaseUpgradeState<float> MaxEP;
    [SerializeField] public BaseUpgradeState<float> TakingDmgMultiple;
    [SerializeField] public ReactiveProperty<float> CurrentEP = new();
    [SerializeField] public BaseUpgradeState<float> SpawnESMultiple;
    [SerializeField] public BaseUpgradeState<float> NeedEP_ForSkillMultiple;
    [SerializeField] public Sprite ES_Sprite;

    [Header("-- Shield")]
    [SerializeField] public List<Shield> ShieldElements = new List<Shield>();


    [Header("-- Bettery")]
    [SerializeField] public ReactiveProperty<int> CurrentBS = new();
    [SerializeField] public int NeedBS_ForMakeBC = 4;
    [SerializeField] public float NeedEP_ForMakeEC = 5;
    [SerializeField] public ReactiveProperty<int> CurrentBC = new();
    [SerializeField] public ReactiveProperty<int> CurrentEC = new();

    [Header("-- Module")]
    [SerializeField] public ReactiveProperty<int> CurrentMS = new();

    [Header("-- Weapon")]
    [SerializeField] public PlayerWeaponController BaseWeapon;

    #endregion

    #region - Skill

    [Space(10)]
    [Header("=== Skill")]

    [Header("-- Weapon")]
    [SerializeField] public SkillWeaponController SkillWeapon;

    #endregion

    #region - Movement

    [Space(10)]
    [Header("=== Movement")]

    [Header("-- State")]
    [SerializeField] public eMovementState MovementState = eMovementState.IdleOrWalk;
    [SerializeField] public BaseUpgradeState<float> WalkSpeed;
    [SerializeField] public BaseUpgradeState<float> WalkSpeedWhenShotMultiple;

    [Header("-- Dash")]
    [SerializeField] public PlayerDashController DashController;

    #endregion

    #region - Buff

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] public List<BuffController> CurrentBuffs;

    #endregion

    #region - Interact

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private List<GameObject> CurrentInteractableGOList;
    [SerializeField] public ReactiveProperty<IInteract> CurrentInteractable = new();

    #endregion

    #region - Boost

    [Space(10)]
    [Header("=== Boost")]
    [SerializeField] private int TargetBoostlv = 0;
    [SerializeField] public int MaxBoostLv = 4;
    [SerializeField] public ReactiveProperty<int> CurrentBoostLv = new();
    [SerializeField] private List<float> DecEnergyPointByLevel;
    [SerializeField] public BaseUpgradeState<float> DecEnergyPointMultiple;

    private delegate void SkillDele();
    private SkillDele ReservationSkillDele = null;

    #endregion

    #region - Lower

    [Space(10)]
    [Header("=== Lower")]
    [SerializeField] public LowerController LowerController;

    #endregion

    #region - Material

    [Space(10)]
    [Header("=== Material")]
    [SerializeField] public Material ThisPlayerMaterial_000;
    [SerializeField] public Material ThisPlayerMaterial_001;

    [Space(10)]
    [SerializeField] private Color ThisPBHColor;
    [SerializeField] private Color ThisPCHColor;
    [SerializeField] private Color ThisEBHColor;
    [SerializeField] private Color ThisECHColor;

    #endregion

    #region - Effect

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] public MakeExplosionImage PlayerMEI;
    [SerializeField] public MakeAfterImage PlayerMAI;

    [Header("-- Hitted")]
    [SerializeField] public AnimationClip PhysicsHittedPointAC;
    [SerializeField] public AnimationClip PhysicsCriticalHittedPointAC;
    [SerializeField] public AnimationClip EnergyHittedPointAC;
    [SerializeField] public AnimationClip EnergyCriticalHittedPointAC;

    [Header("-- DamageType Icon")]
    [SerializeField] private SetStateAnim StateAnim;
    [SerializeField] private AnimationClip PhysicsStateAC;
    [SerializeField] private AnimationClip EnergyStateAC;
    [SerializeField] private AnimationClip ChangeStateAC;

    [Space(10)]
    [Header("-- BoostMode Icon")]
    [SerializeField] private List<SetStateAnim> BoostStateAnimList;
    [SerializeField] private AnimationClip BoostOffAC;
    [SerializeField] private AnimationClip BoostOnAC;
    [SerializeField] private List<SetStateAnim> BoostStateVFXAnimList;
    [SerializeField] private List<AnimationClip> BoostVFXAnimList;

    [Space(10)]
    [Header("-- Inner Img")]
    [SerializeField] private Sprite ChangeState_DamageType;
    [SerializeField] private Sprite ChangeState_BoostUp;
    [SerializeField] private Sprite ChangeState_BoostDown;
    [SerializeField] private Sprite ChangeState_Skill0;
    [SerializeField] private Sprite ChangeState_Skill1;

    [Space(10)]
    [Header("-- Room Move Img")]
    [SerializeField] private SetStateAnim MoveDirStateAnim;
    [SerializeField] private AnimationClip MoveDirAC;

    #endregion

    #region - Aim

    [Space(10)]
    [Header("=== Aim")]
    [SerializeField] public GameObject AimPrefab;
    [SerializeField] public AimRoundController AimRoundController;

    #endregion

    #region - WayPoint

    [Space(10)]
    [Header("=== WayPoint")]
    [SerializeField] public WayPoint ThisWayPoint;

    #endregion

    #region - UI

    [Space(10)]
    [Header("=== BUUI")]
    [SerializeField] public List<string> BUUITabStringList;

    [Space(10)]
    [Header("=== MUUI")]
    [SerializeField] public List<string> MUUITabStringList;

    #endregion

    #endregion

    #region Framework

    private void Start()
    {
        CurrentEC.Value = 100;
        CurrentMS.Value = 100;
        CurrentInteractable
            .Subscribe(interact =>
            {
                MainGameUIManager.Instance.PlayerHUD_UIController.SetStateInteractUI();
                MainGameUIManager.Instance.InteractAnno_UIController.SetOnOffUI();
            });

        SetBaseAnimTween();

        StateAnim.SetAnim(PhysicsStateAC, 0.8f, 1f);
        SetBoostAnim(CurrentBoostLv.Value, MaxBoostLv);
        MoveDirStateAnim.SetAnim(MoveDirAC);
        SetOffRoomMoveDir();
    }

    protected override void Update()
    {
        base.Update();
        AlwaysCaculate();
        //ModuleItemManager.Instance.ActiveSkill_Always();
        SetOnOffMoveDir();
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void LateUpdate()
    {
        MainGameUIManager.Instance.InteractAnno_UIController.SetPosIfNot(CurrentInteractable.Value);
    }

    #endregion

    #region Stage

    public void SetPastStartStage()
    {
        if (this.TryGetComponent(out SortingGroup SG))
        { 
            SG.enabled = true;
            SG.sortingOrder = -3002;
        }
        ThisSR.sortingOrder = 2;

        StateAnim.transform.parent.transform.gameObject.SetActive(false);

    }

    public void SetStartStage()
    {
        if (this.TryGetComponent(out SortingGroup SG))
        {
            SG.enabled = false;
            SG.sortingOrder = 0;
        }

        StateAnim.transform.parent.transform.gameObject.SetActive(true); 

        InputManager.Instance.OnEnableInput();
        StageManager.Instance.IsStartStage = false;
        LayerOrderManager.Instance.NeedLayerObjects.Add(PlayerManager.Instance.PlayerController);

    }

    public void SetEndStage()
    {
        if (this.TryGetComponent(out SortingGroup SG))
        {
            SG.enabled = true;
            SG.sortingOrder = 3001; 
        }

        StateAnim.transform.parent.transform.gameObject.SetActive(false);

        InputManager.Instance.OnDisableInput();
    }

    #endregion

    #region Energy

    public void IncreaseMaxEP(float _AddValue)
    {
        MaxEP.ActualState.Value += _AddValue;
        AddCurrentEP(_AddValue);
    }

    public void AddCurrentEP(float _AddValue)
    {
        float result = CurrentEP.Value + _AddValue;
        result = Math.Max(result, 0);
        result = Math.Min(result, MaxEP.ActualState.Value);

        CurrentEP.Value = result;

        if (CurrentEP.Value <= 0)
        {
            Die();
        }
    }

    public float PercentHP(float _Percent)
    {
        return (_Percent / 100) * MaxEP.ActualState.Value;
    }

    #endregion

    #region Bettery

    public void AddCurrentBS(int _AddValue)
    {
        CurrentBS.Value += _AddValue;
        while(true)
        {
            if (CurrentBS.Value >= NeedBS_ForMakeBC)
            {
                CurrentBS.Value -= NeedBS_ForMakeBC;
                CurrentBC.Value++;
                MainGameUIManager.Instance.PlayerHUD_UIController.CurrentEmptyBC.Complete(0.3f, 0.2f);
            }
            else
            {
                break;
            }
        }
    }

    private void ChargeBettery()
    {
        this.CurrentEP.Value -= NeedEP_ForMakeEC;
        CurrentBC.Value--;
        CurrentEC.Value++;
    }

    #endregion

    #region Shield

    // Get total
    private float GetTotalShield()
    {
        float totalShield = 0;
        if (ShieldElements.Count > 0)
        {
            for (int i = 0;  i < ShieldElements.Count; i++)
            {
                totalShield += ShieldElements[i].ShieldCurrentValue;
            }
        }
        return totalShield;
    }

    // Gain Shield
    public void GainShield(Shield _S)
    {
        if (ShieldElements.Contains(_S))
        {
            ShieldElements.Remove(_S);
        }
        ShieldElements.Insert(0, _S);
        MainGameUIManager.Instance.PlayerHUD_UIController.SetShieldGage(GetTotalShield());
    }

    // Remove Shield
    public void RemoveShield(Shield _S)
    {
        if (ShieldElements.Contains(_S))
        {
            for (int i = 0; i < CurrentBuffs.Count;i++)
            {
                if (CurrentBuffs[i] is BuffShieldController BSC && BSC.ThisShield == _S)
                {
                    BSC.EndBuff();
                }
            }

            ShieldElements.Remove(_S);
        }
        MainGameUIManager.Instance.PlayerHUD_UIController.SetShieldGage(GetTotalShield());
    }



    #endregion

    #region Module

    public void AddCurrentMS(int _AddValue)
    {
        CurrentMS.Value += _AddValue;
    }

    #endregion

    #region Movement

    private void Movement()
    {
        switch(MovementState)
        {
            case eMovementState.IdleOrWalk:
                if(BaseWeapon.CurrentDelayROF >= 1)
                {
                    Walk(InputManager.Instance.InputMoveDir, WalkSpeed.ActualState.Value, AccelerationSpeed);
                }
                else
                {
                    Walk(InputManager.Instance.InputMoveDir, WalkSpeed.ActualState.Value * WalkSpeedWhenShotMultiple.ActualState.Value, AccelerationSpeed);
                }
                break;

            case eMovementState.Dash:
                DashController.Dash();
                break;

            default: break;
        }
    }


    public void CanDashCheck()
    {
        if (!CanChange() || DashController.NeedEP_ForDash * NeedEP_ForSkillMultiple.ActualState.Value >= CurrentEP.Value)
        {
            return;
        }

        InputManager.Instance.IsPlayingSkill = true;
        PlayerMAI.StartGen(0.7f, 0.03f, 0.5f);


        CurrentEP.Value -= DashController.NeedEP_ForDash * NeedEP_ForSkillMultiple.ActualState.Value;
        MovementState = eMovementState.Dash;
    }

    #endregion

    #region Damage

    public void TryHitted(EnemyBulletController _EBC)
    {
        if (IsInvincible)
        { return; }

        // 항상
        Hitted();

        // 피격
        if (!IsAvoid()) // 회피인지?
        {
            // 간소화
            BulletState state = _EBC.BulletState;
            EnemyBuffController buff = _EBC.Enemy.BuffController;

            // 적이 냉기 디버프에 걸린지
            float actualDmg =
                state.BaseDamage * (1f - (buff.ColdStack.CurrentStack * (buff.AbsoluteZeroStack.CurrentStack + 1) * 0.01f));

            // 데미지 구현
            TakeDamaged(actualDmg,
                ((Vector2)transform.position - (Vector2)_EBC.transform.position).normalized,
                state.AbleKnockback,
                state.KnockbackPower,
                state.KnockbackTime);
        }
    }

    public void TryHitted(EnemyAttacker _Attacker)
    {
        if (IsInvincible)
        { return; }

        // 항상
        Hitted();

        // 피격
        if (!IsAvoid()) // 회피인지?
        {
            // 간소화
            AttackerState state = _Attacker.AttackerState;
            EnemyBuffController buff = _Attacker.Enemy.BuffController;

            // 적이 냉기 디버프에 걸린지
            float actualDmg =
                state.BaseDamage * (1f - (buff.ColdStack.CurrentStack * (buff.AbsoluteZeroStack.CurrentStack + 1) * 0.01f));

            // 데미지 구현
            TakeDamaged(actualDmg, 
                ((Vector2)transform.position - (Vector2)_Attacker.transform.position).normalized,
                state.AbleKnockback,
                state.KnockbackPower,
                state.KnockbackTime);
        }
    }

    public bool IsAvoid()
    {
        // 회피
        if (UnityEngine.Random.Range(0f, 1f) < AvoidChance.ActualState.Value)
        {
            Avoided();
            return true;
        }
        return false;
    }

    private void Hitted()
    {
        IsInvincible = true;
        CurrentInvincibleTime = 0f;

        InvincibleSeq = DOTween.Sequence();
        float intervalTime = 0.075f;
        for (int i = 0; i < PlayerMAI.TargetSRList.Count; i++)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(PlayerMAI.TargetSRList[i].DOFade(0, 0));
            seq.AppendInterval(intervalTime);
            seq.Append(PlayerMAI.TargetSRList[i].DOFade(1, 0));
            seq.AppendInterval(intervalTime);
            seq.SetLoops((int)(MaxInvincibleTime / (intervalTime * 2f)), LoopType.Restart);
        }
    }

    private void TakeDamaged(float _DmgValue, Vector2 _HittedDir, bool _AbleKB, float _KBPower, float _KBTime)
    {
        // Multiple
        _DmgValue *= TakingDmgMultiple.BuffedState;

        // Effect
        PlayerManager.Instance.CameraController.PlayDamagedAnim(MaxInvincibleTime, _DmgValue * 0.1f, _HittedDir);

        // Knockback
        if (_AbleKB)
        { GetKnockback(new KnockbackState(_HittedDir, _KBPower, _KBTime)); }

        // Damage
        TakeDamaged(_DmgValue);
    }

    public void TakeDamaged(float _DmgValue)
    {
        float Dmg = _DmgValue;
        MainGameUIManager.Instance.PlayerHUD_UIController.HittedPlayScreen(Dmg);
        if (ShieldElements.Count > 0)
        {
            for (int i = ShieldElements.Count - 1; i >= 0; i--)
            {
                // 쉴드 버프량 1개가 데미지보다 작거나 같으면, 제거하고 다음 쉴드로 영향
                if (ShieldElements[i].ShieldCurrentValue <= Dmg)
                {
                    Dmg -= ShieldElements[i].ShieldCurrentValue;

                    RemoveShield(ShieldElements[i]); // 쉴드 감소
                }
                else // 쉴드 버프가 데미지를 버틸 수 있으면
                {
                    ShieldElements[i].ShieldCurrentValue -= Dmg;
                    Dmg = 0;
                    MainGameUIManager.Instance.PlayerHUD_UIController.SetShieldGage(GetTotalShield());
                    return;
                }
            }
        }
        AddCurrentEP(-Dmg);
        MainGameUIManager.Instance.PlayerHUD_UIController.SetShieldGage(GetTotalShield());
        BuffManager.Instance.Active_Hitted();
    }

    public void TakeExtraDamage(float _DmgValue)
    {
        float Dmg = _DmgValue;
        if (ShieldElements.Count > 0)
        {
            for (int i = ShieldElements.Count - 1; i >= 0; i--)
            {
                // 쉴드 버프량 1개가 데미지보다 작거나 같으면, 제거하고 다음 쉴드로 영향
                if (ShieldElements[i].ShieldCurrentValue <= Dmg)
                {
                    Dmg -= ShieldElements[i].ShieldCurrentValue;

                    RemoveShield(ShieldElements[i]); // 쉴드 감소
                }
                else // 쉴드 버프가 데미지를 버틸 수 있으면
                {
                    ShieldElements[i].ShieldCurrentValue -= Dmg;
                    Dmg = 0;
                    MainGameUIManager.Instance.PlayerHUD_UIController.SetShieldGage(GetTotalShield());
                    return;
                }
            }
        }
        AddCurrentEP(-Dmg);
        MainGameUIManager.Instance.PlayerHUD_UIController.SetShieldGage(GetTotalShield());
    }

    private void Avoided()
    {
        PlayerManager.Instance.CameraController.PlayAvoidAnim(MaxInvincibleTime);

        for (int i = 0; i < 4; i++)
        {
            PlayerMEI.GenExplosionImgs(
                    TargetObject.transform.position,
                    6, 0.15f, 0.75f,
                    2.0f, 0.05f, 0.1f,
                    1.0f, 0.5f, 1.0f,
                    i, ThisPlayerMaterial_000);
        }

    }

    private void Die()
    {

    }

    #endregion

    #region About Casting

    // Condition : Idle or Walk | No Casting Now 
    private bool CanChange()
    {
        if (MovementState != eMovementState.IdleOrWalk || IsCasting)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // + None
    [HideInInspector] private const float CombatModeInterval = 0.75f;
    public void CanChange_CombatModeCheck()
    {
        if (!CanChange()) 
        { return; }

        StateAnim.SetAnim(ChangeStateAC, ChangeState_DamageType, 2f, 1f);

        switch (TargetCombatMode)
        {
            case eCombatMode.Physics:
                TargetCombatMode = eCombatMode.Energy;
                break;

            case eCombatMode.Energy:
                TargetCombatMode = eCombatMode.Physics;
                break;

            default:
                break;
        }

        StartCasting(CombatModeInterval);
    }


    // + Not over the Max Boost Level
    [HideInInspector] private const float BoostModeInterval = 0.5f;
    public void CanChange_BoostModeCheck()
    {
        if (!CanChange() || TargetBoostlv >= MaxBoostLv)
        { return; }

        TargetBoostlv++;
        StateAnim.SetAnim(ChangeStateAC, ChangeState_BoostUp, 2f, 1f);

        StartCasting(BoostModeInterval);
    }


    // + BoostLevel != 0
    [HideInInspector] private const float UnBoostModeInterval = 0.25f;
    public void CanChange_UnBoostModeCheck()
    {
        if (!CanChange() || CurrentBoostLv.Value <= 0)
        { return; }

        TargetBoostlv--;
        StateAnim.SetAnim(ChangeStateAC, ChangeState_BoostDown, 2f, 1f);

        StartCasting(UnBoostModeInterval);
    }


    // + Enough EP | Enough BC
    [HideInInspector] private const float ChargeBetteryInterval = 1f;
    public void CanChange_ChargeBettery()
    {
        if (!CanChange() ||
            NeedEP_ForMakeEC >= this.CurrentEP.Value ||
            CurrentBC.Value <= 0) 
        { return; }

        ReservationSkillDele = ChargeBettery;

        StartCasting(ChargeBetteryInterval);
    }


    // + None
    [HideInInspector] public float ExecutionInterval = 0.75f;
    public void CanChange_Execution()
    {
        if (!CanChange())
        { return; }

        if (CurrentInteractable.Value is EnemyController EC)
        {
            EC.Interact();
        }

        StartCasting(ExecutionInterval);
    }

    // + None
    [HideInInspector] public float Skill0Interval = 0.5f;
    public void CanChange_Skill0()
    {
        if (!CanChange())
        { return; }
        if (!SkillWeapon.Skill_0.CanActive())
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.Skill0.StartNotEnoughEP();
            return;
        }

        ReservationSkillDele = SkillWeapon.Skill_0.ActiveSkill;
        StateAnim.SetAnim(ChangeStateAC, ChangeState_Skill0, 2f, 1f);

        StartCasting(Skill0Interval);
    }

    [HideInInspector] public float Skill1Interval = 0.5f;
    public void CanChange_Skill1()
    {
        if (!CanChange())
        { return; }
        if (!SkillWeapon.Skill_1.CanActive())
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.Skill1.StartNotEnoughEP();
            return;
        }

        ReservationSkillDele = SkillWeapon.Skill_1.ActiveSkill;
        StateAnim.SetAnim(ChangeStateAC, ChangeState_Skill1, 2f, 1f);

        StartCasting(Skill1Interval);
    }

    public void StartCasting(float _CastingTime)
    {
        TargetCastingTime = _CastingTime;
        IsCasting = true;
        MovementState = eMovementState.Casting;
        ThisRb.velocity = Vector2.zero;

        InputManager.Instance.IsPlayingSkill = true;
    }

    #endregion

    #region Interact

    public void TryInteract()
    {
        if(CurrentInteractable.Value != null && CurrentInteractableGOList.Count > 0)
        {
            if (CurrentInteractable.Value is InteractItemController IIC) // Item
            {
                CurrentInteractable.Value.Interact();
                CurrentInteractable.Value = null;
            }
            else if (CurrentInteractable.Value is EnemyController EC) // Enemy
            {
                if (EC.IsDischarge)
                {
                    CanChange_Execution();
                }
                CurrentInteractable.Value = null;
            }
            else if (CurrentInteractable.Value is DownstartElevatorController DEC && DEC.IsOn)
            {
                CurrentInteractable.Value.Interact();
                CurrentInteractable.Value = null;
            }
            else if (CurrentInteractable.Value is DestructibleBuildingController DBC && !DBC.IsBroken)
            {
                CurrentInteractable.Value.Interact();
            }
            else // OneOffShopController |OR| ...
            {
                CurrentInteractable.Value.Interact();
            }

            MainGameUIManager.Instance.PlayerHUD_UIController.SetUseInteractUI();
        }
    }

    #endregion

    #region Caculate

    #region Time

    private void AlwaysCaculate()
    {
        CastingCaculate();
        BoostingCaculate(CurrentBoostLv.Value);
        InvincibleCaculate();
    }

    private void CastingCaculate()
    {
        if (IsCasting)
        {
            if(CurrentCastingTime < TargetCastingTime)
            {
                CurrentCastingTime += Time.deltaTime;
            }
            else
            {
                CurrentCastingTime = 0f;
                IsCasting = false;
                MovementState = eMovementState.IdleOrWalk;
                InputManager.Instance.IsPlayingSkill = false;

                If_CombatMode();
                If_Skill();
                If_Boost();
            }
        }
    }

    private void BoostingCaculate(int _BoostLv)
    {
        if(_BoostLv > 0)
        {
            float decValue = DecEnergyPointByLevel[_BoostLv - 1] * DecEnergyPointMultiple.ActualState.Value;
            AddCurrentEP(-decValue * Time.deltaTime);
        }
    }

    private void InvincibleCaculate()
    {
        if (IsInvincible)
        {
            if (MaxInvincibleTime > CurrentInvincibleTime)
            {
                CurrentInvincibleTime += Time.deltaTime;
            }
            else
            {
                CurrentInvincibleTime = 0f;

                IsInvincible = false;
            }
        }
    }

    #endregion

    #region By Condition

    private void If_CombatMode()
    {
        if(CombatMode != TargetCombatMode)
        {
            CombatMode = TargetCombatMode;

            switch (CombatMode)
            {
                case eCombatMode.Physics:
                    BaseWeapon.DamageType = eDamageType.Physics;
                    break;

                case eCombatMode.Energy:
                    BaseWeapon.DamageType = eDamageType.Energy;
                    break;

                default:
                    break;
            }
        }

        switch (TargetCombatMode)
        {
            case eCombatMode.Physics:
                StateAnim.SetAnim(PhysicsStateAC, 0.8f, 1f);
                InputManager.Instance.AimController.SetPType();
                break;

            case eCombatMode.Energy:
                StateAnim.SetAnim(EnergyStateAC, 0.8f, 1f);
                InputManager.Instance.AimController.SetEType();
                break;

            default:
                break;
        }
    }

    private void If_Skill()
    {
        if(ReservationSkillDele != null)
        {
            ReservationSkillDele();
            ReservationSkillDele = null;
        }
    }

    private void If_Boost()
    {
        if(CurrentBoostLv.Value != TargetBoostlv)
        {
            CurrentBoostLv.Value = TargetBoostlv;
            SetBoostAnim(CurrentBoostLv.Value, MaxBoostLv);
        }
    }

    #endregion

    #region Multiple

    private float MultipleValueFloat(float _OriginalValue, float Multiple)
    {
        return _OriginalValue * Multiple;
    }

    #endregion

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if(_Col.gameObject.transform.parent.TryGetComponent(out IInteract II))
        {
            GameObject TargetGO = _Col.gameObject.transform.parent.gameObject;

            if (!CurrentInteractableGOList.Contains(TargetGO))
            {
                CurrentInteractableGOList.Add(TargetGO); 
            }
        }
    }

    private void OnTriggerStay2D(Collider2D _Col)
    {
        if (CurrentInteractableGOList != null)
        {
            // None
            if (CurrentInteractableGOList.Count == 0)
            {
                CurrentInteractable.Value = null;
            }

            // Only One
            else if (CurrentInteractableGOList.Count == 1)
            {
                if(CurrentInteractableGOList[0].TryGetComponent(out IInteract II))
                {
                    CurrentInteractable.Value = II;
                }
            }

            // A Lot
            else if (CurrentInteractableGOList.Count > 1)
            {
                float shortDis = Vector3.Distance(gameObject.transform.position, CurrentInteractableGOList[0].transform.position);
                foreach (GameObject currentInteractableGO in CurrentInteractableGOList)
                {
                    float Distance = Vector3.Distance(gameObject.transform.position, currentInteractableGO.transform.position);

                    if (Distance < shortDis) 
                    {
                        currentInteractableGO.TryGetComponent(out IInteract II);
                        CurrentInteractable.Value = II;
                        shortDis = Distance;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _Col)
    {
        if (_Col.gameObject.transform.parent.TryGetComponent(out IInteract II))
        {
            GameObject TargetGO = _Col.gameObject.transform.parent.gameObject;

            if (CurrentInteractableGOList.Contains(TargetGO))
            { 
                CurrentInteractableGOList.Remove(TargetGO); 

                if (CurrentInteractableGOList.Count <= 0)
                {
                    CurrentInteractable.Value = null;
                }
            }
        }
    }

    #endregion

    #region Collision

    private void OnCollisionEnter2D(Collision2D _Col)
    {
        if (_Col.gameObject.tag == "PushPlayer")
        {
            Vector2 pushDir = (this.transform.position - _Col.transform.position).normalized;
            ThisRb.AddForce(pushDir);
        }
    }

    #endregion

    #region Effect


    private void SetBoostAnim(int _Index, int _MaxIndex)
    {
        float lowestAnimSpeed = 0.5f;

        // Wheel
        if (_Index >= _MaxIndex)
        {
            for (int i = 0; i < _MaxIndex - 1; i++)
            {
                BoostStateAnimList[i].SetAnim(BoostOnAC, lowestAnimSpeed * (_MaxIndex * 2), 1f);
            }
        }
        else
        {
            for (int i = 0; i < _MaxIndex - 1; i++)
            {
                if (i < _Index)
                {
                    BoostStateAnimList[i].SetAnim(BoostOnAC, lowestAnimSpeed * (_Index - i + 1), 1f);
                }
                else
                {
                    BoostStateAnimList[i].SetAnim(BoostOffAC, lowestAnimSpeed, 1f);
                }
            }
        }

        // VFX

        BoostStateVFXAnimList[0].gameObject.SetActive(false);
        BoostStateVFXAnimList[1].gameObject.SetActive(false);

        switch (_Index)
        {
            case 1:
                BoostStateVFXAnimList[0].gameObject.SetActive(true);
                BoostStateVFXAnimList[0].SetAnim(BoostVFXAnimList[0], 0.5f, 1f);
                break;

            case 2:
                BoostStateVFXAnimList[0].gameObject.SetActive(true);
                BoostStateVFXAnimList[0].SetAnim(BoostVFXAnimList[0], 1f, 1f);
                break;

            case 3:
                BoostStateVFXAnimList[0].gameObject.SetActive(true);
                BoostStateVFXAnimList[0].SetAnim(BoostVFXAnimList[0], 1f, 1f);

                BoostStateVFXAnimList[1].gameObject.SetActive(true);
                BoostStateVFXAnimList[1].SetAnim(BoostVFXAnimList[1], 1f, 1f);
                break;

            case 4:
                BoostStateVFXAnimList[0].gameObject.SetActive(true);
                BoostStateVFXAnimList[0].SetAnim(BoostVFXAnimList[0], 1.5f, 1f);

                BoostStateVFXAnimList[1].gameObject.SetActive(true);
                BoostStateVFXAnimList[1].SetAnim(BoostVFXAnimList[1], 1.5f, 1f);
                break;

            default:
                break;
        }


    }


    private void SetOnRoomMoveDir(Vector2 _Dir)
    {
        if (!MoveDirStateAnim.gameObject.activeSelf)
        { 
            MoveDirStateAnim.transform.localRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, _Dir));
            MoveDirStateAnim.gameObject.SetActive(true);
        }
    }

    private void SetOffRoomMoveDir()
    {
        if (MoveDirStateAnim.gameObject.activeSelf)
        {
            MoveDirStateAnim.gameObject.SetActive(false);
        }
    }


    private void SetOnOffMoveDir()
    {
        if (CurrentInteractable.Value != null && 
            CurrentInteractable.Value is GateController GC &&
            GC.IsOpen)
        { SetOnRoomMoveDir(GC.GateDir); }
        else
        { SetOffRoomMoveDir(); }
    }



    public Color GetCorrectHitted_C(eDamageType _DamageType, bool _IsCritical)
    {
        if (_DamageType == eDamageType.Physics)
        {
            if (!_IsCritical)
            { return ThisPBHColor; }
            else            
            { return ThisPCHColor; }
        }
        else
        {
            if (!_IsCritical)
            { return ThisEBHColor; }
            else            
            { return ThisECHColor; }
        }
    }

    public AnimationClip GetCorrectHitted_AC(eDamageType _DamageType, bool _IsCritical)
    {
        if (_DamageType == eDamageType.Physics)
        {
            if (!_IsCritical)
            { return PhysicsHittedPointAC; }
            else
            { return PhysicsCriticalHittedPointAC; }
        }
        else
        {
            if (!_IsCritical)
            { return EnergyHittedPointAC; }
            else
            { return EnergyCriticalHittedPointAC; }
        }
    }


    #endregion

    #region Buff

    public void Set_GainBuff(BuffController _Buff)
    {
        if (!CurrentBuffs.Contains(_Buff))
        {
            CurrentBuffs.Add(_Buff);

            // 인터페이스
            if (_Buff is IWhen_Hitted hitted && !BuffManager.Instance.iWhen_HittedList.Contains(hitted))
            {
                BuffManager.Instance.iWhen_HittedList.Add(hitted);
            }
        }
    }

    public void Set_ReductBuff()
    {

    }

    public void Set_EndBuff(BuffController _Buff)
    {
        if (CurrentBuffs.Contains(_Buff))
        {
            CurrentBuffs.Remove(_Buff);
        }

        // 인터페이스
        if (_Buff is IWhen_Hitted hitted && BuffManager.Instance.iWhen_HittedList.Contains(hitted))
        {
            BuffManager.Instance.iWhen_HittedList.Remove(hitted);
        }
    }

    #endregion

}

[System.Serializable]
public class BaseUpgradeState<T>
{
    public T BaseState;
    public ReactiveProperty<int> CurrentLevel;
    public List<T> UpgradeValueByLevelRange;
    public List<int> NeedPayByLevelRange;
    public ReactiveProperty<T> ActualState;

    public List<BuffState<T>> BuffList = new List<BuffState<T>>();

    public string Name;
    [TextArea]
    public string Desc;


    // Gain
    public void GainBuff(BuffState<T> _Bs)
    {
        if (!BuffList.Contains(_Bs))
        {
            BuffList.Add(_Bs);
        }
    }

    // Remove
    public void RemoveBuff(BuffState<T> _Bs)
    {
        if (BuffList.Contains(_Bs))
        {
            BuffList.Remove(_Bs);
        }
    }

    public T BuffedState
    { get; set; }

    

    public void SetBuffedState()
    {
        if (ActualState.Value.GetType() == typeof(float))
        {
            float state = 1.0f;
            for (int i = 0; i < BuffList.Count; i++)
            {
                state += float.Parse(BuffList[i].ActualValue.ToString());
            }
            state *= float.Parse(ActualState.Value.ToString());
            BuffedState = (T)(object)state;
            return;
        }

        Debug.Log("null");
        return;
    }

}


[System.Serializable]
public class Shield
{
    public string ShieldID;
    public float ShieldMaxValue;
    public float ShieldCurrentValue;
}

[System.Serializable]
public class BuffState<T>
{
    public string BuffID;
    public T BaseValue;
    public T ActualValue;
}