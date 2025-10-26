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
        int index = AllyManager.StateTypeList.IndexOf(_Data.Type);
        NameTxt.text = ResourceManager.Instance.Get_TunerDescName(index);

        // Value Txt
        string valueTxt = $"{DevTool.Get_RoundFloatString((100f * AllyManager.Get_AllyTunerStateMultiple(_Data.Type) * _Data.Rank))}%</color>";
        ValueTxt.text = _IsIncrease ? valueTxt : valueTxt.Replace("+", "-");
        ValueTxt.color = ResourceManager.Instance.Get_AllyCardColor(_Data.Rank - 1);
    }

    #endregion
}
