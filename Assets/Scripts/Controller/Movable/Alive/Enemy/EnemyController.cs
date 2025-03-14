using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class EnemyController : AliveObjectController, IInteract
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

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] public EnemyHUDController HUD;

    [Space(10)]
    [Header("=== Satellite")]
    [SerializeField] private EnemySolarController LookingSatellite;
    [SerializeField] private RigidbodySolarController WalkingSatellite;

    [Space(10)]
    [Header("=== Nav")]
    [Tooltip("This is Radius")]
    [SerializeField] public float NavRadius = 0.2f;

    [Space(10)]
    [Header("=== Pattern")]
    [Tooltip("This Order of Priority Equle Index")]
    [SerializeField] protected List<OrderOfPriorityEnemyPattern> OrderOfPriorityEnemyPatternList;
    [SerializeField] private ContinuousEnemyPattern CurrentContinuousEnemyPattern = null;
    

    [Space(10)]
    [Header("=== Discharge")]
    [SerializeField] private float RecoverDischargeTime = 4f;

    #endregion

    #region - Hide

    // 패턴
    [HideInInspector] private EnemyPattern CurrentEnemyPattern = null;

    // 움직임을 통제
    [HideInInspector] private GameObject Target;

    [HideInInspector] public float MoveSpeed;

    [HideInInspector] public Vector2 MoveAtPoint = Vector2.zero;
    [HideInInspector] public Vector2 MoveAtDir = Vector2.zero;

    [HideInInspector] public Vector2 LookAtPoint = Vector2.zero;
    [HideInInspector] public Vector2 LookAtDir = Vector2.zero;

    // 방전
    [HideInInspector] public bool IsDischarge = false;

    // 버프
    [HideInInspector] public EnemyBuffController BuffController = null;

    // 패턴
    [HideInInspector] public bool IsPlayingPattern = false;
    [HideInInspector] public IEnumerator CurrentPatternCor = null;

    // 방
    [HideInInspector] private RoomController CurrentRoomController;

    #endregion

    #endregion


    #region Offset

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

                if (_CurrentEP <= 0 && !IsDischarge)
                {
                    IsDischarge = true;
                    StartCoroutine(Play_RecoverDischarge_Cor());
                }
            });
    }

    protected override void Offset_Controller()
    {
        if (DevTool.Get_ComponentTType(this.gameObject, out EnemyBuffController buff))
        {
            BuffController = buff;
            BuffController.Offset(this);
        }
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        Reset_State();

        // VFX
        UnitManager.Instance.Enemy_ExplImgGenerator.Expl_Enemy(TargetObject.transform.position);
        
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

    #region Reset

    private void Reset_State()
    {
        base.IsDead = false;
        CurrentSP.Value = 0;
        CurrentHP.Value = MaxHP;
        CurrentEP.Value = MaxEP;
        IsDischarge = false;

        if (Target == null) // 타겟 Player
        { Target = PlayerManager.Instance.PlayerController.gameObject; }

        if (CurrentRoomController == null) // 현재 Room
        { CurrentRoomController = StageManager.Instance.CurrentRoomController; }

    }

    #endregion

    #region Movement

    private void Play_Movement(float _DeltaTime)
    {
        Update_MoveAtTarget();
        Play_Walk(MoveAtDir, MoveSpeed, _DeltaTime);
    }

    private void Update_MoveAtTarget()
    {
        if (IsDischarge)
        { return; }

        MoveAtDir = MoveAtPoint != Vector2.zero ?
            DevTool.Get_Dir(this.gameObject, MoveAtPoint) : Vector2.zero;
    }

    #endregion

    #region Look

    private void Update_LookAtTarget()
    {
        if (IsDischarge)
        { return; }

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
        if (!IsDischarge)
        {
            float esValue = 0;
            if (CurrentEP.Value > _DmgValue) // EP가 데미지보다 많다면
            {
                PoolingManager.Instance.Get_OP_DmgTxt().Offset_ByEnergyDmg(
                    (Vector2)TargetObject.transform.position + new Vector2(-0.2f, 0.2f),
                    _DmgValue, _IsCritical);

                esValue = _DmgValue;
                Add_CurrentEP(-_DmgValue);
            }
            else // EP가 데미지를 버티지 못한다면
            {
                // UI
                PoolingManager.Instance.Get_OP_DmgTxt().Offset_ByStateDischarge(
                    (Vector2)TargetObject.transform.position + new Vector2(0, 0.2f));

                esValue = CurrentEP.Value;
                CurrentEP.Value = 0;
            }

            // 에너지 조각 생성
            Gen_ES(esValue * PlayerManager.Instance.PlayerController.SpawnESMultiple.ActualState.Value);
        }
        else
        {
            // UI
            PoolingManager.Instance.Get_OP_DmgTxt().Offset_ByStateDischarge(
                (Vector2)TargetObject.transform.position + new Vector2(0, 0.2f));
        }
    }

    #endregion

    #region Die

    // 죽음
    protected override void Set_Die()
    {
        base.Set_Die();

        Set_Die_GenItem();
        Set_Die_Effect();
        Set_Die_Data();
    }

    private void Set_Die_GenItem()
    {
        // Drop Bettery Shard
        Gen_BS(1);

        // Drop Module Shard
        Gen_MS(1);

        // Drop Module Item
        if (DevTool.Is_ChanceSuccess(ItemDropPercent))
        {
            Gen_II();
        }
    }

    private void Set_Die_Effect()
    {
        // Effect
        PlayerManager.Instance.CameraController.Play_KillAnim(PlayerManager.Instance.PlayerController.ExecutionInterval);
        UnitManager.Instance.OnceTime_AnimGenerator.Anim_Attacked_BigSlice(TargetObject.transform.position);
        UnitManager.Instance.Enemy_ExplImgGenerator.Expl_Enemy(TargetObject.transform.position);
    }

    private void Set_Die_Data()
    {
        StopCoroutine(Play_RecoverDischarge_Cor());

        // Remove
        if (EnemyManager.Instance.CurrentEnemyList.Contains(this))
        { EnemyManager.Instance.CurrentEnemyList.Remove(this); }

        if (LayerOrderManager.Instance.NeedLayerObjects.Contains(this))
        { LayerOrderManager.Instance.NeedLayerObjects.Remove(this); }

        // Check Room State
        StageManager.Instance.Play_CompleteKillAll();

        // Set
        this.gameObject.SetActive(false);
        PoolingManager.Instance.Set_EnqueueEnemy(this);
    }

    #endregion

    #region Discharge

    private IEnumerator Play_RecoverDischarge_Cor()
    {
        // 패턴 루틴 종료
        SetOn_Discharge();

        yield return new WaitForSeconds(0.5f);

        HUD.StateUI.EP_ProgressBar.Set_FillFullImgSmooth(RecoverDischargeTime);

        yield return new WaitForSeconds(RecoverDischargeTime);

        CurrentEP.Value = MaxEP;
        IsDischarge = false;

        // 패턴 루틴 시작
        Start_PatternFromNone();
    }

    private void SetOn_Discharge()
    {
        StopCoroutine(CurrentPatternCor);

        CurrentContinuousEnemyPattern = null;
        CurrentEnemyPattern = null;

        MoveAtPoint = Vector2.zero;
        MoveAtDir = Vector2.zero;

        LookAtPoint = Vector2.zero;
        LookAtDir = Vector2.zero;

        MoveSpeed = 0f;
    }

    #endregion

    #region Interact

    public void Play_Interact()
    {
        // 처형
        PlayerManager.Instance.PlayerController.Try_Execution(this);
    }

    #endregion

    #region Execution

    public void Play_Execution()
    {
        PlayerManager.Instance.PlayerController.CurrentInteractable.Value = null;
        Set_Die();
    }

    #endregion

    #region UI

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);
        HUD.ThisCanvas.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Pattern

    private void Start_PatternFromNone()
    {
        OrderOfPriorityEnemyPatternList[OrderOfPriorityEnemyPatternList.Count - 1].EnemyPatternList[0].EnemyPatternList[0].Start_Pattern();
    }

    public void Play_Pattern()
    {
        if (IsDischarge)
        { return; }

        // 이미 있는지 진행 중인 패턴이 있는지 확인
        int OrderOfPattern = -1;
        if (CurrentEnemyPattern != null)
        {
            // 패턴의 순서 값을 저장해 활용
            OrderOfPattern = CurrentContinuousEnemyPattern.EnemyPatternList.IndexOf(CurrentEnemyPattern);
            // 마지막 패턴 이었다면 (끝내기)
            if (OrderOfPattern == CurrentContinuousEnemyPattern.EnemyPatternList.Count - 1)
            {
                CurrentEnemyPattern = null;
                CurrentContinuousEnemyPattern = null;
                OrderOfPattern = -1;
            }
        }

        // 없다면 랜덤 패턴을 찾아서 실행.
        if (OrderOfPattern == -1)
        {
            for (int i = 0; i < OrderOfPriorityEnemyPatternList.Count; i++)
            {
                // 같은 우선도에 있는 패턴 랜덤으로 섞기
                List<ContinuousEnemyPattern> epList = DevTool.Get_ShuffledList(OrderOfPriorityEnemyPatternList[i].EnemyPatternList);
                
                // 만약 사용 가능한 패턴이 있다면 시작
                for (int j = 0; j < epList.Count; j++)
                {
                    if (epList[j].EnemyPatternList[0].Can_PlayPattern())
                    {
                        CurrentContinuousEnemyPattern = epList[j];
                        CurrentEnemyPattern = epList[j].EnemyPatternList[0];

                        CurrentEnemyPattern.Start_Pattern();

                        return;
                    }
                }
            }
        }

        // 있다면 다음 패턴을 찾아서 실행.
        else
        {
            CurrentEnemyPattern = CurrentContinuousEnemyPattern.EnemyPatternList[OrderOfPattern + 1];

            CurrentEnemyPattern.Start_Pattern();
            return;
        }
    }


    #endregion




    #region Nav

    // 길 루트 찾기
    public List<WayPointController> Get_RootWay()
    {
        // 바로 갈 수 있다면
        if (!Is_ExistWall(PlayerManager.Instance.PlayerController.transform))
        {
            /*
#if UNITY_EDITOR
            Debug.DrawRay(this.transform.position,
                        (PlayerManager.Instance.PlayerController.transform.position - this.transform.position),
                        Color.red, 0.3f);
#endif
            */
            return new List<WayPointController> { PlayerManager.Instance.PlayerController.ThisWayPoint };
        }

        // 현재 방에 모든 WayPoint
        List<WayPointController> allWP = StageManager.Instance.CurrentRoomController.RoomRuleController.InRoom_AllWayPoint;

        // 이 객체와 플레이어에 가장 가까운 WayPoint 찾기
        List<List<WayPointController>> rootsFromEnemy = new List<List<WayPointController>> { new List<WayPointController> { Get_ClosetWP(this.transform, allWP) } };
        List<List<WayPointController>> rootsFromPlayer = new List<List<WayPointController>> { new List<WayPointController> { Get_ClosetWP(PlayerManager.Instance.PlayerController.transform, allWP) } };

        int checkOver = 0;
        bool IsEnemyExtensionTurn = true;
        while (true)
        {
            // 각 방향(적과 플레이어)의 끝 지점
            List<WayPointController> endRootPointsFromEnemy = Get_EndPoints(rootsFromEnemy);
            List<WayPointController> endRootPointsFromPlayer = Get_EndPoints(rootsFromPlayer);

            // 결과를 저장할 루트들
            List<List<WayPointController>> resultRoots = new List<List<WayPointController>>();

            // 두 끝 부분이 만난다면, 결과에 추가
            for (int i = 0; i < endRootPointsFromEnemy.Count; i++)
            {
                for (int j = 0; j < endRootPointsFromPlayer.Count; j++)
                {
                    if (endRootPointsFromEnemy[i] == endRootPointsFromPlayer[j])
                    {
                        resultRoots.Add(Get_Combine(rootsFromEnemy[i], rootsFromPlayer[j]));
                    }
                }
            }

            // 결과가 있다면, 결과 중 가장 짧은 루트 구하기
            if (resultRoots.Count > 0)
            {
                List<WayPointController> resultRoot = Get_ClosetRoot(resultRoots);
                resultRoot.Add(PlayerManager.Instance.PlayerController.ThisWayPoint);
                resultRoot = Get_RemoveUnnecessaryRoot(resultRoot);
                /*
#if UNITY_EDITOR
                Debug.DrawRay(this.transform.position,
                        (resultRoot[0].transform.position - this.transform.position),
                        Color.red, 0.3f);
                for (int i = 0; i < resultRoot.Count - 1; i++)
                {
                    Debug.DrawRay(resultRoot[i].transform.position, 
                        (resultRoot[i + 1].transform.position - resultRoot[i].transform.position), 
                        Color.red, 0.3f);
                }
#endif
                */
                return resultRoot;
            }

            // 루트 연장 (루트를 아직 못 찾음)
            if (IsEnemyExtensionTurn)
            { rootsFromEnemy = Get_ExtensionRoots(rootsFromEnemy); }
            else
            { rootsFromPlayer = Get_ExtensionRoots(rootsFromPlayer); }

            IsEnemyExtensionTurn = !IsEnemyExtensionTurn;

            if (++checkOver > 100)
            { break; }
        }


        return null;
    }

    // 루트를 연장하기
    private List<List<WayPointController>> Get_ExtensionRoots(List<List<WayPointController>> _Roots)
    {
        // 결과
        List<List<WayPointController>> extensionedRoots = new List<List<WayPointController>>();

        // 이미 포함하고 있는 WayPoint 판별을 위함
        List<WayPointController> rootSimpleList = DevTool.Remove_DuplicateInList(DevTool.Get_List(_Roots));

        for (int i = 0; i < _Roots.Count; i++)
        {
            // 마지막 끝부분 WP
            WayPointController lastWP = _Roots[i][_Roots[i].Count - 1];
            for (int j = 0; j < lastWP.AdjacentWPList.Count; j++)
            {
                // 마지막 끝부분 WP에 인접한 WP가 현재 루트에 있지않다면 추가
                if (!rootSimpleList.Contains(lastWP.AdjacentWPList[j]))
                {
                    List<WayPointController> addRoot = new List<WayPointController>();
                    addRoot.AddRange(_Roots[i]);
                    addRoot.Add(lastWP.AdjacentWPList[j]);

                    extensionedRoots.Add(addRoot);
                }
            }
        }

        return extensionedRoots;
    }

    // 가장 짧은 루트 구하기
    private List<WayPointController> Get_ClosetRoot(List<List<WayPointController>> _ResultRoots)
    {
        List<WayPointController> closetRoot = _ResultRoots[0];
        float closetDis = Get_RootDistance(_ResultRoots[0]);

        for (int i = 0; i < _ResultRoots.Count; i++)
        {
            float tempDis = Get_RootDistance(_ResultRoots[i]);
            if (closetDis > tempDis)
            {
                closetRoot = _ResultRoots[i];
                closetDis = tempDis;
            }
        }

        return closetRoot;
    }

    // 루트 중 직접 갈 수 있는 부분 중복 된다면 삭제
    private List<WayPointController> Get_RemoveUnnecessaryRoot(List<WayPointController> _Root)
    {
        List<WayPointController> resultRoot = new List<WayPointController>();
        bool NeedInit = false;
        for (int i = 0; i < _Root.Count; i++)
        {
            if (Is_ExistWall(_Root[i].ThisTF)&& 
                !NeedInit)
            {
                if (i != 0)
                { resultRoot.Add(_Root[i - 1]); }
                NeedInit = true;
            }

            if (NeedInit)
            {
                resultRoot.Add(_Root[i]);
            }
        }
        return resultRoot;
    }

    // 한 루트의 길이 구하기
    private float Get_RootDistance(List<WayPointController> _Roots)
    {
        float resultDis = 0;
        for (int i = 0; i < _Roots.Count - 1; i++)
        {
            resultDis += Vector2.Distance(_Roots[i].ThisTF.position, _Roots[i + 1].ThisTF.position);
        }
        return resultDis;
    }

    // 두 객체 루트를 연결한 루트
    private List<WayPointController> Get_Combine(List<WayPointController> _WayFromEnemy, List<WayPointController> _WayFromPlayer)
    {
        List<WayPointController> combinedRoot = new List<WayPointController>();

        // 한쪽 끝 지우기
        List<WayPointController> deletedLastOneWayFromEnemy = new List<WayPointController>();
        deletedLastOneWayFromEnemy = _WayFromEnemy.ToList();
        deletedLastOneWayFromEnemy.Remove(deletedLastOneWayFromEnemy[deletedLastOneWayFromEnemy.Count - 1]);

        // 뒤집기 (한쪽)
        List<WayPointController> reverseWayFromPlayer = Enumerable.Reverse(_WayFromPlayer).ToList();

        // 리스트 합친 결과
        combinedRoot.AddRange(deletedLastOneWayFromEnemy);
        combinedRoot.AddRange(reverseWayFromPlayer);

        return combinedRoot;
    }

    // 끝 부분 WayPoint 구하기
    private List<WayPointController> Get_EndPoints(List<List<WayPointController>> _Roots)
    {
        List<WayPointController> endPoints = new List<WayPointController>();
        for (int i = 0; i < _Roots.Count; i++)
        {
            endPoints.Add(_Roots[i][_Roots[i].Count - 1]);
        }

        return endPoints;
    }

    // 가장 가까운 WayPoint
    private WayPointController Get_ClosetWP(Transform transform, List<WayPointController> _AllWP)
    {
        return DevTool.Get_CastingTType<WayPointController>(DevTool.Get_ClosetGO(DevTool.Get_ConvertTTypeList<WayPointController, GameObject>(_AllWP), transform.gameObject));
    }

    // 이 객체로부터 특정 지점까지 벽이 있는지
    public bool Is_ExistWall(Transform _TargetTF)
    {
        return DevTool.Is_Exist_UseCircle(this.transform, _TargetTF, "Wall", NavRadius);
    }

    #endregion

}