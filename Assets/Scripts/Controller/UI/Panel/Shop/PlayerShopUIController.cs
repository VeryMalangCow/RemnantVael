using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShopUIController : ShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Player Shop")]

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] protected List<TMP_Text> TabSideTxtList;

    #endregion

    #region - Hide 


    #endregion

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            ThisPanelTabList[i].ThisTabBtn.Offset_Txt(TabBtnTxtList[i]);
            TabSideTxtList[i].text = TabBtnTxtList[i];
        }
    }

    #endregion
}
