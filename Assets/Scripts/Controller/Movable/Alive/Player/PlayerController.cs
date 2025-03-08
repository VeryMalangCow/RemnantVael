using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : AliveObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Controller")]
    [SerializeField] public PlayerWeaponController BaseWeapon;
    [SerializeField] public SkillWeaponController SkillWeapon;
    [SerializeField] public PlayerDashController DashController;
    [SerializeField] public LowerController LowerController;
    [SerializeField] public WayPointController ThisWayPoint;

    [Space(10)]
    [Header("=== Generator")]
    [SerializeField] public ExplosionImgGenerator PlayerMEI;
    [SerializeField] public AfterImgGenerator PlayerMAI;

    #region - Combat

    [Space(10)]
    [Header("=== Mode")]
    [SerializeField] private eCombatMode CombatMode = eCombatMode.Physics;
    [SerializeField] private eCombatMode TargetCombatMode = eCombatMode.Physics;

    [Space(10)]
    [Header("=== Casting")]
    [SerializeField] public bool IsCasting = false;
    [SerializeField] private float CurrentCastingTime = 0;
    [SerializeField] private float TargetCastingTime = 0;
    private delegate void SkillDele();
    private SkillDele ReservationSkillDele = null;

    [Space(10)]
    [Header("=== Invincible")]
    [SerializeField] private bool IsInvincible = false;
    [SerializeField] private float MaxInvincibleTime = 0.5f;
    [SerializeField] private float CurrentInvincibleTime = 0f;
    private Sequence InvincibleSeq;

    #endregion

    #region - Gage Point

    [Space(10)]
    [Header("=== Energy")]
    [SerializeField] public Sprite ES_Sprite;
    [HideInInspector] public ReactiveProperty<float> Get_CurrentEP() => CurrentEP;

    [Space(10)]
    [Header("=== Shield")]
    [SerializeField] public List<Shield> ShieldElements = new List<Shield>();

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public ReactiveProperty<int> CurrentBS = new();
    [SerializeField] public ReactiveProperty<int> CurrentBC = new();
    [SerializeField] public ReactiveProperty<int> CurrentEC = new();
    [SerializeField] public ReactiveProperty<int> CurrentMS = new();
    [SerializeField] public int NeedBS_ForMakeBC = 4;
    [SerializeField] public float NeedEP_ForMakeEC = 5;


    #endregion

    #region - Movement

    [Space(10)]
    [Header("=== Movement")]
    [SerializeField] public eMovementState MovementState = eMovementState.IdleOrWalk;

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


    #endregion

    #region - BU State 

    [Space(10)]
    [Header("=== BU State")]
    [SerializeField] public BUState<float> AvoidChance;
    [SerializeField] public BUState<float> MaxEP;
    [SerializeField] public BUState<float> TakingDmgMultiple;
    [SerializeField] public BUState<float> SpawnESMultiple;
    [SerializeField] public BUState<float> NeedEP_ForSkillMultiple;
    [SerializeField] public BUState<float> WalkSpeed;
    [SerializeField] public BUState<float> WalkSpeedWhenShotMultiple;
    [SerializeField] public BUState<float> DecEnergyPointMultiple;

    #endregion

    #region - Visual

    [Space(10)]
    [Header("=== Material")]
    [SerializeField] public List<Material> ThisPlayerMaterialList;

    [Space(10)]
    [Header("=== Color")]
    [SerializeField] private PlayerVisual<Color> ThisClr;

    [Space(10)]
    [Header("=== Hitted Anim")]
    [SerializeField] private PlayerVisual<AnimationClip> ThisHittedPointAC;

    [Header("=== DamageType Anim")]
    [SerializeField] private StateAnimController StateAnim;
    [SerializeField] private TrioData<AnimationClip> DmgTypeStateAC;

    [Space(10)]
    [Header("=== BoostMode Anim")]
    [SerializeField] private CoupleData<List<StateAnimController>> BoostStateAnimController;
    [SerializeField] private CoupleData<AnimationClip> BoostOnOffAC;
    [SerializeField] private List<AnimationClip> BoostVFXAnimList;

    [Space(10)]
    [Header("=== Inner Img")]
    [SerializeField] private Sprite ChangeState_DamageType;
    [SerializeField] private CoupleData<Sprite> ChangeState_BoostUpDown;
    [SerializeField] private CoupleData<Sprite> ChangeState_Skill;

    [Space(10)]
    [Header("-- Room Move Img")]
    [SerializeField] private StateAnimController MoveDirStateAnim;
    [SerializeField] private AnimationClip MoveDirAC;

    #endregion

    #region - Aim

    [Space(10)]
    [Header("=== Aim")]
    [SerializeField] public GameObject AimPrefab;
    [SerializeField] public AimRoundController AimRoundController;

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

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_Subscribe();
        Offset_Anim();

        CurrentEC.Value = 100;
    }

    private void Offset_Subscribe()
    {
        CurrentInteractable
            .Subscribe(interact =>
            {
                MainGameUIManager.Instance.PlayerHUD_UIController.Set_StateInteractUI();
                MainGameUIManager.Instance.InteractAnno_UIController.Set_UI();
                Set_MoveDir();
            });
        CurrentSP
            .Subscribe(value =>
            {
                MainGameUIManager.Instance.PlayerHUD_UIController.Set_ShieldGage(value);
            });
    }

    private void Offset_Anim()
    {
        Set_BaseAnimTween(); 
        StateAnim.Set_Anim(DmgTypeStateAC.TypeA, 0.8f, 1f);
        Set_BoostAnim(CurrentBoostLv.Value, MaxBoostLv);
        MoveDirStateAnim.Set_Anim(MoveDirAC);
        SetOff_RoomMoveDir();
    }

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();
        Caculate_Always();
    }


    private void FixedUpdate()
    {
        Play_Movement();
    }

    private void LateUpdate()
    {
        MainGameUIManager.Instance.InteractAnno_UIController.Set_PosIfNot(CurrentInteractable.Value);
    }

    #endregion

    #region Stage

    // 스테이지 시작 전
    public void Set_PastStartStage()
    {
        if (this.TryGetComponent(out SortingGroup SG))
        { 
            SG.enabled = true;
            SG.sortingOrder = -3002;
        }
        ThisSR.sortingOrder = 2;

        StateAnim.transform.parent.transform.gameObject.SetActive(false);

    }

    // 스테이지 시작
    public void Set_StartStage()
    {
        if (this.TryGetComponent(out SortingGroup SG))
        {
            SG.enabled = false;
            SG.sortingOrder = 0;
        }

        StateAnim.transform.parent.transform.gameObject.SetActive(true); 

        StageManager.Instance.IsStartStage = false;
        LayerOrderManager.Instance.NeedLayerObjects.Add(this);

    }

    // 스테이지 끝
    public void Set_EndStage()
    {
        if (this.TryGetComponent(out SortingGroup SG))
        {
            SG.enabled = true;
            SG.sortingOrder = 3001; 
        }

        StateAnim.transform.parent.transform.gameObject.SetActive(false);
    }

    #endregion

    #region Shield Point

    // Get total
    private float Get_TotalShield()
    {
        if (ShieldElements.Count <= 0)
        { return 0; }

        float totalShield = 0;
        DevTool.Set_ListDele(ShieldElements, new Dele_RefT_U<float, Shield>(Add_ShieldValue), ref totalShield);
        return totalShield;

    }

    private void Add_ShieldValue(ref float _Variable, Shield _Shield)
    {
        DevTool.Add_RefValue(ref _Variable, _Shield.ShieldCurrentValue);
    }

    // Gain Shield
    public void Gain_Shield(Shield _S)
    {
        if (ShieldElements.Contains(_S))
        {
            ShieldElements.Remove(_S);
        }
        ShieldElements.Insert(0, _S);
        CurrentSP.Value = Get_TotalShield();
    }

    // Remove Shield
    public void Remove_Shield(Shield _S)
    {
        if (ShieldElements.Contains(_S))
        {
            DevTool.Set_ListDele(CurrentBuffs, new Dele_T_U<BuffController, Shield>(EndShieldBuff), _S);
            ShieldElements.Remove(_S);
        }
        CurrentSP.Value = Get_TotalShield();
    }

    // 쉴드 버프를 끝냄
    public void EndShieldBuff(BuffController _Buff, Shield _Shield)
    {
        BuffShieldController shieldBuff = DevTool.Get_CastingTType<BuffShieldController>(_Buff);
        if (DevTool.Is_UsableAndEqual(shieldBuff, shieldBuff.ThisShield, _Shield))
        {
            shieldBuff.End_Buff();
        }
    }

    #endregion

    #region Energy Point

    // 에너지 획득
    public void Add_CurrentEP(float _AddValue)
    {
        Add_CurrentEP(_AddValue, MaxEP.ActualState.Value);
        Check_IsDead(CurrentEP.Value);
    }

    #endregion

    #region Shard

    // 배터리 조각 획득
    public void Add_CurrentBS(int _AddValue)
    {
        CurrentBS.Value += _AddValue;
        if (CurrentBS.Value >= NeedBS_ForMakeBC)
        {
            Add_CurrentBC();
        }
    }

    // 모듈 조각 획득
    public void Add_CurrentMS(int _AddValue)
    {
        CurrentMS.Value += _AddValue;
    }

    #endregion

    #region Cell

    // 배터리 셀 획득
    private void Add_CurrentBC()
    {
        MainGameUIManager.Instance.PlayerHUD_UIController.CurrentEmptyBC.Set_Complete(0.3f, 0.2f);
        int BSAmount = CurrentBS.Value / NeedBS_ForMakeBC;
        CurrentBS.Value -= NeedBS_ForMakeBC * BSAmount;
        CurrentBC.Value += BSAmount;
    }

    // 에너지 셀 획득
    private void Add_CurrentEC()
    {
        this.CurrentEP.Value -= NeedEP_ForMakeEC;
        CurrentBC.Value--;
        CurrentEC.Value++;
    }

    #endregion

    #region Movement

    private void Play_Movement()
    {
        switch(MovementState)
        {
            case eMovementState.IdleOrWalk:
                if(!BaseWeapon.Is_Firing())
                {
                    Play_Walk(InputManager.Instance.InputMoveDir, WalkSpeed.ActualState.Value, AccelerationSpeed);
                }
                else
                {
                    Play_Walk(InputManager.Instance.InputMoveDir, WalkSpeed.ActualState.Value * WalkSpeedWhenShotMultiple.ActualState.Value, AccelerationSpeed);
                }
                break;

            case eMovementState.Dash:
                DashController.Play_Dash();
                break;

            default: break;
        }
    }

    private void Play_Walk()
    {

    }


    public void Set_DashCheck()
    {
        if (!Can_Change() || DashController.NeedEP_ForDash * NeedEP_ForSkillMultiple.ActualState.Value >= CurrentEP.Value)
        {
            return;
        }

        InputManager.Instance.IsPlayingSkill = true;
        PlayerMAI.Start_Gen(0.7f, 0.03f, 0.5f);


        CurrentEP.Value -= DashController.NeedEP_ForDash * NeedEP_ForSkillMultiple.ActualState.Value;
        MovementState = eMovementState.Dash;
    }

    #endregion

    #region Damage

    // 타격: 총알
    public void Try_Hitted(EnemyBulletController _EBC)
    {
        if (IsInvincible)
        { return; }

        // 항상
        Play_Hitted();

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            // 간소화
            BulletState state = _EBC.State;
            EnemyBuffController buff = _EBC.Enemy.BuffController;

            // 적이 냉기 디버프에 걸린지
            float actualDmg =
                state.DmgState.Dmg * (1f - (buff.ColdStack.CurrentStack * (buff.AbsoluteZeroStack.CurrentStack + 1) * 0.01f));

            // 데미지 구현
            Take_Damaged(actualDmg,
                ((Vector2)transform.position - (Vector2)_EBC.transform.position).normalized,
                state.KnockbackState.CanKB,
                state.KnockbackState.KBPower,
                state.KnockbackState.KBTime);
        }
    }

    // 타격: 어택커
    public void Try_Hitted(EnemyAttackerController _Attacker)
    {
        if (IsInvincible)
        { return; }

        // 항상
        Play_Hitted();

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            // 간소화
            AttackerState state = _Attacker.AttackerState;
            EnemyBuffController buff = _Attacker.Enemy.BuffController;

            // 적이 냉기 디버프에 걸린지
            float actualDmg =
                state.DmgState.Dmg * (1f - (buff.ColdStack.CurrentStack * (buff.AbsoluteZeroStack.CurrentStack + 1) * 0.01f));

            // 데미지 구현
            Take_Damaged(actualDmg, 
                ((Vector2)transform.position - (Vector2)_Attacker.transform.position).normalized,
                state.KnockbackState.CanKB,
                state.KnockbackState.KBPower,
                state.KnockbackState.KBTime);
        }
    }

    // 타격: 건물어택커
    public void Try_Hitted(BuildAttackerController _Attacker)
    {
        if (IsInvincible)
        { return; }

        // 항상
        Play_Hitted();

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            // 간소화
            AttackerState state = _Attacker.AttackerState;

            // 데미지 구현
            Take_Damaged(state.DmgState.Dmg,
                ((Vector2)transform.position - (Vector2)_Attacker.transform.position).normalized,
                state.KnockbackState.CanKB,
                state.KnockbackState.KBPower,
                state.KnockbackState.KBTime);
        }
    }

    // 회피?
    public bool Is_Avoid()
    {
        // 회피
        if (UnityEngine.Random.Range(0f, 1f) < AvoidChance.ActualState.Value)
        {
            Play_Avoid();
            return true;
        }
        return false;
    }

    // 맞을 때, 효과
    private void Play_Hitted()
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

    // 데미지 계산
    private void Take_Damaged(float _DmgValue, Vector2 _HittedDir, bool _AbleKB, float _KBPower, float _KBTime)
    {
        // Multiple
        _DmgValue *= TakingDmgMultiple.BuffedState;

        // Effect
        PlayerManager.Instance.CameraController.Play_DamagedAnim(MaxInvincibleTime, _DmgValue * 0.1f, _HittedDir);

        // Knockback
        if (_AbleKB)
        { Get_Knockback(new KnockbackState(_HittedDir, _KBPower, _KBTime)); }

        // Damage
        Take_Damaged(_DmgValue);
    }

    // 오직 데미지만 계산 (넉백, 애니메이션 등 없음)
    public void Take_Damaged(float _DmgValue)
    {
        float Dmg = _DmgValue;
        MainGameUIManager.Instance.PlayerHUD_UIController.Play_HittedPlayScreen(Dmg);
        if (ShieldElements.Count > 0)
        {
            for (int i = ShieldElements.Count - 1; i >= 0; i--)
            {
                // 쉴드 버프량 1개가 데미지보다 작거나 같으면, 제거하고 다음 쉴드로 영향
                if (ShieldElements[i].ShieldCurrentValue <= Dmg)
                {
                    Dmg -= ShieldElements[i].ShieldCurrentValue;

                    Remove_Shield(ShieldElements[i]); // 쉴드 감소
                }
                else // 쉴드 버프가 데미지를 버틸 수 있으면
                {
                    ShieldElements[i].ShieldCurrentValue -= Dmg;
                    Dmg = 0;
                    CurrentSP.Value = Get_TotalShield();
                    return;
                }
            }
        }
        Add_CurrentEP(-Dmg);
        CurrentSP.Value = Get_TotalShield();
    }

    // 추가 데미지 계산 (맞을 때 발생하는 이벤트, 넉벡, 애니메이션 등 없음)
    public void Take_ExtraDamage(float _DmgValue)
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

                    Remove_Shield(ShieldElements[i]); // 쉴드 감소
                }
                else // 쉴드 버프가 데미지를 버틸 수 있으면
                {
                    ShieldElements[i].ShieldCurrentValue -= Dmg;
                    Dmg = 0;
                    CurrentSP.Value = Get_TotalShield();
                    return;
                }
            }
        }
        Add_CurrentEP(-Dmg);
        CurrentSP.Value = Get_TotalShield();
    }

    // 회피
    private void Play_Avoid()
    {
        PlayerManager.Instance.CameraController.Play_AvoidAnim(MaxInvincibleTime);

        for (int i = 0; i < 4; i++)
        {
            PlayerMEI.Gen_ExplosionImgs(
                    TargetObject.transform.position,
                    6, 0.15f, 0.75f,
                    2.0f, 0.05f, 0.1f,
                    1.0f, 0.5f, 1.0f,
                    i, ThisPlayerMaterialList[0]);
        }

    }

    #endregion

    #region About Casting

    // Condition : Idle or Walk | No Casting Now 
    private bool Can_Change()
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
    public void Try_CombatModeCheck()
    {
        if (!Can_Change()) 
        { return; }

        StateAnim.Set_Anim(DmgTypeStateAC.TypeSpecial, ChangeState_DamageType, 2f, 1f);

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

        Start_Casting(CombatModeInterval);
    }


    // + Not over the Max Boost Level
    [HideInInspector] private const float BoostModeInterval = 0.5f;
    public void Try_BoostModeCheck()
    {
        if (!Can_Change() || TargetBoostlv >= MaxBoostLv)
        { return; }

        TargetBoostlv++;
        StateAnim.Set_Anim(DmgTypeStateAC.TypeSpecial, ChangeState_BoostUpDown.TypeSpecial, 2f, 1f);

        Start_Casting(BoostModeInterval);
    }


    // + BoostLevel != 0
    [HideInInspector] private const float UnBoostModeInterval = 0.25f;
    public void Try_UnBoostModeCheck()
    {
        if (!Can_Change() || CurrentBoostLv.Value <= 0)
        { return; }

        TargetBoostlv--;
        StateAnim.Set_Anim(DmgTypeStateAC.TypeSpecial, ChangeState_BoostUpDown.TypeBase, 2f, 1f);

        Start_Casting(UnBoostModeInterval);
    }


    // + Enough EP | Enough BC
    [HideInInspector] private const float ChargeBetteryInterval = 1f;
    public void Try_ChargeBettery()
    {
        if (!Can_Change() ||
            NeedEP_ForMakeEC >= this.CurrentEP.Value ||
            CurrentBC.Value <= 0) 
        { return; }

        ReservationSkillDele = Add_CurrentEC;

        Start_Casting(ChargeBetteryInterval);
    }


    // + None
    [HideInInspector] public float ExecutionInterval = 0.75f;
    public void Try_Execution()
    {
        if (!Can_Change())
        { return; }

        if (CurrentInteractable.Value is EnemyController EC)
        {
            EC.Play_Interact();
        }

        Start_Casting(ExecutionInterval);
    }

    // + None
    [HideInInspector] public float Skill0Interval = 0.5f;
    public void Try_Skill0()
    {
        if (!Can_Change())
        { return; }
        if (!SkillWeapon.Skill_0.Can_Active())
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.Skill0.Start_NotEnoughEP();
            return;
        }

        ReservationSkillDele = SkillWeapon.Skill_0.Active_Skill;
        StateAnim.Set_Anim(DmgTypeStateAC.TypeSpecial, ChangeState_Skill.TypeBase, 2f, 1f);

        Start_Casting(Skill0Interval);
    }

    [HideInInspector] public float Skill1Interval = 0.5f;
    public void Try_Skill1()
    {
        if (!Can_Change())
        { return; }
        if (!SkillWeapon.Skill_1.Can_Active())
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.Skill1.Start_NotEnoughEP();
            return;
        }

        ReservationSkillDele = SkillWeapon.Skill_1.Active_Skill;
        StateAnim.Set_Anim(DmgTypeStateAC.TypeSpecial, ChangeState_Skill.TypeSpecial, 2f, 1f);

        Start_Casting(Skill1Interval);
    }

    public void Start_Casting(float _CastingTime)
    {
        TargetCastingTime = _CastingTime;
        IsCasting = true;
        MovementState = eMovementState.Casting;
        ThisRb.velocity = Vector2.zero;

        InputManager.Instance.IsPlayingSkill = true;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if(CurrentInteractable.Value != null && CurrentInteractableGOList.Count > 0)
        {
            if (CurrentInteractable.Value is InteractItemController IIC) // Item
            {
                CurrentInteractable.Value.Play_Interact();
                CurrentInteractable.Value = null;
            }
            else if (CurrentInteractable.Value is EnemyController EC) // Enemy
            {
                if (EC.IsDischarge)
                {
                    Try_Execution();
                }
                CurrentInteractable.Value = null;
            }
            else if (CurrentInteractable.Value is EndingElevatorController DEC && DEC.IsOn)
            {
                CurrentInteractable.Value.Play_Interact();
                CurrentInteractable.Value = null;
            }
            else if (CurrentInteractable.Value is DestructibleBuildController DBC && !DBC.IsBroken)
            {
                CurrentInteractable.Value.Play_Interact();
            }
            else // OneOffShopController |OR| ...
            {
                CurrentInteractable.Value.Play_Interact();
            }

            MainGameUIManager.Instance.PlayerHUD_UIController.Set_UseInteractUI();
        }
    }

    #endregion

    #region Caculate

    #region Time

    private void Caculate_Always()
    {
        Caculate_Casting();
        Caculate_Boosting(CurrentBoostLv.Value);
        Caculate_Invincible();
    }

    private void Caculate_Casting()
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

                Set_CombatMode();
                Set_Skill();
                Set_Boost();
            }
        }
    }

    private void Caculate_Boosting(int _BoostLv)
    {
        if(_BoostLv > 0)
        {
            float decValue = DecEnergyPointByLevel[_BoostLv - 1] * DecEnergyPointMultiple.ActualState.Value;
            Add_CurrentEP(-decValue * Time.deltaTime);
        }
    }

    private void Caculate_Invincible()
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

    private void Set_CombatMode()
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
                StateAnim.Set_Anim(DmgTypeStateAC.TypeA, 0.8f, 1f);
                InputManager.Instance.AimController.Set_PhysicsType();
                break;

            case eCombatMode.Energy:
                StateAnim.Set_Anim(DmgTypeStateAC.TypeB, 0.8f, 1f);
                InputManager.Instance.AimController.Set_EnergyType();
                break;

            default:
                break;
        }
    }

    private void Set_Skill()
    {
        if(ReservationSkillDele != null)
        {
            ReservationSkillDele();
            ReservationSkillDele = null;
        }
    }

    private void Set_Boost()
    {
        if(CurrentBoostLv.Value != TargetBoostlv)
        {
            CurrentBoostLv.Value = TargetBoostlv;
            Set_BoostAnim(CurrentBoostLv.Value, MaxBoostLv);
        }
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

    private void Set_BoostAnim(int _Index, int _MaxIndex)
    {
        float lowestAnimSpeed = 0.5f;

        // Wheel
        if (_Index >= _MaxIndex)
        {
            for (int i = 0; i < _MaxIndex - 1; i++)
            {
                BoostStateAnimController.TypeBase[i].Set_Anim(BoostOnOffAC.TypeSpecial, lowestAnimSpeed * (_MaxIndex * 2), 1f);
            }
        }
        else
        {
            for (int i = 0; i < _MaxIndex - 1; i++)
            {
                if (i < _Index)
                {
                    BoostStateAnimController.TypeBase[i].Set_Anim(BoostOnOffAC.TypeSpecial, lowestAnimSpeed * (_Index - i + 1), 1f);
                }
                else
                {
                    BoostStateAnimController.TypeBase[i].Set_Anim(BoostOnOffAC.TypeBase, lowestAnimSpeed, 1f);
                }
            }
        }

        // VFX

        BoostStateAnimController.TypeSpecial[0].gameObject.SetActive(false);
        BoostStateAnimController.TypeSpecial[1].gameObject.SetActive(false);

        switch (_Index)
        {
            case 1:
                BoostStateAnimController.TypeSpecial[0].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[0].Set_Anim(BoostVFXAnimList[0], 0.5f, 1f);
                break;

            case 2:
                BoostStateAnimController.TypeSpecial[0].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[0].Set_Anim(BoostVFXAnimList[0], 1f, 1f);
                break;

            case 3:
                BoostStateAnimController.TypeSpecial[0].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[0].Set_Anim(BoostVFXAnimList[0], 1f, 1f);

                BoostStateAnimController.TypeSpecial[1].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[1].Set_Anim(BoostVFXAnimList[1], 1f, 1f);
                break;

            case 4:
                BoostStateAnimController.TypeSpecial[0].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[0].Set_Anim(BoostVFXAnimList[0], 1.5f, 1f);

                BoostStateAnimController.TypeSpecial[1].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[1].Set_Anim(BoostVFXAnimList[1], 1.5f, 1f);
                break;

            default:
                break;
        }


    }


    private void SetOn_RoomMoveDir(Vector2 _Dir)
    {
        if (!MoveDirStateAnim.gameObject.activeSelf)
        { 
            MoveDirStateAnim.transform.localRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, _Dir));
            MoveDirStateAnim.gameObject.SetActive(true);
        }
    }

    private void SetOff_RoomMoveDir()
    {
        if (MoveDirStateAnim.gameObject.activeSelf)
        {
            MoveDirStateAnim.gameObject.SetActive(false);
        }
    }


    private void Set_MoveDir()
    {
        if (CurrentInteractable.Value != null && 
            CurrentInteractable.Value is GateController GC &&
            GC.IsOpen)
        { SetOn_RoomMoveDir(GC.GateDir); }
        else
        { SetOff_RoomMoveDir(); }
    }



    public Color Get_Color_CorrectHitted(eDamageType _DamageType, bool _IsCritical)
    {
        return ThisClr.Get_CorrectType(_DamageType).Get_Special(_IsCritical);
    }

    public AnimationClip Get_AnimClip_CorrectHitted(eDamageType _DamageType, bool _IsCritical)
    {
        return ThisHittedPointAC.Get_CorrectType(_DamageType).Get_Special(_IsCritical);
    }


    #endregion

    #region Buff

    public void Set_GainBuff(BuffController _Buff)
    {
        if (!CurrentBuffs.Contains(_Buff))
        {
            CurrentBuffs.Add(_Buff);

            // 인터페이스
            if (_Buff is IWhen_GetElectricity hitted && !BuffManager.Instance.iWhen_HittedList.Contains(hitted))
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
        if (_Buff is IWhen_GetElectricity hitted && BuffManager.Instance.iWhen_HittedList.Contains(hitted))
        {
            BuffManager.Instance.iWhen_HittedList.Remove(hitted);
        }
    }

    #endregion

}
