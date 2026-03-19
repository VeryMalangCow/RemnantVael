using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class FieldUnitAllyController : AllyController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Field Unit")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected float ForEnemyDis = 1.5f;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SortingGroup ThisSG;
    [SerializeField] private AllySolarController ThisSolar;
    [SerializeField] protected DirectionalAllyTypeImgController ThisDirImg;

    [Space(10)]
    [Header("=== Random Pos")]
    [SerializeField] private float randomPosAreaRadius = 5f;

    #endregion

    #region - Hide

    // Movement
    [HideInInspector] protected float FollowInitDelay = 0.2f;

    // For Player
    [HideInInspector] protected float ForPlayerDis = 2.5f;
    [HideInInspector] private readonly float NearPlayerDis = 0.5f;

    // Nav
    [HideInInspector] private readonly float RandomPosDelay = 5f;
    [HideInInspector] private float CurrentRandomPosDelay = 5f;
    [HideInInspector] protected Vector2 RandomPos;

    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_Subscribe();
    }


    private void Offset_Subscribe()
    {
        AllyStateMode
            .Subscribe(value =>
            {
                ThisSolar.Set_AllyStateMode(value);
            });
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        LayerOrderManager.instance.Add_NeedSortObj(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        LayerOrderManager.instance.Remove_NeedSortObj(this);
    }

    protected override void Update()
    {
        base.Update();

        Caculate_RandomPos(Time.deltaTime);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Play_Movement(Time.fixedDeltaTime);
    }

    #endregion

    #region Random Pos

    private void Caculate_RandomPos(float _DeltaTime)
    {
        if (RandomPosDelay > CurrentRandomPosDelay)
        {
            CurrentRandomPosDelay += _DeltaTime;
        }
        else
        {
            CurrentRandomPosDelay -= RandomPosDelay;
            Set_RandomPos();
        }
    }

    private void Set_RandomPos()
    {
        RandomPos = Get_RandomNavPos(randomPosAreaRadius);
    }

    #endregion

    #region Play

    public override void Start_MainCor()
    {
        if (!gameObject.activeSelf) return;

        base.Start_MainCor();

        Set_RandomPos();
    }

    #endregion

    #region Movement

    private void Play_Movement(float _DeltaTime)
    {
        Play_Walk(MoveAtDir, ActualAllyState.MovementSpeed.value, _DeltaTime);
    }

    #endregion

    #region State (Enum)

    protected override void Set_AllyStateMode(eAllyStateMode _Mode)
    {
        base.Set_AllyStateMode(_Mode);

        if (AllyStateMode.Value != _Mode)
        {
            ThisDirImg.Set_Type(_Mode);
        }
    }

    #endregion

    #region Set (State)

    public override void Set_AllState() // 카드와 업그레이드 모두 적용
    {
        base.Set_AllState();

        FollowInitDelay = 0.4f / ActualAllyState.MovementSpeed.value;
    }

    #endregion

    #region Is

    protected bool Is_FollowState(Vector2 _TargetPos, float _Dis, bool _CheckWall)
    {
        bool disCondition = _Dis < Vector2.Distance(_TargetPos, this.transform.position);
        bool wallCondition = _CheckWall ? Is_ExistWall(_TargetPos) : false;

        return disCondition || wallCondition;
    }

    protected void Stop_Follow()
    {
        if (MoveAtDir != Vector2.zero)
            MoveAtDir = Vector2.zero;
    }

    #endregion

    #region Set

    public override void Set_SpawnFirst()
    {
        base.Set_SpawnFirst();

        Set_PosRandomNearPlayer();
    }

    public void Set_PosRandomNearPlayer()
    {
        transform.position =
            (Vector2)PlayerManager.instance.playerController.transform.position +
            (new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * NearPlayerDis);
    }

    #endregion

    #region Set (Sorting)

    public override void Set_SortingOrder(int _SortingOrder)
    {
        ThisSG.sortingOrder = _SortingOrder;

        HUD.ThisCanvas.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Dir

    // Not Normalize (최적화로 정규화를 하지않는 것이 더 도움이 됨)
    public Vector2 Get_ForPlayerDir()
    {
        return (Player.transform.position - this.transform.position);
    }

    public Vector2 Get_ForEnemyDir()
    {
        if (Enemy == null)
            return Vector2.down;

        return (Enemy.transform.position - this.transform.position);
    }


    #endregion
}
