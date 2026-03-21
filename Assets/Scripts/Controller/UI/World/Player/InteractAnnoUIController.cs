using DG.Tweening;
using System.Runtime.ConstrainedExecution;
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
    [SerializeField] private CanvasGroup VisualCG;
    [SerializeField] private Image InnerLImg;
    [SerializeField] private Image InnerRImg;
    [SerializeField] private TMP_Text AnnoTxt;

    [SerializeField] private Color UninteractableColor;

    [HideInInspector] private Color InteractableColor;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_ColorComp();
        ThisCG.alpha = 0;
    }

    public void Offset_ColorComp()
    {
        Color clr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);

        InteractableColor = clr;
        InnerLImg.color = clr;
        InnerRImg.color = clr;
    }

    #endregion

    #region Set

    public void Set_VisualCG(bool _OnOff, float _DurTime = 0.3f)
    {
        DevTool.Set_KillTween(VisualCG);

        VisualCG.DOFade(_OnOff ? 1f : 0f, _DurTime);
    }

    public void Set_UI()
    {
        IInteract ii = PlayerManager.instance.playerController.currentInteractable.Value;
        string txt = DevTool.Get_InteractingAnnoTxt(ii, out bool canInteract);

        if (ii != null && txt != "" && ii is MonoBehaviour mb)
        {
            if (canInteract)
                Set_AnnoColor(InteractableColor, true);
            else
                Set_AnnoColor(UninteractableColor, false);
            

            this.transform.position = mb.transform.position;
            this.AnnoTxt.text = "< " + txt + " >";

            Play_FadeIn();
        }
        else
        {
            Play_FadeOut();
        }
    }

    private void Set_AnnoColor(Color _Clr, bool _IsOn)
    {
        AnnoTxt.color = _IsOn ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
        InnerLImg.color = _Clr;
        InnerRImg.color = _Clr;
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

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        Set_UI();
    }

    #endregion
}
