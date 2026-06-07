using UnityEngine;

public class RepairOperatorController : OperatorController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Repair")]

    [Space(10)]
    [Header("=== Build")]
    [SerializeField] protected DestructibleBuildController targetBuild;

    // Value
    [HideInInspector] private int pay = 50;
    [HideInInspector] private int useAmount = 1;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        payTxt.text = Get_NeedPay().ToString();
    }

    #endregion

    #region Get

    private int Get_NeedPay()
    {
        return (pay * useAmount);
    }

    #endregion

    #region Set

    protected override void Set_AnimValue()
    {
        base.Set_AnimValue();

        iconStateAnim.Set_Anim(new State_Anim(ResourceManager.instance.operator_RepairAC, 1f), 1f);
    }

    public void Set_TargetBuild(DestructibleBuildController targetBuild)
    {
        this.targetBuild = targetBuild;
        targetBuild.repairOper = this;
    }

    #endregion

    #region Is

    public bool Can_Interact()
    {
        return !targetBuild.isBroken;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        canInteract = Can_Interact();
        return ResourceManager.instance.Get_StaticWord(56);
    }

    public override void PlayInteract()
    {
        if (targetBuild == null ||
            targetBuild.Is_MaxDur() ||
            targetBuild.isBroken ||
            PlayerManager.instance.playerController.credit < Get_NeedPay()) return;

        // 소비 아이템
        PlayerManager.instance.playerController.GainCredit(-Get_NeedPay());
        useAmount++;

        // 내구도 회복
        targetBuild.Set_Repair();

        // Pay
        payTxt.text = Get_NeedPay().ToString();

        // Play
        targetBuild.Play_Size();

        SoundManager.instance.Play_2D_SFX_Build("Repair");
    }

    #endregion
}
