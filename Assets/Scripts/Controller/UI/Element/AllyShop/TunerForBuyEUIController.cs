using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TunerForBuyEUIController : OwnBtnEUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Tuner")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private EachTunerEUIController PositiveTuner0;
    [SerializeField] private EachTunerEUIController PositiveTuner1;
    [SerializeField] private EachTunerEUIController NegativeTuner;
    [SerializeField] public TunerRerollBtnEUIController RerollBtnEUI;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        PositiveTuner0.Offset();
        PositiveTuner1.Offset();
        NegativeTuner.Offset();

        RerollBtnEUI.Offset();
    }

    #endregion

    #region Set

    public void Set_UI(AllyTunerData _TunerData, int _NeedOverrider)
    {
        PositiveTuner0.Set_UI(_TunerData.positive0);
        PositiveTuner1.Set_UI(_TunerData.positive1);
        NegativeTuner.Set_UI(_TunerData.negative);

        RerollBtnEUI.Set_UI(_NeedOverrider);
    }

    #endregion

    #region Set (Language)

    public void Set_Language()
    {
        RerollBtnEUI.Set_Language();
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        base.OnPointerEnter(eventData);
        
        Play_Scale(1.05f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        base.OnPointerExit(eventData);

        Play_Scale(1f);
    }


    #endregion

    #region Play

    public Sequence Play_Scale(float _Size, float _DurTime = 0.05f)
    {
        DevTool.Set_KillTween(ThisRT);

        Sequence seq = DOTween.Sequence();

        seq.Append(ThisRT.DOScale(_Size, _DurTime));

        return seq;
    }

    public Sequence Play_ScaleElements(float _Size, float _DurTime = 0.05f)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(PositiveTuner0.Play_Scale(_Size, _DurTime));
        seq.Join(PositiveTuner1.Play_Scale(_Size, _DurTime));
        seq.Join(NegativeTuner.Play_Scale(_Size, _DurTime));

        return seq;
    }

    #endregion
}
