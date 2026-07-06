using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DestructibleBuildController : InteractableBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Destructible")]

    [Space(10)]
    [Header("=== Dur")]
    [SerializeField] public bool isBroken = false;
    [SerializeField] protected int maxDur = 10;
    [HideInInspector] public int currentDur;

    [Space(10)]
    [Header("=== Dur UI")]
    [SerializeField] private SortingGroup durSg;
    [SerializeField] private Transform durParentTf;
    [SerializeField] private float frameIntervalX = 0.08f;

    // State
    [HideInInspector] protected AnimationClip brokenAc = null;
    [HideInInspector] protected AnimationClip brokenStateAc = null;

    // Dur
    [HideInInspector] protected List<SpriteRenderer> durFrameSrList = new List<SpriteRenderer>();
    [HideInInspector] protected List<SpriteRenderer> durInnerSrList = new List<SpriteRenderer>();

    // Operator
    [HideInInspector] public RepairOperatorController repairOper = null;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_Durablity();
        Set_StateAnim();
    }

    private void Offset_Durablity()
    {
        currentDur = maxDur;

        Sprite frameSprite = StaticResourceManager.instance.BuildReso.durablityFrameSprite;
        Material frameMaterial = StaticResourceManager.instance.BuildReso.durabilityMaterial;

        Sprite innerSprite = StaticResourceManager.instance.BuildReso.durablityInnerSprite;
        Material innerMaterial = StaticResourceManager.instance.BuildReso.durabilityMaterial;

        for (int i = 0; i < maxDur; i++)
        {

            Gen_EachInnerUI(i, Gen_EachFrameUI(i, frameSprite, frameMaterial).transform, innerSprite, innerMaterial);
        }
    }

    #endregion

    #region Break

    public virtual void Take_Damage(bool spawnItem, bool soundOn)
    {
        if (!isBroken)
        {
            currentDur--;

            if (currentDur <= 0)
            {
                Play_NowBreak(spawnItem);
            }
            else
            {
                Play_NotYetBreak(spawnItem);
            }

            Set_DurAmount(currentDur);
        }
        else
        {
            Play_AlreadyBreak();
        }

        if (soundOn)
            SoundManager.instance.PlayBuildSfx("Damaged");
    }

    protected virtual void Play_NotYetBreak(bool spawnItem)
    {
        transform.DOShakePosition(0.4f, 0.1f, 20, 90, false, true);

        if (spawnItem) Gen_ItemWhenHitted();
    }

    protected virtual void Play_NowBreak(bool spawnItem)
    {
        transform.DOShakePosition(0.8f, 0.25f, 20, 90, false, true);

        isBroken = true;
        VfxManager.instance.build_ExplImgGenerator.Expl_Build(targetObject.gameObject.transform.position);
        Set_StateAnim();

        if (spawnItem)
        {
            Gen_ItemWhenBreak();
        }
    }

    protected virtual void Play_AlreadyBreak()
    {
        transform.DOShakePosition(0.2f, 0.05f, 10, 90, false, true);
    }

    #endregion

    #region Item

    public virtual void Gen_ItemWhenHitted() { }

    public virtual void Gen_ItemWhenBreak() { }

    #endregion

    #region Anim

    protected override void Set_StateAnim()
    {
        if (!isBroken)
        {
            base.Set_StateAnim();
        }
        else
        {
            DevTool.Set_Anim(ref aoc, at, brokenAc);
            stateAnim.Set_Anim(new State_Anim(brokenStateAc, 1f), 1f);
        }
    }

    #endregion

    #region Dur

    public void Set_DurAmount(int durablity)
    {
        for (int i = 0; i < maxDur; i++)
        {
            if (i < durablity)
                durInnerSrList[i].gameObject.SetActive(true);
            else
                durInnerSrList[i].gameObject.SetActive(false);
            
        }
    }

    #endregion

    #region Gen

    private SpriteRenderer Gen_EachFrameUI(int index, Sprite sprite, Material material)
    {
        SpriteRenderer frameSr = DevTool.Gen_Component_SR(durParentTf, "DurablityFrame_" + index, sprite, material, stateAnim.sr.sortingOrder - 1);
        Set_FrameUIPos(index, frameSr);
        return frameSr;
    }

    private SpriteRenderer Gen_EachInnerUI(int index, Transform parentTf, Sprite sprite, Material material)
    {
        SpriteRenderer innerSr = DevTool.Gen_Component_SR(parentTf, "DurablityInner_" + index, sprite, material, stateAnim.sr.sortingOrder);
        Set_InnerUIPos(innerSr);
        return innerSr;
    }

    #endregion

    #region Set

    private void Set_FrameUIPos(int index, SpriteRenderer sr)
    {
        sr.transform.localPosition = new Vector2((index * frameIntervalX) - DevTool.Get_MinusXPivot(frameIntervalX, maxDur), 0f);
        sr.sortingOrder = -1;
        durFrameSrList.Insert(0, sr);
    }

    private void Set_InnerUIPos(SpriteRenderer sr)
    {
        sr.transform.localPosition = Vector2.zero;
        sr.sortingOrder = 0;
        durInnerSrList.Insert(0, sr);
    }

    public void Set_Repair(int amount = 1)
    {
        currentDur = Mathf.Min(currentDur + amount, maxDur);
        Set_DurAmount(currentDur);
    }

    #endregion

    #region Is

    public bool Is_MaxDur()
    {
        return maxDur <= currentDur;
    }

    #endregion

    #region Sorting

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);

        durSg.sortingOrder = sortingOrder;
    }

    #endregion
}
