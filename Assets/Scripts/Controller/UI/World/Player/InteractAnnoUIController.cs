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

    protected override void Offset_Module()
    {
    }

    protected override void Offset_UI()
    {
        ThisCG.alpha = 0;

        Color clr = PlayerManager.Instance.PlayerController.Get_Color_CorrectHitted(eDamageType.Energy, false);
        InnerLImg.color= clr;
        InnerRImg.color= clr;
    }

    #endregion

    #region Set

    public void Set_UI()
    {
        IInteract ii = PlayerManager.Instance.PlayerController.CurrentInteractable.Value;
        string txt = Get_KindOfCaseString(ii);

        if (ii != null && txt != "")
        {
            if (ii is MonoBehaviour mb)
            {
                SetOn(txt, mb.transform.position);
            }
        }
        else
        {
            SetOff();
        }
    }

    private void SetOn(string _txt, Vector2 _Pos)
    {
        this.transform.position = _Pos;
        this.AnnoTxt.text = "< " + _txt + " >";

        this.gameObject.SetActive(true);

        DOTween.Kill(ThisCG);
        ThisCG.DOFade(1f, 0.2f);
    }

    private void SetOff()
    {
        DOTween.Kill(ThisCG);
        ThisCG.DOFade(0f, 0.2f)
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
            });
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
