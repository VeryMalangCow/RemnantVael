using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class OutMainGameUIController : UIController
{
    #region Value
    [Space(20)]
    [Header("<><><><><> Out Main Game UI")]

    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private ModifyOwnEachBtn ResumeBtn;
    [SerializeField] private ModifyOwnEachBtn OptionBtn;
    [SerializeField] private ModifyOwnEachBtn QuitBtn;

    [HideInInspector] private CanvasGroup ThisCG;
    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        ResumeBtn.Offset();
        ResumeBtn.OwnerUIController = this;
        OptionBtn.Offset();
        OptionBtn.OwnerUIController = this;
        QuitBtn.Offset();
        QuitBtn.OwnerUIController = this;

    }

    protected override void Offset_UI()
    {
        if (ThisCG == null && this.TryGetComponent(out CanvasGroup cg))
        {
            ThisCG = cg;
            ThisCG.alpha = 0f;
        }
        
    }

    #endregion

    #region Interact

    public void TryInteractClick()
    {
        // Base Btns
        if (CurrentBtn == ResumeBtn)
        {
            CloseThisPanel();
        }
        else if (CurrentBtn == OptionBtn)
        {

        }
        else if (CurrentBtn == QuitBtn)
        {
            LoadingSceneManager.Instance.LoadScene("TitleLobby");
        }
    }

    #endregion

    #region Set Panel

    public void OpenThisPanel()
    {
        if (IsTweening)
        { return; }

        IsTweening = true;

        InputManager.Instance.PlayerInput.SwitchCurrentActionMap(ThisPanelInputMapName);
        MainGameUIManager.Instance.CurrentOpening_UIController = this;
        InputManager.Instance.InputMoveDir = Vector2.zero;

        this.gameObject.SetActive(true);
        ThisCG.DOFade(1f, TabDurTime)
            .OnComplete(() =>
            {
                IsTweening = false;
            });
        

    }

    public void CloseThisPanel()
    {
        if (IsTweening)
        { return; }

        IsTweening = true;

        InputManager.Instance.PlayerInput.SwitchCurrentActionMap("Player");
        MainGameUIManager.Instance.CurrentOpening_UIController = null;

        ThisCG.DOFade(0f, TabDurTime)
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                IsTweening = false;
            });
    }

    #endregion
}
