using UnityEngine;

public class RepairOperatorController : OperatorController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Repair")]

    [Space(10)]
    [Header("=== Build")]
    [SerializeField] protected DestructibleBuildController TargetBuildController;

    // Value
    [HideInInspector] private int Pay = 50;
    [HideInInspector] private int UseAmount = 1;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        PayTxt.text = Get_NeedPay().ToString();
    }

    #endregion

    #region Get

    private int Get_NeedPay()
    {
        return (Pay * UseAmount);
    }

    #endregion

    #region Set

    protected override void Set_AnimValue()
    {
        base.Set_AnimValue();

        IconStateAnim.Set_Anim(new State_Anim(ResourceManager.Instance.Operator_RepairAC, 1f), 1f);
    }

    public void Set_TargetBuild(DestructibleBuildController _TargetBuild)
    {
        TargetBuildController = _TargetBuild;
        _TargetBuild.RepairOper = this;
    }

    #endregion

    #region Is

    public bool Can_Interact()
    {
        return !TargetBuildController.IsBroken;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool _CanInteract)
    {
        _CanInteract = Can_Interact();
        return ResourceManager.Instance.Get_StaticWord(56);
    }

    public override void Play_Interact()
    {
        if (TargetBuildController == null ||
            TargetBuildController.Is_MaxDur() ||
            TargetBuildController.IsBroken ||
            PlayerManager.Instance.PlayerController.CurrentCredit.Value < Get_NeedPay()) return;

        // 소비 아이템
        PlayerManager.Instance.PlayerController.Add_CurrentCredit(-Get_NeedPay());
        UseAmount++;

        // 내구도 회복
        TargetBuildController.Set_Repair();

        // Pay
        PayTxt.text = Get_NeedPay().ToString();

        // Play
        TargetBuildController.Play_Size();

        SoundManager.Instance.Play_2D_SFX_Build("Repair");
    }

    #endregion
}
