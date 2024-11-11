using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using static NormalEnemyController;

public class EnemyController : MovableObject, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private string EnemyID;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private eEnemy ThisEnemyType;
    [SerializeField] private float MaxHP;
    [HideInInspector] private ReactiveProperty<float> CurrentHP = new();

    [SerializeField] private float MaxEP;
    [HideInInspector] private ReactiveProperty<float> CurrentEP = new();
    [HideInInspector] public bool IsLethargy = false;
    [SerializeField] private float RecoverLethargyTime = 4f;

    [SerializeField] private float ItemDropPercent = 0.0f;

    [Space(10)]
    [Header("=== Movement")]
    [SerializeField] private eMovementState MovementState;
    [HideInInspector] public Vector2 MoveTargetPoint = Vector2.zero;
    [SerializeField] private Vector2 MoveDir;
    [SerializeField] public float MoveSpeed;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] private Canvas ThisCanvas;
    [SerializeField] private ModifyReductionFocusProgressBar HP_ProgressBar;
    [SerializeField] private ModifyReductionFocusProgressBar EP_ProgressBar;

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] public MakeExplosionImage MEI;

    [HideInInspector] private GameObject TargetPC;
    [HideInInspector] protected RoomController CurrentRoomController;

    [Space(10)]
    [Header("=== Nav")]
    [Tooltip("This is Radius")]
    [SerializeField] private float NavRadius = 0.2f;


    [Space(10)]
    [Header("=== Pattern")]
    [Tooltip("This Order of Priority Equle Index")]
    [SerializeField] protected List<OrderOfPriorityEnemyPattern> OrderOfPriorityEnemyPatternList;
    [SerializeField] public EnemyPattern CurrentEnemyPattern = null;
    [SerializeField] public bool IsPlayingPattern = false;

    #endregion

    #region Fremework

    private void Offset()
    {
        HP_ProgressBar.Offset();
        EP_ProgressBar.Offset();

        CurrentHP.Value = MaxHP;
        CurrentEP.Value = MaxEP;

        OffsetPatternData();
    }

    private void Start()
    {
        Offset();

        CurrentHP
            .Subscribe(_CurrentHP =>
            {
                HP_ProgressBar.SetFillImgSmooth(CurrentHP.Value, MaxHP);
            });

        CurrentEP
            .Subscribe(_CurrentEP =>
            {
                EP_ProgressBar.SetFillImgSmooth(CurrentEP.Value, MaxEP);
            });

    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (TargetPC == null)
        { TargetPC = PlayerManager.Instance.PlayerController.gameObject; }

        if (CurrentRoomController == null)
        { CurrentRoomController = StageManager.Instance.CurrentRoomController; }


        StartTryGetAnyPattern(0.5f);
    }

    protected void FixedUpdate()
    {
        Movement();
    }

    #endregion

    #region Move

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

    #region Damaged

    public void TakeDamage(BulletState _BS, Vector2 _KnockbackDir)
    {
        if (base.IsDead)
        { return; }

        // Knockback
        if (_BS.AbleKnockback)
        {
            Debug.DrawRay((Vector2)this.transform.position, _KnockbackDir, Color.red, 5f);
            GetKnockback(new KnockbackState(_KnockbackDir, _BS.KnockbackPower, _BS.KnockbackTime));
        }

        if (_BS.DamageType == eDamageType.Physics)
        {
            // UI
            PoolingManager.Instance.GetOP_DmgTxt().OffsetByPhysicDmg(
            (Vector2)TargetObject.transform.position + new Vector2(-0.2f, 0.2f),
            _BS.BaseDamage, _BS.IsCritical);



            SetIsDead(CurrentHP.Value, _BS.BaseDamage);
            CurrentHP.Value -= _BS.BaseDamage;
            if (CurrentHP.Value <= 0f)
            {
                base.IsDead = true;
                Die();
            }
        }
        else
        {
            if (!IsLethargy)
            {
                // UI
                PoolingManager.Instance.GetOP_DmgTxt().OffsetByEnergyDmg(
                    (Vector2)TargetObject.transform.position + new Vector2(-0.2f, 0.2f),
                    _BS.BaseDamage, _BS.IsCritical);

                SpawnES(_BS.BaseDamage);
            }
            else
            {
                // UI
                PoolingManager.Instance.GetOP_DmgTxt().OffsetByStateStun(
                    (Vector2)TargetObject.transform.position + new Vector2(0, 0.2f));
            }
        }
    }

    public void TakeDamage(AttackerState _AttackerState, bool _IsCritical, Vector2 _KnockbackDir)
    {
        if (base.IsDead)
        { return; }

        // Knockback
        if (_AttackerState.AbleKnockback)
        {
            Debug.DrawRay((Vector2)this.transform.position, _KnockbackDir, Color.red, 5f);
            GetKnockback(new KnockbackState(_KnockbackDir, _AttackerState.KnockbackPower, _AttackerState.KnockbackTime));
        }


        float baseDamage = _AttackerState.BaseDamage;
        if (_IsCritical)
        {
            baseDamage *= _AttackerState.CD;
        }

        if (_AttackerState.DamageType == eDamageType.Physics)
        {
            // UI
            PoolingManager.Instance.GetOP_DmgTxt().OffsetByPhysicDmg(
            (Vector2)TargetObject.transform.position + new Vector2(-0.2f, 0.2f),
            baseDamage, _IsCritical);

            SetIsDead(CurrentHP.Value, baseDamage);
            CurrentHP.Value -= baseDamage;
            if (CurrentHP.Value <= 0f)
            {
                base.IsDead = true;
                Die();
            }
        }
        else
        {
            if (!IsLethargy)
            {
                // UI
                PoolingManager.Instance.GetOP_DmgTxt().OffsetByEnergyDmg(
                    (Vector2)TargetObject.transform.position + new Vector2(-0.2f, 0.2f),
                    baseDamage, _IsCritical);

                SpawnES(baseDamage);
            }
            else
            {
                // UI
                PoolingManager.Instance.GetOP_DmgTxt().OffsetByStateStun(
                    (Vector2)TargetObject.transform.position + new Vector2(0, 0.2f));
            }
        }
    }

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
        PlayerManager.Instance.CameraController.PlayKillShake(PlayerManager.Instance.PlayerController.ExecutionInterval);

        // Remove
        if (EnemyManager.Instance.CurrentEnemyList.Contains(this))
        { EnemyManager.Instance.CurrentEnemyList.Remove(this); }

        if (LayerOrderManager.Instance.NeedLayerObjects.Contains(this))
        { LayerOrderManager.Instance.NeedLayerObjects.Remove(this); }

        // Check Room State
        StageManager.Instance.Complete_KillAll();

        // Set
        this.gameObject.SetActive(false);

        // Effect
        MEI.GenExplosionImgs(
                    MEI.gameObject.transform.position,
                    16, 0.15f, 0.75f,
                    1.6f, 0.05f, 0.1f,
                    0.8f, 0.5f, 1.0f,
                    0, EnemyManager.Instance.EnemySmokeMaterial);
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
        float targetValue = 0;
        if (CurrentEP.Value > _Value)
        {
            targetValue = _Value;
            CurrentEP.Value -= _Value;
        }
        else if (CurrentEP.Value > 0)
        {
            // UI
            PoolingManager.Instance.GetOP_DmgTxt().OffsetByStateStun(
                    (Vector2)TargetObject.transform.position + new Vector2(0, 0.2f));

            targetValue = CurrentEP.Value;
            CurrentEP.Value = 0;
            IsLethargy = true;
            StartCoroutine(RecoverLethargy());
            Debug.Log(this.gameObject.name + " / Lethargy!!!");
        }
        targetValue *= PlayerManager.Instance.PlayerController.SpawnESMultiple.ActualState.Value;

        EnergyShrapnelController ESC = PoolingManager.Instance.GetOP_EnergyShrapnel();
        ESC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            targetValue);
        ESC.gameObject.SetActive(true);
    }

    #endregion

    #region State

    private IEnumerator RecoverLethargy()
    {
        yield return new WaitForSeconds(0.5f);

        EP_ProgressBar.SetFillFullImgSmooth(RecoverLethargyTime);

        yield return new WaitForSeconds(RecoverLethargyTime);

        CurrentEP.Value = MaxEP;
        IsLethargy = false;
        Debug.Log(this.gameObject.name + " / Recover Lethargy!!!");
    }


    #endregion

    #region Interact

    public void Interact()
    {
        Die();
    }

    #endregion

    #region Nav

    /*public Vector2 FindWay()
    {
        List<Transform> _AllTF = StageManager.Instance.CurrentRoomController.RoomRuleController.InRoom_AllWayPoint;

        if (_AllTF == null || _AllTF.Count < 2)
        { return Vector2.zero; }

        if (!IsExistWall(this.transform, TargetPC.transform))
        {
            Vector2 dirVec = TargetPC.transform.position - this.transform.position;
#if UNITY_EDITOR
            Debug.DrawRay(this.transform.position, dirVec, Color.blue, 2f);
#endif
            return TargetPC.transform.position;
        }


        List<List<Transform>> allWayRoot = new List<List<Transform>>();
        List<List<Transform>> actualAllWayRoot = new List<List<Transform>>();

        // 처음
        Transform currentStartTF = this.transform;
        for (int i = 0; i < _AllTF.Count; i++)
        {
            if (!IsExistWall(currentStartTF, _AllTF[i]))
            {
                allWayRoot.Add(new List<Transform> { _AllTF[i] });
            }
        }


        int temp = 0;
        while (true)
        {
            temp++;
            if (temp > 100)
            {
                break;
            }

            // 갱신 루트를 위한 변수
            List<List<Transform>> newAllWayRoot = new List<List<Transform>>();
            List<Transform> allWayRootForList = GetNormalList(allWayRoot);


            // 원래 루트의 마지막을 가져와서 직접 타겟에 갈 수 있는가
            for (int i = 0; i < allWayRoot.Count; i++)
            {
                Transform tf = allWayRoot[i][allWayRoot[i].Count - 1];
                if (!IsExistWall(tf, TargetPC.transform))
                {
                    newAllWayRoot.Add(allWayRoot[i]);
                }
            }

            // 위 루트에서 한곳이라도 갈 수 있다면 루트로 채용
            if (newAllWayRoot.Count > 0)
            {
                actualAllWayRoot.AddRange(newAllWayRoot);
            }

            // 갈 수 있는 다른 포인트가 존재하는지 판별
            // 원래 존재했던 루트가 이를 가지고 있지 않은지 판별
            for (int i = 0; i < allWayRoot.Count; i++)
            {
                Transform tf = allWayRoot[i][allWayRoot[i].Count - 1];
                List<Transform> tfList = GetConnectedWayPoints(tf, CurrentRoomController.RoomRuleController.InRoom_AllWayPoint);
                for (int j = 0; j < tfList.Count; j++)
                {
                    // 파별 부문
                    if (!IsExistWall(tf, tfList[j]) &&
                        !allWayRootForList.Contains(tfList[j]))
                    {
                        List<Transform> initRoot = new List<Transform>();
                        initRoot.AddRange(allWayRoot[i]);
                        initRoot.Add(tfList[j]);

                        newAllWayRoot.Add(initRoot);
                    }
                }
            }

            allWayRoot.Clear();
            allWayRoot = newAllWayRoot;


            if (_AllTF.Count <= allWayRootForList.Count)
            {
                // 모든 경로를 찾음
                break;
            }
        }

        if (temp > 100)
        {
            // 데이터 초과 방지
            Debug.Log("데이터 초과");
            return Vector2.zero;
        }

        

        // 가장 짧은 거리의 경로 탐색
        List<Transform> usableRoot = actualAllWayRoot[0];
        float usableDis = GetDistance(this.transform, actualAllWayRoot[0], TargetPC.transform);
        for (int i = 1; i < actualAllWayRoot.Count; i++)
        {
            float dis = GetDistance(this.transform, actualAllWayRoot[i], TargetPC.transform);

            if (usableDis > dis)
            {
                usableRoot = actualAllWayRoot[i];
                usableDis = dis;
            }
        }

#if UNITY_EDITOR
        Vector2 dir = usableRoot[0].position - this.transform.position;
        Debug.DrawRay(this.transform.position, dir, Color.blue, 2f);

        for (int i = 1; i < usableRoot.Count; i++)
        {
            dir = usableRoot[i].position - usableRoot[i - 1].position;
            Debug.DrawRay(usableRoot[i - 1].position, dir, Color.blue, 2f);
        }

        dir = TargetPC.transform.position - usableRoot[usableRoot.Count - 1].position;
        Debug.DrawRay(usableRoot[usableRoot.Count - 1].position, dir, Color.blue, 2f);
#endif

        return usableRoot[0].transform.position != null ? usableRoot[0].transform.position : Vector2.zero;
    }

    // 연결된 모든 포인트 가져오기
    private List<Transform> GetConnectedWayPoints(Transform _StartTF, List<Transform> _AllTF)
    {
        List<Transform> tfs = new List<Transform>();

        for (int i = 0; i < _AllTF.Count; i++)
        {
            if (_AllTF[i] == null || _AllTF[i] == _StartTF)
            { continue; }

            if (!IsExistWall(_StartTF, _AllTF[i]))
            {
                tfs.Add(_AllTF[i]);
            }
        }

        return tfs;
    }*/

    public List<WayPoint> GetClosetRoot()
    {
        // 바로 갈 수 있다면
        if (!IsExistWall(this.transform, PlayerManager.Instance.PlayerController.transform))
        {
            return new List<WayPoint> { new WayPoint(PlayerManager.Instance.PlayerController.transform) };
        }    

        // 현재 방에 모든 WayPoint
        List<WayPoint> AllWP = StageManager.Instance.CurrentRoomController.RoomRuleController.InRoom_AllWayPoint;

        // 이 객체와 플레이어에 가장 가까운 WayPoint 찾기
        List<List<WayPoint>> RootsFromEnemy = new List<List<WayPoint>> { new List<WayPoint> { GetClosetWP(this.transform, AllWP) } };
        List<List<WayPoint>> RootsFromPlayer = new List<List<WayPoint>> { new List<WayPoint> { GetClosetWP(PlayerManager.Instance.PlayerController.transform, AllWP) } };

        while (true)
        {
            List<WayPoint> EndRootPointsFromEnemy = GetEndPoints(RootsFromEnemy, false);
            List<WayPoint> EndRootPointsFromPlayer = GetEndPoints(RootsFromPlayer, true);


        }


        return null;
    }

    private List<WayPoint> GetEndPoints(List<List<WayPoint>> _Roots, bool _OrderByFirst)
    {
        return null;
    }

    // 가장 가까운 WayPoint
    private WayPoint GetClosetWP(Transform transform, List<WayPoint> _AllWP)
    {
        return null;
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
    private List<Transform> GetNormalList(List<List<Transform>> _doubleListType)
    {
        List<Transform> result = new List<Transform>();
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
        ThisCanvas.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Pattern

    private void OffsetPatternData()
    {
        for (int i = 0; i < OrderOfPriorityEnemyPatternList.Count; i++)
        {
            for (int j = 0; j < OrderOfPriorityEnemyPatternList[i].EnemyPatternList.Count; j++)
            {
                OrderOfPriorityEnemyPatternList[i].EnemyPatternList[j].Offset(this);
            }
        }
    }

    public void StartTryGetAnyPattern(float _DelayTime)
    {
        StartCoroutine(TryGetAnyPattern(_DelayTime));
    }

    private IEnumerator TryGetAnyPattern(float _DelayTime)
    {
        yield return new WaitForSeconds(_DelayTime);

        bool IsFindPattern = false;
        for (int i = 0; i < OrderOfPriorityEnemyPatternList.Count; i++)
        {
            // 같은 우선도에 있는 패턴 랜덤으로 섞기
            List<EnemyPattern> epList = GameManager.ShuffleList<EnemyPattern>(OrderOfPriorityEnemyPatternList[i].EnemyPatternList);

            for (int j = 0; j < epList.Count; j++)
            {
                if (epList[j].CanPlayPattern())
                {
                    StartPattern(epList[j]);
                    IsFindPattern = true;
                    break;
                }
            }

            if (IsFindPattern)
            {
                break;
            }
        }
    }

    private void StartPattern(EnemyPattern _Pattern)
    {
        if (_Pattern != null)
        {
            _Pattern.StartPattern();
        }
    }

    #endregion
}




#region Order of Priority

[System.Serializable]
public class OrderOfPriorityEnemyPattern
{
    public List<EnemyPattern> EnemyPatternList;
}

#endregion