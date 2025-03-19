using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabBtnEUIController : OwnBtnEUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Tab Btn")]

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private CanvasGroup ThisCG;
    [SerializeField] private CoupleData<float> ToggleOnAlpha;
    [SerializeField] public TMP_Text ThisTxt;
    [SerializeField] public Image ThisImg;
    

    #endregion

    #region Offset

    public void Offset_Txt(string _Txt)
    {
        ThisTxt.text = _Txt;
    }

    #endregion

    #region Set

    public void ToggleOff_ThisBtn()
    {
        Toggle_ThisBtn(false);
    }

    public void ToggleOn_ThisBtn()
    {
        Toggle_ThisBtn(true);
    }

    private void Toggle_ThisBtn(bool _IsOn)
    {
        ThisCG.alpha = ToggleOnAlpha.Get_Special(_IsOn);
    }


    #endregion
}
