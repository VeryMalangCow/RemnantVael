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
    [Header("=== Generator")]
    [SerializeField] public ExplosionImgGenerator MEI;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] protected bool IsOn = false;
    [SerializeField] protected Animator ThisAnimator;
    [SerializeField] private CoupleData<AnimationClip> OnOffAC;
    [SerializeField] private CoupleData<AnimationClip> OnOffStateAC;

    [HideInInspector] protected AnimatorOverrideController AOC;

    #endregion

    #region Set Anim

    protected virtual void Set_StateAnim()
    {
        DevTool.Set_Anim(ref AOC, ThisAnimator, OnOffAC.Get_Special(IsOn));
        ThisStateAnim.Set_Anim(OnOffStateAC.Get_Special(IsOn), 1f, 1f);
    }

    #endregion

    #region Effect

    protected void Gen_ExplosionEffect()
    {
        MEI.Gen_ExplosionImgs(
                    MEI.gameObject.transform.position,
                    32, 0.15f, 0.75f,
                    0.8f, 0.05f, 0.1f,
                    0.3f, 0.5f, 1.0f,
                    0, UnitManager.Instance.ModuleM_000_Explosion);
    }

    #endregion
}
