using UnityEngine;

public class VaultOperatorController : OperatorController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== Build")]
    [SerializeField] protected VaultController targetVault;

    #endregion

    #region Set

    public virtual void Set_TargetBuild(VaultController targetVault)
    {
        this.targetVault = targetVault;
    }

    #endregion

    #region Is

    public bool Can_Interact()
    {
        return !targetVault.isBroken;
    }

    #endregion

    #region Interact

    public override string Get_InteractName(out bool canInteract)
    {
        canInteract = false;
        return "";
    }

    public override void Play_Interact()
    {
        
    }

    #endregion
}
