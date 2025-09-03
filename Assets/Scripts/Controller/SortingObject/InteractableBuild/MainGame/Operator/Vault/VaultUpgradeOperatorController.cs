using UnityEngine;

public class VaultUpgradeOperatorController : VaultOperatorController
{
    #region Value

    // Value
    [HideInInspector] private int Pay = 5;

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
        return (Pay * (TargetVault.CurrentGrade + 1));
    }

    #endregion

    #region Set

    protected override void Set_AnimValue()
    {
        base.Set_AnimValue();

        IconStateAnim.Set_Anim(new State_Anim(UnitManager.Instance.Operator_UpgradeAC, 1f), 1f);
    }

    public override void Set_TargetBuild(VaultController _TargetVault)
    {
        base.Set_TargetBuild(_TargetVault);

        _TargetVault.UpgradeOper = this;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool _CanInteract)
    {
        //base.Get_InteractName(out bool _CanInteract);
        _CanInteract = Can_Interact();
        return ResourceManager.Instance.Get_StaticWord(58);
    }

    public override void Play_Interact()
    {
        base.Play_Interact();

        if (TargetVault == null ||
            TargetVault.Is_MaxGrade() || 
            TargetVault.IsBroken ||
            PlayerManager.Instance.PlayerController.Get_CurrentEP().Value < Get_NeedPay()) return;

        // 소비 아이템
        PlayerManager.Instance.PlayerController.Add_CurrentEP(-Get_NeedPay());

        // 업그레이드
        TargetVault.Set_Upgrade();

        // Pay
        PayTxt.text = Get_NeedPay().ToString();

        // Play
        TargetVault.Play_Size();
    }

    #endregion
}
