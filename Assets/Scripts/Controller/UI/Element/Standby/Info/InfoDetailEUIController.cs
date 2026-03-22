using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InfoDetailEUIController : ElementUIController
{
    #region Value

    [FormerlySerializedAs("NameTxt")][SerializeField] private TMP_Text nameTxt;
    [FormerlySerializedAs("Img")][SerializeField] private Image img;
    [FormerlySerializedAs("DescTxt")][SerializeField] private TMP_Text descTxt;
    [FormerlySerializedAs("InputOrInstrucTxt")][SerializeField] private TMP_Text inputOrInstrucTxt;

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
        WordSet_Just data = ResourceManager.instance.Get_InfoDetail(id);

        nameTxt.text = ResourceManager.instance.Get_InfoName(id);
        img.sprite = ResourceManager.instance.Get_InfoImg(id);
        descTxt.text = data.Get_Word(0).Replace("<el>", "\n").Replace("<c>", ",");
        inputOrInstrucTxt.text = data.Get_Word(1);
    }

    #endregion
}
