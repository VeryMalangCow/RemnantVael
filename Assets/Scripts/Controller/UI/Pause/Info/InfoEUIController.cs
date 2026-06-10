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
        nameTxt.text = ResourceManager.instance.Get_InfoName(Convert.ToInt32(this.gameObject.name));
    }

    #endregion

}
