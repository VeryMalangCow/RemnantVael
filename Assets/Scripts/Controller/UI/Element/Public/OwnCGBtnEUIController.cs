using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnCGBtnEUIController : OwnBtnEUIController
{
    #region Value

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
