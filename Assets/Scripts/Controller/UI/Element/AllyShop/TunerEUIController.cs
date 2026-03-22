using UnityEngine;
using UnityEngine.Serialization;

public class TunerEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Tuner")]

    [Space(10)]
    [Header("=== EUI")]
    [FormerlySerializedAs("PositiveTuner0")][SerializeField] private EachTunerEUIController positiveTuner0;
    [FormerlySerializedAs("PositiveTuner1")][SerializeField] private EachTunerEUIController positiveTuner1;
    [FormerlySerializedAs("NegativeTuner")][SerializeField] private EachTunerEUIController negativeTuner;

    [HideInInspector] public RectTransform rt;

    #endregion

    #region Offset

    public override void Offset()
    {
        positiveTuner0.Offset();
        positiveTuner1.Offset();
        negativeTuner.Offset();

        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
    }

    #endregion

    #region Set

    public void Set_UI(AllyTunerData tunerData)
    {
        positiveTuner0.Set_UI(tunerData.positive0);
        positiveTuner1.Set_UI(tunerData.positive1);
        negativeTuner.Set_UI(tunerData.negative);
    }

    public void Set_UI(AllyBaseTunerData tunerData)
    {
        positiveTuner0.Set_UI(tunerData.positive0);
        positiveTuner1.Set_UI(tunerData.positive1);
        negativeTuner.Set_UI(tunerData.negative);
    }

    #endregion
}
