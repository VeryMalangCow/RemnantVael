
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region Value

    [Header("=== Controller")]
    [SerializeField] public PlayerHUDController PlayerHUDController;
    [SerializeField] public OneOffShopUIController OneOffShopUIController;
    [SerializeField] public UIController CurrentOpeningUIController;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion
}
