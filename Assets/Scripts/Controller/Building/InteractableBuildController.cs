using System.Collections.Generic;
using UnityEngine;

public class InteractableBuildController : SortLayerObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Building")]

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] public ExplosionImgGenerator MEI;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected bool IsOn = false;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] protected Animator ThisAnimator;
    [SerializeField] private AnimationClip OffAC;
    [SerializeField] private AnimationClip OnAC;
    [SerializeField] protected SetStateAnim ThisStateAnim;
    [SerializeField] private AnimationClip OffStateAC;
    [SerializeField] private AnimationClip OnStateAC;

    [HideInInspector] protected AnimatorOverrideController aoc;

    #endregion

    #region Set Anim

    protected virtual void Set_StateAnim()
    {
        if (IsOn)
        {
            aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, OnAC));
            aoc.ApplyOverrides(anims);
            ThisAnimator.runtimeAnimatorController = aoc;
            ThisAnimator.speed = 1f;

            ThisStateAnim.Set_Anim(OnStateAC, 1f, 1f);
        }
        else
        {
            aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, OffAC));
            aoc.ApplyOverrides(anims);
            ThisAnimator.runtimeAnimatorController = aoc;
            ThisAnimator.speed = 1f;

            ThisStateAnim.Set_Anim(OffStateAC, 1f, 1f);
        }
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
