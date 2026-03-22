using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TabBtnEUIController : OwnBtnEUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Tab Btn")]

    [Space(10)]
    [Header("=== Visual")]
    [FormerlySerializedAs("ThisCG")][SerializeField] private CanvasGroup cg;
    [FormerlySerializedAs("ToggleOnAlpha")][SerializeField] private CoupleData<float> toggleOnAlpha;
    [FormerlySerializedAs("ThisTxt")][SerializeField] public TMP_Text txt;
    [FormerlySerializedAs("ThisImg")][SerializeField] public Image img;
    

    #endregion

    #region Offset

    public void Offset_Txt(string txt)
    {
        this.txt.text = txt;
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

    private void Toggle_ThisBtn(bool isOn)
    {
        cg.alpha = toggleOnAlpha.Get_Special(isOn);
    }


    #endregion
}
