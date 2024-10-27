using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BuildingController_OnlyPlayerLayer : HaveShadowThingStatic
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Building")]

    [Space(10)]
    [Header("=== Layer")]
    [SerializeField] private SpriteRenderer TargetSR;
    [SerializeField] private SpriteRenderer ShadowSR;
    [SerializeField] private int LowestLayerOrder = -2000;
    [SerializeField] private int HighestLayerOrder = 2000;

    [SerializeField] private ReactiveProperty<bool> IsUpper = new();
    [HideInInspector] private Transform PlayerTF;

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] public MakeExplosionImage MEI;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] protected bool IsOn = false;

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private Animator ThisAnimator;
    [SerializeField] private AnimationClip OffAC;
    [SerializeField] private AnimationClip OnAC;
    [SerializeField] private SetStateAnim ThisStateAnim;
    [SerializeField] private AnimationClip OffStateAC;
    [SerializeField] private AnimationClip OnStateAC;

    [HideInInspector] private AnimatorOverrideController aoc;
    #endregion

    #region Framework

    private void Awake()
    {
        IsUpper.Value = false;
        IsUpper.Subscribe(isUp =>
        {
            SetLayerOrder();
        });
    }

    protected virtual void Start()
    {
        if (PlayerTF == null)
        {
            if (PlayerManager.Instance != null)
            {
                PlayerTF = PlayerManager.Instance.PlayerController.gameObject.transform;
            }
            else if (TitlePlayerManager.Instance != null)
            {
                PlayerTF = TitlePlayerManager.Instance.PlayerController.gameObject.transform;
            }
        }
    }


    private void Update()
    {
        SetLayerSort();
    }

    private void SetLayerOrder()
    {
        if (IsUpper.Value)
        {
            TargetSR.sortingOrder = LowestLayerOrder;
        }
        else
        {
            TargetSR.sortingOrder = HighestLayerOrder;
        }
    }

    #endregion

    #region Set

    private void SetLayerSort()
    {
        if (PlayerTF != null)
        {
            float targetY = PlayerTF.transform.position.y;
            float thisY = this.gameObject.transform.position.y;

            if (thisY > targetY)
            {
                IsUpper.Value = true;
            }
            else if (thisY < targetY)
            {
                IsUpper.Value = false;
            }
        }
    }

    #region Set Anim

    protected void ApplySetStateAnim()
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
    #endregion
}
