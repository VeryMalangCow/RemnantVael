using DG.Tweening;
using UnityEngine;

public class InteractableBuildController : SortingObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Building")]

    [Space(10)]
    [Header("=== Controller")]
    [SerializeField] protected StateAnimController stateAnim;
    public StateAnimController StateAnim => stateAnim;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public bool isOn = false;
    [SerializeField] protected Animator at;

    // Anim
    [HideInInspector] protected CoupleData<AnimationClip> onOffAc = null;
    [HideInInspector] protected CoupleData<AnimationClip> onOffStateAc = null;

    [HideInInspector] protected AnimatorOverrideController aoc;

    #endregion

    #region Set Anim

    protected virtual void Set_StateAnim()
    {
        DevTool.Set_Anim(ref aoc, at, onOffAc.Get_Special(isOn));
        stateAnim.Set_Anim(new State_Anim(onOffStateAc.Get_Special(isOn), 1f), 1f);
    }

    #endregion

    #region Sorting

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);

        stateAnim.sr.sortingOrder = sortingOrder;
    }

    #endregion

    #region Play

    public void Play_Size()
    {
        DevTool.SetKillTween(gameObject.transform.localScale);

        gameObject.transform.localScale = Vector2.one;

        Sequence seq = DOTween.Sequence();
        seq.Append(gameObject.transform.DOScale(1.2f, 0.1f));
        seq.Append(gameObject.transform.DOScale(1f, 0.1f));
        seq.OnStart(() => { gameObject.transform.localScale = Vector2.one; })
            .OnComplete(() => { gameObject.transform.localScale = Vector2.one; });
    }

    #endregion
}
