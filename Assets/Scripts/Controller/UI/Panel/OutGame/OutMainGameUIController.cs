using DG.Tweening;
using UnityEngine;

public class OutMainGameUIController : PanelUIController
{
    #region Value
    [Space(20)]
    [Header("<><><><><> Out Main Game UI")]

    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private OwnBtnEUIController ResumeBtn;
    [SerializeField] private OwnBtnEUIController OptionBtn;
    [SerializeField] private OwnBtnEUIController QuitBtn;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ResumeBtn.Offset();
        ResumeBtn.OwnerUIController = this;
        OptionBtn.Offset();
        OptionBtn.OwnerUIController = this;
        QuitBtn.Offset();
        QuitBtn.OwnerUIController = this;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        // Base Btns
        if (CurrentBtn == ResumeBtn)
        {
            SetOff_ThisPanel();
        }
        else if (CurrentBtn == OptionBtn)
        {

        }
        else if (CurrentBtn == QuitBtn)
        {
            LoadingSceneManager.Instance.Play_LoadScene("TitleLobby");
        }
    }

    #endregion
}
