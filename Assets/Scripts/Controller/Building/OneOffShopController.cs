using UnityEngine;

public class OneOffShopController : BuildingController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> OneOff Shop")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private bool none = false;

    #endregion

    #region Framework

    private void Start()
    {
        
    }

    #endregion

    public void Interact()
    {
        UIManager.Instance.OneOffShopUIController.OpenThisPanel();
    }
}
