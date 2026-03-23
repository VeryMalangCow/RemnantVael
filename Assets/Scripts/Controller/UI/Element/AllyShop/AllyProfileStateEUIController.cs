using TMPro;
using UnityEngine;

public class AllyProfileStateEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> State EUI")]
    [SerializeField] public TMP_Text nameTxt;
    [SerializeField] public TMP_Text valueTxt;
    [SerializeField] public TMP_Text extraValueTxt;

    #endregion

    #region Offset

    public override void Offset()
    {

    }

    #endregion
}
