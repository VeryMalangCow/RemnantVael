using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EachTunerEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Each Tuner")]

    [Space(10)]
    [Header("=== Main Comp")]
    [SerializeField] private Image ThisImg;
    [SerializeField] private Image ThisRankImg;

    [Space(10)]
    [Header("=== Sub Comp")]
    [SerializeField] private Image ThisFrameImg;
    [SerializeField] private Image ThisArrowImg;

    [HideInInspector] private RectTransform ThisRT;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
    }

    #endregion

    #region Set

    public void Set_UI(AllyEachTunerData _EachTunerData)
    {
        ThisImg.sprite = AllyManager.Instance.Get_BUIcon(_EachTunerData.Type); // Icon
        ThisRankImg.sprite = ModuleItemManager.Instance.Get_CorrectRankIcon(_EachTunerData.Rank); // Rank

        Color frameClr = UnitManager.Instance.AllyCardColorList[_EachTunerData.Rank - 1];
        ThisFrameImg.color = frameClr;
        ThisArrowImg.color = frameClr;
    }

    public void Set_UI(AllyEachBaseTunerData _EachTunerData)
    {
        ThisImg.sprite = AllyManager.Instance.Get_BUIcon(_EachTunerData.Type); // Icon
        ThisRankImg.sprite = ModuleItemManager.Instance.Get_CorrectRankIcon(_EachTunerData.Rank); // Rank

        Color frameClr = UnitManager.Instance.AllyCardColorList[_EachTunerData.Rank - 1];
        ThisFrameImg.color = frameClr;
        ThisArrowImg.color = frameClr;
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

    #endregion
}
