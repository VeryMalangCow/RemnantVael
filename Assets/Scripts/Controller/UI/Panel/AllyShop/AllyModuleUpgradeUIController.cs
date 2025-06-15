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

    public override bool Try_Interact()
    {
        if (base.Try_Interact()) return false;
        if (Is_Interact_CloseBtn()) return true;


        return false;
    }

    private bool Is_Interact_CloseBtn()
    {
        if (CurrentBtn == CloseBtn)
        {
            MainGameUIManager.Instance.AllyModuleUpgrade_UIController.SetOff_ThisPanel();
            return true;
        }
        return false;
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
