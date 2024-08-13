using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerController : MovableObject
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Combat")]

    [Header("-- State")]
    [SerializeField] private eCombatMode CombatMode = eCombatMode.Physics;
    [SerializeField] private eCombatMode TargetCombatMode = eCombatMode.Physics;
    [SerializeField] public bool IsCasting = false;
    [SerializeField] private float CurrentCastingTime = 0;
    [SerializeField] private float TargetCastingTime = 0;

    [Header("-- Energy")]
    [SerializeField] public ReactiveProperty<float> MaxEP = new();
    [SerializeField] public ReactiveProperty<float> CurrentEP = new();
    [SerializeField] public ReactiveProperty<float> RegenerationEP = new();

    [Header("-- Bettery")]
    [SerializeField] public ReactiveProperty<int> CurrentBS = new();
    [SerializeField] public int NeedBS_ForMakeBC = 5;
    [SerializeField] public ReactiveProperty<int> CurrentBC = new();
    [SerializeField] public ReactiveProperty<int> CurrentEC = new();

    [Header("-- Weapon")]
    [SerializeField] public PlayerWeaponController BaseWeapon;

    [Space(10)]
    [Header("=== Movement")]

    [Header("-- State")]
    [SerializeField] protected eMovementState MovementState = eMovementState.IdleOrWalk;

    [Header("-- Dash")]
    [SerializeField] private float CurrentDashCooltime = 0;
    [SerializeField] private float NeedEP_ForDash = 5f;
    [SerializeField] private Vector2 DashTargetDir;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] public List<InteractItemController> CurrentInteractableItemList;
    [SerializeField] public InteractItemController CurrentInteractableItem;

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] private int TargetBoostRank = 0; 
    [SerializeField] private int MaxBoostRank = 4;
    [SerializeField] private ReactiveProperty<int> CurrentBoostRank = new();

    private delegate void SkillDele();
    private SkillDele ReservationSkillDele = null;


    #endregion

    #region Framework

    private void Awake()
    {
        SetStateOffset();
    }

    protected override void Update()
    {
        base.Update();
        AddCurrentEP(RegenerationEP.Value * Time.deltaTime);
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
        MaxEP.Value = PlayerManager.Instance.LifeState.MaxEP;
        CurrentEP.Value = PlayerManager.Instance.LifeState.MaxEP;
        RegenerationEP.Value = PlayerManager.Instance.LifeState.RegenerationEP;
    }

    #endregion

    #region Energy

    public void IncreaseMaxEP(float _AddValue)
    {
        MaxEP.Value += _AddValue;
        AddCurrentEP(_AddValue);

        Debug.Log("최대 EP : " + MaxEP.Value);
    }

    public void AddCurrentEP(float _AddValue)
    {
        float result = CurrentEP.Value + _AddValue;
        result = Math.Max(result, 0);
        result = Math.Min(result, this.MaxEP.Value);

        CurrentEP.Value = result;

        Debug.Log("현재: " + CurrentEP + " / " + "회복량 : " + _AddValue);
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
        if (MovementState == eMovementState.Dash || NeedEP_ForDash >= CurrentEP.Value)
        {
            return;
        }

        DashTargetDir = InputManager.Instance.DirFromPlayerPos.normalized;
        CurrentEP.Value -= NeedEP_ForDash;
        MovementState = eMovementState.Dash;
    }

    #endregion

    #region About Casting

    // Condition : Idle or Walk | No Casting Now 
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
    public void CanChange_CombatModeCheck(float _CastingTime)
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

        StartCasting(_CastingTime);
    }

    // + Not over the Max Boost Level
    public void CanChange_BoostModeCheck(float _CastingTime)
    {
        if (!CanChange() || TargetBoostRank >= MaxBoostRank)
        { return; }

        TargetBoostRank++;
        
        StartCasting(_CastingTime);
    }

    // + BoostLevel != 0
    public void CanChange_UnBoostModeCheck(float _CastingTime)
    {
        if (!CanChange() || CurrentBoostRank.Value <= 0)
        { return; }

        TargetBoostRank = 0;

        StartCasting(_CastingTime);
    }

    // + Enough EP | Enough BC
    public void CanChange_ChargeBettery(float _CastingTime)
    {
        if (!CanChange() ||
            PlayerManager.Instance.LifeState.NeedToMakeBC >= this.CurrentEP.Value ||
            CurrentBC.Value <= 0) 
        { return; }

        ReservationSkillDele = ChargeBettery;

        StartCasting(_CastingTime);
    }


    public void StartCasting(float _CastingTime)
    {
        TargetCastingTime = _CastingTime;
        IsCasting = true;
        MovementState = eMovementState.Casting;
        ThisRb.velocity = Vector2.zero;
    }

    #endregion

    #region Interact

    public void TryInteract()
    {
        if(CurrentInteractableItem != null)
        {
            ItemManager.Instance.GetItemSkill(CurrentInteractableItem.ThisItemData.ID);
            CurrentInteractableItem.gameObject.SetActive(false);
            PoolingManager.Instance.InteractItems.Queue.Enqueue(CurrentInteractableItem);
            CurrentInteractableItem = null;
        }
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
        if (CurrentDashCooltime >= PlayerManager.Instance.MovementState.DashCooltime)
        {
            CurrentDashCooltime = PlayerManager.Instance.MovementState.DashCooltime;
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
            if(CurrentCastingTime < TargetCastingTime)
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
                If_Boost();
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

    private void If_Boost()
    {
        if(CurrentBoostRank.Value != TargetBoostRank)
        {
            CurrentBoostRank.Value = TargetBoostRank;
        }
    }

    #endregion

    #endregion

    #region Interact

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if(_Col.tag == "Interact" && _Col.gameObject.transform.parent.TryGetComponent(out InteractItemController IIC))
        {
            CurrentInteractableItemList.Add(IIC);
        }
    }

    private void OnTriggerStay2D(Collider2D _Col)
    {
        if (CurrentInteractableItemList != null)
        {
            if (CurrentInteractableItemList.Count == 0)
            {
                CurrentInteractableItem = null;
            }
            else if (CurrentInteractableItemList.Count == 1)
            {
                CurrentInteractableItem = CurrentInteractableItemList[0];
            }
            else if (CurrentInteractableItemList.Count > 1)
            {
                float shortDis = Vector3.Distance(gameObject.transform.position, CurrentInteractableItemList[0].transform.position);
                foreach (InteractItemController IC in CurrentInteractableItemList)
                {
                    float Distance = Vector3.Distance(gameObject.transform.position, IC.gameObject.transform.position);

                    if (Distance < shortDis) 
                    {
                        CurrentInteractableItem = IC;
                        shortDis = Distance;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _Col)
    {
        if (_Col.tag == "Interact" && _Col.gameObject.transform.parent.TryGetComponent(out InteractItemController IIC))
        {
            CurrentInteractableItemList.Remove(IIC);
        }
    }

    #endregion
}



