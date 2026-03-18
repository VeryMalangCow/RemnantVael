using System;
using TMPro;
using UnityEngine;

public class InfoEUIController : OwnBtnEUIController
{
    #region Value

    [SerializeField] private TMP_Text NameTxt;

    #endregion

    #region Set Language

    public void Set_LanguageTxt()
    {
        NameTxt.text = ResourceManager.instance.Get_InfoName(Convert.ToInt32(this.gameObject.name));
    }

    #endregion

}
