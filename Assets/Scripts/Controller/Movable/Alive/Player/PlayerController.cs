using UniRx;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections.Generic;
using System;

public class PlayerController : AliveObjectController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private int NameID;
    public int GetNameID => NameID;

    [Space(10)]
    [Header("=== Controller & Generator")]
    [SerializeField] public PlayerWeaponController BaseWeapon;
    [SerializeField] public SkillWeaponController SkillWeapon;
    [SerializeField] public PlayerDashController DashController;
    [SerializeField] public RigidbodyAnimSolarController LowerController;

    [Space(5)]
    [SerializeField] public AfterImgGenerator AfterImgGenerator;

    [Space(10)]
    [Header("=== BU State")]
    [SerializeField] public BUState<float> AvoidChance;
    [SerializeField] public BUState<float> MaxEP;
    [SerializeField] public BUState<float> TakingDmgMultiple;
    [SerializeField] public BUState<float> SpawnESMultiple;
    [SerializeField] public BUState<float> NeedEP_ForSkillMultiple;
    [SerializeField] public BUState<float> WalkSpeed;
    [SerializeField] public BUState<float> WalkSpeedWhenShotMultiple;

    [Space(10)]
    [Header("=== Visual Comp")]

    [Space(5)]
    [Header("-- SG")]
    [SerializeField] private SortingGroup BodySG;

    [Space(5)]
    [Header("-- Anim")]
    [SerializeField] private StateAnimController StateAnim;
    [SerializeField] private StateAnimController MoveDirStateAnim;
    [SerializeField] private List<MovableDepthController> BaseAnimDepthList;
    [SerializeField] private CoupleData<List<StateAnimController>> BoostStateAnimController;

    [Space(5)]
    [Header("-- VFX")]
    [SerializeField] private TrailRenderer ThisTrail;

    [Space(10)]
    [Header("=== Visual Reso")]

    [SerializeField] private AnimationClip MoveDirAC;
    [SerializeField] private Sprite ChangeState_DamageType;

    [SerializeField] public List<Material> MaterialList;
    [SerializeField] private List<AnimationClip> BoostVFXAnimList;

    [SerializeField] private PlayerVisual<Color> ThisClr;
    [SerializeField] private PlayerVisual<Gradient> ThisGradient;

    [SerializeField] private PlayerVisual<AnimationClip> ThisHittedPointAC;
    [SerializeField] private TrioData<AnimationClip> DmgTypeStateAC;
    [SerializeField] private CoupleData<AnimationClip> BoostOnOffAC;

    [SerializeField] private CoupleData<Sprite> ChangeState_BoostUpDown;
    [SerializeField] private CoupleData<Sprite> ChangeState_Skill;

    [SerializeField] public GameObject AimPrefab;
    [SerializeField] public GameObject AimRoundPrefab;

    [SerializeField] public Sprite BattleProdSprite;

    [Space(10)]
    [Header("=== Sound")]
    [SerializeField] private ASQueueSet ASQueueSet;
    [SerializeField] private AudioSource MovementAS;

    #endregion

    #region - Hide

    // Lower
    [HideInInspector] private bool IsLowerTweening = false;

    // Stage Type
    [HideInInspector] private SortingGroup ThisSG;
    [HideInInspector] private SpriteRenderer ShadowSR;

    // Combat Mode
    [HideInInspector] private eDamageType DmgMode = eDamageType.Physics;

    // Invincible
    [HideInInspector] private bool IsInvincible = false;

    // Point
    [HideInInspector] public ReactiveProperty<float> Get_CurrentEP() => CurrentEP;

    // Movement
    [HideInInspector] public eMovementState MovementState = eMovementState.IdleOrWalk;

    // Boost
    [HideInInspector] public ReactiveProperty<int> CurrentBoostLv = new();

    // Shield
    [HideInInspector] public List<Shield> ShieldElements = new List<Shield>();

    // Buff
    [HideInInspector] public List<BuffController> CurrentBuffs = new List<BuffController>();

    // Interact
    [SerializeField] public List<GameObject> CurrentInteractableGOList = new List<GameObject>();
    [HideInInspector] public ReactiveProperty<IInteract> CurrentInteractable = new();

    // Item
    [HideInInspector] public ReactiveProperty<int> CurrentBetteryShard = new();
    [HideInInspector] public ReactiveProperty<int> CurrentBettery = new();
    [HideInInspector] public ReactiveProperty<int> CurrentChargedBettery = new();
    [HideInInspector] public ReactiveProperty<int> CurrentModuleShard = new();
    [HideInInspector] public ReactiveProperty<int> CurrentOverrider = new();
    [HideInInspector] public ReactiveProperty<int> CurrentCredit = new();

    // Ally

    // Ally Presence
    [HideInInspector] public ReactiveProperty<int> StrikeTeamPresence = new();
    [HideInInspector] public ReactiveProperty<int> UplinkTeamPresence = new();
    [HideInInspector] public ReactiveProperty<int> NeoTeamPresence = new();
    [HideInInspector] public int NeedIntervalPresence = 5;
    [HideInInspector] public ReactiveProperty<int> NeedStrikeTeamPresence = new();
    [HideInInspector] public ReactiveProperty<int> NeedUplinkTeamPresence = new();
    [HideInInspector] public ReactiveProperty<int> NeedNeoTeamPresence = new();

    // Ally Reputation
    [HideInInspector] private ReactiveProperty<float> Reputation = new ReactiveProperty<float>();
    [HideInInspector] public float Get_Reputation { get { return Reputation.Value; } }




    // BaseAnim
    [HideInInspector] private Sequence BaseSeq = null;
    [HideInInspector] private readonly float BaseYLimit = 0.02f;
    [HideInInspector] private readonly float BaseTweenReTime = 0.25f;
    [HideInInspector] private readonly float WheelLowestAnimSpeed = 0.5f;

    // Book
    [HideInInspector] private eDamageType TargetDmgMode = eDamageType.Physics;
    [HideInInspector] private ChargeCooltimeData CastingTime = new ChargeCooltimeData();
    [HideInInspector] private Dele ReservationDele = null;
    [HideInInspector] private const float InvincibleTime = 0.5f;

    #endregion

    #region - Data

    [HideInInspector] public static readonly int MaxBoostLv = 4;
    [HideInInspector] public static readonly int MaxRank = 5;
    [HideInInspector] public readonly int NeedBS_ForMakeBC = 4;
    [HideInInspector] public readonly float NeedEP_ForMakeEC = 5f;
    
    #endregion

    #endregion


    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_FirstSetting();
        Offset_Subscribe();
        Offset_Controller();
        Offset_Reputation();
    }

    private void Offset_FirstSetting()
    {
        // State Anim
        Reset_StateAnim();

        // State Anim : Dmg Type
        StateAnim.Set_Anim(new State_Anim(DmgTypeStateAC.typeA, 0.8f), 1f);

        // State Anim : Boost
        Set_BoostAnim(CurrentBoostLv.Value);

        // State Anim: Room Move
        MoveDirStateAnim.Set_Anim(new State_Anim(MoveDirAC));
        SetOff_RoomMoveDir();

        // Base Tween
        BaseSeq = DOTween.Sequence();

        BaseSeq.Join(DOTween.To(() => TargetRange, x => TargetRange = x, TargetRange + BaseYLimit, BaseTweenReTime)
            .SetEase(Ease.Linear));

        if (BaseAnimDepthList != null && BaseAnimDepthList.Count > 0)
        {
            for (int i = 0; i < BaseAnimDepthList.Count; i++)
            {
                MovableDepthController movable = BaseAnimDepthList[i];
                BaseSeq.Join(DOTween.To(() => movable.TargetRange, x => movable.TargetRange = x, movable.TargetRange + BaseYLimit, BaseTweenReTime)
                    .SetEase(Ease.Linear));
            }
        }

        BaseSeq.SetLoops(-1, LoopType.Yoyo);

        ResourceManager.instance.unlockedClr = Get_CorrectColor(eDamageType.Energy, false);

        // Item
        CurrentChargedBettery.Value = 0;
        CurrentCredit.Value = 0;
        CurrentOverrider.Value = 0;
        CurrentModuleShard.Value = 0;

        // Presence
        NeedStrikeTeamPresence.Value = NeedIntervalPresence;
        NeedUplinkTeamPresence.Value = NeedIntervalPresence;
        NeedNeoTeamPresence.Value = NeedIntervalPresence;

        StrikeTeamPresence.Value = 0;
        UplinkTeamPresence.Value = 0;
        NeoTeamPresence.Value = 0;
    }

    private void Offset_Subscribe()
    {
        CurrentInteractable
            .Subscribe(interact =>
            {
                MainGameUIManager.instance.playerHUD_UIController.Set_InteractUI();
                MainGameUIManager.instance.interactAnno_UIController.Set_UI();
                Set_MoveDir();
            });
        CurrentSP
            .Subscribe(value =>
            {
                MainGameUIManager.instance.playerHUD_UIController.Set_ShieldGage(value);
            });
    }

    private void Offset_Controller()
    {
        DashController.Offset();
        ThisSG = DevTool.Get_ComponentTType<SortingGroup>(gameObject); 
        ShadowSR = DevTool.Get_ComponentTType<SpriteRenderer>(transform.GetChild(0).gameObject);

        ASQueueSet.Offset(); 
        MovementAS.volume = 0.2f;
    }

    private void Offset_Reputation()
    {
        Reputation.Subscribe(value =>
            {
                MainGameUIManager.instance.playerHUD_UIController.Set_AllyReputation(value);
            });

        Reputation.Value = 1f;
    }


    #endregion

    #region Sorting

    public override void Set_SortingOrder(int _SortingOrder)
    {
        BodySG.sortingOrder = _SortingOrder;

        ThisTrail.sortingOrder = _SortingOrder - 1;
        // base.Set_SortingOrder(_SortingOrder);
    }

    #endregion

    #region Framework

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Update_Caculate(Time.fixedDeltaTime);
        Play_Movement(Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        MainGameUIManager.instance.interactAnno_UIController.Set_PosIfNot(CurrentInteractable.Value);
        Set_Tween(LowerController.transform, ThisRb.velocity);
    }

    #endregion

    #region Stage Part Time

    // 스테이지 시작 전
    public void Set_PastStartStage()
    {
        ThisSG.enabled = true;
        ThisSG.sortingOrder = -3002;

        ThisSR.sortingOrder = 2;

        ShadowSR.sortingOrder = -10;

        StateAnim.transform.parent.transform.gameObject.SetActive(false);
    }

    // 스테이지 시작
    public void Set_StartStage()
    {
        ThisSG.enabled = false;
        ThisSG.sortingOrder = 0;
        ThisSR.sortingOrder = 0;

        StateAnim.transform.parent.transform.gameObject.SetActive(true); 

        StageManager.instance.isStartStage = false;
        LayerOrderManager.instance.Add_NeedSortObj(this);

        SetOn_Trail();
    }

    // 스테이지 끝
    public void Set_EndStage()
    {
        ThisSG.enabled = true;
        ThisSG.sortingOrder = 3001;

        StateAnim.transform.parent.transform.gameObject.SetActive(false);

        SetOff_Trail();
    }

    #endregion

    #region Shield Point

    // 현재 총 쉴드값
    private float Get_TotalShield()
    {
        if (ShieldElements.Count <= 0)
        { return 0; }

        float totalShield = 0;
        DevTool.Set_ListDele(ShieldElements, new Dele_RefT_U<float, Shield>(Add_ShieldValue), ref totalShield);
        return totalShield;

        void Add_ShieldValue(ref float _Variable, Shield _Shield)
        {
            DevTool.Add_RefValue(ref _Variable, _Shield.shieldCurrentValue);
        }
    }

    // 쉴드 획득
    public void Gain_Shield(Shield _S)
    {
        DevTool.Remove_InList(ShieldElements, _S);

        ShieldElements.Insert(0, _S);
        CurrentSP.Value = Get_TotalShield();
    }

    // 쉴드 제거
    public void Remove_Shield(Shield _S)
    {
        if (ShieldElements.Contains(_S))
        {
            DevTool.Set_ListDele(CurrentBuffs, new Dele_T_U<BuffController, Shield>(End_ShieldBuff), _S);
            ShieldElements.Remove(_S);
        }
        CurrentSP.Value = Get_TotalShield();

        void End_ShieldBuff(BuffController _Buff, Shield _Shield)
        {
            BuffShieldController shieldBuff = DevTool.Get_CastingTType<BuffShieldController>(_Buff);
            if (DevTool.Is_UsableAndEqual(shieldBuff, shieldBuff.thisShield, _Shield))
            {
                shieldBuff.End_Buff();
            }
        }
    }

    #endregion

    #region Energy Point

    // 에너지 획득
    public void Add_CurrentEP(float _AddValue)
    {
        Add_CurrentEP(_AddValue, MaxEP.actualState.Value);
        Check_IsDead(CurrentEP.Value);
    }

    public float Get_PercentEP(float _Percent)
    {
        return DevTool.Get_Percent(_Percent, MaxEP.actualState.Value);
    }

    #endregion

    #region Item

    // 배터리 조각 획득
    public void Add_CurrentBetteryShard(int _AddValue)
    {
        CurrentBetteryShard.Value = Mathf.Max(CurrentBetteryShard.Value + _AddValue, 0);
        if (CurrentBetteryShard.Value >= NeedBS_ForMakeBC)
        {
            Add_CurrentBettery();
        }
    }

    // 모듈 조각 획득
    public void Add_CurrentModuleShard(int _AddValue)
    {
        CurrentModuleShard.Value = Mathf.Max(CurrentModuleShard.Value + _AddValue, 0);
    }

    // 배터리 획득
    public void Add_CurrentBettery(int _AddValue)
    {
        CurrentBettery.Value = Mathf.Max(CurrentBettery.Value + _AddValue, 0);
    }

    private void Add_CurrentBettery()
    {
        MainGameUIManager.instance.playerHUD_UIController.CurrentEmptyBC.Set_Complete(
            _FadeInTime: 0.3f,
            _StayTime: 0.1f, 
            _FadeOutTime: 0.5f);

        int BSAmount = CurrentBetteryShard.Value / NeedBS_ForMakeBC;
        CurrentBetteryShard.Value -= NeedBS_ForMakeBC * BSAmount;
        CurrentBettery.Value += BSAmount;
    }

    // 오버라이더 획득
    public void Add_CurrentOverrider(int _AddValue)
    {
        CurrentOverrider.Value = Mathf.Max(CurrentOverrider.Value + _AddValue, 0);
    }

    // 크레딧 획득
    public void Add_CurrentCredit(int _AddValue)
    {
        CurrentCredit.Value = Mathf.Max(CurrentCredit.Value + _AddValue, 0);
    }

    // 충전 배터리 생성
    private void Make_ChargedBettery()
    {
        this.CurrentEP.Value -= NeedEP_ForMakeEC;
        CurrentBettery.Value--;
        CurrentChargedBettery.Value++;
    }

    // 충전 배터리 충분한가
    public bool Is_EnoughChargedBettery(int _NeedAmount)
    {
        return CurrentChargedBettery.Value >= _NeedAmount ? true : false;
    }

    // 에너지 셀을 소비
    public void Use_ChargedBettery(int _UseAmount)
    {
        CurrentChargedBettery.Value = Math.Max(CurrentChargedBettery.Value - _UseAmount, 0);
    }

    #endregion

    #region Movement

    private void Play_Movement(float _DeltaTime)
    {
        if (MovementState == eMovementState.IdleOrWalk)
        {
            Play_Walk(_DeltaTime);
        }
        else if (MovementState == eMovementState.Dash)
        {
            DashController.Play_Dash(_DeltaTime);
        }
    }

    private void Play_Walk(float _DeltaTime)
    {
        float multiple =
            BaseWeapon.IsShooting ? WalkSpeedWhenShotMultiple.actualState.Value : 1f;
        Play_Walk(
            InputManager.instance.inputMoveDir, WalkSpeed.actualState.Value * multiple, _DeltaTime);
    }

    public void Try_Dash()
    {
        if (Can_Change() && DashController.Is_EnoughEP())
        {
            InputManager.instance.isPlayingBuffered = true;
            AfterImgGenerator.Start_Gen(0.7f, 0.03f, 0.5f);
            Add_CurrentEP(-DashController.Get_ActualNeedEP());
            MovementState = eMovementState.Dash;
        }
    }

    #endregion

    #region Movement Lower (Tween)

    private void Set_Tween(Transform _TargetT, Vector2 _RbVel)
    {
        if (_RbVel != Vector2.zero && IsLowerTweening == false) // On
        {
            SetOn_Tween(_TargetT);
        }
        else if (_RbVel == Vector2.zero && IsLowerTweening == true) // Off
        {
            SetOff_Tween(_TargetT);
        }
    }

    private void SetOn_Tween(Transform _TargetTF)
    {
        IsLowerTweening = true;
        _TargetTF.DOShakePosition(1f, 0.01f, 20, 0, false, false)
            .SetLoops(-1, LoopType.Restart)
            .OnStart(() => MovementAS.volume = 0.25f )
            .OnKill(() => MovementAS.volume = 0.1f );
    }

    private void SetOff_Tween(Transform _TargetTF)
    {
        IsLowerTweening = false;
        DOTween.Kill(_TargetTF);
    }

    #endregion

    #region Casting

    // 상태 변경이나 스킬, 대시 등을 사용할 수 있는 상황인가?
    private bool Can_Change()
    {
        if (MovementState != eMovementState.IdleOrWalk)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private const float CombatModeInterval = 0.25f;
    private const float BoostModeInterval = 0.1f;
    private const float UnBoostModeInterval = 0.1f;
    private const float ChargeBetteryInterval = 1f;
    public readonly float SkillInterval = 0.5f;

    // 조건: + None
    public void Try_CombatModeCheck()
    {
        if (!Can_Change()) 
        { return; }

        StateAnim.Set_Anim(
            new State_Anim(DmgTypeStateAC.typeSpecial, 2f), 
            _InnerSprite: ChangeState_DamageType);

        TargetDmgMode = TargetDmgMode == eDamageType.Physics ?
            eDamageType.Energy : eDamageType.Physics;

        Start_Casting(CombatModeInterval);
    }

    // 조건: + EP, BC가 충분한가?
    public void Try_ChargeBettery()
    {
        if (!Can_Change() ||
            NeedEP_ForMakeEC >= this.CurrentEP.Value ||
            CurrentBettery.Value <= 0) 
        { return; }

        ReservationDele = Make_ChargedBettery;

        Start_Casting(ChargeBetteryInterval);
    }

    // 조건: + ex) 스킬을 사용할 수 없다면, Error 문구
    public void Try_Skill0()
    {
        Try_Skill(0, _Sprite: ChangeState_Skill.typeBase);
    }

    public void Try_Skill1()
    {
        Try_Skill(1, _Sprite: ChangeState_Skill.typeSpecial);
    }

    private void Try_Skill(int _Index, Sprite _Sprite)
    {
        if (!Can_Change())
        { return; }

        if (!SkillWeapon.SkillList[_Index].Can_Active())
        {
            MainGameUIManager.instance.playerHUD_UIController.SkillList[_Index].Play_ErrorUI();
            return;
        }

        ReservationDele = SkillWeapon.SkillList[_Index].Active_Skill;
        StateAnim.Set_Anim(
            new State_Anim(DmgTypeStateAC.typeSpecial, 2f),
            _Sprite);

        Start_Casting(SkillInterval);
    }


    public void Start_Casting(float _CastingTime)
    {
        CastingTime.max = _CastingTime;
        MovementState = eMovementState.Casting;
        ThisRb.velocity = Vector2.zero;

        InputManager.instance.isPlayingBuffered = true;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (CurrentInteractableGOList.Count <= 0 ||
            CurrentInteractable.Value == null)
        { return; }

        CurrentInteractable.Value.Play_Interact();
        
        MainGameUIManager.instance.playerHUD_UIController.Play_UseInteractUI();
    }

    #endregion

    #region Update Caculate

    private void Update_Caculate(float _DeltaTime)
    {
        Caculate_Casting(_DeltaTime);
        //Caculate_Boosting(_DeltaTime, CurrentBoostLv.Value);
    }

    private void Caculate_Casting(float _DeltaTime)
    {
        if (MovementState != eMovementState.Casting)
        { return; }

        if (CastingTime.Is_Charge(_DeltaTime)) // 캐스팅 완료
        {
            MovementState = eMovementState.IdleOrWalk;
            InputManager.instance.isPlayingBuffered = false;

            Set_CombatMode();
            Set_Skill();
        }
    }

/*
    private void Caculate_Boosting(float _DeltaTime, int _BoostLv)
    {
        if(_BoostLv > 0)
        {
            float decValue = DecEnergyPointByLevel[_BoostLv - 1] * DecEnergyPointMultiple.ActualState.Value;
            Add_CurrentEP(-decValue * _DeltaTime);
        }
    }
*/

    #endregion

    #region Check CastingType

    // 캐스팅에서 바뀐부분을 찾아서 적용
    
    private void Set_CombatMode()
    {
        if(DmgMode != TargetDmgMode)
        {
            DmgMode = TargetDmgMode;
            BaseWeapon.DamageType = TargetDmgMode;

            Reset_StateAnim();
            InputManager.instance.aimController.Set_DmgType(TargetDmgMode);
        }
    }

    private void Set_Skill()
    {
        if(ReservationDele != null)
        {
            ReservationDele();
            ReservationDele = null;

            Reset_StateAnim();
        }
    }

    // 모든 캐스팅이 끝나면, 본래의 스탯애니메이션으로 돌아옴
    private void Reset_StateAnim()
    {
        StateAnim.Set_Anim(
                new State_Anim(TargetDmgMode == eDamageType.Physics ?
                    DmgTypeStateAC.typeA : DmgTypeStateAC.typeB, 0.8f));
    }

    #endregion

    #region Boost State


    // 머리위에 스탯 상태 
    private void Set_BoostAnim(int _Index)
    {
        Set_BoostAnim_StateWheel(_Index);
        Set_BoostAnim_StateExtraVFX(_Index);
    }

    // 스탯의 위아래로 도는 바퀴
    private void Set_BoostAnim_StateWheel(int _Index)
    {
        for (int i = 0; i < MaxBoostLv - 1; i++)
        {
            // 부스팅 : 언부스팅
            AnimationClip ac = i < _Index ?
                BoostOnOffAC.typeSpecial : BoostOnOffAC.typeBase;
            float animSpeed = i < _Index ?
                WheelLowestAnimSpeed * (_Index - i + 1) : WheelLowestAnimSpeed;

            BoostStateAnimController.typeBase[i].Set_Anim(new State_Anim(ac, animSpeed));
        }
    }

    // 스탯의 좌우로 도는 이펙트
    private void Set_BoostAnim_StateExtraVFX(int _Index)
    {
        BoostStateAnimController.typeSpecial[0].gameObject.SetActive(false);
        BoostStateAnimController.typeSpecial[1].gameObject.SetActive(false);

        float animSpeed = _Index * 0.5f;
        if (_Index > 0)
        { 
            Set_EachBoostAnim_StateExtraVFX(0, animSpeed); 
        }
        if (_Index > 2)
        {
            Set_EachBoostAnim_StateExtraVFX(1, animSpeed);
        }
    }

    private void Set_EachBoostAnim_StateExtraVFX(int _Index, float _AnimSpeed)
    {
        BoostStateAnimController.typeSpecial[_Index].gameObject.SetActive(true);
        BoostStateAnimController.typeSpecial[_Index].Set_Anim(new State_Anim(BoostVFXAnimList[_Index], _AnimSpeed));

    }

    #endregion

    #region Room Move State

    private void SetOn_RoomMoveDir(Vector2 _Dir)
    {
        MoveDirStateAnim.gameObject.SetActive(true);
        MoveDirStateAnim.transform.localRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, _Dir));
    }

    private void SetOff_RoomMoveDir()
    {
        MoveDirStateAnim.gameObject.SetActive(false);
    }


    private void Set_MoveDir()
    {
        if (DevTool.Can_CastingTType(CurrentInteractable.Value, out GateController gate) && gate.IsOpen)
        { SetOn_RoomMoveDir(gate.GateDir); }
        else
        { SetOff_RoomMoveDir(); }
    }

    #endregion

    #region Visual

    public Color Get_CorrectColor(eDamageType _DamageType, bool _IsCritical)
    {
        return Get_CorrectTComponent(ThisClr, _DamageType, _IsCritical);
    }

    public Gradient Get_CorrectGradient(eDamageType _DamageType, bool _IsCritical)
    {
        return Get_CorrectTComponent(ThisGradient, _DamageType, _IsCritical);
    }

    public AnimationClip Get_CorrectAC(eDamageType _DamageType, bool _IsCritical)
    {
        return Get_CorrectTComponent(ThisHittedPointAC, _DamageType, _IsCritical);
    }

    private T Get_CorrectTComponent<T>(PlayerVisual<T> _T, eDamageType _DamageType, bool _IsCritical)
    {
        return _T.Get_CorrectType(_DamageType).Get_Special(_IsCritical);
    }

    #endregion

    #region Trigger

    // Enter
    protected override void OnTriggerEnter2D(Collider2D _Col) // 판별을 위한 IInteract GO 추가
    {
        base.OnTriggerEnter2D(_Col);

        GameObject targetGO = _Col.gameObject.transform.parent.gameObject;
        if (DevTool.Get_ComponentTType<IInteract>(targetGO) != null)
        {
            DevTool.Add_InList(CurrentInteractableGOList, targetGO);
        }
    }

    // Exit
    private void OnTriggerExit2D(Collider2D _Col) // 판별에 필요없는 IInteract GO 삭제
    {
        GameObject targetGO = _Col.gameObject.transform.parent.gameObject;
        if (DevTool.Get_ComponentTType<IInteract>(targetGO) != null)
        {
            DevTool.Remove_InList(CurrentInteractableGOList, targetGO);

            // 모두 삭제되었다면
            if (CurrentInteractableGOList.Count <= 0)
            {
                CurrentInteractable.Value = null;
            }
        }
    }

    // Stay: Set Current IInteract
    private void OnTriggerStay2D(Collider2D _Col)
    {
        // None
        if (CurrentInteractableGOList.Count <= 0)
        {
            CurrentInteractable.Value = null;
        }

        // 하나만 존재
        else if (CurrentInteractableGOList.Count == 1)
        {
            if (DevTool.Get_ComponentTType(CurrentInteractableGOList[0], out IInteract i))
            {
                CurrentInteractable.Value = i;
            }
        }
        else // 다수 존재
        {
            if (DevTool.Get_ComponentTType(
                    DevTool.Get_ClosetGO(CurrentInteractableGOList, this.gameObject),
                    out IInteract i))
            {
                CurrentInteractable.Value = i;
            }
        }
    }

    #endregion

    #region Hitted

    // 타격: 총알
    public void Try_Hitted(EnemyBulletController _Bullet)
    {
        if (isUltraMode)
        {
            Debug.Log("FOR NEOWIZ QUEST: 울트라 모드");
            return;
        }

        if (IsInvincible || IsDead)
        { return; }

        IsInvincible = true;

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            BulletState state = _Bullet.State;
            
            // 데미지 구현 (Dmg: 적의 냉기 디버프 계산)
            Take_Damaged(
                DevTool.Get_DmgEffectByCold(state.dmgState.dmg, _Bullet.Enemy.BuffController),
                DevTool.Get_Dir(_Bullet.gameObject, gameObject),
                state.knockbackState);
        }
    }

    // 타격: 어택커
    public void Try_Hitted(EnemyAttackerController _Attacker)
    {
        if (isUltraMode)
        {
            Debug.Log("FOR NEOWIZ QUEST: 울트라 모드");
            return;
        }

        if (IsInvincible || IsDead)
        { return; }

        IsInvincible = true;

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            AttackerState state = _Attacker.AttackerState;

            // 데미지 구현 (Dmg: 적의 냉기 디버프 계산)
            Take_Damaged(
                DevTool.Get_DmgEffectByCold(state.dmgState.dmg, _Attacker.Enemy.BuffController),
                DevTool.Get_Dir(_Attacker.gameObject, gameObject),
                state.knockbackState);
        }
    }

    // 타격: 어택커
    public void Try_Hitted(EnemyExplosionController _Explosion)
    {
        if (isUltraMode)
        {
            Debug.Log("FOR NEOWIZ QUEST: 울트라 모드");
            return;
        }

        if (IsInvincible || IsDead)
        { return; }

        IsInvincible = true;

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            ExplosionState state = _Explosion.State;

            // 데미지 구현 (Dmg: 적의 냉기 디버프 계산)
            Take_Damaged(
                DevTool.Get_DmgEffectByCold(state.dmgState.dmg, _Explosion.Enemy.BuffController),
                DevTool.Get_Dir(_Explosion.gameObject, gameObject),
                state.knockbackState);
        }
    }


    // 타격: 건물어택커
    public void Try_Hitted(TrapObjectController _Attacker)
    {
        if (isUltraMode)
        {
            Debug.Log("FOR NEOWIZ QUEST: 울트라 모드");
            return;
        }

        if (IsInvincible || IsDead)
        { return; }

        IsInvincible = true;

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
        }
    }

    #endregion

    #region Damaged

    // 데미지 계산
    private void Take_Damaged(float _DmgValue, Vector2 _HittedDir, KnockbackState _State_KB)
    {
        if (isUltraMode)
        {
            Debug.Log("FOR NEOWIZ QUEST: 울트라 모드");
            return;
        }

        // Multiple
        _DmgValue *= TakingDmgMultiple.buffedState;

        // Effect
        // Knockback
        if (_State_KB.canKB)
        { 
            Gain_Knockback(new CurrentKnockbackState(_HittedDir, _State_KB.kbPower, _State_KB.kbTime));
        }

        // Damage
        Take_Damaged(_DmgValue, _HittedDir, _ShowHUDEffect: true);
    }

    // 오직 데미지만 계산 (넉백, 애니메이션 등 설정)
    public void Take_Damaged(float _DmgValue, Vector2 _HittedDir, bool _ShowHUDEffect = true)
    {
        if (isUltraMode)
        {
            Debug.Log("FOR NEOWIZ QUEST: 울트라 모드");
            return;
        }

        AllyRequestManager.instance.Play_TakingDamage();

        if (_ShowHUDEffect)
        {
            MainGameUIManager.instance.playerHUD_UIController.Play_HittedPlayScreen(_DmgValue, 0.1f);
            MainGameUIManager.instance.playerHUD_UIController.Play_HittedPlayInfo(_DmgValue, InvincibleTime);
        }

        if (_HittedDir != Vector2.zero)
        {
            PlayerManager.instance.cameraController.Play_DamagedAnim(InvincibleTime, _DmgValue * 0.1f, _HittedDir);
        }
       

        if (ShieldElements.Count > 0)
        {
            for (int i = ShieldElements.Count - 1; i >= 0; i--)
            {
                // 쉴드 버프량 1개가 데미지보다 작거나 같으면, 제거하고 다음 쉴드로 영향
                if (ShieldElements[i].shieldCurrentValue <= _DmgValue)
                {
                    _DmgValue -= ShieldElements[i].shieldCurrentValue;

                    Remove_Shield(ShieldElements[i]); // 쉴드 감소
                }
                else // 쉴드 버프가 데미지를 버틸 수 있으면
                {
                    ShieldElements[i].shieldCurrentValue -= _DmgValue;
                    _DmgValue = 0;
                    CurrentSP.Value = Get_TotalShield();
                    return;
                }
            }
        }
        CurrentSP.Value = Get_TotalShield();
        Add_CurrentEP(-_DmgValue);
    }

    #endregion

    #region Died

    // Dead!
    protected override void Set_Die()
    {
        base.Set_Die();

        SoundManager.instance.Play_2D_SFX_Player("Killed");

        EventManager.instance.Set_Input(false);

        LayerOrderManager.instance.Remove_NeedSortObj(this);

        EnemyManager.instance.SetOff_AllEnemyPattern();

        this.gameObject.SetActive(false);

        MainGameUIManager.instance.Play_DeadProd();
    }

    #endregion

    #region Avoid

    // 회피?
    public bool Is_Avoid()
    {
        // 회피
        if (DevTool.Is_ChanceSuccess(AvoidChance.actualState.Value))
        {
            Play_Avoid();
            SoundManager.instance.Play_2D_SFX_Player(ASQueueSet.Get_T(), "Avoid");
            return true;
        }
        else
        {
            SetOn_Invincible();
            SoundManager.instance.Play_2D_SFX_Player(ASQueueSet.Get_T(), "Hitted");
            return false;
        }
    }

    // 회피
    private void Play_Avoid()
    {
        PlayerManager.instance.cameraController.Play_AvoidAnim(InvincibleTime);
        UnitManager.instance.player_ExplImgGenerator.Expl_Player_Avoid(id, TargetObject.transform.position);
        MainGameUIManager.instance.playerHUD_UIController.Play_AvoidPlayInfo(InvincibleTime);
    }
    
    // 회피하지 못함 => 무적
    private void SetOn_Invincible()
    {
        float intervalTime = InvincibleTime * 0.125f; // (1/8)
        for (int i = 0; i < AfterImgGenerator.TargetSRList.Count; i++)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(AfterImgGenerator.TargetSRList[i].DOFade(0, 0));
            seq.AppendInterval(intervalTime);
            seq.Append(AfterImgGenerator.TargetSRList[i].DOFade(1, 0));
            seq.AppendInterval(intervalTime);
            seq.SetLoops(4, LoopType.Restart);
        }
    }

    // 회피 끝
    public void SetOff_Invincible()
    {
        IsInvincible = false;
    }

    #endregion

    #region Prison Panelty

    public void Set_PrisonPanelty()
    {
        Add_CurrentCredit(-(int)(CurrentCredit.Value * 0.2f));
        Add_CurrentOverrider(-(int)(CurrentOverrider.Value * 0.2f));
        Add_CurrentModuleShard(-(int)(CurrentModuleShard.Value * 0.2f));

        Take_Damaged(Get_CurrentEP().Value * 0.2f, Vector2.zero, false);

        MainGameUIManager.instance.playerHUD_UIController.Play_PrisonPanelty();
    }

    #endregion

    #region Ally

    public void Try_STAllyLvUp()
    {
        if (StageManager.instance.currentRoomController.RoomRuleController.RoomType != eRoomType.Completed)
            return;

        if (NeedStrikeTeamPresence.Value <= StrikeTeamPresence.Value)
        {
            NeedStrikeTeamPresence.Value += NeedIntervalPresence;

            MainGameUIManager.instance.allyCard_UIController.TypeIndex = 0;
            MainGameUIManager.instance.allyCard_UIController.SetOn_ThisPanel();
        }
    }

    public void Try_UTAllyLvUp()
    {
        if (StageManager.instance.currentRoomController.RoomRuleController.RoomType != eRoomType.Completed)
            return;

        if (NeedUplinkTeamPresence.Value <= UplinkTeamPresence.Value)
        {
            NeedUplinkTeamPresence.Value += NeedIntervalPresence;

            MainGameUIManager.instance.allyCard_UIController.TypeIndex = 1;
            MainGameUIManager.instance.allyCard_UIController.SetOn_ThisPanel();
        }
    }

    public void Try_NTAllyLvUp()
    {
        if (StageManager.instance.currentRoomController.RoomRuleController.RoomType != eRoomType.Completed)
            return;

        if (NeedNeoTeamPresence.Value <= NeoTeamPresence.Value)
        {
            NeedNeoTeamPresence.Value += NeedIntervalPresence;

            MainGameUIManager.instance.allyCard_UIController.TypeIndex = 2;
            MainGameUIManager.instance.allyCard_UIController.SetOn_ThisPanel();
        }
    }

    #endregion

    #region Ping

    public void Try_PingEnemy(AimController _Aim)
    {
        EnemyController enemy = EnemyManager.instance.Get_ClosestEnemy(_Aim.gameObject, out float dis);
        if (enemy != null && dis <= 3)
            PlayerManager.instance.SetOn_PingEnemy(enemy);
        else
            PlayerManager.instance.SetOff_PingEnemy();
    }

    #endregion

    #region Trail

    public void SetOn_Trail()
    {
        ThisTrail.Clear();
        ThisTrail.emitting = true;
    }

    public void SetOff_Trail()
    {
        ThisTrail.emitting = false;
        ThisTrail.Clear();
    }

    #endregion

    #region Reputation


    public void Gain_Reputation(float _Value)
    {
        Reputation.Value = Mathf.Min(Reputation.Value + _Value, 100f);
    }

    public void Reduce_Reputation(float _Value)
    {
        Reputation.Value = Mathf.Max(Reputation.Value - _Value, 0f);
    }

    #endregion

    #region Sound

    public AudioSource Get_AS()
    {
        return ASQueueSet.Get_T();
    }

    #endregion

    private static bool isUltraMode = false;

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Alpha4) && StageManager.instance.targetStageID != 99)
        {
            /*
            BaseWeapon.BaseDamage.BuffedState = 300f;
            BaseWeapon.AccuracyRate.ActualState.Value = 100f;
            WalkSpeed.ActualState.Value = 15f;
            */

            CurrentChargedBettery.Value = 9999;
            CurrentCredit.Value = 9999;
            CurrentOverrider.Value = 9999;
            CurrentModuleShard.Value = 9999;

            StrikeTeamPresence.Value = 100;
            UplinkTeamPresence.Value = 100;
            NeoTeamPresence.Value = 100;

            PlayerManager.instance.Gain_KeyCard(0, 99);
            PlayerManager.instance.Gain_KeyCard(1, 99);
            PlayerManager.instance.Gain_KeyCard(2, 99);
            PlayerManager.instance.Gain_KeyCard(3, 99);
            PlayerManager.instance.Gain_KeyCard(4, 99);

            Debug.Log("Alpha4: Get Many Goods");
        }

        else if (Input.GetKeyDown(KeyCode.Alpha5) && StageManager.instance.targetStageID != 99)
        {
            UnitManager.instance.Test_Cor();

            Debug.Log("Alpha5: Spawn Builds");
        }

        else if (Input.GetKeyDown(KeyCode.Alpha6) && StageManager.instance.targetStageID != 99)
        {
            isUltraMode = !isUltraMode;
            UnitManager.instance.Set_UltraModeGO(isUltraMode);

            Debug.Log("Alpha6: Ultra Mode " + isUltraMode);
        }
    }

}
