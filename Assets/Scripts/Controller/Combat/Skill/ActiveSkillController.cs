using UniRx;
using UnityEngine;

public class ActiveSkillController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Active Skill")]
    [Header("=== Cooltime")]
    [SerializeField] protected int maxChargeAmount = 1;
    [SerializeField] protected int currentChargeAmount = 0;
    [SerializeField] protected float currentCooltime = 0f;

    [Header("=== State")]
    [SerializeField] public Sprite icon;
    [SerializeField] public ReactiveProperty<float> needEP = new();

    [Header("=== BU State")]
    [SerializeField] public BUState<float> maxCooltime;
    [SerializeField] public BUState<int> tier;
    [SerializeField] public BUState<float> power;

    [HideInInspector] protected DepthController depthController;
    [HideInInspector] public PlayerController playerController;

    #endregion

    #region Offset

    private void Offset()
    {
        depthController = DevTool.Get_ComponentTType<DepthController>(gameObject);
        playerController = PlayerManager.instance.playerController;
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
        return maxChargeAmount <= currentChargeAmount;
    }

    // 쿨타임이 다 찼는가?
    private bool Is_FullCooltime()
    {
        return maxCooltime.actualState <= currentCooltime;
    }

    // 쿨타임 계산
    private void Caculate_Cooltime()
    {
        if (!Is_FullCharge())
        {
            currentCooltime += Time.deltaTime;

            if (Is_FullCooltime())
            {
                currentChargeAmount++;
                currentCooltime = Is_FullCharge() ? 0 : currentCooltime - maxCooltime.actualState;
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
            return 1 - (currentCooltime / maxCooltime.actualState);
        }
    }

    #endregion

    #region Active

    public bool Can_Active()
    {
        return currentChargeAmount > 0 &&
            (PlayerManager.instance.playerController.currentEp > needEP.Value * PlayerManager.instance.playerController.needEP_ForSkillMultiple.actualState) &&
            playerController.movementState == eMovementState.IdleOrWalk;
    }

    public virtual void Active_Skill()
    {
        // Consume
        currentChargeAmount--;
        playerController.AddCurrentEp(
            -(needEP.Value * PlayerManager.instance.playerController.needEP_ForSkillMultiple.actualState));

        AllyRequestManager.instance.Play_UsingSkill();
    }


    protected void Start_SkillUI()
    {
        // 스킬 라인 효과 이미지
        MainGameUIManager.instance.hud.SkillView.skillList
           [DevTool.Get_IndexInList(PlayerManager.instance.playerController.skillWeapon.skillList, this)]
           .Play_StartInnerUI(); 
        // Aim
        InputManager.instance.aimController.Set_ActivingSkill(
            DevTool.Get_IndexInList(PlayerManager.instance.playerController.skillWeapon.skillList, this),
            true);
    }

    protected void End_SkillUI()
    {
        // 스킬 라인 효과 이미지
        MainGameUIManager.instance.hud.SkillView.skillList
           [DevTool.Get_IndexInList(PlayerManager.instance.playerController.skillWeapon.skillList, this)]
           .Play_EndInnerUI();
        // Aim
        InputManager.instance.aimController.Set_ActivingSkill(
            DevTool.Get_IndexInList(PlayerManager.instance.playerController.skillWeapon.skillList, this),
            false);
    }

    #endregion
}
