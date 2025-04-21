using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleUIController : SinglePanelUIController
{
    #region Value

    #region - Hide

    [HideInInspector] protected CanvasGroup ThisCG;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisCG = DevTool.Get_ComponentTType(gameObject, out CanvasGroup cg) ? cg : null;
    }

    #endregion
}
