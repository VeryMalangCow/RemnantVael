using TMPro;
using UnityEngine;

public class AllyProfileStateEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> State EUI")]
    [SerializeField] public TMP_Text NameTxt;
    [SerializeField] public TMP_Text ValueTxt;
    [SerializeField] public TMP_Text ExtraValueTxt;

    #endregion

    #region Offset

    public override void Offset()
    {

    }

    #endregion
}
