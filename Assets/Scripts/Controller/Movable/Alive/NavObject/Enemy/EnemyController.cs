using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class EnemyController : NavObjectController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SortingGroup ThisSG;
    [SerializeField] private AudioSource ThisAudioSource;
    [SerializeField] public EnemyBuffController BuffController;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private eEnemy ThisEnemyType;
    [SerializeField] private float MaxHP;
    [SerializeField] private float MaxEP = 100f;
    [SerializeField] private float ChargeEPSpeed = 1f;

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] private EnemyDropItemPercent EnemyDropItemPercent;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] public EnemyHUDController HUD;

    [Space(10)]
    [Header("=== Satellite")]
    [SerializeField] private EnemySolarController LookingSatellite;
    [SerializeField] private RigidbodyAnimSolarController WalkingSatellite;

    [Space(10)]
    [Header("=== Ping Data")]
    [SerializeField] public Vector2 PingOffsetVec;
    [SerializeField] public Vector2 PingSizeVec;

    [Space(10)]
    [Header("=== Pattern")]
    [Tooltip("This Order of Priority Equle Index")]
    [SerializeField] protected List<OrderOfPriorityEnemyPattern> OrderOfPriorityEnemyPatternList;
    [SerializeField] protected ContinuousEnemyPattern SpecialPattern;


    #endregion

    #region - Hide


    // Discharge
    [HideInInspector] private bool IsDischarge = false;
    [HideInInspector] private float DischargeDelayTime = 1f;
    [HideInInspector] private float DischargeDelayCurrentTime = 0f;

    // FullCharge
    [HideInInspector] protected bool IsFullCharge = false;
    [HideInInspector] private bool IsPlayingSpecialPattern = false;

    // 패턴
    [HideInInspector] private EnemyPattern CurrentEnemyPattern = null;

    // 움직임을 통제
    [HideInInspector] private GameObject Target;

    [HideInInspector] public Vector2 LookAtPoint = Vector2.zero;
    [HideInInspector] public Vector2 LookAtDir = Vector2.zero;


    // 패턴
    [HideInInspector] private ContinuousEnemyPattern CurrentContinuousEnemyPattern = null;
    [HideInInspector] public bool IsPlayingPattern = false;
    [HideInInspector] public IEnumerator CurrentPatternCor = null;

    // 방
    [HideInInspector] private RoomController CurrentRoomController;

    // 죽음
    [HideInInspector] private eDamageType DieStateType;


    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_FirstSetting();
        Offset_Subscribe();
        Offset_Controller();

        EnemyManager.Instance.PoolingAllEnemyList.Add(this);
    }

    private void Offset_FirstSetting()
    {
        HUD.Offset();
    }

    private void Offset_Subscribe()
    {
        CurrentSP
            .Subscribe(_CurrentSP =>
            {
                HUD.StateUI.SP_ProgressBar.Set_FillImgSmooth(CurrentSP.Value, MaxHP);

                if (CurrentSP.Value <= 0)
                {
                    CurrentSP.Value = 0;
                    HUD.StateUI.SP_ProgressBar.Set_NoNum();
                    HUD.StateUI.HP_ProgressBar.Set_FillImgSmooth(CurrentHP.Value, MaxHP);
                    HUD.StateUI.EP_ProgressBar.Set_FillImgSmooth(CurrentEP.Value, MaxEP);

                    if (BuffController.ShieldBuff.IsOn)
                    { BuffController.ShieldBuff.Remove_AllStack(); }

                }
                else
                {
                    HUD.StateUI.HP_ProgressBar.Set_NoNum();
                    HUD.StateUI.EP_ProgressBar.Set_NoNum();
                }
            });

        CurrentHP
            .Subscribe(_CurrentHP =>
            {
                HUD.StateUI.HP_ProgressBar.Set_FillImgSmooth(CurrentHP.Value, MaxHP);

                if (CurrentSP.Value > 0)
                { HUD.StateUI.HP_ProgressBar.Set_NoNum(); }
            });

        CurrentEP
            .Subscribe(_CurrentEP =>
            {
                HUD.StateUI.EP_ProgressBar.Set_FillImgSmooth(CurrentEP.Value, MaxEP);

                if (CurrentSP.Value > 0)
                { HUD.StateUI.EP_ProgressBar.Set_NoNum(); }
            });
    }

    private void Offset_Controller()
    {
        BuffController.Offset(this);
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        Reset_State();

        DevTool.Add_InList(EnemyManager.Instance.CurrentEnemyList, this);
    }

    protected override void Update()
    {
        base.Update();

        Caculate_Charge(Time.deltaTime);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Update_LookAtTarget();
        Play_Movement(Time.fixedDeltaTime);
    }

    #endregion

    #region Movement

    private void Play_Movement(float _DeltaTime)
    {
        Play_Walk(MoveAtDir, MoveSpeed, _DeltaTime);
    }

    #endregion

    #region Energy

    private void Caculate_Charge(float _DeltaTime)
    {
        if (IsDischarge) // 방전 회복 딜레이
        {
            Caculate_DischargeDelay(_DeltaTime);
        }
        else if (!IsFullCharge) // 충전
        {
            Add_CurrentEP(ChargeEPSpeed * _DeltaTime);
        }
    }

    private void Caculate_DischargeDelay(float _DeltaTime)
    {
        DischargeDelayCurrentTime += _DeltaTime;
        if (DischargeDelayTime <= DischargeDelayCurrentTime)
        {
            DischargeDelayCurrentTime = 0f;
            IsDischarge = false;
        }
    }

    public void Reset_ChargeState()
    {
        IsDischarge = true;
        DischargeDelayCurrentTime = 0f;
        CurrentEP.Value = 0f;

        IsFullCharge = false;
        IsPlayingSpecialPattern = false;

        HUD.Reset_HUD();
    }

    #endregion

    #region HP

    // HP
    protected float Get_PercentHP()
    {
        return (CurrentHP.Value / MaxHP) * 100f;
    }

    #endregion

    #region Reset

    private void Reset_State()
    {
        base.IsDead = false;
        CurrentSP.Value = 0;
        CurrentHP.Value = MaxHP;

        // Energy
        CurrentEP.Value = 0;

        // Discharge
        Reset_ChargeState();

        if (Target == null) // 타겟 Player
        { Target = PlayerManager.Instance.PlayerController.gameObject; }

        if (CurrentRoomController == null) // 현재 Room
        { CurrentRoomController = StageManager.Instance.CurrentRoomController; }

    }

    #endregion

    #region Look

    private void Update_LookAtTarget()
    {
        if (IsDead) return;

        // 바라볼 방향값 계산
        LookAtDir = Target != null ? 
            DevTool.Get_Dir(this.gameObject, Target) : Vector2.down;
    }

    #endregion

    #region Life State Gain

    // 쉴드 설정
    // => 낮은 값이 설정되어도 적용할 것인가?
    public void Set_CurrentSP(float _SetValue, bool _LesserIsOk = false)
    {
        Set_CurrentSP(_SetValue, MaxHP, _LesserIsOk);
    }
    public void Set_CurrentSP_Zero()
    {
        Set_CurrentSP(0, MaxHP, _LesserIsOk: true);
    }

    private void Add_CurrentSP(float _AddValue)
    {
        Add_CurrentSP(_AddValue, MaxHP);
    }

    private void Add_CurrentHP(float _AddValue)
    { 
        Add_CurrentHP(_AddValue, MaxHP);
        Check_IsDead(CurrentHP.Value);
    }

    private void Add_CurrentEP(float _AddValue)
    {
        if (IsFullCharge) return;

        Add_CurrentEP(_AddValue, MaxEP);

        // 방전
        if (CurrentEP.Value <= 0)
        {
            IsDischarge = true;
            DischargeDelayCurrentTime = 0f;
        }

        // 풀 충전
        if (CurrentEP.Value >= MaxEP)
        {
            IsFullCharge = true;
            HUD.Set_Charged(IsFullCharge);
        }
    }

    public float Get_PercentHP(float _Percent)
    {
        return DevTool.Get_Percent(_Percent, MaxHP);
    }

    #endregion

    #region Hitted

    // 총알 데미지
    public void Try_Hitted(BulletController _Bullet)
    {
        if (IsDead)
        { return; }

        BulletState state = _Bullet.State;

        // Damage
        Take_Damaged(
            state, 
            state.IsCritical, 
            DevTool.Get_DirFromAngle(_Bullet.transform.eulerAngles.z));

        if (state.IsStatus)
        {
            switch (state.StatusType)
            {
                case eStatusEffect.Flame:
                    BuffController.FlameStack.Gain_Stack(1, true, state.OwnerData);
                    break;
                case eStatusEffect.Cold:
                    BuffController.ColdStack.Gain_Stack(1, true, state.OwnerData);
                    break;
                case eStatusEffect.Electricity:
                    BuffController.ElectricityStack.Gain_Stack(1, true, state.OwnerData);
                    break;
                case eStatusEffect.Corrosion:
                    BuffController.CorrosionStack.Gain_Stack(1, true, state.OwnerData);
                    break;
            }
        }
    }

    // 어택커 데미지
    public void Try_Hitted(AttackerController _Attacker)
    {
        if (IsDead)
        { return; }

        AttackerState state = _Attacker.AttackerState;

        // Damage
        Take_Damaged(
            state,
            DevTool.Is_ChanceSuccess(state.CriticalState.CC),
            DevTool.Get_Dir(_Attacker.gameObject, this.gameObject));
    }

    // 폭발 데미지
    public void Try_Hitted(ExplosionController _Explosion)
    {
        if (IsDead)
        { return; }

        ExplosionState state = _Explosion.State;

        // Damage
        Take_Damaged(
            state,
            DevTool.Is_ChanceSuccess(state.CriticalState.CC),
            DevTool.Get_Dir(_Explosion.gameObject, this.gameObject));

        Try_GainStack(state.IsFire, BuffController.FlameStack, state.OwnerData);
        Try_GainStack(state.IsCold, BuffController.ColdStack, state.OwnerData);
        Try_GainStack(state.IsElectricity, BuffController.ElectricityStack, state.OwnerData);
        Try_GainStack(state.IsCorrosion, BuffController.CorrosionStack, state.OwnerData);
    }

    private void Try_GainStack(bool _Is, StatusEffect_Temporary_WithAmount _TargetDebuff, CombatOwner _CombatOwner)
    {
        if (_Is) _TargetDebuff.Gain_Stack(1, true, _CombatOwner);
    }

    #endregion

    #region Damaged (Type)

    // 데미지, 넉백, 크리티컬, 모듈 호과 등
    private void Take_Damaged(CombatState _State, bool _IsCritical, Vector2 _DirKB)
    {
        float actualDmg = _State.DmgState.Dmg;

        // INTERFACE: 맞을 때 효과 
        if (_State.OwnerData.Owner == eCombatOwner.Player)
        {
            ModuleItemManager.Instance.Active_Hit(this);
            ModuleItemManager.Instance.ActiveSync_Hit();
        }
        else if (_State.OwnerData.Owner == eCombatOwner.Ally)
        {
            AllyManager.Instance.AllAlly[_State.OwnerData.ID].ActiveAlly_Hit();
        }

        // KB
        if (_State.KnockbackState.CanKB)
        {
            Gain_Knockback(new CurrentKnockbackState(_DirKB, _State.KnockbackState.KBPower, _State.KnockbackState.KBTime));
        }
        // 치명타 계산
        if (_IsCritical)
        {
            actualDmg *= _State.CriticalState.CD;

            // INTERFACE: 치명타를 맞을 때 효과 
            if (_State.OwnerData.Owner == eCombatOwner.Player)
            {
                ModuleItemManager.Instance.Active_CriticalHit(this);
                ModuleItemManager.Instance.ActiveSync_CriticalHit();
            }
            else if (_State.OwnerData.Owner == eCombatOwner.Ally)
            {
                AllyManager.Instance.AllAlly[_State.OwnerData.ID].ActiveAlly_CriticalHit();
            }
        }

        // 데미지 구현 (Dmg: 적의 부식 디버프 계산)
        Take_Damage(DevTool.Get_DmgEffectByCorrosion(actualDmg, BuffController),
            _State.DmgState.DmgType,
            _IsCritical);

        // 사운드
        if (!IsDead)
        { SoundManager.Instance.Play_2D_SFX(ThisAudioSource, "Enemy_Hitted"); }
        else
        { SoundManager.Instance.Play_2D_SFX("Enemy_Hitted"); }
    }

    #endregion

    #region Damaged (Caculate)

    // 오직 데미지만을 계산
    public void Take_Damage(float _DmgValue, eDamageType _DmgType, bool _IsCritical = false)
    {
        // 쉴드 계산
        if (CurrentSP.Value > 0)
        {
            float uiTxt = 0f;
            float uiX = 0f;
            if (CurrentSP.Value > _DmgValue)
            {
                uiTxt = _DmgValue;
                uiX = 0.2f;

                Add_CurrentSP(-_DmgValue);
                _DmgValue = 0f;
            }
            else
            {
                uiTxt = CurrentSP.Value;
                uiX = 0.1f;

                _DmgValue -= CurrentSP.Value;
                Set_CurrentSP_Zero();
            }

            PoolingManager.Instance.Get_OP_DmgTxt().Offset_ByShieldDmg(
                    (Vector2)TargetObject.transform.position + new Vector2(uiX, 0.2f),
                    uiTxt, _IsCritical);
        }

        if (_DmgValue <= 0)
        { return; }

        // 직접 데미지
        // 물리 값
        if (_DmgType == eDamageType.Physics) 
        {
            Take_Damaged_Physics(_DmgValue, _IsCritical);
        }
        // 에너지 값
        else 
        {
            Take_Damaged_Energy(_DmgValue, _IsCritical);
        }
    }

    // 물리 데미지를 받음
    private void Take_Damaged_Physics(float _DmgValue, bool _IsCritical)
    {
        // UI
        PoolingManager.Instance.Get_OP_DmgTxt().Offset_ByPhysicDmg(
            (Vector2)TargetObject.transform.position + new Vector2(-0.2f, 0.2f),
            _DmgValue, _IsCritical);

        Add_CurrentHP(-_DmgValue);
    }

    // 에너지 데미지를 받음
    private void Take_Damaged_Energy(float _DmgValue, bool _IsCritical)
    {
        // UI
        PoolingManager.Instance.Get_OP_DmgTxt().Offset_ByEnergyDmg(
            (Vector2)TargetObject.transform.position + new Vector2(-0.2f, 0.2f),
            _DmgValue, _IsCritical);

        Add_CurrentEP(-_DmgValue);
    }

    #endregion

    #region Die

    // 죽음
    protected override void Set_Die()
    {
        base.Set_Die();

        ThisAudioSource.Stop();

        Set_Die_GenItem();
        Set_Die_Effect();
        Set_Die_Data();
        Set_Die_Extra();
    }

    protected virtual void Set_Die_GenItem()
    {
        EnemyDropItemPercent genP = EnemyDropItemPercent;
        Gen_BS(Random.Range(
            genP.BSAmountMinMax.TypeBase, genP.BSAmountMinMax.TypeSpecial)); // 베터리 조각
        Gen_MS(Random.Range(
            genP.MSAmountMinMax.TypeBase, genP.MSAmountMinMax.TypeSpecial)); // 모듈 조각
        Gen_Credit(Random.Range(
            genP.CreditAmountMinMax.TypeBase, genP.CreditAmountMinMax.TypeSpecial)); // 크레딧
        Gen_Overrider(Random.Range(
            genP.OverriderAmountMinMax.TypeBase, genP.OverriderAmountMinMax.TypeSpecial)); // 오버라이더
        Gen_J(Random.Range(
            genP.JouleAmountMinMax.TypeBase, genP.JouleAmountMinMax.TypeSpecial)
            * PlayerManager.Instance.PlayerController.SpawnESMultiple.ActualState.Value); // 줄
        
        // Drop Module Item
        if (DevTool.Is_ChanceSuccess(genP.ModuleDropPercent))
            Gen_ModuleItem(DevTool.Get_Rank(genP.ModuleRankPercents));
        
        // Drop Keycard
        if (DevTool.Is_ChanceSuccess(genP.keycardDropPercent))
            Gen_KeycardItem(Random.Range(1, MainGameUIManager.Instance.Get_KindOfKeyCardAmount()));
    }

    private void Set_Die_Effect()
    {
        // Effect
        PlayerManager.Instance.CameraController.Play_KillAnim(_Dur: 0.2f);
        UnitManager.Instance.OnceTime_AnimGenerator.Anim_Attacked_BigSlice(TargetObject.transform.position);
        UnitManager.Instance.Enemy_ExplImgGenerator.Expl_Enemy(TargetObject.transform.position);
    }

    private void Set_Die_Data()
    {
        EndAll_Pattern();
        End_Nav();
        HUD.Reset_HUD();

        // Remove
        DevTool.Remove_InList(EnemyManager.Instance.CurrentEnemyList, this);
        DevTool.Remove_InList(LayerOrderManager.Instance.NeedSortingObjects, this);

        // Check Room State
        StageManager.Instance.Play_CompleteKillAll();

        // Ping
        if (PlayerManager.Instance.Is_PingedEnemy(this))
            PlayerManager.Instance.SetOff_PingEnemy();

        // Set
        this.gameObject.SetActive(false);
    }

    protected abstract void Set_Die_Extra();

    #endregion

    #region Sorting

    public override void Set_SortingOrder(int _SortingOrder)
    {
        ThisSG.sortingOrder = _SortingOrder;

        HUD.ThisCanvas.sortingOrder = _SortingOrder;

        PlayerManager.Instance.Set_SortingOrderPing(this, _SortingOrder);
    }

    #endregion

    #region Pattern

    protected void EndAll_Pattern()
    {
        if (CurrentEnemyPattern != null)
            CurrentEnemyPattern.End_Pattern();
        
        if (CurrentPatternCor != null)
            StopCoroutine(CurrentPatternCor);

        CurrentContinuousEnemyPattern = null;
        CurrentEnemyPattern = null;

        LookAtPoint = Vector2.zero;
        LookAtDir = Vector2.zero;
    }

    protected void Start_PatternFromNone()
    {
        OrderOfPriorityEnemyPatternList[OrderOfPriorityEnemyPatternList.Count - 1].EnemyPatternList[0].EnemyPatternList[0].Start_Pattern();
    }

    private int Get_NextPatternIndex()
    {
        int orderOfPattern = -1;
        if (CurrentEnemyPattern != null)
        {
            // 패턴의 순서 값을 저장해 활용
            orderOfPattern = CurrentContinuousEnemyPattern.EnemyPatternList.IndexOf(CurrentEnemyPattern);

            // 마지막 패턴 이었다면 (끝내기)
            if (orderOfPattern == CurrentContinuousEnemyPattern.EnemyPatternList.Count - 1)
            {
                CurrentEnemyPattern = null;
                CurrentContinuousEnemyPattern = null;
                orderOfPattern = -1;
            }
        }
        return orderOfPattern;
    }

    public virtual void Play_Pattern()
    {
        if (IsDead) return;

        if (TryPlay_ChargeStatePattern()) return;

        // 이미 있는지 진행 중인 패턴이 있는지 확인
        int orderOfPattern = Get_NextPatternIndex();

        if (orderOfPattern == -1) // 다음 패턴이 => 없다면 랜덤 패턴을 찾아서 실행.
        {
            Play_NewPattern();
        }
        else // => 있다면 다음 패턴을 찾아서 실행.
        {
            Play_NextPattern(orderOfPattern);
        }
    }

    private void Play_NewPattern()
    {
        for (int i = 0; i < OrderOfPriorityEnemyPatternList.Count; i++)
        {
            // 같은 우선도에 있는 패턴 랜덤으로 섞기
            List<ContinuousEnemyPattern> patternList = DevTool.Get_ShuffledList(OrderOfPriorityEnemyPatternList[i].EnemyPatternList);

            // 만약 사용 가능한 패턴이 있다면 시작
            for (int j = 0; j < patternList.Count; j++)
            {
                if (patternList[j].EnemyPatternList[0].Can_PlayPattern())
                {
                    CurrentContinuousEnemyPattern = patternList[j];
                    CurrentEnemyPattern = patternList[j].EnemyPatternList[0];
                    CurrentEnemyPattern.Start_Pattern();

                    return;
                }
            }
        }
    }

    private void Play_NextPattern(int _OrderOfPattern)
    {
        CurrentEnemyPattern = CurrentContinuousEnemyPattern.EnemyPatternList[_OrderOfPattern + 1];
        CurrentEnemyPattern.Start_Pattern();
    }

    private bool TryPlay_ChargeStatePattern()
    {
        if (IsFullCharge && !IsPlayingSpecialPattern) // 풀 차징 시
        {
            IsPlayingSpecialPattern = true;
            CurrentContinuousEnemyPattern = SpecialPattern;
            CurrentEnemyPattern = SpecialPattern.EnemyPatternList[0];
            CurrentEnemyPattern.Start_Pattern();

            return true;
        }

        return false;
    }

    #endregion
}