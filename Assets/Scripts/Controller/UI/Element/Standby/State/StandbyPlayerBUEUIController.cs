using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandbyPlayerBUEUIController : ElementUIController
{
    #region Value

    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private Image[] FillImgArr;

    #endregion

    #region Offset

    public override void Offset()
    {
        Set(0);
    }

    #endregion

    #region Set

    public void Set_Color(Color _Clr, Color _TxtClr)
    {
        NameTxt.color = _TxtClr;

        for (int i = 0; i < FillImgArr.Length; i++)
            FillImgArr[i].color = _Clr;
    }

    public void Set(int _Lv)
    {
        for (int i = 0; i < FillImgArr.Length; i++)
            FillImgArr[i].gameObject.SetActive(i < _Lv);
    }

    #endregion

    #region Language

    public void Set_LanguageTxt(string _Txt)
    {
        NameTxt.text = _Txt;
    }

    #endregion
}
