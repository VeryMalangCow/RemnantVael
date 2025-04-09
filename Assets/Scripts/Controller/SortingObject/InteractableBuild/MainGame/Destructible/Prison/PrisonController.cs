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
    [SerializeField] public PrisonPuzzleOperatorController PuzzleOper;
    [SerializeField] public PrisonPayOperatorController PayOper;

    // Grade
    [HideInInspector] private int MaxRating = 4;

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

    private void Set_AnimValue()
    {
        OnOffAC = UnitManager.Instance.Prison_OnOffAC;
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
