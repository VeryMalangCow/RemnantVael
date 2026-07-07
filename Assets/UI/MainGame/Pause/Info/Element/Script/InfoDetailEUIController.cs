using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDetailEUIController : ElementUIController
{
    #region Value

    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private Image img;
    [SerializeField] private TMP_Text descTxt;
    [SerializeField] private TMP_Text inputOrInstrucTxt;

    [HideInInspector] private int id = -1;

    #endregion

    #region Offset

    public override void Offset()
    {
        id = Convert.ToInt32(this.gameObject.name);
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Set Language

    public void Set_LanguageTxt()
    {
        var reso = StaticResourceManager.instance;

        nameTxt.text = reso.infoNames.GetLanguage(id);
        img.sprite = StaticResourceManager.instance.EventReso.infoSprites[id];
        descTxt.text = reso.infoDescs.GetLanguage(id).Replace("<el>", "\n").Replace("<c>", ",");
        inputOrInstrucTxt.text = reso.infoKeys.GetLanguage(id);
    }

    #endregion
}
