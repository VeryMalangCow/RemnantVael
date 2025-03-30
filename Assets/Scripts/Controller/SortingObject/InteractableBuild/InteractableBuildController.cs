using UnityEngine;

public class InteractableBuildController : SortingObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Building")]

    [Space(10)]
    [Header("=== Controller")]
    [SerializeField] protected StateAnimController ThisStateAnim;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] protected bool IsOn = false;
    [SerializeField] protected Animator ThisAnimator;

    // Anim
    [HideInInspector] protected CoupleData<AnimationClip> OnOffAC = null;
    [HideInInspector] protected CoupleData<AnimationClip> OnOffStateAC = null;

    [HideInInspector] protected AnimatorOverrideController AOC;

    #endregion

    #region Set Anim

    protected virtual void Set_StateAnim()
    {
        DevTool.Set_Anim(ref AOC, ThisAnimator, OnOffAC.Get_Special(IsOn));
        ThisStateAnim.Set_Anim(new State_Anim(OnOffStateAC.Get_Special(IsOn), 1f), 1f);
    }

    #endregion

    #region Sorting

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);

        ThisStateAnim.ThisSR.sortingOrder = _SortingOrder;
    }

    #endregion
}
