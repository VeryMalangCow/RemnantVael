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
    [SerializeField] public List<GameObject> CurrentInteractableGOList;
    [SerializeField] public IInteract CurrentInteractable;
    //[SerializeField] public 

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] private int TargetBoostRank = 0; 
    [SerializeField] private int MaxBoostRank = 4;
    [SerializeField] public ReactiveProperty<int> CurrentBoostRank = new();

    private delegate void SkillDele();
    private SkillDele ReservationSkillDele = null;

    [Space(10)]
    [Header("=== Main Sprite")]
    [SerializeField] private MakeAfterImage MakeAfterImage;

    #endregion

    #region Framework

    private void Awake()
    {
        SetStateOffset();
    }

    protected override void Update()
    {
        base.Update();
        ItemManager.Instance.ActiveSkill_Always(CurrentBoostRank.Value);
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
    }

    #endregion

    #region Energy

    public void IncreaseMaxEP(float _AddValue)
    {
        MaxEP.Value += _AddValue;
        AddCurrentEP(_AddValue);

        Debug.Log("ÃÖ´ë EP : " + MaxEP.Value);
    }

    public void AddCurrentEP(float _AddValue)
    {
        float result = CurrentEP.Value + _AddValue;
        result = Math.Max(result, 0);
        result = Math.Min(result, this.MaxEP.Value);

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
            MakeAfterImage.EndGen();
        }
    }

    public void CanDashCheck()
    {
        if (MovementState == eMovementState.Dash || NeedEP_ForDash >= CurrentEP.Value)
        {
            return;
        }

        MakeAfterImage.StartGen(0.03f, 0.5f);
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
        if(CurrentInteractable != null)
        {
            CurrentInteractable.Interact();
            if (CurrentInteractable is InteractItemController IIC)
            {
                CurrentInteractable = null;
            }
            else if (CurrentInteractable is ShopController SC)
            {

            }
            
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
        if(_Col.tag == "Interact" && _Col.gameObject.transform.parent.TryGetComponent(out IInteract II))
        {
            GameObject TargetGO = _Col.gameObject.transform.parent.gameObject;

            if (!CurrentInteractableGOList.Contains(TargetGO))
            {
                CurrentInteractableGOList.Add(TargetGO); 
            }
        }
    }

    private void OnTriggerStay2D(Collider2D _Col)
    {
        if (CurrentInteractableGOList != null)
        {
            // None
            if (CurrentInteractableGOList.Count == 0)
            {
                CurrentInteractable = null;
            }
            // Only One
            else if (CurrentInteractableGOList.Count == 1)
            {
                if(CurrentInteractableGOList[0].TryGetComponent(out IInteract II))
                {
                    CurrentInteractable = II;
                }
            }
            // A Lot
            else if (CurrentInteractableGOList.Count > 1)
            {
                float shortDis = Vector3.Distance(gameObject.transform.position, CurrentInteractableGOList[0].transform.position);
                foreach (GameObject currentInteractableGO in CurrentInteractableGOList)
                {
                    float Distance = Vector3.Distance(gameObject.transform.position, currentInteractableGO.transform.position);

                    if (Distance < shortDis) 
                    {
                        currentInteractableGO.TryGetComponent(out IInteract II);
                        CurrentInteractable = II;
                        shortDis = Distance;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D _Col)
    {
        if (_Col.tag == "Interact" && _Col.gameObject.transform.parent.TryGetComponent(out IInteract II))
        {
            GameObject TargetGO = _Col.gameObject.transform.parent.gameObject;

            if (CurrentInteractableGOList.Contains(TargetGO))
            { 
                CurrentInteractableGOList.Remove(TargetGO); 
            }
        }
    }

    #endregion
}



