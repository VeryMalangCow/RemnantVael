using System.Collections.Generic;
using UnityEngine;

public class OnlyOnceTimeAnimation : MonoBehaviour
{
    [SerializeField] private Animator ThisAnimator;
    [SerializeField] private SpriteRenderer ThisSpriteRenderer;
    [HideInInspector] private AnimatorOverrideController aoc;

    public void StartAnim(AnimationClip _AC, Vector2 _SpawnedPos, Material _Material, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        StartAnim(_AC, _SpawnedPos, _Material, Quaternion.identity, _AnimSpeed, _AnimSize);
    }

    public void StartAnim(AnimationClip _AC, Vector2 _SpawnedPos, Material _Material, Quaternion _Rotation, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _AC));
        aoc.ApplyOverrides(anims);
        ThisAnimator.runtimeAnimatorController = aoc;

        ThisAnimator.speed = _AnimSpeed;
        ThisSpriteRenderer.material = _Material;

        this.gameObject.transform.rotation = _Rotation;
        this.gameObject.transform.localScale = Vector2.one * _AnimSize;
        this.gameObject.transform.position = _SpawnedPos;

        this.gameObject.SetActive(true);
    }

    private void EndAnim()
    {
        ThisAnimator.speed = 0f;
        this.gameObject.SetActive(false);
        if (aoc != null)
        { aoc = null; }
        PoolingManager.Instance.OnlyOnceAnimators.Queue.Enqueue(this);
    }

    private void Update()
    {
        if (ThisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        {
            EndAnim();
        }
    }
}
