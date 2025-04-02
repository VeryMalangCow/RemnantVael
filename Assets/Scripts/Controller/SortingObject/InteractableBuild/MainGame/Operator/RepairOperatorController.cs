using UnityEngine;

public class RepairOperatorController : InteractableBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Operator ")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected StateAnimController IconStateAnim;

    #endregion

    #region Sorting

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);
        IconStateAnim.ThisSR.sortingOrder = _SortingOrder - 1;
    }

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_AnimValue();
        Set_StateAnim();

        base.Offset();
    }

    #endregion

    #region Set

    private void Set_AnimValue()
    {
        OnOffAC = UnitManager.Instance.Operator_OnOffAC;
        OnOffStateAC = UnitManager.Instance.Operator_LightAC;

        IconStateAnim.Set_Anim(new State_Anim(UnitManager.Instance.Operator_RepairAC, 1f), 1f);
    }

    #endregion
}
