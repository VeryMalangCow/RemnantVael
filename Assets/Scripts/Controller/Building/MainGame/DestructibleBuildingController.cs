using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBuildingController : BuildingController_OnlyPlayerLayer
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Building")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private int ThisDurablity = 5;
    [SerializeField] protected bool IsBroken = false;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private AnimationClip BrokenAC;
    [SerializeField] private AnimationClip BrokenStateAC;

    #endregion

    #region About Break

    public void TakeDamage()
    {
        if (!IsBroken)
        {
            ThisDurablity--;
            this.transform.DOShakePosition(0.4f, 0.15f, 20, 90, false, true);
            if (ThisDurablity <= 0)
            {
                Break();
            }
        }
        else
        {
            this.transform.DOShakePosition(0.2f, 0.05f, 10, 90, false, true);
        }
    }

    protected virtual void Break()
    {
        IsBroken = true;

        ApplySetStateAnim();
    }

    #endregion

    #region Anim

    protected override void ApplySetStateAnim()
    {
        if (!IsBroken)
        {
            base.ApplySetStateAnim();
        }
        else
        {
            aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, BrokenAC));
            aoc.ApplyOverrides(anims);
            ThisAnimator.runtimeAnimatorController = aoc;
            ThisAnimator.speed = 1f;

            ThisStateAnim.SetAnim(BrokenStateAC, 1f, 1f);
        }
    }

    #endregion
}
