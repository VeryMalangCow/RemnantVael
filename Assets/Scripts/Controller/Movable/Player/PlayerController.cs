using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerController : MovableObject
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    #region - Combat

    [Space(10)]
    [Header("=== Combat")]

    [Header("-- State")]
    [SerializeField] private eCombatMode CombatMode = eCombatMode.Physics;
    [SerializeField] private eCombatMode TargetCombatMode = eCombatMode.Physics;
    [SerializeField] public bool IsCasting = false;
    [SerializeField] private float CurrentCastingTime = 0;
    [SerializeField] private float TargetCastingTime = 0;
    [SerializeField] public BaseUpgradeState<float> AvoidChance;

    [Header("-- Energy")]
    [SerializeField] public BaseUpgradeState<float> MaxEP;
    [SerializeField] public ReactiveProperty<float> CurrentEP = new();
    [SerializeField] public BaseUpgradeState<float> SpawnESMultiple;
    [SerializeField] public BaseUpgradeState<float> NeedEP_ForSkillMultiple;

    [Header("-- Bettery")]
    [SerializeField] public ReactiveProperty<int> CurrentBS = new();
    [SerializeField] public int NeedBS_ForMakeBC = 4;
    [SerializeField] public float NeedEP_ForMakeEC = 5;
    [SerializeField] public ReactiveProperty<int> CurrentBC = new();
    [SerializeField] public ReactiveProperty<int> CurrentEC = new();

    [Header("-- Weapon")]
    [SerializeField] public PlayerWeaponController BaseWeapon;

    #endregion

    #region -Skill

    [Space(10)]
    [Header("=== Skill")]

    [Header("-- Weapon")]
    [SerializeField] public SkillWeaponController SkillWeapon;

    #endregion

    #region - Movement

    [Space(10)]
    [Header("=== Movement")]

    [Header("-- State")]
    [SerializeField] public eMovementState MovementState = eMovementState.IdleOrWalk;
    [SerializeField] public BaseUpgradeState<float> WalkSpeed;
    [SerializeField] public BaseUpgradeState<float> WalkSpeedWhenShotMultiple;

    [Header("-- Dash")]
    [SerializeField] public PlayerDashController DashController;

    #endregion

    #region - Interact

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private List<GameObject> CurrentInteractableGOList;
    [SerializeField] private IInteract CurrentInteractable;

    #endregion

    #region - Boost

    [Space(10)]
    [Header("=== Boost")]
    [SerializeField] private int TargetBoostlv = 0;
    [SerializeField] public int MaxBoostLv = 5;
    [SerializeField] public ReactiveProperty<int> CurrentBoostLv = new();
    [SerializeField] private List<float> DecEnergyPointByLevel;
    [SerializeField] public BaseUpgradeState<float> DecEnergyPointMultiple;

    private delegate void SkillDele();
    private SkillDele ReservationSkillDele = null;

    #endregion

    #region - Other

    [Space(10)]
    [Header("=== Main Sprite")]
    [SerializeField] public MakeAfterImage MakeAfterImage;

    #endregion

    #endregion

    #region Framework

    private void Awake()
    {
        Debug.Log("Test Set EC");
        CurrentEC.Value = 100;
    }

    protected override void Update()
    {
        base.Update();
        BoostItemManager.Instance.ActiveSkill_Always();
    }


    private void FixedUpdate()
    {
        AlwaysCaculate();
        Movement();
    }

    #endregion

    #region Energy

    public void IncreaseMaxEP(float _AddValue)
    {
        MaxEP.ActualState.Value += _AddValue;
        AddCurrentEP(_AddValue);
    }

    public void AddCurrentEP(float _AddValue)
    {
        float result = CurrentEP.Value + _AddValue;
        result = Math.Max(result, 0);
        result = Math.Min(result, MaxEP.ActualState.Value);

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
                MainGameUIManager.Instance.PlayerHUD_UIController.CurrentEmptyBC.Complete(0.3f, 0.2f);
            }
            else
            {
                break;
            }
        }
    }

    private void ChargeBettery()
    {
        this.CurrentEP.Value -= NeedEP_ForMakeEC;
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
                if(BaseWeapon.CurrentDelayROF >= 1)
                {
                    Walk(InputPlayerManager.Instance.InputMoveDir, WalkSpeed.ActualState.Value, AccelerationSpeed);
                }
                else
                {
                    Walk(InputPlayerManager.Instance.InputMoveDir, WalkSpeed.ActualState.Value * WalkSpeedWhenShotMultiple.ActualState.Value, AccelerationSpeed);
                }
                break;

            case eMovementState.Dash:
                DashController.Dash();
                break;

            default: break;
        }
    }


    public void CanDashCheck()
    {
        if (!CanChange() || DashController.NeedEP_ForDash * NeedEP_ForSkillMultiple.ActualState.Value >= CurrentEP.Value)
        {
            return;
        }

        InputPlayerManager.Instance.IsPlayingSkill = true;
        MakeAfterImage.StartGen(0.03f, 0.5f);
        CurrentEP.Value -= DashController.NeedEP_ForDash * NeedEP_ForSkillMultiple.ActualState.Value;
        MovementState = eMovementState.Dash;
    }

    #endregion

    #region Combat

    private void Damaged()
    {
        if(UnityEngine.Random.Range(0f, 1f) < AvoidChance.ActualState.Value)
        {

        }
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
    [HideInInspector] private const float CombatModeInterval = 0.5f;
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

        StartCasting(CombatModeInterval);
    }


    // + Not over the Max Boost Level
    [HideInInspector] private const float BoostModeInterval = 0.25f;
    public void CanChange_BoostModeCheck()
    {
        if (!CanChange() || TargetBoostlv >= MaxBoostLv)
        { return; }

        TargetBoostlv++;
        
        StartCasting(BoostModeInterval);
    }


    // + BoostLevel != 0
    [HideInInspector] private const float UnBoostModeInterval = 0.1f;
    public void CanChange_UnBoostModeCheck()
    {
        if (!CanChange() || CurrentBoostLv.Value <= 0)
        { return; }

        TargetBoostlv--;

        StartCasting(UnBoostModeInterval);
    }


    // + Enough EP | Enough BC
    [HideInInspector] private const float ChargeBetteryInterval = 1f;
    public void CanChange_ChargeBettery()
    {
        if (!CanChange() ||
            NeedEP_ForMakeEC >= this.CurrentEP.Value ||
            CurrentBC.Value <= 0) 
        { return; }

        ReservationSkillDele = ChargeBettery;

        StartCasting(ChargeBetteryInterval);
    }


    // + None
    [HideInInspector] public float ExecutionInterval = 0.75f;
    public void CanChange_Execution()
    {
        if (!CanChange())
        { return; }

        if (CurrentInteractable is EnemyController EC)
        {
            EC.Interact();
        }

        StartCasting(ExecutionInterval);
    }


    public void StartCasting(float _CastingTime)
    {
        TargetCastingTime = _CastingTime;
        IsCasting = true;
        MovementState = eMovementState.Casting;
        ThisRb.velocity = Vector2.zero;

        InputPlayerManager.Instance.IsPlayingSkill = true;
    }

    #endregion

    #region Interact

    public void TryInteract()
    {
        if(CurrentInteractable != null)
        {
            if (CurrentInteractable is InteractItemController IIC) // Item
            {
                CurrentInteractable.Interact();
                CurrentInteractable = null;
            }
            else if (CurrentInteractable is EnemyController EC) // Enemy
            {
                if(EC.IsLethargy)
                {
                    CanChange_Execution();
                }
                CurrentInteractable = null;
            }
            else // OneOffShopController |OR| ...
            {
                CurrentInteractable.Interact();
            }
            
        }
    }

    #endregion

    #region Caculate

    private void AlwaysCaculate()
    {
        CastingCaculate();
        BoostingCaculate(CurrentBoostLv.Value);
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
                InputPlayerManager.Instance.IsPlayingSkill = false;

                If_CombatMode();
                If_Skill();
                If_Boost();
            }
        }
    }


    private void BoostingCaculate(int _BoostLv)
    {
        if(_BoostLv > 0)
        {
            float decValue = DecEnergyPointByLevel[_BoostLv - 1] * DecEnergyPointMultiple.ActualState.Value;
            AddCurrentEP(-decValue * Time.deltaTime);
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
                    BaseWeapon.DamageType = eDamageType.Physics;
                    break;

                case eCombatMode.Energy:
                    BaseWeapon.DamageType = eDamageType.Energy;
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
        if(CurrentBoostLv.Value != TargetBoostlv)
        {
            CurrentBoostLv.Value = TargetBoostlv;
        }
    }

    #endregion

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if(_Col.gameObject.transform.parent.TryGetComponent(out IInteract II))
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
        if (_Col.gameObject.transform.parent.TryGetComponent(out IInteract II))
        {
            GameObject TargetGO = _Col.gameObject.transform.parent.gameObject;

            if (CurrentInteractableGOList.Contains(TargetGO))
            { 
                CurrentInteractableGOList.Remove(TargetGO); 
            }
        }
    }

    #endregion

    #region Collision

    private void OnCollisionEnter2D(Collision2D _Col)
    {
        if (_Col.gameObject.tag == "PushPlayer")
        {
            Vector2 pushDir = (this.transform.position - _Col.transform.position).normalized;
            ThisRb.AddForce(pushDir);
        }
    }

    #endregion
}

[System.Serializable]
public class BaseUpgradeState<T>
{
    public T BaseState;
    public ReactiveProperty<int> CurrentLevel;
    public List<T> UpgradeValueByLevelRange;
    public List<int> NeedPayByLevelRange;
    public ReactiveProperty<T> ActualState;
}