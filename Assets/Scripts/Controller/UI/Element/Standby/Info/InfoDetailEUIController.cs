using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDetailEUIController : ElementUIController
{
    #region Value

    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private Image Img;
    [SerializeField] private TMP_Text DescTxt;
    [SerializeField] private TMP_Text InputOrInstrucTxt;

    [HideInInspector] private int ID = -1;

    #endregion

    #region Offset

    public override void Offset()
    {
        ID = Convert.ToInt32(this.gameObject.name);
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Set Language

    public void Set_LanguageTxt()
    {
        WordSet_Just data = ResourceManager.instance.Get_InfoDetail(ID);

        NameTxt.text = ResourceManager.instance.Get_InfoName(ID);
        Img.sprite = ResourceManager.instance.Get_InfoImg(ID);
        DescTxt.text = data.Get_Word(0).Replace("<el>", "\n").Replace("<c>", ",");
        InputOrInstrucTxt.text = data.Get_Word(1);
    }

    #endregion
}
