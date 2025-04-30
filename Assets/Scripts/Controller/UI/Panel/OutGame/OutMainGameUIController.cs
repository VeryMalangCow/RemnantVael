using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutMainGameUIController : SinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Out Main Game UI")]

    [Space(10)]
    [Header("=== Inner")]
    [SerializeField] private Transform InnerParentTF;
    [SerializeField] private Transform BaseInteractingPanelInnerParentTF;
    [SerializeField] private Image ResumeInnerImg;
    [SerializeField] private Image OptionInnerImg;
    [SerializeField] private Image QuitInnerImg;

    [Space(10)]
    [Header("=== Btn")]
    [SerializeField] private OwnBtnEUIController ResumeBtn;
    [SerializeField] private OwnBtnEUIController OptionBtn;
    [SerializeField] private OwnBtnEUIController QuitBtn;

    [Space(10)]
    [Header("=== Img")]
    [SerializeField] private Image BasePanelBtnImg;

    [Space(20)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text BasePanelBtnTxt;
    [SerializeField] private TMP_Text BaseInteractingPanelTxt;

    [Space(10)]
    [Header("=== Base")]
    [SerializeField] private RectTransform BasePanelRT;
    [SerializeField] private CanvasGroup BaseInteractingPanelCG;

    [Space(10)]
    [Header("=== Option")]
    [SerializeField] private RectTransform OptionPanelRT;
    [SerializeField] private OwnBtnEUIController OptionBackBtn;
    [SerializeField] private OwnBtnEUIController OptionApplyBtn;
    [SerializeField] private TMP_Text OptionWarningTxt;

    [Space(10)]
    [Header("=== Option Element")]
    [SerializeField] private LRSlidingItemEUIController LanguagePanelEUI;

    #endregion

    #region - Hide

    // Inner
    [HideInInspector] private List<Image> InnerImgs;

    // Panel
    [HideInInspector] private float InteractBasePanelPosX;
    [HideInInspector] private float InteractOptionPanelPosX;

    [HideInInspector] private bool IsInteractTweening = false;
    [HideInInspector] private OutMainGameUIType CurrentType = OutMainGameUIType.BasePanel;


    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_EUI();
        Offset_Btn(); 
        Offset_Txt();
        Offset_ColorComp();
        Offset_PosValue();
    }

    private void Offset_EUI()
    {
        OptionBackBtn.OwnerUIController = this;
        OptionBackBtn.Offset();

        OptionApplyBtn.OwnerUIController = this;
        OptionApplyBtn.Offset();

        LanguagePanelEUI.Set_OwnerUIController(this);
        LanguagePanelEUI.Offset();
    }

    private void Offset_Btn()
    {
        ResumeBtn.OwnerUIController = this;
        OptionBtn.OwnerUIController = this;
        QuitBtn.OwnerUIController = this;

        ResumeBtn.Offset();
        OptionBtn.Offset();
        QuitBtn.Offset();

        InnerImgs = DevTool.Get_ChildList<Image>(InnerParentTF);
    }

    private void Offset_Txt()
    {
        Set_LanguageTxt();
    }

    private void Offset_ColorComp()
    {
        MainColorCompList.Add(BasePanelBtnTxt);
        SubColorCompList.Add(BasePanelBtnImg);

        MainColorCompList.AddRange(DevTool.Get_ChildList<Image>(BaseInteractingPanelInnerParentTF));
        SubColorCompList.AddRange(InnerImgs);

        MainColorCompList.AddRange(LanguagePanelEUI.Get_InnerMainColorList());
        
        Color clr = new Color(1, 1, 1, 0.1f);
        ResumeInnerImg.color = clr;
        OptionInnerImg.color = clr;
        QuitInnerImg.color = clr;

        MainColorCompList.Add(ResumeInnerImg);
        MainColorCompList.Add(OptionInnerImg);
        MainColorCompList.Add(QuitInnerImg);

        Color mainClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }

    private void Offset_PosValue()
    {
        InteractBasePanelPosX = 1000f;
        InteractOptionPanelPosX = OptionPanelRT.rect.width;

        BasePanelRT.anchoredPosition = Vector2.zero;
        OptionPanelRT.anchoredPosition = Vector2.zero;

        BaseInteractingPanelCG.alpha = 0f;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (Is_Interact_BasePanel()) return;
        if (Is_Interact_OptionPanel()) return;
    }

    public void Try_InteractBack()
    {
        if (IsInteractTweening) return;

        switch (CurrentType)
        {
            case OutMainGameUIType.BasePanel:
                SetOff_ThisPanel();
                break;
            case OutMainGameUIType.OptionPanel:
                SetOff_OptionPanel();
                break;

            default:
                break;
        }
    }

    #endregion

    #region Interact (Base Panel)

    private bool Is_Interact_BasePanel()
    {
        if (CurrentType != OutMainGameUIType.BasePanel) return false;

        // Base Btns
        if (CurrentBtn == ResumeBtn)
        {
            SetOff_ThisPanel();
        }
        else if (CurrentBtn == OptionBtn)
        {
            SetOn_OptionPanel();
        }
        else if (CurrentBtn == QuitBtn)
        {
            LoadingSceneManager.Instance.Play_LoadScene("TitleLobby");
        }

        return true;
    }

    #endregion

    #region Interact (Option Panel)

    private bool Is_Interact_OptionPanel()
    {
        if (CurrentType != OutMainGameUIType.OptionPanel) return false;

        if (CurrentBtn == OptionBackBtn)
        {
            SetOff_OptionPanel();
            return true;
        }
        else if (CurrentBtn == OptionApplyBtn)
        {
            Set_OptionValueApply();
            OptionWarningTxt.gameObject.SetActive(false);
            return true;
        }
        else if(Is_Interact_OptionElement(LanguagePanelEUI)) return true;

        return false;
    }

    private bool Is_Interact_OptionElement(LRSlidingItemEUIController _LRSlidingEUI)
    {
        if (CurrentBtn == _LRSlidingEUI.LeftBtn)
        {
            _LRSlidingEUI.Change_Left();
            OptionWarningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (CurrentBtn == _LRSlidingEUI.RightBtn)
        {
            _LRSlidingEUI.Change_Right();
            OptionWarningTxt.gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    #endregion

    #region Set (Option)

    private void Set_OptionValueApply()
    {
        UnitManager.Instance.Set_LanguageFont(LanguagePanelEUI.Get_CurrentIndex());
    }

    private void SetOn_OptionPanel()
    {
        if (IsInteractTweening) return;

        IsInteractTweening = true;
        CurrentType = OutMainGameUIType.OptionPanel;
        BaseInteractingPanelTxt.text = CSVManager.Instance.Get_StaticWord(20);

        // Element
        OptionWarningTxt.gameObject.SetActive(false);
        LanguagePanelEUI.Set_Item(GameManager.LanguageID);

        Sequence seq = DOTween.Sequence();

        seq.Join(BasePanelRT.DOAnchorPosX(-InteractBasePanelPosX, 0.2f));
        seq.Join(OptionPanelRT.DOAnchorPosX(-InteractOptionPanelPosX, 0.2f));
        seq.Join(BaseInteractingPanelCG.DOFade(1f, 0.2f));
        seq.OnComplete(() =>
        {
            IsInteractTweening = false; 
        });
    }

    private void SetOff_OptionPanel()
    {
        if (IsInteractTweening) return;
        IsInteractTweening = true;
        CurrentType = OutMainGameUIType.BasePanel;

        Sequence seq = DOTween.Sequence();

        seq.Join(BasePanelRT.DOAnchorPosX(0f, 0.2f));
        seq.Join(OptionPanelRT.DOAnchorPosX(100f, 0.2f));
        seq.Join(BaseInteractingPanelCG.DOFade(0f, 0.2f));
        seq.OnComplete(() =>
        {
            IsInteractTweening = false;
        });
    }

    #endregion

    #region Set (LanguageTxt)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        BasePanelBtnTxt.text = CSVManager.Instance.Get_StaticWord(22);
        OptionWarningTxt.text = CSVManager.Instance.Get_StaticDesc(31);

        DevTool.Get_ComponentTType<TMP_Text>(OptionApplyBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(OptionApplyBtn, 0)).gameObject).text = CSVManager.Instance.Get_StaticWord(91);
        LanguagePanelEUI.HeaderTxt.text = CSVManager.Instance.Get_StaticWord(92);

        DevTool.Get_ComponentTType<TMP_Text>(ResumeBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(ResumeBtn, 0)).gameObject).text = CSVManager.Instance.Get_StaticWord(19);
        DevTool.Get_ComponentTType<TMP_Text>(OptionBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(OptionBtn, 0)).gameObject).text = CSVManager.Instance.Get_StaticWord(20);
        DevTool.Get_ComponentTType<TMP_Text>(QuitBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(QuitBtn, 0)).gameObject).text = CSVManager.Instance.Get_StaticWord(21);

        BaseInteractingPanelTxt.text = CSVManager.Instance.Get_StaticWord(20);
    }

    #endregion
}
