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
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private CanvasGroup visualCg;
    [SerializeField] private Image innerLImg;
    [SerializeField] private Image innerRImg;
    [SerializeField] private TMP_Text annoTxt;

    [SerializeField] private Color uninteractableClr;

    [HideInInspector] private Color interactableClr;

    #endregion

    #region Offset

    public override void Offset(Camera camera)
    {
        base.Offset(camera);

        Offset_ColorComp();
        cg.alpha = 0;
    }

    public void Offset_ColorComp()
    {
        Color clr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);

        interactableClr = clr;
        innerLImg.color = clr;
        innerRImg.color = clr;
    }

    #endregion

    #region Set

    public void Set_VisualCG(bool onOff, float durTime = 0.3f)
    {
        DevTool.SetKillTween(visualCg);

        visualCg.DOFade(onOff ? 1f : 0f, durTime);
    }

    public void Set_UI()
    {
        IInteract ii = PlayerManager.instance.playerController.currentInteractable.Value;
        string txt = DevTool.Get_InteractingAnnoTxt(ii, out bool canInteract);

        if (ii != null && txt != "" && ii is MonoBehaviour mb)
        {
            if (canInteract)
                Set_AnnoColor(interactableClr, true);
            else
                Set_AnnoColor(uninteractableClr, false);
            

            this.transform.position = mb.transform.position;
            this.annoTxt.text = "< " + txt + " >";

            Play_FadeIn();
        }
        else
        {
            Play_FadeOut();
        }
    }

    private void Set_AnnoColor(Color clr, bool isOn)
    {
        annoTxt.color = isOn ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
        innerLImg.color = clr;
        innerRImg.color = clr;
    }

    #endregion

    #region Tween

    private void Play_FadeIn()
    {
        DevTool.Set_CompleteTween(cg);
        cg.DOFade(1f, 0.2f)
            .OnStart(() => { this.gameObject.SetActive(true); });
    }

    private void Play_FadeOut()
    {
        DevTool.Set_CompleteTween(cg);
        cg.DOFade(0f, 0.2f)
            .OnComplete(() => { this.gameObject.SetActive(false); });
    }

    #endregion

    #region Follow

    public void Set_PosIfNot(IInteract ii)
    {
        if (ii != null && 
            ((MonoBehaviour)ii).transform.position != this.transform.position)
        { 
            this.transform.position = ((MonoBehaviour)ii).transform.position; 
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
