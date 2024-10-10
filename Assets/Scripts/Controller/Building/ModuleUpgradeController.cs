
public class ModuleUpgradeController : BuildingController, IInteract
{
    #region Value

    /*[Space(20)]
    [Header("<><><><><> OneOff Shop")]

    [Space(10)]
    [Header("=== Value")]*/

    #endregion

    #region Interact

    public void Interact()
    {
       MainGameUIManager.Instance.ModuleUpgrade_UIController.OpenThisPanel(MainGameUIManager.Instance.ModuleUpgrade_UIController.TabDurTime);
    }

    #endregion
}
