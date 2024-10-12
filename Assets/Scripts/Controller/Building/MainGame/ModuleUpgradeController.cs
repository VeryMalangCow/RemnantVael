
public class ModuleUpgradeController : BuildingController_OnlyPlayerLayer, IInteract
{
    #region Interact

    public void Interact()
    {
       MainGameUIManager.Instance.ModuleUpgrade_UIController.OpenThisPanel(MainGameUIManager.Instance.ModuleUpgrade_UIController.TabDurTime);
    }

    #endregion
}
