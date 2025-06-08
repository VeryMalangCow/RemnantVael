using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyShopUIController : ShopUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Ally Shop")]

    [Space(10)]
    [Header("=== Ally List")]
    [SerializeField] private List<string> AllyNameList;

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();


    }

    #endregion
}
