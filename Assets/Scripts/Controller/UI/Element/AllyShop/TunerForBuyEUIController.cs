using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class TunerForBuyEUIController : OwnBtnEUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Tuner")]

    [Space(10)]
    [Header("=== EUI")]
    [FormerlySerializedAs("PositiveTuner0")][SerializeField] private EachTunerEUIController positiveTuner0;
    [FormerlySerializedAs("PositiveTuner1")][SerializeField] private EachTunerEUIController positiveTuner1;
    [FormerlySerializedAs("NegativeTuner")][SerializeField] private EachTunerEUIController negativeTuner;
    [FormerlySerializedAs("RerollBtnEUI")][SerializeField] public TunerRerollBtnEUIController rerollBtnEUI;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        positiveTuner0.Offset();
        positiveTuner1.Offset();
        negativeTuner.Offset();

        rerollBtnEUI.Offset();
    }

    #endregion

    #region Set

    public void Set_UI(AllyTunerData tunerData, int needOverrider)
    {
        positiveTuner0.Set_UI(tunerData.positive0);
        positiveTuner1.Set_UI(tunerData.positive1);
        negativeTuner.Set_UI(tunerData.negative);

        rerollBtnEUI.Set_UI(needOverrider);
    }

    #endregion

    #region Set (Language)

    public void Set_Language()
    {
        rerollBtnEUI.Set_Language();
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

    public Sequence Play_Scale(float size, float durTime = 0.05f)
    {
        DevTool.Set_KillTween(ThisRT);

        Sequence seq = DOTween.Sequence();

        seq.Append(ThisRT.DOScale(size, durTime));

        return seq;
    }

    public Sequence Play_ScaleElements(float size, float durTime = 0.05f)
    {
        Sequence seq = DOTween.Sequence();

        seq.Join(positiveTuner0.Play_Scale(size, durTime));
        seq.Join(positiveTuner1.Play_Scale(size, durTime));
        seq.Join(negativeTuner.Play_Scale(size, durTime));

        return seq;
    }

    #endregion
}
