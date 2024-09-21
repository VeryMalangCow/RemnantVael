using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region Value

    [Header("=== Controller")]
    [SerializeField] public PlayerHUDController PlayerHUD_UIController;
    [SerializeField] public BaseUpgradeUIController BaseUpgrade_UIController;
    [SerializeField] public ModuleUpgradeUIController ModuleUpgrade_UIController;

    [SerializeField] public UIController CurrentOpening_UIController;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion
}
