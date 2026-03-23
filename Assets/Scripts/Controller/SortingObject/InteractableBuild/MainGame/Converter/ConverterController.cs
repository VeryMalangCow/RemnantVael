using UnityEngine;

public abstract class ConverterController : SortingObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> PremiumCredit ")]

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] protected Animator at;

    // AC
    [HideInInspector] private EachConverterReso eachConverterReso = null;

    [HideInInspector] protected AnimatorOverrideController aoc;

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
        eachConverterReso = ResourceManager.instance.Get_ConverterReso(id);
    }

    #endregion

    #region Anim

    private void Set_ArtVisual()
    {
        DevTool.Set_Anim(ref aoc, at, eachConverterReso.ac);
        thisSr.material = eachConverterReso.material;
    }

    #endregion

}
