using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AllyProfileStateEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> State EUI")]
    [FormerlySerializedAs("NameTxt")][SerializeField] public TMP_Text nameTxt;
    [FormerlySerializedAs("ValueTxt")][SerializeField] public TMP_Text valueTxt;
    [FormerlySerializedAs("ExtraValueTxt")][SerializeField] public TMP_Text extraValueTxt;

    #endregion

    #region Offset

    public override void Offset()
    {

    }

    #endregion
}
