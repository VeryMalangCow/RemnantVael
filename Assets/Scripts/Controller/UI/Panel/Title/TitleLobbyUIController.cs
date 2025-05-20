using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TitleLobbyUIController : TitleSinglePanelUIController
{
    #region Value

    #region - Inspector

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

    #region - Hide

    // Value
    [HideInInspector] private bool IsStarting = false;

    #endregion

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

        IsStarting = false;
    }

    #endregion

    #region Input

    public void Try_Interact()
    {
        if (CurrentBtn == null || IsStarting)
        { return; }

        TitleInputManager.Instance.Play_MousePointerClick();

        if (CurrentBtn == StartBtn)
        {
            IsStarting = true;
            StartCoroutine(Play_Starting_Cor(2f));
        }
        else if (CurrentBtn == OptionBtn)
        { 
            Debug.Log("옵션 창 키기");
        }
        else if (CurrentBtn == QuitBtn)
        { 
            Application.Quit(); 
        }
    }

    #endregion

    #region Play

    private IEnumerator Play_Starting_Cor(float _DelayTime)
    {
        TitleLobbyUIManager.Instance.Get_JustFadeIn(_DelayTime);

        yield return new WaitForSeconds(_DelayTime + 0.2f);

        LoadingSceneManager.Instance.Play_LoadScene("MainGame");
    }

    #endregion
}
