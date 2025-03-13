using UniRx;
using UnityEngine;

public class ActiveSkillController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Active Skill")]
    [Header("=== Cooltime")]
    [SerializeField] protected int MaxChargeAmount = 1;
    [SerializeField] protected int CurrentChargeAmount = 0;
    [SerializeField] protected float CurrentCooltime = 0f;

    [Header("=== State")]
    [SerializeField] public Sprite ThisIcon;
    [SerializeField] public ReactiveProperty<float> NeedEP = new();

    [Header("=== BU State")]
    [SerializeField] public BUState<float> MaxCooltime;
    [SerializeField] public BUState<int> Tier;
    [SerializeField] public BUState<float> Power;

    [HideInInspector] protected DepthController DepthController;
    [HideInInspector] public PlayerController PlayerController;

    #endregion

    #region Offset

    private void Offset()
    {
        DepthController = DevTool.Get_ComponentTType<DepthController>(gameObject);
        PlayerController = PlayerManager.Instance.PlayerController;
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();
    }

    protected virtual void Update()
    {
        Caculate_Cooltime();
    }

    #endregion

    #region Cooltime

    // 최대 충전량인가?
    private bool Is_FullCharge()
    {
        return MaxChargeAmount <= CurrentChargeAmount;
    }

    // 쿨타임이 다 찼는가?
    private bool Is_FullCooltime()
    {
        return MaxCooltime.ActualState.Value <= CurrentCooltime;
    }

    // 쿨타임 계산
    private void Caculate_Cooltime()
    {
        if (!Is_FullCharge())
        {
            CurrentCooltime += Time.deltaTime;

            if (Is_FullCooltime())
            {
                CurrentChargeAmount++;
                CurrentCooltime = Is_FullCharge() ? 0 : CurrentCooltime - MaxCooltime.ActualState.Value;
            }
        }
    }

    #endregion

    #region UI

    // 현재 쿨타임 UI FillAmount 수치
    public float Get_FillAmount()
    {
        if (Is_FullCharge())
        {
            return 0;
        }
        else
        {
            return 1 - (CurrentCooltime / MaxCooltime.ActualState.Value);
        }
    }

    #endregion

    #region Active

    public bool Can_Active()
    {
        return CurrentChargeAmount > 0 &&
            (PlayerManager.Instance.PlayerController.Get_CurrentEP().Value > NeedEP.Value * PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Value) &&
            PlayerController.MovementState == eMovementState.IdleOrWalk;
    }

    public virtual void Active_Skill()
    {
        // Consume
        CurrentChargeAmount--;
        PlayerController.Add_CurrentEP(
            -(NeedEP.Value * PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Value));

        // Aim UI
        
    }


    protected void Start_SkillUI()
    {
        // 스킬 라인 효과 이미지
        MainGameUIManager.Instance.PlayerHUD_UIController.SkillList
           [DevTool.Get_IndexInList(PlayerManager.Instance.PlayerController.SkillWeapon.SkillList, this)]
           .Set_StartUI(); 
        // Aim
        InputManager.Instance.AimController.Set_ActivingSkill(
            DevTool.Get_IndexInList(PlayerManager.Instance.PlayerController.SkillWeapon.SkillList, this),
            true);
    }

    protected void End_SkillUI()
    {
        // 스킬 라인 효과 이미지
        MainGameUIManager.Instance.PlayerHUD_UIController.SkillList
           [DevTool.Get_IndexInList(PlayerManager.Instance.PlayerController.SkillWeapon.SkillList, this)]
           .Set_EndUI();
        // Aim
        InputManager.Instance.AimController.Set_ActivingSkill(
            DevTool.Get_IndexInList(PlayerManager.Instance.PlayerController.SkillWeapon.SkillList, this),
            false);
    }

    #endregion
}
