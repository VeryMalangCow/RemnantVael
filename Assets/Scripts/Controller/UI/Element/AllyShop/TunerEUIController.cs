using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunerEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Tuner")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private EachTunerEUIController PositiveTuner0;
    [SerializeField] private EachTunerEUIController PositiveTuner1;
    [SerializeField] private EachTunerEUIController NegativeTuner;

    [HideInInspector] public RectTransform ThisRT;

    #endregion

    #region Offset

    public override void Offset()
    {
        PositiveTuner0.Offset();
        PositiveTuner1.Offset();
        NegativeTuner.Offset();

        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
    }

    #endregion

    #region Set

    public void Set_UI(AllyTunerData _TunerData)
    {
        PositiveTuner0.Set_UI(_TunerData.Positive0);
        PositiveTuner1.Set_UI(_TunerData.Positive1);
        NegativeTuner.Set_UI(_TunerData.Negative);
    }

    public void Set_UI(AllyBaseTunerData _TunerData)
    {
        PositiveTuner0.Set_UI(_TunerData.Positive0);
        PositiveTuner1.Set_UI(_TunerData.Positive1);
        NegativeTuner.Set_UI(_TunerData.Negative);
    }

    #endregion
}
