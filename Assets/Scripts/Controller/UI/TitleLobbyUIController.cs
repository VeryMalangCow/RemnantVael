
using DG.Tweening;
using UnityEngine;

public class TitleLobbyUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Title Lobby UI Controller")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform BtnsRT;
    [SerializeField] private CanvasGroup BGCG;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float DurTime;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {

    }

    protected override void Offset_UI()
    {
        BtnsRT.anchoredPosition = Vector2.zero;
        BGCG.alpha = 1f;
    }

    #endregion

    #region On Off

    public override void OpenThisPanel(float _DurTime)
    {
        if (DOTween.IsTweening("TitleUIPanel"))
        { return; }

        Sequence seq = DOTween.Sequence();
        seq.Join(BtnsRT.DOAnchorPosX(0f, DurTime));
        seq.Join(BGCG.DOFade(1f, DurTime));

        seq.SetId("TitleUIPanel")
            .OnStart(() =>
            { this.gameObject.SetActive(true); });
    }

    public override void CloseThisPanel(float _DurTime)
    {
        if (DOTween.IsTweening("TitleUIPanel"))
        { return; }

        Sequence seq = DOTween.Sequence();
        seq.Join(BtnsRT.DOAnchorPosX(0f, DurTime));
        seq.Join(BGCG.DOFade(1f, DurTime));

        seq.SetId("TitleUIPanel")
            .OnComplete(() =>
            { this.gameObject.SetActive(false); });
    }

    #endregion
}
