using System;
using TMPro;
using UnityEngine;

public class InfoDetailEUIController : ElementUIController
{
    #region Value

    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private TMP_Text DescTxt;
    [SerializeField] private TMP_Text InputOrInstrucTxt;

    #endregion

    #region Offset

    public override void Offset()
    {
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Set Language

    public void Set_LanguageTxt()
    {
        WordSet_Just data = ResourceManager.Instance.Get_InfoDetail(Convert.ToInt32(this.gameObject.name));

        NameTxt.text = ResourceManager.Instance.Get_InfoName(Convert.ToInt32(this.gameObject.name));
        DescTxt.text = data.Get_Word(0).Replace("<el>", "\n").Replace("<c>", ",");
        InputOrInstrucTxt.text = data.Get_Word(1);
    }

    #endregion
}
