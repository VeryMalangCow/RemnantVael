using System;
using System.Collections;
using UnityEngine;

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

    [HideInInspector] private float FollowInitDelay = 0.2f;
    [HideInInspector] private float ForPlayerDis = 1f;

    [HideInInspector] private PlayerController Player;

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

    protected IEnumerator Play_Main_Cor()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (PingController.IsPinged)
            {
                Set_PingedState();
                yield return new WaitForSeconds(FollowInitDelay);
            }
            else
            {
                Set_NoPingedState();
                yield return new WaitForSeconds(FollowInitDelay);
            }
        }
    }

    #endregion

    #region Is

    private bool Is_FollowState()
    {
        if (ForPlayerDis < Vector2.Distance(Player.transform.position, this.transform.position)) 
            return true;

        return false;
    }

    #endregion

    #region Set (Ping)

    private void Set_PingedState()
    {

    }


    private void Set_NoPingedState()
    {
        // 따라가기
        if (Is_FollowState())
        {
            Set_NavDir(Player.transform);
        }
        // 정지
        else
        {
            if (MoveAtDir != Vector2.zero)
                MoveAtDir = Vector2.zero;
        }
    }

    #endregion

    #region Set

    public void Set_PosRandomNearPlayer()
    {
        transform.position =
            (Vector2)PlayerManager.Instance.PlayerController.transform.position +
            (new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * NearPlayerDis);
    }

    #endregion
}
