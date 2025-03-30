using UnityEngine;

public class ModuleVaultController : VaultController
{
    #region Offset

    protected override void Offset()
    {
        IconStateAnim.Set_Anim(new State_Anim(UnitManager.Instance.Vault_ModuleIconAC));

        base.Offset();
    }

    #endregion
}
