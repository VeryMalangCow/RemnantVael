using UniRx;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections.Generic;

public class PlayerController : AliveObjectController
{
    #region Value

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

    #region - Hide

    // Stage Type
    [HideInInspector] private SortingGroup ThisSG;
    [HideInInspector] private SpriteRenderer ShadowSR;

    // Combat Mode
    [HideInInspector] private eCombatMode CombatMode = eCombatMode.Physics;

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
    [HideInInspector] private List<GameObject> CurrentInteractableGOList = new List<GameObject>();
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

    // Book
    [HideInInspector] private eCombatMode TargetCombatMode = eCombatMode.Physics;
    [HideInInspector] private CooltimeData CastingTime = new CooltimeData();
    [HideInInspector] private Dele ReservationDele = null;
    [HideInInspector] private CooltimeData InvincibleTime = new CooltimeData(0.5f, 0f);
    [HideInInspector] private int TargetBoostLv = 0;

    #endregion

    #region - Set Data

    [HideInInspector] public readonly int MaxBoostLv = 4;
    [HideInInspector] public readonly int NeedBS_ForMakeBC = 4;
    [HideInInspector] public readonly float NeedEP_ForMakeEC = 5f;
    [HideInInspector] private List<float> DecEnergyPointByLevel
        = new List<float>() { 1f, 2f, 3.5f, 5.5f };

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_Controller();

        Offset_Subscribe();

        Offset_BaseAnim();
        StateAnim.Set_Anim(new State_Anim(DmgTypeStateAC.TypeA, 0.8f), 1f);
        Set_BoostAnim(CurrentBoostLv.Value, MaxBoostLv);
        MoveDirStateAnim.Set_Anim(new State_Anim(MoveDirAC));

        SetOff_RoomMoveDir();


        // Test
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

    private void Offset_Controller()
    {
        DashController.Offset();
        ThisSG = DevTool.Get_ComponentTType<SortingGroup>(gameObject); 
        ShadowSR = DevTool.Get_ComponentTType<SpriteRenderer>(transform.GetChild(0).gameObject);
    }

    protected void Offset_BaseAnim()
    {
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
    }

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        Caculate_Always(Time.deltaTime);
    }


    private void FixedUpdate()
    {
        Play_Movement(Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        MainGameUIManager.Instance.InteractAnno_UIController.Set_PosIfNot(CurrentInteractable.Value);
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
        LayerOrderManager.Instance.NeedLayerObjects.Add(this);
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
        if (ShieldElements.Contains(_S))
        {
            ShieldElements.Remove(_S);
        }
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
        Play_Walk(
            InputManager.Instance.InputMoveDir,
            BaseWeapon.IsShooting ?
                WalkSpeed.ActualState.Value * WalkSpeedWhenShotMultiple.ActualState.Value :
                WalkSpeed.ActualState.Value,
            _DeltaTime);
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

    // nd
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
    public void Try_Hitted(TrapObjectController _Attacker)
    {
        if (IsInvincible)
        { return; }

        // 항상
        Play_Hitted();

        // 피격
        if (!Is_Avoid()) // 회피인지?
        {
            
            /*
            // 간소화
            AttackerState state = _Attacker.AttackerState;

            // 데미지 구현
            Take_Damaged(state.DmgState.Dmg,
                ((Vector2)transform.position - (Vector2)_Attacker.transform.position).normalized,
                state.KnockbackState.CanKB,
                state.KnockbackState.KBPower,
                state.KnockbackState.KBTime);
            */
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
        InvincibleTime.Current = 0f;

        float intervalTime = 0.075f;
        for (int i = 0; i < AfterImgGenerator.TargetSRList.Count; i++)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(AfterImgGenerator.TargetSRList[i].DOFade(0, 0));
            seq.AppendInterval(intervalTime);
            seq.Append(AfterImgGenerator.TargetSRList[i].DOFade(1, 0));
            seq.AppendInterval(intervalTime);
            seq.SetLoops((int)(InvincibleTime.Max / (intervalTime * 2f)), LoopType.Restart);
        }
    }

    // 데미지 계산
    private void Take_Damaged(float _DmgValue, Vector2 _HittedDir, bool _AbleKB, float _KBPower, float _KBTime)
    {
        // Multiple
        _DmgValue *= TakingDmgMultiple.BuffedState;

        // Effect
        PlayerManager.Instance.CameraController.Play_DamagedAnim(InvincibleTime.Max, _DmgValue * 0.1f, _HittedDir);

        // Knockback
        if (_AbleKB)
        { Gain_Knockback(new CurrentKnockbackState(_HittedDir, _KBPower, _KBTime)); }

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
        PlayerManager.Instance.CameraController.Play_AvoidAnim(InvincibleTime.Max);

        for (int i = 0; i < 4; i++)
        {
            UnitManager.Instance.Player_ExplImgGenerator.Expl_Player_Avoid(ID, TargetObject.transform.position);
        }
    }

    #endregion

    #region About Casting

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

        TargetCombatMode = TargetCombatMode == eCombatMode.Physics ?
            eCombatMode.Energy : eCombatMode.Physics;

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
    public void Try_Execution()
    {
        if (!Can_Change())
        { return; }

        if (DevTool.Get_CastingTType(CurrentInteractable.Value, out EnemyController ec))
        {
            ec.Play_Interact();
        }

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

    #region Caculate Time

    private void Caculate_Always(float _DeltaTime)
    {
        Caculate_Casting(_DeltaTime);
        Caculate_Boosting(_DeltaTime, CurrentBoostLv.Value);
        Caculate_Invincible(_DeltaTime);
    }

    private void Caculate_Casting(float _DeltaTime)
    {
        if (MovementState == eMovementState.Casting)
        {
            if(CastingTime.Current < CastingTime.Max)
            {
                CastingTime.Current += Time.deltaTime;
            }
            else
            {
                CastingTime.Current = 0f;
                MovementState = eMovementState.IdleOrWalk;
                InputManager.Instance.IsPlayingSkill = false;

                Set_CombatMode();
                Set_Skill();
                Set_Boost();
            }
        }
    }

    private void Caculate_Boosting(float _DeltaTime, int _BoostLv)
    {
        if(_BoostLv > 0)
        {
            float decValue = DecEnergyPointByLevel[_BoostLv - 1] * DecEnergyPointMultiple.ActualState.Value;
            Add_CurrentEP(-decValue * Time.deltaTime);
        }
    }

    private void Caculate_Invincible(float _DeltaTime)
    {
        if (IsInvincible)
        {
            if (InvincibleTime.Max > InvincibleTime.Current)
            {
                InvincibleTime.Current += Time.deltaTime;
            }
            else
            {
                InvincibleTime.Current = 0f;

                IsInvincible = false;
            }
        }
    }

    #endregion

    #region Cacualate By Condition

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
                StateAnim.Set_Anim(new State_Anim(DmgTypeStateAC.TypeA, 0.8f), 1f);
                InputManager.Instance.AimController.Set_PhysicsType();
                break;

            case eCombatMode.Energy:
                StateAnim.Set_Anim(new State_Anim(DmgTypeStateAC.TypeB, 0.8f), 1f);
                InputManager.Instance.AimController.Set_EnergyType();
                break;

            default:
                break;
        }
    }

    private void Set_Skill()
    {
        if(ReservationDele != null)
        {
            ReservationDele();
            ReservationDele = null;
        }
    }

    private void Set_Boost()
    {
        if(CurrentBoostLv.Value != TargetBoostLv)
        {
            CurrentBoostLv.Value = TargetBoostLv;
            Set_BoostAnim(CurrentBoostLv.Value, MaxBoostLv);
        }
    }

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
                BoostStateAnimController.TypeBase[i].Set_Anim(new State_Anim(BoostOnOffAC.TypeSpecial, lowestAnimSpeed * (_MaxIndex * 2)), 1f);
            }
        }
        else
        {
            for (int i = 0; i < _MaxIndex - 1; i++)
            {
                if (i < _Index)
                {
                    BoostStateAnimController.TypeBase[i].Set_Anim(new State_Anim(BoostOnOffAC.TypeSpecial, lowestAnimSpeed * (_Index - i + 1)), 1f);
                }
                else
                {
                    BoostStateAnimController.TypeBase[i].Set_Anim(new State_Anim(BoostOnOffAC.TypeBase, lowestAnimSpeed), 1f);
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
                BoostStateAnimController.TypeSpecial[0].Set_Anim(new State_Anim(BoostVFXAnimList[0], 0.5f), 1f);
                break;

            case 2:
                BoostStateAnimController.TypeSpecial[0].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[0].Set_Anim(new State_Anim(BoostVFXAnimList[0], 1f), 1f);
                break;

            case 3:
                BoostStateAnimController.TypeSpecial[0].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[0].Set_Anim(new State_Anim(BoostVFXAnimList[0], 1f), 1f);

                BoostStateAnimController.TypeSpecial[1].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[1].Set_Anim(new State_Anim(BoostVFXAnimList[1], 1f), 1f);
                break;

            case 4:
                BoostStateAnimController.TypeSpecial[0].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[0].Set_Anim(new State_Anim(BoostVFXAnimList[0], 1.5f), 1f);

                BoostStateAnimController.TypeSpecial[1].gameObject.SetActive(true);
                BoostStateAnimController.TypeSpecial[1].Set_Anim(new State_Anim(BoostVFXAnimList[1], 1.5f), 1f);
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
