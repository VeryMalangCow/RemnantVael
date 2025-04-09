using UnityEngine;

public class PrisonController : InteractableBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Prison ")]

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] public int DangerRating = 0;

    // Oper
    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public Animator ThisUpsideAT;

    [Space(10)]
    [Header("=== Operator")]
    [SerializeField] public PrisonPuzzleOperatorController PuzzleOper;
    [SerializeField] public PrisonPayOperatorController PayOper;

    // Grade
    [HideInInspector] private int MaxRating = 4;

    // AC
    [HideInInspector] private CoupleData<AnimationClip> OnOffAC_Upside;
    [HideInInspector] private AnimatorOverrideController UpsideAOC;

    #endregion


    #region Offset

    protected override void Offset()
    {
        Set_AnimValue();
        Set_Rating(0);

        base.Offset();
    }

    #endregion

    #region Set

    protected override void Set_StateAnim()
    {
        base.Set_StateAnim();

        DevTool.Set_Anim(ref UpsideAOC, ThisUpsideAT, OnOffAC_Upside.Get_Special(IsOn));
    }

    private void Set_AnimValue()
    {
        OnOffAC = UnitManager.Instance.Prison_OnOffAC;
        OnOffAC_Upside = UnitManager.Instance.Prison_OnOffUpsideAC;
        OnOffStateAC = UnitManager.Instance.Prison_StateAC;
    }

    private void Set_Rating(int _Rate)
    {
        DangerRating = Mathf.Clamp(_Rate, 0, MaxRating);

        Set_AnimValue();
        Set_StateAnim();
    }

    #endregion
}
