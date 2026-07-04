using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
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
        rankImg.sprite = StaticResourceManager.instance.ItemIcon.rankIcons[eachTunerData.rank - 1]; // Rank

        var clr = StaticResourceManager.instance.AllyPrefab.allyCardSpriteSets[eachTunerData.rank - 1].clr;
        frameImg.color = clr;
        arrowImg.color = clr;
    }

    public void Set_UI(AllyEachBaseTunerData eachTunerData)
    {
        img.sprite = AllyManager.instance.Get_BUIcon(eachTunerData.type); // Icon
        rankImg.sprite = StaticResourceManager.instance.ItemIcon.rankIcons[eachTunerData.rank - 1];; // Rank

        var clr = StaticResourceManager.instance.AllyPrefab.allyCardSpriteSets[eachTunerData.rank - 1].clr;
        frameImg.color = clr;
        arrowImg.color = clr;
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
