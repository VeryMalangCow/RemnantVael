using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : AliveObjectController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private eEnemy ThisEnemyType;
    [SerializeField] private float MaxHP;
    [SerializeField] private float MaxEP;

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] private float ItemDropPercent = 0.0f;
    [SerializeField] private List<float> ItemRankPercents;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] public EnemyHUDController HUD;

    [Space(10)]
    [Header("=== Satellite")]
    [SerializeField] private EnemySolarController LookingSatellite;
    [SerializeField] private RigidbodySolarController WalkingSatellite;

    [Space(10)]
    [Header("=== Ping Data")]
    [SerializeField] public Vector2 PingOffsetVec;
    [SerializeField] public Vector2 PingSizeVec;

    [Space(10)]
    [Header("=== Nav")]
    [SerializeField] private NavMeshAgent ThisNavMeshAgent;

    [Space(10)]
    [Header("=== Pattern")]
    [Tooltip("This Order of Priority Equle Index")]
    [SerializeField] protected List<OrderOfPriorityEnemyPattern> OrderOfPriorityEnemyPatternList;
    

    #endregion

    #region - Hide

    // 패턴
    [HideInInspector] private EnemyPattern CurrentEnemyPattern = null;

    // 움직임을 통제
    [HideInInspector] private GameObject Target;

    [HideInInspector] public Vector2 LookAtPoint = Vector2.zero;
    [HideInInspector] public Vector2 LookAtDir = Vector2.zero;

    // 버프
    [HideInInspector] public EnemyBuffController BuffController = null;

    // 패턴
    [HideInInspector] private ContinuousEnemyPattern CurrentContinuousEnemyPattern = null;
    [HideInInspector] public bool IsPlayingPattern = false;
    [HideInInspector] public IEnumerator CurrentPatternCor = null;

    // 방
    [HideInInspector] private RoomController CurrentRoomController;

    // 죽음
    [HideInInspector] private eDamageType DieStateType;

    // 사운드
    [HideInInspector] private AudioSource ThisAudioSource;

    #endregion

    #endregion


    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_Nav();
    }

    protected override void Offset_FirstSetting()
    {
        HUD.Offset(this);
    }

    protected override void Offset_Subscribe()
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

    protected override void Offset_Controller()
    {
        if (DevTool.Get_ComponentTType(this.gameObject, out EnemyBuffController buff))
        {
            BuffController = buff;
            BuffController.Offset(this);
        }

        ThisAudioSource = DevTool.Get_ComponentTType<AudioSource>(gameObject);
    }

    private void Offset_Nav()
    {
        ThisNavMeshAgent.updateRotation = false;
        ThisNavMeshAgent.updateUpAxis = false;
        ThisNavMeshAgent.enabled = false;
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        Reset_State();

        DevTool.Add_InList(EnemyManager.Instance.CurrentEnemyList, this);

        // Pattern
        Start_PatternFromNone();
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
        Play_Walk(Vector2.zero, 0, _DeltaTime);
    }

    #endregion

    #region Reset

    private void Reset_State()
    {
        base.IsDead = false;
        CurrentSP.Value = 0;
        CurrentHP.Value = MaxHP;
        CurrentEP.Value = MaxEP;

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
        Add_CurrentEP(_AddValue, MaxEP);
        Check_IsDead(CurrentEP.Value);
    }

    public float Get_PercentHP(float _Percent)
    {
        return DevTool.Get_Percent(_Percent, MaxHP);
    }

    #endregion

    #region Hitted

    // 총알 데미지
    public void Try_Hitted(PlayerBulletController _Bullet)
    {
        if (IsDead)
        { return; }

        BulletState state = _Bullet.State;

        // Damage
        Take_Damaged(
            state, 
            state.IsCritical, 
            DevTool.Get_DirFromAngle(_Bullet.transform.eulerAngles.z));
    }

    // 어택커 데미지
    public void Try_Hitted(PlayerAttackerController _Attacker)
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

    #endregion

    #region Damaged

    // 데미지, 넉백, 크리티컬, 모듈 호과 등
    private void Take_Damaged(CombatState _State_Combat, bool _IsCritical, Vector2 _DirKB)
    {
        float actualDmg = _State_Combat.DmgState.Dmg;

        // INTERFACE: 맞을 때 효과 
        ModuleItemManager.Instance.Active_Hit(this); 

        // KB
        if (_State_Combat.KnockbackState.CanKB)
        {
            Gain_Knockback(new CurrentKnockbackState(_DirKB, _State_Combat.KnockbackState.KBPower, _State_Combat.KnockbackState.KBTime));
        }
        // 치명타 계산
        if (_IsCritical)
        {
            actualDmg *= _State_Combat.CriticalState.CD;
            ModuleItemManager.Instance.Active_CriticalHit(this); // INTERFACE: 치명타를 맞을 때 효과 
        }

        // 데미지 구현 (Dmg: 적의 부식 디버프 계산)
        Take_Damage(DevTool.Get_DmgEffectByCorrosion(actualDmg, BuffController),
            _State_Combat.DmgState.DmgType,
            _IsCritical);

        // 사운드
        if (!IsDead)
        { SoundManager.Instance.Play_2D_SFX(ThisAudioSource, "Enemy_Hitted"); }
        else
        { SoundManager.Instance.Play_2D_SFX("Enemy_Hitted"); }
    }

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
        if (CurrentEP.Value <= 0) 
            DieStateType = eDamageType.Energy;
        else 
            DieStateType = eDamageType.Physics;

        ThisAudioSource.Stop();

        Set_Die_GenItem();
        Set_Die_Effect();
        Set_Die_Data();
    }

    private void Set_Die_GenItem()
    {
        Gen_BS(1);
        Gen_MS(1);
        Gen_Credit(10);

        if (DieStateType == eDamageType.Physics)
            Gen_Overrider(1);
        else
            Gen_J(10 * PlayerManager.Instance.PlayerController.SpawnESMultiple.ActualState.Value);
        

        // Drop Module Item
        if (DevTool.Is_ChanceSuccess(ItemDropPercent))
        {
            Gen_II(DevTool.Get_Rank(ItemRankPercents));
        }
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
        PoolingManager.Instance.Set_EnqueueEnemy(this);
    }

    #endregion

    #region Sorting

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);
        HUD.ThisCanvas.sortingOrder = _SortingOrder;

        PlayerManager.Instance.Set_SortingOrderPing(this, _SortingOrder);
    }

    #endregion

    #region Pattern

    private void EndAll_Pattern()
    {
        if (CurrentEnemyPattern != null) CurrentEnemyPattern.End_Pattern();
        StopCoroutine(CurrentPatternCor);

        CurrentContinuousEnemyPattern = null;
        CurrentEnemyPattern = null;

        LookAtPoint = Vector2.zero;
        LookAtDir = Vector2.zero;
    }

    private void Start_PatternFromNone()
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

    public void Play_Pattern()
    {
        if (IsDead) return; 

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

    #endregion

    #region Nav
    
    public void Start_Nav(float _FollowSpeed)
    {
        ThisNavMeshAgent.speed = _FollowSpeed;
        ThisNavMeshAgent.enabled = true;
    }

    public void Get_NavPos(Transform _TargetTF)
    {
        ThisNavMeshAgent.SetDestination(_TargetTF.position);
    }

    public void End_Nav()
    {
        ThisNavMeshAgent.enabled = false;
    }

    public bool Is_ExistWall(Transform _TargetTF)
    {
        return DevTool.Is_Exist_UseLine(this.transform, _TargetTF, "Wall");
    }

    #endregion
}