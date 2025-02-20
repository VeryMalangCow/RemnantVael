using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class EnemyController : MovableObject, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] public int EnemyID;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private eEnemy ThisEnemyType;
    [SerializeField] private float MaxHP;
    [HideInInspector] private ReactiveProperty<float> CurrentHP = new();
    [HideInInspector] private ReactiveProperty<float> CurrentSP = new();

    [SerializeField] private float MaxEP;
    [HideInInspector] private ReactiveProperty<float> CurrentEP = new();
    [HideInInspector] public bool IsDischarge = false;
    [SerializeField] private float RecoverLethargyTime = 4f;

    [SerializeField] private float ItemDropPercent = 0.0f;

    [Space(10)]
    [Header("=== Buff")]
    [HideInInspector] public EnemyBuffController BuffController = null;

    [Space(10)]
    [Header("=== Movement")]
    [SerializeField] private eMovementState MovementState;
    [SerializeField] private GameObject Target;
    [HideInInspector] public Vector2 MoveTargetPoint = Vector2.zero;
    [HideInInspector] public Vector2 LookTargetPoint = Vector2.zero;
    [HideInInspector] protected Vector2 LookAtDir = Vector2.zero;
    [SerializeField] public Vector2 MoveDir;
    [SerializeField] public float MoveSpeed;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] public EnemyHUDController HUD;

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] public MakeExplosionImage MEI;
    [SerializeField] public Material ThisSmokeM;
    [SerializeField] public AnimationClip HittedAC_0;
    [SerializeField] public AnimationClip HittedAC_1;
    [SerializeField] public AnimationClip HittedAC_2;

    [HideInInspector] protected RoomController CurrentRoomController;

    [Space(10)]
    [Header("=== Nav")]
    [Tooltip("This is Radius")]
    [SerializeField] private float NavRadius = 0.2f;


    [Space(10)]
    [Header("=== Pattern")]
    [Tooltip("This Order of Priority Equle Index")]
    [SerializeField] protected List<OrderOfPriorityEnemyPattern> OrderOfPriorityEnemyPatternList;
    [SerializeField] public ContinuousEnemyPattern CurrentContinuousEnemyPattern = null;
    [SerializeField] public EnemyPattern CurrentEnemyPattern = null;
    [SerializeField] public bool IsPlayingPattern = false;
    [HideInInspector] public IEnumerator CurrentPatternCor = null;


    #endregion

    #region Fremework

    private void Offset()
    {
        HUD.Offset(this);

        if (this.gameObject.TryGetComponent(out EnemyBuffController EBC))
        {
            BuffController = EBC;
            BuffController.Offset(this);
        }

        CurrentSP
            .Subscribe(_CurrentSP =>
            {
                HUD.StateUI.SP_ProgressBar.SetFillImgSmooth(CurrentSP.Value, MaxHP);

                if (CurrentSP.Value <= 0)
                {
                    HUD.StateUI.SP_ProgressBar.SetNoNum();
                    HUD.StateUI.HP_ProgressBar.SetFillImgSmooth(CurrentHP.Value, MaxHP);
                    HUD.StateUI.EP_ProgressBar.SetFillImgSmooth(CurrentEP.Value, MaxEP);

                    if (BuffController.ShieldBuff.IsOn)
                    { BuffController.ShieldBuff.RemoveAllStack(); }
                }
                else
                {
                    HUD.StateUI.HP_ProgressBar.SetNoNum();
                    HUD.StateUI.EP_ProgressBar.SetNoNum();
                }
            });

        CurrentHP
            .Subscribe(_CurrentHP =>
            {
                HUD.StateUI.HP_ProgressBar.SetFillImgSmooth(CurrentHP.Value, MaxHP);

                if (CurrentSP.Value > 0)
                { HUD.StateUI.HP_ProgressBar.SetNoNum(); }
            });

        CurrentEP
            .Subscribe(_CurrentEP =>
            {
                HUD.StateUI.EP_ProgressBar.SetFillImgSmooth(CurrentEP.Value, MaxEP);

                if (CurrentSP.Value > 0)
                { HUD.StateUI.EP_ProgressBar.SetNoNum(); }
            });

    }

    private void Start()
    {
        Offset();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        base.IsDead = false;
        CurrentSP.Value = 0;
        CurrentHP.Value = MaxHP;
        CurrentEP.Value = MaxEP;

        if (Target == null)
        { Target = PlayerManager.Instance.PlayerController.gameObject; }

        if (CurrentRoomController == null)
        { CurrentRoomController = StageManager.Instance.CurrentRoomController; }

        ExplosionEffect();
        StartPatternFromNone();
    }

    protected void FixedUpdate()
    {
        Movement();
        LookAtTarget();
    }

    #endregion

    #region Movement

    private void Movement()
    {
        switch (MovementState)
        {
            case eMovementState.IdleOrWalk:
                if (MoveTargetPoint != Vector2.zero)
                {
                    MoveDir = (MoveTargetPoint - (Vector2)this.transform.position).normalized;

                }
                Walk(MoveDir, MoveSpeed, AccelerationSpeed);
                break;

            default: break;
        }
    }


    #endregion

    #region Look At

    private void LookAtTarget()
    {
        if (IsDischarge)
        { return; }

        // 바라볼 타겟이 있다면
        if (LookTargetPoint != Vector2.zero && Target != null)
        {
            LookAtDir = ((Vector2)Target.transform.position - (Vector2)this.transform.position).normalized;
        }
        // 바라볼 타겟이 없다면
        else if (LookAtDir != Vector2.zero && LookTargetPoint == Vector2.zero)
        {
            LookAtDir = Vector2.zero;
        }
    }

    #endregion

    #region Life State Gain

    public void SetPercentSP(float _Value)
    {
        float actualValue = (_Value / 100f) * MaxHP;
        
        if (actualValue > CurrentSP.Value)
        {
            SetSP(actualValue);
        }
    }

    public void GainPercentSP(float _Value)
    {
        float actualValue = (_Value / 100f) * MaxHP;
        GainSP(actualValue);
    }
    public void GainPercentHP(float _Value)
    {
        float actualValue = (_Value / 100f) * MaxHP;
        GainHP(actualValue);
    }
    public void GainPercentEP(float _Value)
    {
        float actualValue = (_Value / 100f) * MaxEP;
        GainEP(actualValue);
    }


    private void GainSP(float _Value)
    { GainPoint(CurrentSP, MaxHP, _Value); }

    private void GainHP(float _Value)
    { GainPoint(CurrentHP, MaxHP, _Value); }

    private void GainEP(float _Value)
    { GainPoint(CurrentEP, MaxEP, _Value); }


    private void SetSP(float _Value)
    { SetPoint(CurrentSP, MaxHP, _Value); }
    private void SetHP(float _Value)
    { SetPoint(CurrentHP, MaxHP, _Value); }
    private void SetEP(float _Value)
    { SetPoint(CurrentEP, MaxEP, _Value); }


    private void SetPoint(ReactiveProperty<float> _Variable, float _Max, float _Value)
    { _Variable.Value = Mathf.Clamp(_Value, 0, _Max); }

    private void GainPoint(ReactiveProperty<float> _Variable, float _Max, float _Value)
    { _Variable.Value = Mathf.Clamp(_Variable.Value + _Value, 0, _Max); }

    #endregion

    #region Damaged

    // 총알 데미지
    public void TakeDamaged(BulletState _BS, Vector2 _KnockbackDir)
    {
        if (base.IsDead)
        { return; }

        // Damage
        TakeDamaged(
            _BS.DamageType, 
            _BS.AbleKnockback, _KnockbackDir, _BS.KnockbackPower, _BS.KnockbackTime,
            _BS.IsCritical, _BS.CD, 
            _BS.BaseDamage);
    }

    // 어택커 데미지
    public void TakeDamaged(AttackerState _AS, bool _IsCritical, Vector2 _KnockbackDir)
    {
        if (base.IsDead)
        { return; }

        // Damage
        TakeDamaged(
            _AS.DamageType, 
            _AS.AbleKnockback, _KnockbackDir, _AS.KnockbackPower, _AS.KnockbackTime,
            _IsCritical, _AS.CD, 
            _AS.BaseDamage);
    }


    // 데미지만을 계산하는 방식
    private void TakeDamaged(
        eDamageType _DamageType, 
        bool _AbleKnockback, Vector2 _KnockbackDir, float _KnockbackPower, float _KnockbackTime,
        bool _IsCritical, float _CriticalDMG, 
        float _ActualDMG)
    {
        ModuleItemManager.Instance.Active_Hit(this); // INTERFACE: 맞을 때 효과 

        // Knockback
        if (_AbleKnockback)
        {
            //Debug.DrawRay((Vector2)this.transform.position, _KnockbackDir, Color.red, 5f);
            GetKnockback(new KnockbackState(_KnockbackDir, _KnockbackPower, _KnockbackTime));
        }

        // 치명타 계산
        if (_IsCritical)
        {
            _ActualDMG *= _CriticalDMG;
            ModuleItemManager.Instance.Active_CriticalHit(this); // INTERFACE: 치명타를 맞을 때 효과 
        }

        // 적이 부식 디버프에 걸린지
        _ActualDMG *= (1f + 
            (BuffController.CorrosionStack.CurrentStack * (BuffController.DecayStack.CurrentStack + 1) * 0.01f));

        // 쉴드 계산
        _ActualDMG = TakeShieldDamaged(_IsCritical, _ActualDMG);
        if (_ActualDMG <= 0)
        { return; }

        // 직접 데미지
        if (_DamageType == eDamageType.Physics) // 물리 값
        {
            TakePhysicsDamaged(_IsCritical, _ActualDMG);
        }
        else // 에너지 값
        {
            TakeEnergyDamaged(_IsCritical, _ActualDMG, true);
        }
    }


    // 쉴드 데미지를 받음
    private float TakeShieldDamaged(bool _IsCritical, float _ActualDMG)
    {
        // 쉴드 계산

        if (CurrentSP.Value > 0)
        {
            if (CurrentSP.Value > _ActualDMG)
            {
                // UI
                PoolingManager.Instance.GetOP_DmgTxt().OffsetByShieldDmg(
                    (Vector2)TargetObject.transform.position + new Vector2(0.2f, 0.2f),
                    _ActualDMG, _IsCritical);

                GainSP(-_ActualDMG);
                _ActualDMG = 0;
            }
            else
            {
                // UI
                PoolingManager.Instance.GetOP_DmgTxt().OffsetByShieldDmg(
                    (Vector2)TargetObject.transform.position + new Vector2(0.2f, 0.2f),
                    CurrentSP.Value, _IsCritical);

                _ActualDMG -= CurrentSP.Value;
                CurrentSP.Value = 0f;
            }
        }

        return _ActualDMG;
    }

    // 물리 데미지를 받음
    private void TakePhysicsDamaged(bool _IsCritical, float _ActualDMG)
    {
        // UI
        PoolingManager.Instance.GetOP_DmgTxt().OffsetByPhysicDmg(
        (Vector2)TargetObject.transform.position + new Vector2(-0.2f, 0.2f),
        _ActualDMG, _IsCritical);

        SetIsDead(CurrentHP.Value, _ActualDMG);
        GainHP(-_ActualDMG);
        if (CurrentHP.Value <= 0f)
        {
            base.IsDead = true;
            Die();
        }
    }

    // 에너지 데미지를 받음
    private void TakeEnergyDamaged(bool _IsCritical, float _ActualDMG, bool _SpawnES)
    {
        if (!IsDischarge)
        {
            // UI
            PoolingManager.Instance.GetOP_DmgTxt().OffsetByEnergyDmg(
                (Vector2)TargetObject.transform.position + new Vector2(-0.2f, 0.2f),
                _ActualDMG, _IsCritical);

            float targetValue = 0;
            if (CurrentEP.Value > _ActualDMG) // EP가 데미지보다 많다면
            {
                targetValue = _ActualDMG;
                GainEP(-_ActualDMG);
            }
            else // EP가 데미지를 버티지 못한다면
            {
                // UI
                PoolingManager.Instance.GetOP_DmgTxt().OffsetByStateStun(
                        (Vector2)TargetObject.transform.position + new Vector2(0, 0.2f));

                targetValue = CurrentEP.Value;
                CurrentEP.Value = 0;
                IsDischarge = true;
                StartCoroutine(RecoverLethargy());
            }

            // 에너지 조각 생성
            if (_SpawnES)
            {
                targetValue *= PlayerManager.Instance.PlayerController.SpawnESMultiple.ActualState.Value;
                SpawnES(targetValue);
            }
        }
        else
        {
            // UI
            PoolingManager.Instance.GetOP_DmgTxt().OffsetByStateStun(
                (Vector2)TargetObject.transform.position + new Vector2(0, 0.2f));
        }

        
    }


    // 추가 효과가 없는 데미지 계산
    public void TakeDamaged_NoneExtraEffect(eDamageType _DmgType, float _ActualDMG)
    {
        // 쉴드 계산
        _ActualDMG = TakeShieldDamaged(false, _ActualDMG);
        if (_ActualDMG <= 0)
        { return; }

        // 직접 계산
        if (_DmgType == eDamageType.Physics)
        {
            TakePhysicsDamaged(false, _ActualDMG);
        }
        else
        {
            TakeEnergyDamaged(false, _ActualDMG, false);
        }
    }


    // 죽음
    private void Die()
    {
        // Drop Bettery S
        SpawnBS(1);

        // Drop Module S
        SpawnMS(1);

        // Drop Item
        SpawnII();

        // Effect
        StopCoroutine(RecoverLethargy());
        PlayerManager.Instance.CameraController.PlayKillAnim(PlayerManager.Instance.PlayerController.ExecutionInterval);

        // Remove
        if (EnemyManager.Instance.CurrentEnemyList.Contains(this))
        { EnemyManager.Instance.CurrentEnemyList.Remove(this); }

        if (LayerOrderManager.Instance.NeedLayerObjects.Contains(this))
        { LayerOrderManager.Instance.NeedLayerObjects.Remove(this); }

        // Check Room State
        StageManager.Instance.Complete_KillAll();

        // Set
        this.gameObject.SetActive(false);
        PoolingManager.Instance.EnqueueEnemy(this);

        // Effect
        DieEffect();
    }

    #endregion

    #region Spawn Item

    // Interactable Item
    private void SpawnII()
    {
        if (ItemDropPercent < UnityEngine.Random.Range(0f, 1f))
        { return; }

        InteractItemController IIC = PoolingManager.Instance.GetOP_InteractableItem();
        IIC.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        IIC.SetState(this.transform.position, 1, 1);
    }

    // Energy Shrapnel
    private void SpawnES(float _Value)
    {
        EnergyShrapnelController ESC = PoolingManager.Instance.GetOP_EnergyShrapnel();
        ESC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            _Value);
        ESC.gameObject.SetActive(true);
    }

    #endregion

    #region State

    private IEnumerator RecoverLethargy()
    {
        // 패턴 루틴 종료
        StopPattern();

        yield return new WaitForSeconds(0.5f);

        HUD.StateUI.EP_ProgressBar.SetFillFullImgSmooth(RecoverLethargyTime);

        yield return new WaitForSeconds(RecoverLethargyTime);

        CurrentEP.Value = MaxEP;
        IsDischarge = false;

        // 패턴 루틴 시작
        StartPatternFromNone();
    }

    private void StopPattern()
    {
        StopCoroutine(CurrentPatternCor);

        CurrentContinuousEnemyPattern = null;
        CurrentEnemyPattern = null;

        MoveTargetPoint = Vector2.zero;
        LookTargetPoint = Vector2.zero;
        MoveDir = Vector2.zero;
        MoveSpeed = 0f;
    }

    #endregion

    #region Interact

    public void Interact()
    {
        Die();
    }

    #endregion

    #region Nav

    // 길 루트 찾기
    public List<WayPoint> FindWay()
    {
        // 바로 갈 수 있다면
        if (!IsExistWall(this.transform, PlayerManager.Instance.PlayerController.transform))
        {
            /*
#if UNITY_EDITOR
            Debug.DrawRay(this.transform.position,
                        (PlayerManager.Instance.PlayerController.transform.position - this.transform.position),
                        Color.red, 0.3f);
#endif
            */
            return new List<WayPoint> { PlayerManager.Instance.PlayerController.ThisWayPoint };
        }    

        // 현재 방에 모든 WayPoint
        List<WayPoint> allWP = StageManager.Instance.CurrentRoomController.RoomRuleController.InRoom_AllWayPoint;

        // 이 객체와 플레이어에 가장 가까운 WayPoint 찾기
        List<List<WayPoint>> rootsFromEnemy = new List<List<WayPoint>> { new List<WayPoint> { GetClosetWP(this.transform, allWP) } };
        List<List<WayPoint>> rootsFromPlayer = new List<List<WayPoint>> { new List<WayPoint> { GetClosetWP(PlayerManager.Instance.PlayerController.transform, allWP) } };

        int checkOver = 0;
        bool IsEnemyExtensionTurn = true;
        while (true)
        {
            // 각 방향(적과 플레이어)의 끝 지점
            List<WayPoint> endRootPointsFromEnemy = GetEndPoints(rootsFromEnemy);
            List<WayPoint> endRootPointsFromPlayer = GetEndPoints(rootsFromPlayer);

            // 결과를 저장할 루트들
            List<List<WayPoint>> resultRoots = new List<List<WayPoint>>();

            // 두 끝 부분이 만난다면, 결과에 추가
            for (int i = 0; i < endRootPointsFromEnemy.Count; i++)
            {
                for (int j = 0; j < endRootPointsFromPlayer.Count; j++)
                {
                    if (endRootPointsFromEnemy[i] == endRootPointsFromPlayer[j])
                    {
                        resultRoots.Add(GetCombine(rootsFromEnemy[i], rootsFromPlayer[j]));
                    }
                }
            }

            // 결과가 있다면, 결과 중 가장 짧은 루트 구하기
            if (resultRoots.Count > 0)
            {
                List<WayPoint> resultRoot = GetClosetRoot(resultRoots);
                resultRoot.Add(PlayerManager.Instance.PlayerController.ThisWayPoint);
                resultRoot = GetRemoveUnnecessaryRoot(resultRoot);
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
            { rootsFromEnemy = GetExtensionRoots(rootsFromEnemy); }
            else
            { rootsFromPlayer = GetExtensionRoots(rootsFromPlayer); }

            IsEnemyExtensionTurn = !IsEnemyExtensionTurn;

            if (++checkOver > 100)
            { break; }
        }


        return null;
    }

    // 루트를 연장하기
    private List<List<WayPoint>> GetExtensionRoots(List<List<WayPoint>> _Roots)
    {
        // 결과
        List<List<WayPoint>> extensionedRoots = new List<List<WayPoint>>();

        // 이미 포함하고 있는 WayPoint 판별을 위함
        List<WayPoint> rootSimpleList = GetNormalList<WayPoint>(_Roots);


        for (int i = 0; i < _Roots.Count; i++)
        {
            // 마지막 끝부분 WP
            WayPoint lastWP = _Roots[i][_Roots[i].Count - 1];
            for (int j = 0; j < lastWP.AdjacentWPList.Count; j++)
            {
                // 마지막 끝부분 WP에 인접한 WP가 현재 루트에 있지않다면 추가
                if (!rootSimpleList.Contains(lastWP.AdjacentWPList[j]))
                {
                    List<WayPoint> addRoot = new List<WayPoint>();
                    addRoot.AddRange(_Roots[i]);
                    addRoot.Add(lastWP.AdjacentWPList[j]);

                    extensionedRoots.Add(addRoot);
                }
            }
        }

        return extensionedRoots;
    }

    // 가장 짧은 루트 구하기
    private List<WayPoint> GetClosetRoot(List<List<WayPoint>> _ResultRoots)
    {
        List<WayPoint> closetRoot = _ResultRoots[0];
        float closetDis = GetRootDistance(_ResultRoots[0]);

        for (int i = 0; i < _ResultRoots.Count; i++) 
        {
            float tempDis = GetRootDistance(_ResultRoots[i]);
            if (closetDis > tempDis)
            {
                closetRoot = _ResultRoots[i];
                closetDis = tempDis;
            }
        }

        return closetRoot;
    }

    // 루트 중 직접 갈 수 있는 부분 중복 된다면 삭제
    private List<WayPoint> GetRemoveUnnecessaryRoot(List<WayPoint> _Root)
    {
        List<WayPoint> resultRoot = new List<WayPoint>();
        bool NeedInit = false;
        for (int i = 0; i < _Root.Count; i++)
        {
            if (IsExistWall(this.transform, _Root[i].ThisTF) && !NeedInit)
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
    private float GetRootDistance(List<WayPoint> _Roots)
    {
        float resultDis = 0;
        for (int i = 0; i < _Roots.Count - 1; i++)
        {
            resultDis += Vector2.Distance(_Roots[i].ThisTF.position, _Roots[i + 1].ThisTF.position);
        }
        return resultDis;
    }

    // 두 객체 루트를 연결한 루트
    private List<WayPoint> GetCombine(List<WayPoint> _WayFromEnemy, List<WayPoint> _WayFromPlayer)
    {
        List<WayPoint> combinedRoot = new List<WayPoint>();

        // 한쪽 끝 지우기
        List<WayPoint> deletedLastOneWayFromEnemy = new List<WayPoint>();
        deletedLastOneWayFromEnemy = _WayFromEnemy.ToList();
        deletedLastOneWayFromEnemy.Remove(deletedLastOneWayFromEnemy[deletedLastOneWayFromEnemy.Count - 1]);

        // 뒤집기 (한쪽)
        List<WayPoint> reverseWayFromPlayer = Enumerable.Reverse(_WayFromPlayer).ToList();

        // 리스트 합친 결과
        combinedRoot.AddRange(deletedLastOneWayFromEnemy);
        combinedRoot.AddRange(reverseWayFromPlayer);

        return combinedRoot;
    }

    // 끝 부분 WayPoint 구하기
    private List<WayPoint> GetEndPoints(List<List<WayPoint>> _Roots)
    {
        List<WayPoint> endPoints = new List<WayPoint>();
        for (int i = 0; i < _Roots.Count; i++)
        {
            endPoints.Add(_Roots[i][_Roots[i].Count - 1]);
        }

        return endPoints;
    }

    // 가장 가까운 WayPoint
    private WayPoint GetClosetWP(Transform transform, List<WayPoint> _AllWP)
    {
        WayPoint closetWP = _AllWP[0];
        float closetDis = Vector2.Distance(closetWP.ThisTF.position, transform.position);

        for (int i = 1; i < _AllWP.Count; i++)
        {
            float tempDis = Vector2.Distance(_AllWP[i].ThisTF.position, transform.position);
            if (closetDis > tempDis)
            {
                closetWP = _AllWP[i];
                closetDis = tempDis;
            }
        }

        return closetWP;
    }

    // 중간에 벽이 있는지
    public bool IsExistWall(Transform _StartTF, Transform _EndTF)
    {
        Vector2 dirVec = _EndTF.position - _StartTF.position;
        RaycastHit2D hit = Physics2D.CircleCast(_StartTF.position, NavRadius, dirVec, Vector2.Distance(Vector2.zero, dirVec), LayerMask.GetMask("Wall"));

        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 이중 List를 단순 List로 변경
    private List<T> GetNormalList<T>(List<List<T>> _doubleListType)
    {
        List<T> result = new List<T>();
        for (int i = 0; i < _doubleListType.Count; i++)
        {
            result.AddRange(_doubleListType[i]);
        }
        result = result.Distinct().ToList();
        return result;
    }

    // Root의 거리 총합 계산
    private float GetDistance(Transform _Start, List<Transform> _Root, Transform _Target)
    {
        float dis = 0f;
        dis += Vector2.Distance(_Start.position, _Root[0].position);

        if (_Root.Count >= 2)
        {
            for (int j = 0; j < _Root.Count - 1; j++)
            {
                dis += Vector2.Distance(_Root[j].position, _Root[j + 1].position);
            }
        }

        dis += Vector2.Distance(_Target.position, _Root[_Root.Count - 1].position);
        return dis;
    }

    #endregion

    #region UI

    public override void SetSortingOrder(int _SortingOrder)
    {
        base.SetSortingOrder(_SortingOrder);
        HUD.ThisCanvas.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Pattern

    private void StartPatternFromNone()
    {
        OrderOfPriorityEnemyPatternList[OrderOfPriorityEnemyPatternList.Count - 1].EnemyPatternList[0].EnemyPatternList[0].StartPattern();
    }

    public void TryGetAnyPattern()
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
                List<ContinuousEnemyPattern> epList = GameManager.ShuffleList<ContinuousEnemyPattern>(OrderOfPriorityEnemyPatternList[i].EnemyPatternList);
                
                // 만약 사용 가능한 패턴이 있다면 시작
                for (int j = 0; j < epList.Count; j++)
                {
                    if (epList[j].EnemyPatternList[0].CanPlayPattern())
                    {
                        CurrentContinuousEnemyPattern = epList[j];
                        CurrentEnemyPattern = epList[j].EnemyPatternList[0];

                        CurrentEnemyPattern.StartPattern();

                        return;
                    }
                }
            }
        }

        // 있다면 다음 패턴을 찾아서 실행.
        else
        {
            CurrentEnemyPattern = CurrentContinuousEnemyPattern.EnemyPatternList[OrderOfPattern + 1];

            CurrentEnemyPattern.StartPattern();
            return;
        }
    }


    #endregion

    #region Effect

    public void ExplosionEffect()
    {
        this.MEI.GenExplosionImgs(
            this.MEI.gameObject.transform.position,
            32, 0.15f, 0.75f,
            0.9f, 0.05f, 0.1f,
            0.4f, 0.5f, 1.0f,
            0, ThisSmokeM);
    }

    public void HittedPointEffect(Vector2 _SpanwedPos, eDamageType _DamageType, bool _IsCritical, Quaternion _Rotation)
    {
        // Hitted Anim

        Vector3 currentRotation = _Rotation.eulerAngles;

        Quaternion q = Quaternion.identity;
        currentRotation.z += 180;
        q.eulerAngles = currentRotation;

        OnlyOnceTimeAnimation oota = PoolingManager.Instance.GetOP_OnlyOnceAnimator();
        oota.StartAnim(
            HittedAC_0,
            _SpanwedPos,
            UnitManager.Instance.ModuleM_000_Explosion,
            PlayerManager.Instance.PlayerController.GetCorrectHitted_C(_DamageType, _IsCritical),
            q, 
            1.5f, 1f);

        Quaternion q2 = Quaternion.identity;
        currentRotation.z += Random.Range(-45, 45);
        q2.eulerAngles = currentRotation;

        OnlyOnceTimeAnimation oota2 = PoolingManager.Instance.GetOP_OnlyOnceAnimator();
        oota2.StartAnim(
            HittedAC_1,
            _SpanwedPos,
            UnitManager.Instance.ModuleM_000_Explosion,
            PlayerManager.Instance.PlayerController.GetCorrectHitted_C(_DamageType, _IsCritical),
            q2,
            2.5f, 1.2f);

        // SlowMotion
        PlayerManager.Instance.CameraController.PlayHitEnemyAnim();
    }

    private void DieEffect()
    {
        Quaternion q = Quaternion.identity;
        Vector3 currentRotation = q.eulerAngles;
        currentRotation.z += Random.Range(-20, 20);
        q.eulerAngles = currentRotation;

        OnlyOnceTimeAnimation oota2 = PoolingManager.Instance.GetOP_OnlyOnceAnimator();
        oota2.StartAnim(
            HittedAC_2,
            this.TargetObject.transform.position,
            UnitManager.Instance.ModuleM_000_Explosion,
            q,
            2.5f, 2f);

        ExplosionEffect();
    }

    #endregion

}

#region Pattern

[System.Serializable]
public class ContinuousEnemyPattern
{
    public List<EnemyPattern> EnemyPatternList;
}

[System.Serializable]
public class OrderOfPriorityEnemyPattern
{
    public List<ContinuousEnemyPattern> EnemyPatternList;
}

#endregion