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
    [SerializeField] private Image img;
    [SerializeField] private Image rankImg;

    [Space(10)]
    [Header("=== Sub Comp")]
    [SerializeField] private Image frameImg;
    [SerializeField] private Image arrowImg;

    [HideInInspector] private RectTransform rt;

    #endregion

    #region Offset

    public override void Offset()
    {
        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
    }

    #endregion

    #region Set

    public void Set_UI(AllyEachTunerData eachTunerData)
    {
        img.sprite = AllyManager.instance.Get_BUIcon(eachTunerData.type); // Icon
        rankImg.sprite = ResourceManager.instance.Get_RankIcon(eachTunerData.rank); // Rank

        Color frameClr = ResourceManager.instance.Get_AllyCardColor(eachTunerData.rank - 1);
        frameImg.color = frameClr;
        arrowImg.color = frameClr;
    }

    public void Set_UI(AllyEachBaseTunerData eachTunerData)
    {
        img.sprite = AllyManager.instance.Get_BUIcon(eachTunerData.type); // Icon
        rankImg.sprite = ResourceManager.instance.Get_RankIcon(eachTunerData.rank); // Rank

        Color frameClr = ResourceManager.instance.Get_AllyCardColor(eachTunerData.rank - 1);
        frameImg.color = frameClr;
        arrowImg.color = frameClr;
    }

    #endregion

    #region Play

    public Sequence Play_Scale(float size, float durTime = 0.05f)
    {
        DevTool.SetKillTween(rt);
        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOScale(size, durTime));

        return seq;
    }

    #endregion
}
