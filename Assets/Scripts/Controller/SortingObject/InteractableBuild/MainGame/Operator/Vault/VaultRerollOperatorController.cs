using UnityEngine;

public class VaultRerollOperatorController : VaultOperatorController
{
    #region Value

    // Value
    [HideInInspector] private int Pay = 5;
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

        IconStateAnim.Set_Anim(new State_Anim(ResourceManager.Instance.operator_RerollAC, 1f), 1f);
    }

    public override void Set_TargetBuild(VaultController _TargetVault)
    {
        base.Set_TargetBuild(_TargetVault);

        _TargetVault.RerollOper = this;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool _CanInteract)
    {
        //base.Get_InteractName(out bool _CanInteract);
        _CanInteract = Can_Interact();
        return ResourceManager.Instance.Get_StaticWord(57);
    }

    public override void Play_Interact()
    {
        base.Play_Interact();

        if (TargetVault == null ||
            TargetVault.IsBroken ||
            PlayerManager.Instance.playerController.CurrentOverrider.Value < Get_NeedPay()) return;

        // 소비 아이템
        PlayerManager.Instance.playerController.Add_CurrentOverrider(-Get_NeedPay());
        UseAmount++;

        // 리롤
        TargetVault.Change_ToOtherVault();

        // Pay
        PayTxt.text = Get_NeedPay().ToString();

        // Play
        TargetVault.Play_Size();

        // Sound
        SoundManager.Instance.Play_2D_SFX_Build("Replacement");
    }

    #endregion
}
