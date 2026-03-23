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
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private CoupleData<float> toggleOnAlpha;
    [SerializeField] public TMP_Text txt;
    [SerializeField] public Image img;
    

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
