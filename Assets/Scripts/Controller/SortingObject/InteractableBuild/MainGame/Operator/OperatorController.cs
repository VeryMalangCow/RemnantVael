using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class OperatorController : InteractableBuildController, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Operator ")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected StateAnimController IconStateAnim;

    [SerializeField] protected SortingGroup PaySG;
    [SerializeField] protected TMP_Text PayTxt;

    #endregion

    #region Offset

    protected override void Offset()
    {
        Set_AnimValue();
        Set_StateAnim();

        base.Offset();
    }

    #endregion

    #region Sorting

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);
        IconStateAnim.ThisSR.sortingOrder = _SortingOrder - 1;

        PaySG.sortingOrder = _SortingOrder + 1;
    }

    #endregion

    #region Set

    protected virtual void Set_AnimValue()
    {
        OnOffAC = UnitManager.Instance.Operator_OnOffAC;
        OnOffStateAC = UnitManager.Instance.Operator_LightAC;
    }

    public void Set_TargetBuildBroken()
    {
        IsOn = false;
        Set_StateAnim();
        PaySG.gameObject.SetActive(false);
    }

    #endregion

    #region Interact

    public abstract void Play_Interact();

    #endregion
}
