using DG.Tweening;
using UnityEngine;

public class TitleLobbyUIController : TitleSinglePanelUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Title Lobby UI Controller")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform BtnsRT;
    [SerializeField] private CanvasGroup BGCG;

    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private TitleOwnBtnEUIController StartBtn;
    [SerializeField] private TitleOwnBtnEUIController OptionBtn;
    [SerializeField] private TitleOwnBtnEUIController QuitBtn;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float DurTime;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private IInteract CurrentInteractable;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        StartBtn.Offset();
        StartBtn.OwnerUIController = this;

        OptionBtn.Offset();
        OptionBtn.OwnerUIController = this;

        QuitBtn.Offset();
        QuitBtn.OwnerUIController = this;

        BtnsRT.anchoredPosition = Vector2.zero;
        BGCG.alpha = 1f;

    }

    #endregion

    #region Input

    public void Try_Interact()
    {
/*
        if (CurrentBtn == null)
        { return; }

        if (CurrentBtn == StartBtn)
        {
            Debug.Log("시작");
        }
        else if (CurrentBtn == OptionBtn)
        { 
            Debug.Log("옵션 창 키기");
        }
        else if (CurrentBtn == QuitBtn)
        { 
            Application.Quit(); 
        }
*/

    }

    #endregion

    #region Set Panel

    public void SetOn_ThisPanel()
    {
        if (DOTween.IsTweening("TitleUIPanel"))
        { return; }

        Sequence seq = DOTween.Sequence(); 
        this.gameObject.SetActive(true);

        seq.Join(BtnsRT.DOAnchorPosX(0f, DurTime));
        seq.Join(BGCG.DOFade(1f, DurTime));

        seq.SetId("TitleUIPanel");
    }

    public void SetOff_ThisPanel()
    {
        //base.CloseThisPanel();

        if (DOTween.IsTweening("TitleUIPanel"))
        { return; }

        Sequence seq = DOTween.Sequence();
        seq.Join(BtnsRT.DOAnchorPosX(-500f, DurTime));
        seq.Join(BGCG.DOFade(0f, DurTime));

        seq.SetId("TitleUIPanel")
            .OnComplete(() =>
            { this.gameObject.SetActive(false); });
    }

    #endregion
}
