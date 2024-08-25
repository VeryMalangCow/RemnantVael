using UnityEngine;

public class OneOffShopController : BuildingController, IInteract
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
        UIManager.Instance.OneOffShopUIController.OpenThisPanel(UIManager.Instance.OneOffShopUIController.TabDurTime);
    }

    #endregion
}
