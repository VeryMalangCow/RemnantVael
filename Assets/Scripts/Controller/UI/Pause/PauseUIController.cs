using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIController : SinglePanelUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Out Main Game UI")]

    [Space(10)][SerializeField] private PauseStateView stateViewPrefab;
    private PauseStateView stateView;
    [Space(10)][SerializeField] private PauseOptionView optionViewPrefab;
    private PauseOptionView optionView;
    [Space(10)][SerializeField] private PauseInfoView infoViewPrefab;
    private PauseInfoView infoView;

    [Space(5)][SerializeField] private Transform viewParentTf;

    #region - Inspector

    [Space(10)]
    [Header("=== Inner")]
    [SerializeField] private Transform innerParentTf;
    [SerializeField] private Transform baseInteractingPanelInnerParentTf;
    [SerializeField] private Image[] innerImgArr;

    [Space(10)]
    [Header("=== Btn")]
    [SerializeField] private OwnBtnEUIController resumeBtn;
    [SerializeField] private OwnBtnEUIController stateBtn;
    [SerializeField] private OwnBtnEUIController optionBtn;
    [SerializeField] private OwnBtnEUIController infoBtn;
    [SerializeField] private OwnBtnEUIController returnBtn;
    [SerializeField] private OwnBtnEUIController quitBtn;

    [Space(10)]
    [Header("=== Img")]
    [SerializeField] private Image basePanelBtnImg;

    [Space(20)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text basePanelBtnTxt;
    [SerializeField] private TMP_Text baseInteractingPanelTxt;

    [Space(10)]
    [Header("=== Base")]
    [SerializeField] private RectTransform basePanelRt;
    [SerializeField] private CanvasGroup baseInteractingPanelCg;

    #endregion

    #region - Hide

    // Inner
    [HideInInspector] private List<Image> innerImgs;

    // Panel
    [HideInInspector] private float interactBasePanelPosX;
    [HideInInspector] public float interactPanelPosX;

    [HideInInspector] private bool isInteractTweening = false;
    [HideInInspector] private OutMainGameUIType currentType = OutMainGameUIType.BasePanel;


    #endregion

    #endregion

    #region Init

    public IEnumerator InitAsync(Color mainClr, Color subClr)
    {
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        stateView = Instantiate(stateViewPrefab, viewParentTf);
        stateViewPrefab = null;
        yield return stateView.InitAsync(this);

#if UNITY_EDITOR
        sw.Restart();
#endif
        optionView = Instantiate(optionViewPrefab, viewParentTf);
        optionView.Init(this);
        optionViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Pause UI : <color=yellow>Option View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        infoView = Instantiate(infoViewPrefab, viewParentTf);
        infoView.Init(this);
        infoViewPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"Pause UI : <color=yellow>Info View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

        resumeBtn.ownerUIController = this;
        infoBtn.ownerUIController = this;
        optionBtn.ownerUIController = this;
        stateBtn.ownerUIController = this;
        returnBtn.ownerUIController = this;
        quitBtn.ownerUIController = this;

        resumeBtn.Offset();
        stateBtn.Offset();
        optionBtn.Offset();
        infoBtn.Offset();
        returnBtn.Offset();
        quitBtn.Offset();

        innerImgs = DevTool.Get_ChildList<Image>(innerParentTf);

        SetLanguageTxt();

        mainColorCompList.Add(basePanelBtnTxt);
        subColorCompList.Add(basePanelBtnImg);

        mainColorCompList.AddRange(DevTool.Get_ChildList<Image>(baseInteractingPanelInnerParentTf));
        subColorCompList.AddRange(innerImgs);

        mainColorCompList.AddRange(optionView.Get_MainColorComps());

        Color clr = new Color(1, 1, 1, 0.1f);
        for (int i = 0; i < innerImgArr.Length; i++)
            innerImgArr[i].color = clr;

        mainColorCompList.AddRange(innerImgArr);

        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;

        stateView.Set_Color(mainClr, subClr);

        interactBasePanelPosX = 1000f;
        interactPanelPosX = optionView.panelRt.rect.width;
        optionView.panelRt.gameObject.SetActive(false);

        basePanelRt.anchoredPosition = Vector2.zero;
        optionView.panelRt.anchoredPosition = Vector2.zero;

        baseInteractingPanelCg.alpha = 0f;
        yield return null;
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        SoundManager.instance.Play_2D_SFX_UI("Click_Reject");
        PlayerManager.instance.cameraController.Stop_SlowMotion();
        Time.timeScale = 0f;
    }

    public override void SetOff_ThisPanel()
    {
        base.SetOff_ThisPanel();

        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");
        Time.timeScale = 1f;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_BasePanel()) return;
        if (Is_Interact_OptionPanel()) return;
        if (Is_Interact_StatePanel()) return;
        if (Is_Interact_InfoPanel()) return;
    }

    public void Try_InteractBack()
    {
        if (isInteractTweening) return;

        switch (currentType)
        {
            case OutMainGameUIType.BasePanel:
                SetOff_ThisPanel();
                break;
            case OutMainGameUIType.OptionPanel:
                SetOff_Panel(optionView.panelRt);
                break;

            case OutMainGameUIType.StatePanel:
                SetOff_Panel(stateView.panelRt);
                break;

            case OutMainGameUIType.InfoPanel:
                SetOff_Panel(infoView.panelRt);
                infoView.Set_Panel(false);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Interact (Base Panel)

    private bool Is_Interact_BasePanel()
    {
        if (currentType != OutMainGameUIType.BasePanel) return false;

        // Base Btns
        if (currentBtn == resumeBtn)
        {
            SetOff_ThisPanel();
        }
        else if (currentBtn == stateBtn)
        {
            SetOn_StatePanel();
        }
        else if (currentBtn == optionBtn)
        {
            SetOn_OptionPanel();
        }
        else if (currentBtn == infoBtn)
        {
            SetOn_InfoPanel();
        }
        else if (currentBtn == returnBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_Reject");
            EventManager.instance.Set_Input(false);
            LoadingSceneManager.instance.Play_LoadScene("MainGame");
        }
        else if (currentBtn == quitBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_Reject");
            EventManager.instance.Set_Input(false);
            LoadingSceneManager.instance.Play_LoadScene("TitleLobby");
        }

        return true;
    }

    #endregion

    #region Interact (Option Panel)

    private bool Is_Interact_OptionPanel()
    {
        if (currentType != OutMainGameUIType.OptionPanel) return false;

        if (currentBtn == optionView.backBtn)
        {
            SetOff_Panel(optionView.panelRt);
            return true;
        }
        else if (currentBtn == optionView.applyBtn)
        {
            Set_OptionValueApply();
            optionView.warningTxt.gameObject.SetActive(false);
            return true;
        }
        else if (Is_Interact_OptionElement(optionView.languagePanelEui)) return true;
        else if (Is_Interact_OptionElement(optionView.screenModePanelEui)) return true;
        else if (Is_Interact_OptionElement(optionView.resolutionPanelEui)) return true;
        else if (Is_Interact_OptionElement(optionView.bgmVolumePanelEui)) return true;
        else if (Is_Interact_OptionElement(optionView.sfxVolumePanelEui)) return true;
        else if (Is_Interact_OptionElement(optionView.fpsPanelEui)) return true;

        return false;
    }

    private bool Is_Interact_OptionElement(LRSlidingItemEUIController lrSlidingEui)
    {
        if (currentBtn == lrSlidingEui.leftBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            lrSlidingEui.Change_Left();
            optionView.warningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (currentBtn == lrSlidingEui.rightBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            lrSlidingEui.Change_Right();
            optionView.warningTxt.gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    private bool Is_Interact_OptionElement(FillScrollbarEUIController scrollEui)
    {
        if (currentBtn == scrollEui.leftBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            scrollEui.Dec();
            optionView.warningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (currentBtn == scrollEui.rightBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            scrollEui.Inc();
            optionView.warningTxt.gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    #endregion

    #region Interact (State Panel)

    private bool Is_Interact_StatePanel()
    {
        if (currentType != OutMainGameUIType.StatePanel) return false;

        if (currentBtn == stateView.backBtn)
        {
            SetOff_Panel(stateView.panelRt);
            return true;
        }
        if (currentBtn == stateView.changeTypeBtn)
        {
            stateView.Change_Panel();
            return true;
        }

        return false;
    }

    #endregion

    #region Interact (Info Panel)

    private bool Is_Interact_InfoPanel()
    {
        if (currentType != OutMainGameUIType.InfoPanel) return false;

        if (currentBtn == infoView.backBtn)
        {
            SetOff_Panel(infoView.panelRt);
            infoView.Set_Panel(false);
            return true;
        }
        if (infoView.Is_ListBtn(currentBtn))
        {
            infoView.SetOn_DetailWindow((InfoEUIController)currentBtn);
            return true;
        }

        return false;
    }

    #endregion

    #region Set (Option)

    private void Set_OptionValueApply()
    {
        if (!optionView.warningTxt.gameObject.activeSelf) return;

        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");

        ResourceManager.instance.Set_LanguageFont(optionView.languagePanelEui.Get_CurrentIndex());
        GameManager.instance.Set_Screen(
            (eResolution)optionView.resolutionPanelEui.Get_CurrentIndex(),
            (eScreenMode)optionView.screenModePanelEui.Get_CurrentIndex());
        GameManager.instance.Set_FPS((eFPS)optionView.fpsPanelEui.Get_CurrentIndex());
        SoundManager.instance.Set_BgmVolume(optionView.bgmVolumePanelEui.Get_Value());
        SoundManager.instance.Set_SfxVolume(optionView.sfxVolumePanelEui.Get_Value());

        SaveDataManager.instance.Save_OptionJsonData();
    }

    private void SetOn_OptionPanel()
    {
        if (isInteractTweening) return;

        baseInteractingPanelTxt.text = ResourceManager.instance.Get_StaticWord(20);
        SetOn_Panel(OutMainGameUIType.OptionPanel, optionView.panelRt);

        optionView.Set_Panel();
    }

    #endregion

    #region Set (State)

    private void SetOn_StatePanel()
    {
        if (isInteractTweening) return;

        baseInteractingPanelTxt.text = ResourceManager.instance.Get_StaticWord(102);
        SetOn_Panel(OutMainGameUIType.StatePanel, stateView.panelRt);

        stateView.Set_Panel(true);
        stateView.Set_State(AllyManager.instance.allAlly);
    }

    #endregion

    #region Set (Info)

    private void SetOn_InfoPanel()
    {
        if (isInteractTweening) return;

        baseInteractingPanelTxt.text = ResourceManager.instance.Get_StaticWord(144);
        SetOn_Panel(OutMainGameUIType.InfoPanel, infoView.panelRt);

        infoView.Set_Panel(true);
    }

    #endregion

    #region Set (Capsule)

    private void SetOn_Panel(OutMainGameUIType type, RectTransform rt)
    {
        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        isInteractTweening = true;
        currentType = type;
        rt.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        seq.Join(basePanelRt.DOAnchorPosX(-interactBasePanelPosX, 0.2f));
        seq.Join(rt.DOAnchorPosX(-interactPanelPosX, 0.2f));
        seq.Join(baseInteractingPanelCg.DOFade(1f, 0.2f));
        seq.OnComplete(() =>
        {
            isInteractTweening = false;
        });
        seq.SetUpdate(true);
    }

    private void SetOff_Panel(RectTransform rt)
    {
        SoundManager.instance.Play_2D_SFX_UI("Click_Reject");

        if (isInteractTweening) return;
        isInteractTweening = true;
        currentType = OutMainGameUIType.BasePanel;

        Sequence seq = DOTween.Sequence();

        seq.Join(basePanelRt.DOAnchorPosX(0f, 0.2f));
        seq.Join(rt.DOAnchorPosX(100f, 0.2f));
        seq.Join(baseInteractingPanelCg.DOFade(0f, 0.2f));
        seq.OnComplete(() =>
        {
            isInteractTweening = false;
            rt.gameObject.SetActive(false);
        });
        seq.SetUpdate(true);
    }

    #endregion

    #region Set (LanguageTxt)

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

        basePanelBtnTxt.text = ResourceManager.instance.Get_StaticWord(22);
        DevTool.Get_ComponentTType<TMP_Text>(resumeBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(resumeBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(19);
        DevTool.Get_ComponentTType<TMP_Text>(stateBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(stateBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(102);
        DevTool.Get_ComponentTType<TMP_Text>(optionBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(optionBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(20);
        DevTool.Get_ComponentTType<TMP_Text>(infoBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(infoBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(144);
        DevTool.Get_ComponentTType<TMP_Text>(returnBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(returnBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(143);
        DevTool.Get_ComponentTType<TMP_Text>(quitBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(quitBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(21);

        optionView.Set_LanguageTxt();
        stateView.Set_LanguageTxt();
        infoView.Set_LanguageTxt();

        int langId = 0;
        switch (currentType)
        {
            case OutMainGameUIType.StatePanel: langId = 102; break;
            case OutMainGameUIType.OptionPanel: langId = 20; break;
            case OutMainGameUIType.InfoPanel: langId = 144; break;

            default: break;
        }
        baseInteractingPanelTxt.text = ResourceManager.instance.Get_StaticWord(langId);
    }

    #endregion
}
