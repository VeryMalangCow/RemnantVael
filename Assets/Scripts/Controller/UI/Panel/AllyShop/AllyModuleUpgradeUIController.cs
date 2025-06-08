using UnityEngine;

public class AllyModuleUpgradeUIController : AllyShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Module Upgrade Shop")]

    [Space(10)]
    [Header("=== Cell")]
    [SerializeField] private int test = 1;

    #endregion

    #region - Hide

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Set_LanguageTxt();
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {

    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Label
        LabelName = ResourceManager.Instance.Get_StaticWord(95) + " " + ResourceManager.Instance.Get_StaticWord(27) + " " + ResourceManager.Instance.Get_StaticWord(2);
        LabelTxt.text = LabelName;

        base.Set_LanguageTxt();
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        // Dur
        ThisDurEUI.Set_Dur(AllyModuleUpgradeController.UsingShop.CurrentDur);
    }

    #endregion
}
