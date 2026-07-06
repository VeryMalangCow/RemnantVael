using UnityEngine;

public class VaultRerollOperatorController : VaultOperatorController
{
    #region Value

    // Value
    [HideInInspector] private int pay = 5;
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

        var prefab = StaticResourceManager.instance.BuildReso.vaultRerollOperPrefab;

        iconStateAnim.Set_Anim(new State_Anim(prefab.animation, 1f), 1f);
        iconStateAnim.sr.material = prefab.panelMaterial;

        annoIcon.sprite = prefab.annoIcon;
        annoIcon.material = prefab.annoMaterial;
    }
    public override void Set_TargetBuild(VaultController targetVault)
    {
        base.Set_TargetBuild(targetVault);

        targetVault.rerollOper = this;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        //base.Get_InteractName(out bool _CanInteract);
        canInteract = Can_Interact();
        return ResourceManager.instance.Get_StaticWord(57);
    }

    public override void PlayInteract()
    {
        base.PlayInteract();

        if (targetVault == null ||
            targetVault.isBroken ||
            PlayerManager.instance.playerController.overrider < Get_NeedPay()) return;

        // 소비 아이템
        PlayerManager.instance.playerController.UseOverrider(Get_NeedPay());
        useAmount++;

        // 리롤
        targetVault.Change_ToOtherVault();

        // Pay
        payTxt.text = Get_NeedPay().ToString();

        // Play
        targetVault.Play_Size();

        // Sound
        SoundManager.instance.PlayBuildSfx(transform.position, "Replacement");
    }

    #endregion
}
