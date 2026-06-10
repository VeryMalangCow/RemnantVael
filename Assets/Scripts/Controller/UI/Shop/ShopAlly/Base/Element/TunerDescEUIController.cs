using TMPro;
using UnityEngine;

public class TunerDescEUIController : ElementUIController
{
    #region Value

    [SerializeField] public TMP_Text increaseTxt;

    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text valueTxt;

    #endregion

    #region Offset

    public override void Offset()
    {

    }

    #endregion

    #region Set

    public void Set_UI(AllyEachTunerData data, bool isIncrease)
    {
        // Name Txt
        int index = AllyManager.stateTypeList.IndexOf(data.type);
        nameTxt.text = ResourceManager.instance.Get_TunerDescName(index);

        // Value Txt
        string valueTxt = $"{DevTool.Get_RoundFloatString((100f * AllyManager.Get_AllyTunerStateMultiple(data.type) * data.rank))}%</color>";
        this.valueTxt.text = isIncrease ? valueTxt : valueTxt.Replace("+", "-");
        this.valueTxt.color = ResourceManager.instance.Get_AllyCardColor(data.rank - 1);
    }

    #endregion
}
