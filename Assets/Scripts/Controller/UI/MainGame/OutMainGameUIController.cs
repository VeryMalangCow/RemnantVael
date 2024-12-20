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
}
