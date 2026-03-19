using System;
using TMPro;
using UnityEngine;

public class TunerDescEUIController : ElementUIController
{
    #region Value

    [SerializeField] public TMP_Text IncreaseTxt;

    [SerializeField] private TMP_Text NameTxt;
    [SerializeField] private TMP_Text ValueTxt;

    #endregion

    #region Offset

    public override void Offset()
    {

    }

    #endregion

    #region Set

    public void Set_UI(AllyEachTunerData _Data, bool _IsIncrease)
    {
        // Name Txt
        int index = AllyManager.stateTypeList.IndexOf(_Data.type);
        NameTxt.text = ResourceManager.instance.Get_TunerDescName(index);

        // Value Txt
        string valueTxt = $"{DevTool.Get_RoundFloatString((100f * AllyManager.Get_AllyTunerStateMultiple(_Data.type) * _Data.rank))}%</color>";
        ValueTxt.text = _IsIncrease ? valueTxt : valueTxt.Replace("+", "-");
        ValueTxt.color = ResourceManager.instance.Get_AllyCardColor(_Data.rank - 1);
    }

    #endregion
}
