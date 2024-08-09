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
    [SerializeField] private eCombatMode TargetCombatMode = eCombatMode.Physics;
    [SerializeField] public bool IsCasting = false;
    [SerializeField] private float CurrentCastingTime = 0;

    [Header("-- Energy")]
    [SerializeField] public ReactiveProperty<float> CurrentEP = new();

    [Header("-- Bettery")]
    [SerializeField] public ReactiveProperty<int> CurrentBS = new();
    [SerializeField] public int NeedBS_ForMakeBC = 5;
    [SerializeField] public ReactiveProperty<int> CurrentBC = new();
    [SerializeField] public ReactiveProperty<int> CurrentEC = new();

    [Header("-- Weapon")]
    [SerializeField] public PlayerWeaponController BaseWeapon;


    [Header("=== Movement")]

    [Header("-- State")]
    [SerializeField] protected eMovementState MovementState = eMovementState.IdleOrWalk;

    [Header("-- Dash")]
    [SerializeField] private int CurrentDashCharge = 0;
    [SerializeField] private float CurrentDashCooltime = 0;
    [SerializeField] private Vector2 DashTargetDir;

    [Header("=== Skill")]
    [SerializeField] public int asdzxc = 0;

    private delegate void SkillDele();
    private SkillDele ReservationSkillDele = null;


    #endregion

    #region Framework

    private void Awake()
    {
        SetStateOffset();
    }

    private void FixedUpdate()
    {
        AlwaysCaculate();
        Movement();
    }

    #endregion

    #region State

    private void SetStateOffset()
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
                UIManager.Instance.PlayerHUDController.CurrentEmptyBC.Complete(0.3f, 0.2f);
            }
            else
            {
                break;
            }
        }
    }

    private void ChargeBettery()
    {
        this.CurrentEP.Value -= PlayerManager.Instance.LifeState.NeedToMakeBC;
        CurrentBC.Value--;
        CurrentEC.Value++;
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

    #region About Casting

    //Condition : Idle or Walk | No Casting Now 
    private bool CanChange()
    {
        if (MovementState != eMovementState.IdleOrWalk || IsCasting)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // + None
    public void CanChange_CombatModeCheck()
    {
        if (!CanChange()) 
        { return; } 

        switch (TargetCombatMode)
        {
            case eCombatMode.Physics:
                TargetCombatMode = eCombatMode.Energy;
                break;

            case eCombatMode.Energy:
                TargetCombatMode = eCombatMode.Physics;
                break;

            default:
                break;
        }

        StartCasting();
    }

    // + Enough EP | Enough BC
    public void CanChange_ChargeBettery()
    {
        if (!CanChange() ||
            PlayerManager.Instance.LifeState.NeedToMakeBC >= this.CurrentEP.Value ||
            CurrentBC.Value <= 0) 
        { return; }

        ReservationSkillDele = ChargeBettery;

        StartCasting();
    }

    public void StartCasting()
    {
        IsCasting = true;
        MovementState = eMovementState.Casting;
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

                If_CombatMode();
                If_Skill();
            }
        }
    }

    #region By Condition

    private void If_CombatMode()
    {
        if(CombatMode != TargetCombatMode)
        {
            CombatMode = TargetCombatMode;

            switch (TargetCombatMode)
            {
                case eCombatMode.Physics:
                    BaseWeapon.ThisBulletState_forSendData.DamageType = eDamageType.Physics;
                    break;

                case eCombatMode.Energy:
                    BaseWeapon.ThisBulletState_forSendData.DamageType = eDamageType.Energy;
                    break;

                default:
                    break;
            }
        }
    }

    private void If_Skill()
    {
        if(ReservationSkillDele != null)
        {
            ReservationSkillDele();
            ReservationSkillDele = null;
        }
    }
    #endregion

    #endregion
}



