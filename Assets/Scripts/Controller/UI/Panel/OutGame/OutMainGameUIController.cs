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
    [SerializeField] private LRSlidingItemEUIController ScreenModePanelEUI;
    [SerializeField] private LRSlidingItemEUIController ResolutionPanelEUI;
    [SerializeField] private LRSlidingItemEUIController FPSPanelEUI;
    [SerializeField] private FillScrollbarEUIController BGMVolumePanelEUI;
    [SerializeField] private FillScrollbarEUIController SFXVolumePanelEUI;

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

        ScreenModePanelEUI.Set_OwnerUIController(this);
        ScreenModePanelEUI.Offset();

        ResolutionPanelEUI.Set_OwnerUIController(this);
        ResolutionPanelEUI.Offset();

        FPSPanelEUI.Set_OwnerUIController(this);
        FPSPanelEUI.Offset();

        BGMVolumePanelEUI.Set_OwnerUIController(this);
        BGMVolumePanelEUI.Offset();

        SFXVolumePanelEUI.Set_OwnerUIController(this);
        SFXVolumePanelEUI.Offset();
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
        MainColorCompList.AddRange(ScreenModePanelEUI.Get_InnerMainColorList());
        MainColorCompList.AddRange(ResolutionPanelEUI.Get_InnerMainColorList());
        MainColorCompList.AddRange(FPSPanelEUI.Get_InnerMainColorList());
        MainColorCompList.AddRange(BGMVolumePanelEUI.Get_InnerMainColorList());
        MainColorCompList.AddRange(SFXVolumePanelEUI.Get_InnerMainColorList());

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

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        PlayerManager.Instance.CameraController.Stop_SlowMotion();
        Time.timeScale = 0f;
    }

    public override void SetOff_ThisPanel()
    {
        base.SetOff_ThisPanel();

        Time.timeScale = 1f;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        InputManager.Instance.Play_MousePointerClick();

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
        else if (Is_Interact_OptionElement(LanguagePanelEUI)) return true;
        else if (Is_Interact_OptionElement(ScreenModePanelEUI)) return true;
        else if (Is_Interact_OptionElement(ResolutionPanelEUI)) return true;
        else if (Is_Interact_OptionElement(BGMVolumePanelEUI)) return true;
        else if (Is_Interact_OptionElement(SFXVolumePanelEUI)) return true;
        else if (Is_Interact_OptionElement(FPSPanelEUI)) return true;

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

    private bool Is_Interact_OptionElement(FillScrollbarEUIController _ScrollEUI)
    {
        if (CurrentBtn == _ScrollEUI.LeftBtn)
        {
            _ScrollEUI.Dec();
            OptionWarningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (CurrentBtn == _ScrollEUI.RightBtn)
        {
            _ScrollEUI.Inc();
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
        GameManager.Instance.Set_Screen(
            (eResolution)ResolutionPanelEUI.Get_CurrentIndex(),
            (eScreenMode)ScreenModePanelEUI.Get_CurrentIndex());
        GameManager.Instance.Set_FPS((eFPS)FPSPanelEUI.Get_CurrentIndex());
        SoundManager.Instance.Set_BgmVolume(BGMVolumePanelEUI.Get_Value());
        SoundManager.Instance.Set_SfxVolume(SFXVolumePanelEUI.Get_Value());
    }

    private void SetOn_OptionPanel()
    {
        if (IsInteractTweening) return;

        IsInteractTweening = true;
        CurrentType = OutMainGameUIType.OptionPanel;
        BaseInteractingPanelTxt.text = ResourceManager.Instance.Get_StaticWord(20);

        // Element
        OptionWarningTxt.gameObject.SetActive(false);
        LanguagePanelEUI.Set_Item(GameManager.LanguageID);
        ScreenModePanelEUI.Set_Item((int)GameManager.ScreenMode);
        ResolutionPanelEUI.Set_Item((int)GameManager.ResolutionMode);
        FPSPanelEUI.Set_Item((int)GameManager.FPS);
        BGMVolumePanelEUI.Set_Value(SoundManager.Instance.BVolume);
        SFXVolumePanelEUI.Set_Value(SoundManager.Instance.SVolume);

        Sequence seq = DOTween.Sequence();

        seq.Join(BasePanelRT.DOAnchorPosX(-InteractBasePanelPosX, 0.2f));
        seq.Join(OptionPanelRT.DOAnchorPosX(-InteractOptionPanelPosX, 0.2f));
        seq.Join(BaseInteractingPanelCG.DOFade(1f, 0.2f));
        seq.OnComplete(() =>
        {
            IsInteractTweening = false; 
        });
        seq.SetUpdate(true);    
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
        seq.SetUpdate(true);
    }

    #endregion

    #region Set (LanguageTxt)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        BasePanelBtnTxt.text = ResourceManager.Instance.Get_StaticWord(22);
        OptionWarningTxt.text = ResourceManager.Instance.Get_StaticDesc(31);

        DevTool.Get_ComponentTType<TMP_Text>(OptionApplyBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(OptionApplyBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(91);
        LanguagePanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(92);
        ScreenModePanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(140);
        ResolutionPanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(137);
        FPSPanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(141);
        BGMVolumePanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(138);
        SFXVolumePanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(139);

        DevTool.Get_ComponentTType<TMP_Text>(ResumeBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(ResumeBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(19);
        DevTool.Get_ComponentTType<TMP_Text>(OptionBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(OptionBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(20);
        DevTool.Get_ComponentTType<TMP_Text>(QuitBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(QuitBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(21);

        BaseInteractingPanelTxt.text = ResourceManager.Instance.Get_StaticWord(20);
    }

    #endregion
}
