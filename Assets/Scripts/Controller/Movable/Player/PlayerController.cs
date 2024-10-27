using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerController : MovableObject
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]
    [SerializeField] public int ID;

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

    #region - Skill

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

    #region - Lower

    [Space(10)]
    [Header("=== Lower")]
    [SerializeField] public LowerController LowerController;

    #endregion

    #region - Other

    [Space(10)]
    [Header("=== Main Sprite")]
    [SerializeField] public List<MakeAfterImage> MakeAfterImgList;

    #endregion

    #region - Effect

    [Space(10)]
    [Header("=== Effect")]
    [Header("-- Hitted")]
    [SerializeField] public AnimationClip PhysicsHittedPointAC;
    [SerializeField] public AnimationClip PhysicsCriticalHittedPointAC;
    [SerializeField] public AnimationClip EnergyHittedPointAC;
    [SerializeField] public AnimationClip EnergyCriticalHittedPointAC;

    [Header("-- DamageType Icon")]
    [SerializeField] private SetStateAnim StateAnim;
    [SerializeField] private AnimationClip PhysicsStateAC;
    [SerializeField] private AnimationClip EnergyStateAC;
    [SerializeField] private AnimationClip ChangeStateAC;
    [SerializeField] private Sprite ChangeState_DamageType;

    [Space(10)]
    [Header("-- BoostMode Icon")]
    [SerializeField] private List<SetStateAnim> BoostStateAnimList;
    [SerializeField] private AnimationClip BoostOffAC;
    [SerializeField] private AnimationClip BoostOnAC;
    [SerializeField] private List<SetStateAnim> BoostStateVFXAnimList;
    [SerializeField] private List<AnimationClip> BoostVFXAnimList;
    [SerializeField] private Sprite ChangeState_BoostUp;
    [SerializeField] private Sprite ChangeState_BoostDown;

    #endregion

    #endregion

    #region Framework

    private void Start()
    {
        CurrentEC.Value = 100;

        SetBaseAnimTween();

        StateAnim.SetAnim(PhysicsStateAC, 0.8f, 1f);
        SetBoostAnim(CurrentBoostLv.Value, MaxBoostLv);
    }

    protected override void Update()
    {
        base.Update();
        AlwaysCaculate();
        BoostItemManager.Instance.ActiveSkill_Always();
    }


    private void FixedUpdate()
    {
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
                    Walk(InputManager.Instance.InputMoveDir, WalkSpeed.ActualState.Value, AccelerationSpeed);
                }
                else
                {
                    Walk(InputManager.Instance.InputMoveDir, WalkSpeed.ActualState.Value * WalkSpeedWhenShotMultiple.ActualState.Value, AccelerationSpeed);
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

        InputManager.Instance.IsPlayingSkill = true;
        SetOnAfterImg();


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
    [HideInInspector] private const float CombatModeInterval = 0.75f;
    public void CanChange_CombatModeCheck()
    {
        if (!CanChange()) 
        { return; }

        StateAnim.SetAnim(ChangeStateAC, ChangeState_DamageType, 0.8f, 1f);

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
    [HideInInspector] private const float BoostModeInterval = 0.5f;
    public void CanChange_BoostModeCheck()
    {
        if (!CanChange() || TargetBoostlv >= MaxBoostLv)
        { return; }

        TargetBoostlv++;
        StateAnim.SetAnim(ChangeStateAC, ChangeState_BoostUp, 0.8f, 1f);

        StartCasting(BoostModeInterval);
    }


    // + BoostLevel != 0
    [HideInInspector] private const float UnBoostModeInterval = 0.25f;
    public void CanChange_UnBoostModeCheck()
    {
        if (!CanChange() || CurrentBoostLv.Value <= 0)
        { return; }

        TargetBoostlv--;
        StateAnim.SetAnim(ChangeStateAC, ChangeState_BoostDown, 0.8f, 1f);

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

    // + None
    [HideInInspector] public float Skill0Interval = 0.5f;
    public void CanChange_Skill0()
    {
        if (!CanChange() ||
            !SkillWeapon.Skill_0.CanActive())
        { return; }

        ReservationSkillDele = SkillWeapon.Skill_0.ActiveSkill;

        StartCasting(Skill0Interval);
    }

    [HideInInspector] public float Skill1Interval = 0.5f;
    public void CanChange_Skill1()
    {
        if (!CanChange() ||
            !SkillWeapon.Skill_1.CanActive())
        { return; }

        ReservationSkillDele = SkillWeapon.Skill_1.ActiveSkill;

        StartCasting(Skill1Interval);
    }

    public void StartCasting(float _CastingTime)
    {
        TargetCastingTime = _CastingTime;
        IsCasting = true;
        MovementState = eMovementState.Casting;
        ThisRb.velocity = Vector2.zero;

        InputManager.Instance.IsPlayingSkill = true;
    }

    #endregion

    #region Interact

    public void TryInteract()
    {
        if(CurrentInteractable != null && CurrentInteractableGOList.Count > 0)
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
                InputManager.Instance.IsPlayingSkill = false;

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

            switch (CombatMode)
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

        switch (TargetCombatMode)
        {
            case eCombatMode.Physics:
                StateAnim.SetAnim(PhysicsStateAC, 0.8f, 1f);
                break;

            case eCombatMode.Energy:
                StateAnim.SetAnim(EnergyStateAC, 0.8f, 1f);
                break;

            default:
                break;
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
            SetBoostAnim(CurrentBoostLv.Value, MaxBoostLv);
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

                if (CurrentInteractableGOList.Count <= 0)
                {
                    CurrentInteractable = null;
                }
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

    #region Effect

    public void SetOnAfterImg()
    {
        for (int i = 0; i < MakeAfterImgList.Count; i++)
        {
            MakeAfterImgList[i].StartGen(0.7f, 0.03f, 0.5f);
        }
    }

    public void SetOffAfterImg()
    {
        for (int i = 0; i < MakeAfterImgList.Count; i++)
        {
            MakeAfterImgList[i].EndGen();
        }
    }

    private void SetBoostAnim(int _Index, int _MaxIndex)
    {
        float lowestAnimSpeed = 0.5f;

        // Wheel
        if (_Index >= _MaxIndex)
        {
            for (int i = 0; i < _MaxIndex - 1; i++)
            {
                BoostStateAnimList[i].SetAnim(BoostOnAC, lowestAnimSpeed * (_MaxIndex * 2), 1f);
            }
        }
        else
        {
            for (int i = 0; i < _MaxIndex - 1; i++)
            {
                if (i < _Index)
                {
                    BoostStateAnimList[i].SetAnim(BoostOnAC, lowestAnimSpeed * (_Index - i + 1), 1f);
                }
                else
                {
                    BoostStateAnimList[i].SetAnim(BoostOffAC, lowestAnimSpeed, 1f);
                }
            }
        }

        // VFX
        switch (_Index)
        {
            case 0:
                BoostStateVFXAnimList[0].gameObject.SetActive(false);

                BoostStateVFXAnimList[1].gameObject.SetActive(false);
                break;

            case 1:
                BoostStateVFXAnimList[0].gameObject.SetActive(true);
                BoostStateVFXAnimList[0].SetAnim(BoostVFXAnimList[0], 0.5f, 1f);

                BoostStateVFXAnimList[1].gameObject.SetActive(false);
                break;

            case 2:
                BoostStateVFXAnimList[0].gameObject.SetActive(true);
                BoostStateVFXAnimList[0].SetAnim(BoostVFXAnimList[0], 1f, 1f);

                BoostStateVFXAnimList[1].gameObject.SetActive(false);
                break;

            case 3:
                BoostStateVFXAnimList[0].gameObject.SetActive(true);
                BoostStateVFXAnimList[0].SetAnim(BoostVFXAnimList[0], 1f, 1f);

                BoostStateVFXAnimList[1].gameObject.SetActive(true);
                BoostStateVFXAnimList[1].SetAnim(BoostVFXAnimList[1], 1f, 1f);
                break;

            case 4:
                BoostStateVFXAnimList[0].gameObject.SetActive(true);
                BoostStateVFXAnimList[0].SetAnim(BoostVFXAnimList[0], 1.5f, 1f);

                BoostStateVFXAnimList[1].gameObject.SetActive(true);
                BoostStateVFXAnimList[1].SetAnim(BoostVFXAnimList[1], 1.5f, 1f);
                break;

            default:
                break;
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

    [TextArea] 
    public string Desc;
}