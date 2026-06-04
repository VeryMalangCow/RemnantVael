using UnityEngine;

public class VaultUpgradeOperatorController : VaultOperatorController
{
    #region Value

    // Value
    [HideInInspector] private int pay = 5;

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
        return (pay * (targetVault.currentGrade + 1));
    }

    #endregion

    #region Set

    protected override void Set_AnimValue()
    {
        base.Set_AnimValue();

        iconStateAnim.Set_Anim(new State_Anim(ResourceManager.instance.operator_UpgradeAC, 1f), 1f);
    }

    public override void Set_TargetBuild(VaultController targetVault)
    {
        base.Set_TargetBuild(targetVault);

        targetVault.upgradeOper = this;

        // Pay
        payTxt.text = Get_NeedPay().ToString();
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        //base.Get_InteractName(out bool _CanInteract);
        canInteract = Can_Interact();
        return ResourceManager.instance.Get_StaticWord(58);
    }

    public override void Play_Interact()
    {
        base.Play_Interact();

        if (targetVault == null ||
            targetVault.Is_MaxGrade() || 
            targetVault.isBroken ||
            PlayerManager.instance.playerController.currentEp < Get_NeedPay()) return;

        // 소비 아이템
        PlayerManager.instance.playerController.AddCurrentEp(-Get_NeedPay());

        // 업그레이드
        targetVault.Set_Upgrade();

        // Pay
        payTxt.text = Get_NeedPay().ToString();

        // Play
        targetVault.Play_Size();

        // Sound
        SoundManager.instance.Play_2D_SFX_Build("Enchance");
    }

    #endregion
}
