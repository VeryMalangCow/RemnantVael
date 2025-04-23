using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumShapeColorRollCellEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Img")]
    [SerializeField] private Transform RollImgParentTF;

    #endregion

    #region - Hide

    [HideInInspector] private List<Image> RollImgList;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {

    }

    #endregion
}
