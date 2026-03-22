using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class OwnCGBtnEUIController : OwnBtnEUIController
{
    #region Value

    [FormerlySerializedAs("ThisTxt")][SerializeField] public TMP_Text txt;
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
