using System.Collections.Generic;
using UnityEngine;

public class InteractableBuildingController : SortLayerObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Building")]

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] public MakeExplosionImage MEI;

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

    protected virtual void ApplySetStateAnim()
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

            ThisStateAnim.SetAnim(OnStateAC, 1f, 1f);
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

            ThisStateAnim.SetAnim(OffStateAC, 1f, 1f);
        }
    }

    #endregion
}
