using System;
using System.Collections;
using UnityEngine;

// Ally는 Follow를 기본으로 가짐 (플레이어에게 가는 것이 필요하기 때문)
public class AllyController : NavObjectController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float MaxEP = 100f;

    #endregion

    #region - Hide

    [HideInInspector] protected float FollowInitDelay = 0.2f;

    // For Player
    [HideInInspector] protected PlayerController Player;
    [HideInInspector] protected float ForPlayerDis = 1f;

    // For Enemy
    [HideInInspector] protected EnemyController Enemy;
    [HideInInspector] protected float ForEnemyDis = 1.5f;

    [HideInInspector] private IEnumerator ThisMainCor = null;

    [HideInInspector] private readonly float NearPlayerDis = 0.5f;
    #endregion

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Player = PlayerManager.Instance.PlayerController;
    }

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        DevTool.Add_InList(AllyManager.Instance.AllAllies, this);
        DevTool.Add_InList(LayerOrderManager.Instance.NeedSortingObjects, this);

        CurrentEP.Value = MaxEP;
        Set_MovementSpeed(AllyManager.Instance.BaseMoveSpeed);


        Start_MainCor();
    }

    private void OnDisable()
    {
        DevTool.Remove_InList(AllyManager.Instance.AllAllies, this);
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
        Play_Walk(MoveAtDir, MoveSpeed, _DeltaTime);
    }

    public void Set_MovementSpeed(float _Speed)
    {
        MoveSpeed = _Speed;
        FollowInitDelay = 0.4f / MoveSpeed;

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
}
