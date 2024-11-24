using UniRx;
using System.Collections.Generic;
using UnityEngine;

public class SetEightDirAnim : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Eight Dir Anim")]

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform RotationTargetTF;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] private List<AnimationClip> ThisEightACList;

    [HideInInspector] private Animator ThisAnimator;
    [HideInInspector] private ReactiveProperty<int> CurrentIndex = new();
    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    #region Framework

    private void Start()
    {
        CurrentIndex.Value = 5;
        if (TryGetComponent(out Animator animator))
        {
            ThisAnimator = animator;
        }

        CurrentIndex.Subscribe(index =>
        {
            // Anim
            aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, ThisEightACList[index]));
            aoc.ApplyOverrides(anims);
            ThisAnimator.runtimeAnimatorController = aoc;
        });
    }

    private void LateUpdate()
    {
        if (GetIndex(RotationTargetTF.localRotation.eulerAngles.y) != CurrentIndex.Value)
        {
            CurrentIndex.Value = GetIndex(RotationTargetTF.localRotation.eulerAngles.y);
        }
    }

    #endregion

    #region Anim

    public void SetAnimSpeed(float _Value)
    {
        if (ThisAnimator == null)
        { return; }

        ThisAnimator.speed = _Value;
    }

    #endregion

    #region Sprite by Angle

    private int GetIndex(float _EulerAngleY)
    {
        int index = 0;
        float angle = _EulerAngleY + 67.5f;
        angle = angle >= 360 ? angle -= 360 : angle;

        index = (int)(angle / 45);
        return index;
    }

    #endregion
}
