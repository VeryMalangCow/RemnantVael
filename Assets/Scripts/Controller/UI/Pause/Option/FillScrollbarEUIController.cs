using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillScrollbarEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public TMP_Text headerTxt;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private Image fillImg;
    [SerializeField] private TMP_Text valueTxt;

    [Space(10)]
    [Header("=== LR")]
    [SerializeField] public OwnBtnEUIController leftBtn;
    [SerializeField] public OwnBtnEUIController rightBtn;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainClrTmps;
    public TMP_Text[] MainClrTmps { get { return mainClrTmps; } }

    #endregion

    #region Offset

    public void Set_OwnerUIController(SinglePanelUIController ownerUIController)
    {
        leftBtn.ownerUIController = ownerUIController;
        rightBtn.ownerUIController = ownerUIController;
    }

    public override void Offset()
    {
        leftBtn.Offset();
        rightBtn.Offset();
    }

    #endregion

    #region Inc Dec

    public void Dec(Dele setFunc = null)
    {
        Set_Value(scrollbar.value - 0.05f);

        if (setFunc != null)
            setFunc();
    }

    public void Inc(Dele setFunc = null)
    {
        Set_Value(scrollbar.value + 0.05f);

        if (setFunc != null)
            setFunc();
    }

    #endregion

    #region Set

    public void Set_Value(float value)
    {
        scrollbar.value = Mathf.Clamp(value, 0, 1);
        valueTxt.text = Mathf.Round(scrollbar.value * 100).ToString();
        fillImg.fillAmount = scrollbar.value;
    }

    #endregion

    #region Get

    public float Get_Value()
    {
        return scrollbar.value;
    }

    #endregion
}
