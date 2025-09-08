using UnityEngine;

public abstract class ConverterController : SortingObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> PremiumCredit ")]

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] protected Animator ThisAnimator;

    // AC
    [HideInInspector] private EachConverterReso EachConverterReso = null;

    [HideInInspector] protected AnimatorOverrideController AOC;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_Anim();
        Set_ArtVisual();
    }

    private void Offset_Anim()
    {
        EachConverterReso = UnitManager.Instance.ConverterReso.ConverterResoList[ID];
    }

    #endregion

    #region Anim

    private void Set_ArtVisual()
    {
        DevTool.Set_Anim(ref AOC, ThisAnimator, EachConverterReso.AC);
        ThisSR.material = EachConverterReso.Material;
    }

    #endregion

}
