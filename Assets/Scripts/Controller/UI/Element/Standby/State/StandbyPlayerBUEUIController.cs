using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandbyPlayerBUEUIController : ElementUIController
{
    #region Value

    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private Image[] fillImgArr;

    #endregion

    #region Offset

    public override void Offset()
    {
        Set(0);
    }

    #endregion

    #region Set

    public void Set_Color(Color clr, Color txtClr)
    {
        nameTxt.color = txtClr;

        for (int i = 0; i < fillImgArr.Length; i++)
            fillImgArr[i].color = clr;
    }

    public void Set(int lv)
    {
        for (int i = 0; i < fillImgArr.Length; i++)
            fillImgArr[i].gameObject.SetActive(i < lv);
    }

    #endregion

    #region Language

    public void Set_LanguageTxt(string txt)
    {
        nameTxt.text = txt;
    }

    #endregion
}
