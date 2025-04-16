using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePanelUIController : UIController
{

    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Panel")]

    [Space(10)]
    [Header("=== Input Map")]
    [SerializeField] protected string ThisPanelInputMapName;

    #endregion

    #region - Hide

    // Btn
    [HideInInspector] public OwnBtnEUIController CurrentBtn = null;

    // Visual
    [HideInInspector] public List<Component> MainColorCompList = new List<Component>();
    [HideInInspector] public List<Component> SubColorCompList = new List<Component>();

    #endregion

    #endregion
}
