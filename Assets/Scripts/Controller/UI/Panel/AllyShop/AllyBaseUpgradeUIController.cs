using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyBaseUpgradeUIController : PanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Base Upgrade Shop")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] private TMP_Text LabelTxt;

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] public DurablityEUIController ThisDurEUI;
    [SerializeField] public MessageWindowEUIController ThisMsgEUI;

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public TMP_Text BCTxt;
    [SerializeField] public TMP_Text ECTxt;
    
    [Space(10)]
    [Header("=== Close")]
    [SerializeField] private OwnBtnEUIController CloseBtn;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] public Image FrameInnerImg;
    [SerializeField] private List<TMP_Text> TabSideTxtList;

    #endregion

    #region - Hide

    // String
    [HideInInspector] public static string LabelName;
    [HideInInspector] public static List<string> TabBtnTxtList;

    #endregion

    #endregion
}
