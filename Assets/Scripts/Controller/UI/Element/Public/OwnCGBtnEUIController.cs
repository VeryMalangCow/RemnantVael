using TMPro;
using UnityEngine;

public class OwnCGBtnEUIController : OwnBtnEUIController
{
    #region Value

    [SerializeField] public TMP_Text txt;
    [HideInInspector] public CanvasGroup cg;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        cg = DevTool.Get_ComponentTType(gameObject, out CanvasGroup _cg) ? _cg : null;
    }

    #endregion

}
