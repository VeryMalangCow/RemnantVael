
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
       UIManager.Instance.ModuleUpgrade_UIController.OpenThisPanel(UIManager.Instance.ModuleUpgrade_UIController.TabDurTime);
    }

    #endregion
}
