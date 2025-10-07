using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FillScrollbarEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public TMP_Text HeaderTxt;
    [SerializeField] private Scrollbar Scrollbar;
    [SerializeField] private Image FillImg;
    [SerializeField] private TMP_Text ValueTxt;

    [Space(10)]
    [Header("=== LR")]
    [SerializeField] public OwnBtnEUIController LeftBtn;
    [SerializeField] public OwnBtnEUIController RightBtn;

    #endregion

    #region Offset

    public void Set_OwnerUIController(SinglePanelUIController _OwnerUIController)
    {
        LeftBtn.OwnerUIController = _OwnerUIController;
        RightBtn.OwnerUIController = _OwnerUIController;
    }

    public override void Offset()
    {
        LeftBtn.Offset();
        RightBtn.Offset();
    }

    #endregion

    #region Inc Dec

    public void Dec(Dele _SetFunc = null)
    {
        Set_Value(Scrollbar.value - 0.05f);

        if (_SetFunc != null)
            _SetFunc();
    }

    public void Inc(Dele _SetFunc = null)
    {
        Set_Value(Scrollbar.value + 0.05f);

        if (_SetFunc != null)
            _SetFunc();
    }

    #endregion

    #region Color

    public List<Component> Get_InnerMainColorList()
    {
        return new List<Component>
        {
            DevTool.Get_ComponentTType<TMP_Text>(LeftBtn.transform.GetChild(0).gameObject),
            DevTool.Get_ComponentTType<TMP_Text>(RightBtn.transform.GetChild(0).gameObject),
            HeaderTxt
        };
    }

    #endregion

    #region Set

    public void Set_Value(float _Value)
    {
        Scrollbar.value = Mathf.Clamp(_Value, 0, 1);
        ValueTxt.text = Mathf.Round(Scrollbar.value * 100).ToString();
        FillImg.fillAmount = Scrollbar.value;
    }

    #endregion

    #region Get

    public float Get_Value()
    {
        return Scrollbar.value;
    }

    #endregion
}
