using UniRx;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections.Generic;

public class PlayerController : AliveObjectController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Controller & Generator")]
    [SerializeField] public PlayerWeaponController BaseWeapon;
    [SerializeField] public SkillWeaponController SkillWeapon;
    [SerializeField] public PlayerDashController DashController;
    [SerializeField] public RigidbodySolarController LowerController;
    [SerializeField] public WayPointController ThisWayPoint;

    [Space(5)]
    [SerializeField] public AfterImgGenerator AfterImgGenerator;

    [Space(10)]
    [Header("=== Energy")]
    [SerializeField] public Sprite ES_Sprite;

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

    [Space(10)]
    [Header("=== Visual Comp")]

    [Space(5)]
    [SerializeField] private StateAnimController StateAnim;
    [SerializeField] private StateAnimController MoveDirStateAnim;
    [SerializeField] private List<MovableDepthController> BaseAnimDepthList;
    [SerializeField] private CoupleData<List<StateAnimController>> BoostStateAnimController;


    [Space(10)]
    [Header("=== Visual Reso")]

    [SerializeField] private AnimationClip MoveDirAC;
    [SerializeField] private Sprite ChangeState_DamageType;

    [SerializeField] public List<Material> MaterialList;
    [SerializeField] private List<AnimationClip> BoostVFXAnimList;

    [SerializeField] private PlayerVisual<Color> ThisClr;

    [SerializeField] private PlayerVisual<AnimationClip> ThisHittedPointAC;
    [SerializeField] private TrioData<AnimationClip> DmgTypeStateAC;
    [SerializeField] private CoupleData<AnimationClip> BoostOnOffAC;

    [SerializeField] private CoupleData<Sprite> ChangeState_BoostUpDown;
    [SerializeField] private CoupleData<Sprite> ChangeState_Skill;

    [SerializeField] public GameObject AimPrefab;
    [SerializeField] public GameObject AimRoundPrefab;

    [SerializeField] public List<string> BUUITabStringList;
    [SerializeField] public List<string> MUUITabStringList;

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
    [HideInInspector] public ReactiveProperty<int> CurrentBS = new();
    [HideInInspector] public ReactiveProperty<int> CurrentBC = new();
    [HideInInspector] public ReactiveProperty<int> CurrentEC = new();
    [HideInInspector] public ReactiveProperty<int> CurrentMS = new();

    // BaseAnim
    [HideInInspector] private Sequence BaseSeq = null;
    [HideInInspector] private readonly float BaseYLimit = 0.02f;
    [HideInInspector] private readonly float BaseTweenReTime = 0.25f;
    [HideInInspector] private readonly float WheelLowestAnimSpeed = 0.5f;

    // Book
    [HideInInspector] private eDamageType TargetDmgMode = eDamageType.Physics;
    [HideInInspector] private CooltimeData CastingTime = new CooltimeData();
    [HideInInspector] private Dele ReservationDele = null;
    [HideInInspector] private CooltimeData InvincibleTime = new CooltimeData(0.5f);
    [HideInInspector] private int TargetBoostLv = 0;

    #endregion

    #region - Data

    [HideInInspector] public readonly int MaxBoostLv = 4;
    [HideInInspector] public readonly int NeedBS_ForMakeBC = 4;
    [HideInInspector] public readonly float NeedEP_ForMakeEC = 5f;
    [HideInInspector] private List<float> DecEnergyPointByLevel
        = new List<float>() { 1f, 2f, 3.5f, 5.5f };

    #endregion

    #endregion


    #region Offset

    protected override void Offset_FirstSetting()
    {
        // State Anim
        Reset_StateAnim();

        // State Anim : Dmg Type
        StateAnim.Set_Anim(new State_Anim(DmgTypeStateAC.TypeA, 0.8f), 1f);

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


        Debug.Log("Test EC");
        CurrentEC.Value = 100;
    }

    protected override void Offset_Subscribe()
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

    protected override void Offset_Controller()
    {
        DashController.Offset();
        ThisSG = DevTool.Get_ComponentTType<SortingGroup>(gameObject); 
        ShadowSR = DevTool.Get_ComponentTType<SpriteRenderer>(transform.GetChild(0).gameObject);
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
        MainGameUIManager.Instance.InteractAnno_UIController.Set_PosIfNot(CurrentInteractable.Value);
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
        
        StateAnim.transform.parent.transform.gameObject.SetActive(true); 

        StageManager.Instance.IsStartStage = false;
        LayerOrderManager.Instance.NeedSortingObjects.Add(this);
    }

    // 스테이지 끝
    public void Set_EndStage()
    {
        ThisSG.enabled = true;
        ThisSG.sortingOrder = 3001;

        StateAnim.transform.parent.transform.gameObject.SetActive(false);
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
            DevTool.Add_RefValue(ref _Variable, _Shield.ShieldCurrentValue);
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
            if (DevTool.Is_UsableAndEqual(shieldBuff, shieldBuff.ThisShield, _Shield))
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
        Add_CurrentEP(_AddValue, MaxEP.ActualState.Value);
        Check_IsDead(CurrentEP.Value);
    }

    public float Get_PercentEP(float _Percent)
    {
        return DevTool.Get_Percent(_Percent, MaxEP.ActualState.Value);
    }

    #endregion

    #region Item

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

    // 배터리 셀 획득
    private void Add_CurrentBC()
    {
        MainGameUIManager.Instance.PlayerHUD_UIController.CurrentEmptyBC.Set_Complete(0.3f, 0.2f);
        int BSAmount = CurrentBS.Value / NeedBS_ForMakeBC;
        CurrentBS.Value -= NeedBS_ForMakeBC * BSAmount;
        CurrentBC.Value += BSAmount;
    }

    // 에너지 셀 획득
    private void Make_EC()
    {
        this.CurrentEP.Value -= NeedEP_ForMakeEC;
        CurrentBC.Value--;
        CurrentEC.Value++;
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
            BaseWeapon.IsShooting ? WalkSpeedWhenShotMultiple.ActualState.Value : 1f;
        Play_Walk(
            InputManager.Instance.InputMoveDir, WalkSpeed.ActualState.Value * multiple, _DeltaTime);
    }

    public void Try_Dash()
    {
        if (Can_Change() && DashController.Is_EnoughEP())
        {
            InputManager.Instance.IsPlayingSkill = true;
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
            .SetLoops(-1, LoopType.Restart);
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
    public readonly float ExecutionInterval = 0.75f;

    // 조건: + None
    public void Try_CombatModeCheck()
    {
        if (!Can_Change()) 
        { return; }

        StateAnim.Set_Anim(
            new State_Anim(DmgTypeStateAC.TypeSpecial, 2f), 
            _InnerSprite: ChangeState_DamageType);

        TargetDmgMode = TargetDmgMode == eDamageType.Physics ?
            eDamageType.Energy : eDamageType.Physics;

        Start_Casting(CombatModeInterval);
    }


    // 조건: + 부스트 최대 레벨을 넘지 않도록
    public void Try_BoostModeCheck()
    {
        if (!Can_Change() || TargetBoostLv >= MaxBoostLv)
        { return; }

        TargetBoostLv++;
        StateAnim.Set_Anim(
            new State_Anim(DmgTypeStateAC.TypeSpecial, 2f),
            _InnerSprite: ChangeState_BoostUpDown.TypeSpecial);

        Start_Casting(BoostModeInterval);
    }



    // 조건: + 부스트 레벨이 0 아래가 되지 않도록
    public void Try_UnBoostModeCheck()
    {
        if (!Can_Change() || CurrentBoostLv.Value <= 0)
        { return; }

        TargetBoostLv--;
        StateAnim.Set_Anim(
            new State_Anim(DmgTypeStateAC.TypeSpecial, 2f),
            _InnerSprite: ChangeState_BoostUpDown.TypeBase);

        Start_Casting(UnBoostModeInterval);
    }


    // 조건: + EP, BC가 충분한가?
    public void Try_ChargeBettery()
    {
        if (!Can_Change() ||
            NeedEP_ForMakeEC >= this.CurrentEP.Value ||
            CurrentBC.Value <= 0) 
        { return; }

        ReservationDele = Make_EC;

        Start_Casting(ChargeBetteryInterval);
    }


    // 조건: + None
    public void Try_Execution(EnemyController _Enemy)
    {
        if (!Can_Change())
        { return; }

        _Enemy.Play_Execution();
        
        Start_Casting(ExecutionInterval);
    }

    // 조건: + ex) 스킬을 사용할 수 없다면, Error 문구
    public void Try_Skill0()
    {
        Try_Skill(0, _Sprite: ChangeState_Skill.TypeBase);
    }

    public void Try_Skill1()
    {
        Try_Skill(1, _Sprite: ChangeState_Skill.TypeSpecial);
    }

    private void Try_Skill(int _Index, Sprite _Sprite)
    {
        if (!Can_Change())
        { return; }

        if (!SkillWeapon.SkillList[_Index].Can_Active())
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.SkillList[_Index].Play_ErrorUI();
            return;
        }

        ReservationDele = SkillWeapon.SkillList[_Index].Active_Skill;
        StateAnim.Set_Anim(
            new State_Anim(DmgTypeStateAC.TypeSpecial, 2f),
            _Sprite);

        Start_Casting(SkillInterval);
    }


    public void Start_Casting(float _CastingTime)
    {
        CastingTime.Max = _CastingTime;
        MovementState = eMovementState.Casting;
        ThisRb.velocity = Vector2.zero;

        InputManager.Instance.IsPlayingSkill = true;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (CurrentInteractableGOList.Count <= 0 ||
            CurrentInteractable.Value == null)
        { return; }

        CurrentInteractable.Value.Play_Interact();
        
        MainGameUIManager.Instance.PlayerHUD_UIController.Set_UseInteractUI();
    }

    #endregion

    #region Update Caculate

    private void Update_Caculate(float _DeltaTime)
    {
        Caculate_Casting(_DeltaTime);
        Caculate_Boosting(_DeltaTime, CurrentBoostLv.Value);
        Caculate_Invincible(_DeltaTime);
    }

    private void Caculate_Casting(float _DeltaTime)
    {
        if (MovementState != eMovementState.Casting)
        { return; }

        if (CastingTime.Is_Charge(_DeltaTime)) // 캐스팅 완료
        {
            CastingTime.Current = 0f;
            MovementState = eMovementState.IdleOrWalk;
            InputManager.Instance.IsPlayingSkill = false;

            Set_CombatMode();
            Set_Skill();
            Set_Boost();
        }
    }

    private void Caculate_Boosting(float _DeltaTime, int _BoostLv)
    {
        if(_BoostLv > 0)
        {
            float decValue = DecEnergyPointByLevel[_BoostLv - 1] * DecEnergyPointMultiple.ActualState.Value;
            Add_CurrentEP(-decValue * _DeltaTime);
        }
    }

    private void Caculate_Invincible(float _DeltaTime)
    {
        if (IsInvincible)
        {
            if (InvincibleTime.Is_Charge(_DeltaTime))
            {
                InvincibleTime.Current = 0f;
                IsInvincible = false;
            }
        }
    }

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
            InputManager.Instance.AimController.Set_DmgType(TargetDmgMode);
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

    private void Set_Boost()
    {
        if(CurrentBoostLv.Value != TargetBoostLv)
        {
            CurrentBoostLv.Value = TargetBoostLv;
            Set_BoostAnim(CurrentBoostLv.Value);
            Reset_StateAnim();
        }
    }

    // 모든 캐스팅이 끝나면, 본래의 스탯애니메이션으로 돌아옴
    private void Reset_StateAnim()
    {
        StateAnim.Set_Anim(
                new State_Anim(TargetDmgMode == eDamageType.Physics ?
                    DmgTypeStateAC.TypeA : DmgTypeStateAC.TypeB, 0.8f));
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
                BoostOnOffAC.TypeSpecial : BoostOnOffAC.TypeBase;
            float animSpeed = i < _Index ?
                WheelLowestAnimSpeed * (_Index - i + 1) : WheelLowestAnimSpeed;

            BoostStateAnimController.TypeBase[i].Set_Anim(new State_Anim(ac, animSpeed));
        }
    }

    // 스탯의 좌우로 도는 이펙트
    private void Set_BoostAnim_StateExtraVFX(int _Index)
    {
        BoostStateAnimController.TypeSpecial[0].gameObject.SetActive(false);
        BoostStateAnimController.TypeSpecial[1].gameObject.SetActive(false);

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
        BoostStateAnimController.TypeSpecial[_Index].gameObject.SetActive(true);
        BoostStateAnimController.TypeSpecial[_Index].Set_Anim(new State_Anim(BoostVFXAnimList[_Index], _AnimSpeed));

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
        return ThisClr.Get_CorrectType(_DamageType).Get_Special(_IsCritical);
    }

    public AnimationClip Get_CorrectAC(eDamageType _DamageType, bool _IsCritical)
    {
        return ThisHittedPointAC.Get_CorrectType(_DamageType).Get_Special(_IsCritical);
    }

    #endregion

    #region Trigger

    // Enter
    private void OnTriggerEnter2D(Collider2D _Col) // 판별을 위한 IInteract GO 추가
    {
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
        if (IsInvincible && IsDead)
        { return; }

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            BulletState state = _Bullet.State;
            
            // 데미지 구현 (Dmg: 적의 냉기 디버프 계산)
            Take_Damaged(
                DevTool.Get_DmgEffectByCold(state.DmgState.Dmg, _Bullet.Enemy.BuffController),
                DevTool.Get_Dir(this.gameObject, _Bullet.gameObject),
                state.KnockbackState);
        }
    }

    // 타격: 어택커
    public void Try_Hitted(EnemyAttackerController _Attacker)
    {
        if (IsInvincible && IsDead)
        { return; }

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            AttackerState state = _Attacker.AttackerState;

            // 데미지 구현 (Dmg: 적의 냉기 디버프 계산)
            Take_Damaged(
                DevTool.Get_DmgEffectByCold(state.DmgState.Dmg, _Attacker.Enemy.BuffController),
                DevTool.Get_Dir(this.gameObject, _Attacker.gameObject),
                state.KnockbackState);
        }
    }

    // 타격: 건물어택커
    public void Try_Hitted(TrapObjectController _Attacker)
    {
        if (IsInvincible && IsDead)
        { return; }

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
        // Multiple
        _DmgValue *= TakingDmgMultiple.BuffedState;

        // Effect
        // Knockback
        if (_State_KB.CanKB)
        { 
            Gain_Knockback(new CurrentKnockbackState(_HittedDir, _State_KB.KBPower, _State_KB.KBTime));
        }

        // Damage
        Take_Damaged(_DmgValue, _HittedDir, _ShowHUDEffect: true);
    }

    // 오직 데미지만 계산 (넉백, 애니메이션 등 설정)
    public void Take_Damaged(float _DmgValue, Vector2 _HittedDir, bool _ShowHUDEffect = true)
    {
        if (_ShowHUDEffect)
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.Play_HittedPlayScreen(_DmgValue);
        }

        if (_HittedDir != Vector2.zero)
        {
            PlayerManager.Instance.CameraController.Play_DamagedAnim(InvincibleTime.Max, _DmgValue * 0.1f, _HittedDir);
        }
       

        if (ShieldElements.Count > 0)
        {
            for (int i = ShieldElements.Count - 1; i >= 0; i--)
            {
                // 쉴드 버프량 1개가 데미지보다 작거나 같으면, 제거하고 다음 쉴드로 영향
                if (ShieldElements[i].ShieldCurrentValue <= _DmgValue)
                {
                    _DmgValue -= ShieldElements[i].ShieldCurrentValue;

                    Remove_Shield(ShieldElements[i]); // 쉴드 감소
                }
                else // 쉴드 버프가 데미지를 버틸 수 있으면
                {
                    ShieldElements[i].ShieldCurrentValue -= _DmgValue;
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

    #region Avoid

    // 회피?
    public bool Is_Avoid()
    {
        // 회피
        if (DevTool.Is_ChanceSuccess(AvoidChance.ActualState.Value))
        {
            Play_Avoid();
            return true;
        }
        else
        {
            Set_Invincible();
            return false;
        }
    }

    // 회피
    private void Play_Avoid()
    {
        PlayerManager.Instance.CameraController.Play_AvoidAnim(InvincibleTime.Max);
        UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_Avoid(ID, TargetObject.transform.position);
    }
    
    // 회피하지 못함 => 무적
    private void Set_Invincible()
    {
        IsInvincible = true;
        InvincibleTime.Current = 0f;

        float intervalTime = InvincibleTime.Max * 0.125f; // (1/8)
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

    #endregion
}
