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
    [SerializeField] protected float forEnemyDis = 1.5f;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SortingGroup sg;
    [SerializeField] private AllySolarController solar;
    [SerializeField] protected DirectionalAllyTypeImgController dirImg;

    [Space(10)]
    [Header("=== Random Pos")]
    [SerializeField] private float randomPosAreaRadius = 5f;

    #endregion

    #region - Hide

    // Movement
    [HideInInspector] protected float followInitDelay = 0.2f;

    // For Player
    [HideInInspector] protected float forPlayerDis = 2.5f;
    [HideInInspector] private readonly float nearPlayerDis = 0.5f;

    // Nav
    [HideInInspector] private readonly float randomPosDelay = 5f;
    [HideInInspector] private float currentRandomPosDelay = 5f;
    [HideInInspector] protected Vector2 randomPos;

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
        allyStateMode
            .Subscribe(value =>
            {
                solar.Set_AllyStateMode(value);
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

    private void Caculate_RandomPos(float deltaTime)
    {
        if (randomPosDelay > currentRandomPosDelay)
        {
            currentRandomPosDelay += deltaTime;
        }
        else
        {
            currentRandomPosDelay -= randomPosDelay;
            Set_RandomPos();
        }
    }

    private void Set_RandomPos()
    {
        randomPos = Get_RandomNavPos(randomPosAreaRadius);
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

    private void Play_Movement(float deltaTime)
    {
        Play_Walk(moveAtDir, actualAllyState.movementSpeed.value, deltaTime);
    }

    #endregion

    #region State (Enum)

    protected override void Set_AllyStateMode(eAllyStateMode mode)
    {
        base.Set_AllyStateMode(mode);

        if (allyStateMode.Value != mode)
        {
            dirImg.Set_Type(mode);
        }
    }

    #endregion

    #region Set (State)

    public override void Set_AllState() // 카드와 업그레이드 모두 적용
    {
        base.Set_AllState();

        followInitDelay = 0.4f / actualAllyState.movementSpeed.value;
    }

    #endregion

    #region Is

    protected bool Is_FollowState(Vector2 targetPos, float dis, bool checkWall)
    {
        bool disCondition = dis < Vector2.Distance(targetPos, this.transform.position);
        bool wallCondition = checkWall ? Is_ExistWall(targetPos) : false;

        return disCondition || wallCondition;
    }

    protected void Stop_Follow()
    {
        if (moveAtDir != Vector2.zero)
            moveAtDir = Vector2.zero;
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
            (new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * nearPlayerDis);
    }

    #endregion

    #region Set (Sorting)

    public override void Set_SortingOrder(int sortingOrder)
    {
        sg.sortingOrder = sortingOrder;

        hud.canvas.sortingOrder = sortingOrder;
    }

    #endregion

    #region Dir

    // Not Normalize (최적화로 정규화를 하지않는 것이 더 도움이 됨)
    public Vector2 Get_ForPlayerDir()
    {
        return (player.transform.position - this.transform.position);
    }

    public Vector2 Get_ForEnemyDir()
    {
        if (enemy == null)
            return Vector2.down;

        return (enemy.transform.position - this.transform.position);
    }


    #endregion
}
