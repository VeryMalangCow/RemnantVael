using System;
using TMPro;
using UnityEngine;

public class InfoEUIController : OwnBtnEUIController
{
    #region Value

    [SerializeField] private TMP_Text nameTxt;

    #endregion

    #region Set Language

    public void Set_LanguageTxt()
    {
        nameTxt.text = StaticResourceManager.instance.infoNames.GetLanguage(Convert.ToInt32(gameObject.name));
    }

    #endregion

}
