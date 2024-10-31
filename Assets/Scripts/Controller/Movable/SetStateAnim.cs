using System.Collections.Generic;
using UnityEngine;

public class SetStateAnim : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Layer")]
    [SerializeField] private SpriteRenderer TargetSR;
    [SerializeField] private int AddSort;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private Animator ThisAnimator;
    [SerializeField] private SpriteRenderer ThisSR;
    [SerializeField] private SpriteRenderer ThisInnerSR;

    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    #region Framework

    /*private void Update()
    {
        SetLayerSort(TargetSR.sortingOrder + AddSort);
    }*/

    #endregion

    #region Anim

    public void SetAnim(AnimationClip _AC, Sprite _InnerSprite, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        SetAnim(_AC, _AnimSpeed, _AnimSize);

        ThisInnerSR.sprite = _InnerSprite;
        ThisInnerSR.gameObject.SetActive(true);
    }

    public void SetAnim(AnimationClip _AC, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        ThisInnerSR.gameObject.SetActive(false);

        aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _AC));
        aoc.ApplyOverrides(anims);
        ThisAnimator.runtimeAnimatorController = aoc;

        ThisAnimator.speed = _AnimSpeed;
        ThisSR.transform.localScale = Vector2.one * _AnimSize;
    }

    #endregion

    #region Layer

    private void SetLayerSort(int _TargetSort)
    {
        if (ThisSR.sortingOrder != _TargetSort)
        {
            ThisSR.sortingOrder = _TargetSort;
            ThisInnerSR.sortingOrder = _TargetSort;
        }
    }

    #endregion
}