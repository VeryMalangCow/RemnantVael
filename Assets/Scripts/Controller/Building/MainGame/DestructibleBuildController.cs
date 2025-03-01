using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBuildController : InteractableBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Destructible")]

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] private int ThisMaxDurablity = 10;
    [HideInInspector] public int ThisDurablity;
    [SerializeField] public bool IsBroken = false;
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
        Set_Durablity();
    }

    #endregion

    #region About Break

    public virtual void Take_Damage(bool _SpawnItem)
    {
        if (!IsBroken)
        {
            ThisDurablity--;
            if (ThisDurablity <= 0)
            {
                this.transform.DOShakePosition(0.7f, 0.2f, 20, 90, false, true);
                Set_Break(_SpawnItem);
            }
            else
            {
                this.transform.DOShakePosition(0.4f, 0.1f, 20, 90, false, true);
                if (_SpawnItem)
                { Gen_Item(); }
            }
            Set_DurablityAmount(ThisDurablity);
        }
        else
        {
            this.transform.DOShakePosition(0.2f, 0.05f, 10, 90, false, true);
        }
    }

    protected virtual void Set_Break(bool _SpawnItem)
    {
        IsBroken = true;
        Gen_ExplosionEffect();
        Set_StateAnim();
    }

    public virtual void Gen_Item()
    {

    }

    #endregion

    #region Anim

    protected override void Set_StateAnim()
    {
        if (!IsBroken)
        {
            base.Set_StateAnim();
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

            ThisStateAnim.Set_Anim(BrokenStateAC, 1f, 1f);
        }
    }

    #endregion

    #region Durablity


    private void Set_Durablity()
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

    private void Set_DurablityAmount(int _Durablity)
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
