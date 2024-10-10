
public class BaseUpgradeController : BuildingController, IInteract
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
        MainGameUIManager.Instance.BaseUpgrade_UIController.OpenThisPanel(MainGameUIManager.Instance.BaseUpgrade_UIController.TabDurTime);
    }

    #endregion
}
