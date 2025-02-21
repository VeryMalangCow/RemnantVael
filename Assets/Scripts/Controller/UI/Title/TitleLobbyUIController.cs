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
    [Header("=== Btns")]
    [SerializeField] private ModifyOwnEachBtn StartBtn;
    [SerializeField] private ModifyOwnEachBtn OptionBtn;
    [SerializeField] private ModifyOwnEachBtn QuitBtn;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float DurTime;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private IInteract CurrentInteractable;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        StartBtn.Offset();
        StartBtn.OwnerUIController = this;

        OptionBtn.Offset();
        OptionBtn.OwnerUIController = this;

        QuitBtn.Offset();
        QuitBtn.OwnerUIController = this;
    }

    protected override void Offset_UI()
    {
        BtnsRT.anchoredPosition = Vector2.zero;
        BGCG.alpha = 1f;
    }

    #endregion

    #region Input

    public void TitleInput()
    {
        if (CurrentBtn == null)
        { return; }

        if (CurrentBtn == StartBtn)
        { CloseThisPanel(); }
        else if (CurrentBtn == OptionBtn)
        { Debug.Log("옵션 창 키기"); }
        else if (CurrentBtn == QuitBtn)
        { Application.Quit(); }


    }

    #endregion

    #region Set Panel

    public override void OpenThisPanel()
    {
        //base.OpenThisPanel();

        if (DOTween.IsTweening("TitleUIPanel"))
        { return; }

        TitleInputManager.Instance.PlayerInput.SwitchCurrentActionMap(ThisPanelInputMapName);
        //InputManager.Instance.SetAim(false);

        TitleInputManager.Instance.InputMoveDir = Vector2.zero;

        Sequence seq = DOTween.Sequence(); 
        this.gameObject.SetActive(true);

        seq.Join(BtnsRT.DOAnchorPosX(0f, DurTime));
        seq.Join(BGCG.DOFade(1f, DurTime));

        seq.SetId("TitleUIPanel");
    }

    public override void CloseThisPanel()
    {
        //base.CloseThisPanel();

        if (DOTween.IsTweening("TitleUIPanel"))
        { return; }

        TitleInputManager.Instance.PlayerInput.SwitchCurrentActionMap("Player");
        //InputManager.Instance.SetAim(true);

        Sequence seq = DOTween.Sequence();
        seq.Join(BtnsRT.DOAnchorPosX(-500f, DurTime));
        seq.Join(BGCG.DOFade(0f, DurTime));

        seq.SetId("TitleUIPanel")
            .OnComplete(() =>
            { this.gameObject.SetActive(false); });
    }

    #endregion
}
