using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TitleFillScrollbarEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [FormerlySerializedAs("HeaderTxt")][SerializeField] public TMP_Text headerTxt;
    [FormerlySerializedAs("Scrollbar")][SerializeField] private Scrollbar scrollbar;
    [FormerlySerializedAs("FillImg")][SerializeField] private Image fillImg;
    [FormerlySerializedAs("ValueTxt")][SerializeField] private TMP_Text valueTxt;

    [Space(10)]
    [Header("=== LR")]
    [FormerlySerializedAs("LeftBtn")][SerializeField] public TitleOwnBtnEUIController leftBtn;
    [FormerlySerializedAs("RightBtn")][SerializeField] public TitleOwnBtnEUIController rightBtn;

    #endregion

    #region Offset

    public void Set_OwnerUIController(TitleLobbyUIController ownerUIController)
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

    #region Color

    public List<Component> Get_InnerMainColorList()
    {
        return new List<Component>
        {
            DevTool.Get_ComponentTType<TMP_Text>(leftBtn.transform.GetChild(0).gameObject),
            DevTool.Get_ComponentTType<TMP_Text>(rightBtn.transform.GetChild(0).gameObject),
            headerTxt
        };
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
