using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBuildController : InteractableBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Destructible")]

    [Space(10)]
    [Header("=== Dur")]
    [SerializeField] public bool IsBroken = false;
    [SerializeField] private int MaxDur = 10;
    [HideInInspector] public int CurrentDur;

    [Space(10)]
    [Header("=== Dur UI")]
    [SerializeField] private Transform DurParentTF;
    [SerializeField] private Material BuildingMaterial;
    [SerializeField] private float FrameIntervalX = 0.08f;
    [HideInInspector] private List<SpriteRenderer> DurInnerSRList = new List<SpriteRenderer>();

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private AnimationClip BrokenAC;
    [SerializeField] private AnimationClip BrokenStateAC;

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
        CurrentDur = MaxDur;
        for (int i = 0; i < MaxDur; i++)
        {
            Gen_EachInnerUI(i, Gen_EachFrameUI(i).transform);
        }
    }

    #endregion

    #region Break

    public virtual void Take_Damage(bool _SpawnItem)
    {
        if (!IsBroken)
        {
            CurrentDur--;

            if (CurrentDur <= 0)
            {
                Play_NowBreak(_SpawnItem);
            }
            else
            {
                Play_NotYetBreak(_SpawnItem);
            }

            Set_DurAmount(CurrentDur);
        }
        else
        {
            Play_AlreadyBreak();
        }
    }

    protected virtual void Play_NotYetBreak(bool _SpawnItem)
    {
        transform.DOShakePosition(0.4f, 0.1f, 20, 90, false, true);
        if (_SpawnItem)
        {
            Gen_ItemWhenHitted();
        }
    }

    protected virtual void Play_NowBreak(bool _SpawnItem)
    {
        transform.DOShakePosition(0.8f, 0.25f, 20, 90, false, true);

        IsBroken = true;
        Gen_ExplosionEffect();
        Set_StateAnim();

        if (_SpawnItem)
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
        if (!IsBroken)
        {
            base.Set_StateAnim();
        }
        else
        {
            DevTool.Set_Anim(ref AOC, ThisAnimator, BrokenAC);
            ThisStateAnim.Set_Anim(BrokenStateAC, 1f, 1f);
        }
    }

    #endregion

    #region Dur

    private void Set_DurAmount(int _Durablity)
    {
        for (int i = 0; i < MaxDur; i++)
        {
            if (i < _Durablity)
            {
                DurInnerSRList[i].gameObject.SetActive(true);
            }
            else
            {
                DurInnerSRList[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Gen

    private SpriteRenderer Gen_EachFrameUI(int _Index)
    {
        SpriteRenderer frameSr = DevTool.Gen_Component_SR(
                DurParentTF,
                "DurablityFrame_" + _Index,
                UnitManager.Instance.BuildingDurFrame,
                BuildingMaterial,
                ThisStateAnim.ThisSR.sortingOrder - 1);

        Set_FrameUIPos(_Index, frameSr);
        return frameSr;
    }

    private SpriteRenderer Gen_EachInnerUI(int _Index, Transform _ParentTF)
    {
        SpriteRenderer innerSr = DevTool.Gen_Component_SR(
                _ParentTF,
                "DurablityInner_" + _Index,
                UnitManager.Instance.BuildingDurInner,
                BuildingMaterial,
                ThisStateAnim.ThisSR.sortingOrder);
        Set_InnerUIPos(innerSr);
        return innerSr;
    }

    #endregion

    #region Set

    private void Set_FrameUIPos(int _Index, SpriteRenderer _SR)
    {
        _SR.transform.localPosition = new Vector2((_Index * FrameIntervalX) - DevTool.Get_MinusXPivot(FrameIntervalX, MaxDur), 0f);
    }

    private void Set_InnerUIPos(SpriteRenderer _SR)
    {
        _SR.transform.localPosition = Vector2.zero;
        DurInnerSRList.Insert(0, _SR);
    }

    #endregion
}
