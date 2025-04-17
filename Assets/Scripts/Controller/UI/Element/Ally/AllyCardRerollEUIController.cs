using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllyCardRerollEUIController : OwnBtnEUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private TMP_Text NeedAmountTxt;

    // Value
    [HideInInspector] private int NeedAmount = 0;

    // Owner
    [HideInInspector] public AllyCardEUIController TargetCardEUIController;

    #endregion
}
