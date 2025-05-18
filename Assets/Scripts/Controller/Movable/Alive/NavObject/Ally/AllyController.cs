using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

// Ally는 Follow를 기본으로 가짐 (플레이어에게 가는 것이 필요하기 때문)
public class AllyController : NavObjectController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected AllyState MultipleAllyState;
    [SerializeField] private float MaxEP = 100f;
    [SerializeField] protected float ForEnemyDis = 1.5f;
    [SerializeField] private ReactiveProperty<eAllyStateMode> AllyStateMode = new();


    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SortingGroup ThisSG;
    [SerializeField] private AllySolarController ThisSolar;
    [SerializeField] protected DirectionalAllyTypeImgController ThisDirImg;

    #endregion

    #region - Hide

    // State
    [HideInInspector] protected float FollowInitDelay = 0.2f;

    [HideInInspector] protected AllyState ActualAllyState = new AllyState();

    // For Player
    [HideInInspector] protected PlayerController Player;
    [HideInInspector] protected float ForPlayerDis = 1f;

    // For Enemy
    [HideInInspector] protected EnemyController Enemy;

    [HideInInspector] private IEnumerator ThisMainCor = null;

    [HideInInspector] private readonly float NearPlayerDis = 0.5f;
    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Player = PlayerManager.Instance.PlayerController;
        DevTool.Add_InList(AllyManager.Instance.AllAllies, this);

        AllyStateMode
            .Subscribe(value =>
            {
                ThisSolar.Set_AllyStateMode(value);
            });

        Set_AllyStateMode(AllyStateMode.Value);
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        DevTool.Add_InList(LayerOrderManager.Instance.NeedSortingObjects, this);

        CurrentEP.Value = MaxEP;
        Set_AllState(AllyManager.Instance.GetAllyState);

        Start_MainCor();
    }

    private void OnDisable()
    {
        //DevTool.Remove_InList(AllyManager.Instance.AllAllies, this);
        DevTool.Remove_InList(LayerOrderManager.Instance.NeedSortingObjects, this);

        Stop_MainCor();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Play_Movement(Time.fixedDeltaTime);
    }

    #endregion

    #region Movement

    private void Play_Movement(float _DeltaTime)
    {
        Play_Walk(MoveAtDir, ActualAllyState.MovementSpeed, _DeltaTime);
    }

    #endregion

    #region State (Enum)

    protected void Set_AllyStateMode(eAllyStateMode _Mode)
    {
        if (AllyStateMode.Value != _Mode)
        {
            AllyStateMode.Value = _Mode;
            ThisDirImg.Set_Type(_Mode);
        }
    }

    #endregion

    #region Set (State)

    public void Set_AllState(AllyState _StateValue)
    {
        Set_MovementSpeed(_StateValue.MovementSpeed);
        Set_Dmg(_StateValue.Dmg);
        Set_Rof(_StateValue.Rof);
    }


    private void Set_MovementSpeed(float _Value)
    {
        ActualAllyState.MovementSpeed = _Value * MultipleAllyState.MovementSpeed;
        FollowInitDelay = 0.4f / ActualAllyState.MovementSpeed;
    }

    private void Set_Dmg(float _Value)
    {
        ActualAllyState.Dmg = _Value * MultipleAllyState.Dmg;
    }

    private void Set_Rof(float _Value)
    {
        ActualAllyState.Rof = _Value * MultipleAllyState.Rof;
    }

    #endregion

    #region EP

    public void AddCurrentEP(float _AddValue)
    {
        CurrentEP.Value = Math.Clamp(CurrentEP.Value + _AddValue, 0, MaxEP);
    }

    public void TakeDamage(float _DmgValue)
    {
        AddCurrentEP(-_DmgValue);
    }

    #endregion

    #region Play

    public void Start_MainCor()
    {
        if (!gameObject.activeSelf) return;

        ThisMainCor = Play_Main_Cor();
        StartCoroutine(ThisMainCor);
    }

    public void Stop_MainCor()
    {
        if (ThisMainCor == null)
            return;

        StopCoroutine(ThisMainCor);
        ThisMainCor = null;

        if (MoveAtDir != Vector2.zero)
            MoveAtDir = Vector2.zero;
    }

    protected virtual IEnumerator Play_Main_Cor()
    {
        yield return new WaitForSeconds(0.5f);
    }

    #endregion

    #region Is

    protected bool Is_FollowState(Transform _TargetTF, float _Dis, bool _CheckWall)
    {
        bool disCondition = _Dis < Vector2.Distance(_TargetTF.position, this.transform.position);
        bool wallCondition = _CheckWall ? Is_ExistWall(_TargetTF) : false;

        return disCondition || wallCondition;
    }

    protected void Stop_Follow()
    {
        if (MoveAtDir != Vector2.zero)
            MoveAtDir = Vector2.zero;
        
    }

    #endregion

    #region Set

    public void Set_PosRandomNearPlayer()
    {
        transform.position =
            (Vector2)PlayerManager.Instance.PlayerController.transform.position +
            (new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * NearPlayerDis);
    }

    public void Set_TargetEnemy(EnemyController _Enemy)
    {
        Enemy = _Enemy;
    }

    #endregion

    #region Set (Sorting)

    public override void Set_SortingOrder(int _SortingOrder)
    {
        ThisSG.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Get (Dir)

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
