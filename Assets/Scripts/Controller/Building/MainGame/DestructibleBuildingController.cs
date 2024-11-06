using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBuildingController : InteractableBuildingController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Destructible")]

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] private int ThisMaxDurablity = 5;
    [HideInInspector] public int ThisDurablity;
    [SerializeField] protected bool IsBroken = false;
    [SerializeField] private Transform DurablitySpriteParentTF;
    [SerializeField] private Sprite DurablityFrame;
    [SerializeField] private Sprite DurablityInner;
    [SerializeField] private Material BuildingMaterial;
    [SerializeField] private float FrameIntervalX = 0.08f;
    [HideInInspector] private List<SpriteRenderer> DurablityInnerSRList = new List<SpriteRenderer>();

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private AnimationClip BrokenAC;
    [SerializeField] private AnimationClip BrokenStateAC;

    #endregion

    #region Framework

    protected virtual void Start()
    {
        SetDurablity();
    }

    #endregion

    #region About Break

    public virtual void TakeDamage(bool _SpawnItem)
    {
        if (!IsBroken)
        {
            ThisDurablity--;
            if (ThisDurablity <= 0)
            {
                this.transform.DOShakePosition(0.7f, 0.2f, 20, 90, false, true);
                Break(_SpawnItem);
            }
            else
            {
                this.transform.DOShakePosition(0.4f, 0.1f, 20, 90, false, true);
                if (_SpawnItem)
                { SpawnItem(); }
            }
            SetDurablityAmount(ThisDurablity);
        }
        else
        {
            this.transform.DOShakePosition(0.2f, 0.05f, 10, 90, false, true);
        }
    }

    protected virtual void Break(bool _SpawnItem)
    {
        IsBroken = true;

        ApplySetStateAnim();
    }

    public virtual void SpawnItem()
    {

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

    #region Durablity


    private void SetDurablity()
    {
        ThisDurablity = ThisMaxDurablity;
        float baseMinusX = (FrameIntervalX / 2) * (ThisMaxDurablity - 1);
        for (int i = 0; i < ThisMaxDurablity; i++)
        {
            // Frame
            GameObject frame = new GameObject("DurablityFrame_" + i);
            frame.transform.SetParent(DurablitySpriteParentTF);

            SpriteRenderer frameSr = frame.AddComponent<SpriteRenderer>();
            frameSr.material = BuildingMaterial;
            frameSr.sprite = DurablityFrame;
            frameSr.sortingOrder = ThisStateAnim.ThisSR.sortingOrder - 1;

            frameSr.transform.localPosition = new Vector2((i * FrameIntervalX) - baseMinusX, 0f);


            // Inner
            GameObject inner = new GameObject("DurablityInner_" + i);
            inner.transform.SetParent(frame.transform);

            SpriteRenderer innerSr = inner.AddComponent<SpriteRenderer>();
            innerSr.material = BuildingMaterial;
            innerSr.sprite = DurablityInner;
            innerSr.sortingOrder = ThisStateAnim.ThisSR.sortingOrder;

            inner.transform.localPosition = Vector2.zero;



            DurablityInnerSRList.Insert(0, innerSr);
        }
    }

    private void SetDurablityAmount(int _Durablity)
    {
        for (int i = 0; i < ThisMaxDurablity; i++)
        {
            if (i < _Durablity)
            {
                DurablityInnerSRList[i].gameObject.SetActive(true);
            }
            else
            {
                DurablityInnerSRList[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion
}
