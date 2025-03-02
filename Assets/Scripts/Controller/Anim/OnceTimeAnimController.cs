using System.Collections.Generic;
using UnityEngine;

public class OnceTimeAnimController : MonoBehaviour
{
    #region Value

    [SerializeField] private Animator ThisAnimator;
    [SerializeField] private SpriteRenderer ThisSpriteRenderer;
    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    #region Framework

    private void Update()
    {
        if (ThisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        {
            End_Anim();
        }
    }

    #endregion

    #region Anim

    public void Start_Anim(AnimationClip _AC, Vector2 _SpawnedPos, Material _Material, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        Start_Anim(_AC, _SpawnedPos, _Material, Quaternion.identity, _AnimSpeed, _AnimSize);
    }

    public void Start_Anim(AnimationClip _AC, Vector2 _SpawnedPos, Material _Material, Quaternion _Rotation, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        Start_Anim(_AC, _SpawnedPos, _Material, Color.white, _Rotation, _AnimSpeed, _AnimSize);
    }

    public void Start_Anim(AnimationClip _AC, Vector2 _SpawnedPos, Material _Material, Color _Clr, Quaternion _Rotation, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        aoc = new AnimatorOverrideController(ThisAnimator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _AC));
        aoc.ApplyOverrides(anims);
        ThisAnimator.runtimeAnimatorController = aoc;

        ThisAnimator.speed = _AnimSpeed;
        ThisSpriteRenderer.material = _Material;
        ThisSpriteRenderer.color = _Clr;

        this.gameObject.transform.rotation = _Rotation;
        this.gameObject.transform.localScale = Vector2.one * _AnimSize;
        this.gameObject.transform.position = _SpawnedPos;

        this.gameObject.SetActive(true);
    }

    private void End_Anim()
    {
        ThisAnimator.speed = 0f;
        this.gameObject.SetActive(false);
        if (aoc != null)
        { aoc = null; }
        PoolingManager.Instance.OnlyOnceAnimators.Queue.Enqueue(this);
    }

    #endregion
}
