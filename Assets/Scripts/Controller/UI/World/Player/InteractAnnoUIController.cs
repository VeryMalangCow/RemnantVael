using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractAnnoUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Interact Anno")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private CanvasGroup ThisCG;
    [SerializeField] private Image InnerLImg;
    [SerializeField] private Image InnerRImg;
    [SerializeField] private TMP_Text AnnoTxt;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisCG.alpha = 0;

        Color clr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);
        InnerLImg.color = clr;
        InnerRImg.color = clr;
    }

    #endregion

    #region Set

    public void Set_UI()
    {
        IInteract ii = PlayerManager.Instance.PlayerController.CurrentInteractable.Value;
        string txt = DevTool.Get_InteractingAnnoTxt(ii);

        if (ii != null && txt != "" && ii is MonoBehaviour mb)
        {
            this.transform.position = mb.transform.position;
            this.AnnoTxt.text = "< " + txt + " >";

            Play_FadeIn();
        }
        else
        {
            Play_FadeOut();
        }
    }

    #endregion

    #region Tween

    private void Play_FadeIn()
    {
        DevTool.Set_CompleteTween(ThisCG);
        ThisCG.DOFade(1f, 0.2f)
            .OnStart(() => { this.gameObject.SetActive(true); });
    }

    private void Play_FadeOut()
    {
        DevTool.Set_CompleteTween(ThisCG);
        ThisCG.DOFade(0f, 0.2f)
            .OnComplete(() => { this.gameObject.SetActive(false); });
    }

    #endregion

    #region Follow

    public void Set_PosIfNot(IInteract _II)
    {
        if (_II != null && 
            ((MonoBehaviour)_II).transform.position != this.transform.position)
        { 
            this.transform.position = ((MonoBehaviour)_II).transform.position; 
        }
    }

    #endregion
}
