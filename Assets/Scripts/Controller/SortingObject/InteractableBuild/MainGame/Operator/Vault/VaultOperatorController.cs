using UnityEngine;

public class VaultOperatorController : OperatorController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== Build")]
    [SerializeField] protected VaultController TargetVault;

    #endregion

    #region Set

    public virtual void Set_TargetBuild(VaultController _TargetVault)
    {
        TargetVault = _TargetVault;
    }

    #endregion

    #region Is

    public bool Can_Interact()
    {
        return !TargetVault.IsBroken;
    }

    #endregion

    #region Interact

    public override void Play_Interact()
    {
        
    }

    #endregion
}
