using System;
using UniRx;
using UnityEngine;

public class PlayerController : MovableObject
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Header("=== Combat")]

    [Header("-- State")]
    [SerializeField] private eCombatMode CombatMode = eCombatMode.Physics;
    [SerializeField] public bool IsCasting = false;
    [SerializeField] private float CurrentCastingTime = 0;

    [Header("-- Energy")]
    [SerializeField] public ReactiveProperty<float> CurrentEP = new();

    [Header("-- Bettery")]
    [SerializeField] public ReactiveProperty<int> CurrentBS = new();
    [SerializeField] private int NeedBS_ForMakeBC = 5;
    [SerializeField] public ReactiveProperty<int> CurrentBC = new();

    [Header("-- Weapon")]
    [SerializeField] public PlayerWeaponController BaseWeapon;


    [Header("=== Movement")]

    [Header("-- State")]
    [SerializeField] protected eMovementState MovementState = eMovementState.IdleOrWalk;

    [Header("-- Dash")]
    [SerializeField] private int CurrentDashCharge = 0;
    [SerializeField] private float CurrentDashCooltime = 0;
    [SerializeField] private Vector2 DashTargetDir;


    #endregion

    #region Framework

    private void Awake()
    {
        SetState();
    }

    private void FixedUpdate()
    {
        AlwaysCaculate();
        Movement();
    }

    #endregion

    #region State

    private void SetState()
    {
        // Life
        CurrentEP.Value = PlayerManager.Instance.LifeState.MaxEP;
    }

    #endregion

    #region Energy

    public void AddCurrentEP(float _AddValue)
    {
        float result = CurrentEP.Value + _AddValue;
        result = Math.Max(result, 0);
        result = Math.Min(result, PlayerManager.Instance.LifeState.MaxEP);

        Debug.Log("현재: " + CurrentEP + " / " + "회복량 : " + _AddValue);

        CurrentEP.Value = result;
    }

    #endregion

    #region Bettery

    public void AddCurrentBS(int _AddValue)
    {
        CurrentBS.Value += _AddValue;
        while(true)
        {
            if (CurrentBS.Value >= NeedBS_ForMakeBC)
            {
                CurrentBS.Value -= NeedBS_ForMakeBC;
                CurrentBC.Value++;
                UIManager.Instance.PlayerHUDController.CurrentEmptyBC.Complete(0.2f);
            }
            else
            {
                break;
            }
        }
        Debug.Log("현재 배터리 조각: " + CurrentBS.Value + " / " + "현재 배터리 셀: " + CurrentBC.Value);
    }


    #endregion

    #region Movement

    private void Movement()
    {
        switch(MovementState)
        {
            case eMovementState.IdleOrWalk:
                Walk(InputManager.Instance.InputMoveDir, PlayerManager.Instance.MovementState.WalkSpeed, AccelerationSpeed);
                break;

            case eMovementState.Dash:
                Dash(DashTargetDir, PlayerManager.Instance.MovementState.DashDur);
                break;

            default: break;
        }
    }

    protected override void Dash(Vector2 _DashDir, float _TargetDashProcessTime)
    {
        base.Dash(_DashDir, _TargetDashProcessTime);
        if (CurrentDashProcessTime >= _TargetDashProcessTime)
        {
            MovementState = eMovementState.IdleOrWalk;
        }
    }

    public void CanDashCheck()
    {
        if (MovementState == eMovementState.Dash || CurrentDashCharge <= 0)
        {
            return;
        }

        DashTargetDir = InputManager.Instance.DirFromPlayerPos.normalized;
        CurrentDashCharge--;
        MovementState = eMovementState.Dash;
    }

    #endregion

    #region Combat Mode

    public void CanChangeCombatModeCheck()
    {
        if (MovementState != eMovementState.IdleOrWalk || IsCasting)
        {
            return;
        }

        IsCasting = true;
        MovementState = eMovementState.Stop;
        ThisRb.velocity = Vector2.zero;
    }


    #endregion

    #region Caculate

    private void AlwaysCaculate()
    {
        DashCaculate();
        CastingCaculate();
    }

    private void DashCaculate()
    {
        if (CurrentDashCharge >= PlayerManager.Instance.MovementState.MaxDashCharge)
        { return; }

        if (CurrentDashCooltime >= PlayerManager.Instance.MovementState.DashCooltime)
        {
            CurrentDashCharge++;
            CurrentDashCooltime = 0f;
            Debug.Log("대쉬량 : " + CurrentDashCharge);
        }
        else
        {
            CurrentDashCooltime += Time.deltaTime;
        }
    }

    private void CastingCaculate()
    {
        if (IsCasting)
        {
            if(CurrentCastingTime < PlayerManager.Instance.UtilityState.MaxCastingTime)
            {
                CurrentCastingTime += Time.deltaTime;
            }
            else
            {
                CurrentCastingTime = 0f;
                IsCasting = false;
                MovementState = eMovementState.IdleOrWalk;
                switch (CombatMode)
                {
                    case eCombatMode.Physics:
                        CombatMode = eCombatMode.Energy;
                        BaseWeapon.ThisBulletState_forSendData.DamageType = eDamageType.Energy;
                        break;

                    case eCombatMode.Energy:
                        CombatMode = eCombatMode.Physics;
                        BaseWeapon.ThisBulletState_forSendData.DamageType = eDamageType.Physics;
                        break;

                    default:
                        break;
                }
            }
        }
    }

    #endregion
}



