using TMPro;
using UnityEngine;

public class OwnCGBtnEUIController : OwnBtnEUIController
{
    #region Value

    [SerializeField] public TMP_Text ThisTxt;
    [HideInInspector] public CanvasGroup ThisCG;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisCG = DevTool.Get_ComponentTType(gameObject, out CanvasGroup cg) ? cg : null;
    }

    #endregion

}
