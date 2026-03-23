using DG.Tweening;
using LeTai.TrueShadow;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleLobbyUIController : TitleSinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Title Lobby UI Controller")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform btnsRt;
    [SerializeField] private CanvasGroup bgCg;

    [Space(10)]
    [Header("=== Element")]
    [SerializeField] private List<TitleElement> allTitleEui;
    [SerializeField] private List<TitleTSElement> allTitleTsEui;

    [Space(5)]
    [Header("-- Smoke")]
    [SerializeField] private List<TitleSmokeEUIController> allTitleSmokeEui;

    [Space(5)]
    [Header("-- Cloud")]
    [SerializeField] private List<RectTransform> cloudRtList_BackMoon;
    [SerializeField] private List<RectTransform> cloudRtList_FrontMoon;
    [SerializeField] private float cloudMovingLimit = 3000f;
    [SerializeField] private float cloudMovingTime = 1f;

    [Space(10)]
    [Header("=== Prefab")]
    [SerializeField] public GameObject smokeCellEuiPrefab;
    
    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private TitleOwnBtnEUIController startBtn;
    [SerializeField] private TMP_Text startTxt;
    [SerializeField] private TitleOwnBtnEUIController optionBtn;
    [SerializeField] private TMP_Text optionTxt;
    [SerializeField] private TitleOwnBtnEUIController quitBtn;
    [SerializeField] private TMP_Text quitTxt;
    [SerializeField] private RectTransform selectedRt;

    [Space(10)]
    [Header("=== Reset Panel")]
    [SerializeField] private GameObject resetPanelGo;
    [SerializeField] private TitleOwnBtnEUIController resetBtn;
    [SerializeField] private TitleOwnBtnEUIController resetSureYesBtn;
    [SerializeField] private TitleOwnBtnEUIController resetSureNoBtn;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float durTime;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private IInteract currentInteractable;

    [Space(10)]
    [Header("=== Option")]
    [SerializeField] private OptionUIController optionUi;
    [Serializable]
    public class OptionUIController
    {
        [SerializeField] public RectTransform panelRt;
        [SerializeField] public Image bgImg;
        [SerializeField] public TitleOwnBtnEUIController backBtn;
        [SerializeField] public TitleOwnBtnEUIController applyBtn;
        [SerializeField] public TMP_Text warningTxt;
        [SerializeField] public bool isOn = false;

        [Space(10)]
        [SerializeField] public TitleLRSlidingItemEUIController languagePanelEui;
        [SerializeField] public TitleLRSlidingItemEUIController screenModePanelEui;
        [SerializeField] public TitleLRSlidingItemEUIController resolutionPanelEui;
        [SerializeField] public TitleLRSlidingItemEUIController fpsPanelEui;
        [SerializeField] public TitleFillScrollbarEUIController bgmVolumePanelEui;
        [SerializeField] public TitleFillScrollbarEUIController sfxVolumePanelEui;

        public void Offset(TitleLobbyUIController uiController)
        {
            SetOff_Panel();

            backBtn.ownerUIController = uiController;
            backBtn.Offset();

            applyBtn.ownerUIController = uiController;
            applyBtn.Offset();

            languagePanelEui.Set_OwnerUIController(uiController);
            languagePanelEui.Offset();

            screenModePanelEui.Set_OwnerUIController(uiController);
            screenModePanelEui.Offset();

            resolutionPanelEui.Set_OwnerUIController(uiController);
            resolutionPanelEui.Offset();

            fpsPanelEui.Set_OwnerUIController(uiController);
            fpsPanelEui.Offset();

            bgmVolumePanelEui.Set_OwnerUIController(uiController);
            bgmVolumePanelEui.Offset();

            sfxVolumePanelEui.Set_OwnerUIController(uiController);
            sfxVolumePanelEui.Offset();
        }

        public void SetOn_Panel()
        {
            panelRt.gameObject.SetActive(true);
            bgImg.gameObject.SetActive(true);
            warningTxt.gameObject.SetActive(false);
            languagePanelEui.Set_Item(GameManager.languageID);
            screenModePanelEui.Set_Item((int)GameManager.screenMode);
            resolutionPanelEui.Set_Item((int)GameManager.resolutionMode);
            fpsPanelEui.Set_Item((int)GameManager.fps);
            bgmVolumePanelEui.Set_Value(SoundManager.instance.bgmVolume);
            sfxVolumePanelEui.Set_Value(SoundManager.instance.sfxVolume);

            isOn = true;
        }

        public void SetOff_Panel()
        {
            panelRt.gameObject.SetActive(false);
            bgImg.gameObject.SetActive(false);

            isOn = false;
        }

        public void Set_LanguageTxt()
        {
            warningTxt.text = ResourceManager.instance.Get_StaticDesc(31);

            DevTool.Get_ComponentTType<TMP_Text>(applyBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(applyBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(91);
            languagePanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(92);
            screenModePanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(140);
            resolutionPanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(137);
            fpsPanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(141);
            bgmVolumePanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(138);
            sfxVolumePanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(139);
        }
    }

    #endregion

    #region - Hide

    // Value
    [HideInInspector] public bool isInIntro = true;
    [HideInInspector] private bool isStarting = false;
    [HideInInspector] private float euiMovingPowerMultiple = 0.002f;

    // Btn
    [HideInInspector] private List<TitleOwnBtnEUIController> titleAllBtns;
    [HideInInspector] private TitleOwnBtnEUIController currentMouseBtn = null;

    [HideInInspector] private List<Tween> couldTween = null;

    // Option
    [HideInInspector] private bool isInteractTweening = false;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        DevTool.Get_ComponentTType<Canvas>(gameObject).planeDistance = 10;

        startBtn.Offset();
        startBtn.ownerUIController = this;

        optionBtn.Offset();
        optionBtn.ownerUIController = this;

        quitBtn.Offset();
        quitBtn.ownerUIController = this;

        titleAllBtns = new List<TitleOwnBtnEUIController>
        { startBtn, optionBtn, quitBtn };

        resetBtn.Offset();
        resetBtn.ownerUIController = this;

        resetSureYesBtn.Offset();
        resetSureYesBtn.ownerUIController = this;

        resetSureNoBtn.Offset();
        resetSureNoBtn.ownerUIController = this;

        bgCg.alpha = 1f;

        isStarting = false;

        for (int i = 0; i < allTitleSmokeEui.Count; i++)
        {
            allTitleSmokeEui[i].ownerUIController = this;
            allTitleSmokeEui[i].Offset();
        }

        couldTween = new List<Tween>
        {
            Play_CloudMoving_FromLeft(cloudRtList_BackMoon[0], cloudMovingTime),
            Play_CloudMoving_FromLeft(cloudRtList_FrontMoon[0], cloudMovingTime),
            Play_CloudMoving_FromCenter(cloudRtList_BackMoon[1], cloudMovingTime),
            Play_CloudMoving_FromCenter(cloudRtList_FrontMoon[1], cloudMovingTime)
        };

        optionUi.Offset(this);
        Set_LanguageTxt();
    }

    #endregion

    #region Play

    private void SetOff_Play()
    {
        for (int i = 0; i < allTitleSmokeEui.Count; i++)
            allTitleSmokeEui[i].Stop_VFX();

        for (int i = 0; i < couldTween.Count; i++)
            DevTool.Set_KillTween(couldTween[i]);

        couldTween = null;
    }

    #endregion

    #region Cloud

    private Tween Play_CloudMoving_FromLeft(RectTransform cloudRt, float durTime)
    {
        Tween tween = cloudRt.DOAnchorPosX(cloudMovingLimit, durTime)
            .OnComplete(() =>
            {
                cloudRt.anchoredPosition = new Vector2(-cloudMovingLimit, 0);
            })
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);


        return tween;
    }

    private Tween Play_CloudMoving_FromCenter(RectTransform cloudRt, float durTime)
    {
        Tween tween = cloudRt.DOAnchorPosX(cloudMovingLimit, durTime * 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                cloudRt.anchoredPosition = new Vector2(-cloudMovingLimit, 0);
                cloudRt.DOAnchorPosX(cloudMovingLimit, durTime).OnComplete(() =>
                {
                    cloudRt.anchoredPosition = new Vector2(-cloudMovingLimit, 0);
                })
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart);
            });

        return tween;
    }

    #endregion

    #region Framework

    private void Start()
    {
        Set_UIElementTS(true);

        Set_CurrentBtn(startBtn);
    }

    private void LateUpdate()
    {
        Vector2 movingPower = Get_MovingPowerVec() * -1;
        Set_UIElementTF(movingPower);
    }


    #endregion

    #region UI Element (TS)

    private void Set_UIElementTS(bool isLoop)
    {
        for (int i = 0; i < allTitleTsEui.Count; i++)
        {
            int index = i;
            TitleTSElement trueShadowElementSet = allTitleTsEui[index];
            Get_EachUIElementTS(trueShadowElementSet.thisTSList, trueShadowElementSet.max, trueShadowElementSet.min, trueShadowElementSet.durTime, isLoop);
        }
    }


    private void Get_EachUIElementTS(List<TrueShadow> tsList, float max, float min, float durTime, bool loop = true)
    {
        for (int i = 0; i < tsList.Count; i++)
        {
            int index = i;

            tsList[index].Size = min;

            Tween tween = DOTween.To(
                () => tsList[index].Size,
                x => tsList[index].Size = x,
                max, durTime * 0.5f)
                .SetEase(Ease.Linear);

            if (loop)
                tween.SetLoops(-1, LoopType.Yoyo);
        }
    }

    #endregion

    #region UI Element (Pos)

    private void Set_UIElementTF(Vector2 movingPower)
    {
        for (int i = 0; i < allTitleEui.Count; i++)
        {
            Vector2 thisVec = new Vector2(allTitleEui[i].movingPowerX, allTitleEui[i].movingPowerY);
            Vector2 targetVec = thisVec * movingPower;
            Set_EachUIElementPos(allTitleEui[i].movingRT, targetVec);
        }
    }

    private void Set_EachUIElementPos(RectTransform rt, Vector2 targetVec)
    {
        rt.anchoredPosition = targetVec;
    }

    private Vector2 Get_MovingPowerVec()
    {
        if (TitleInputManager.instance == null) 
            return Vector2.zero;

        return euiMovingPowerMultiple * TitleInputManager.instance.Get_AnchorMousePos();
    }

    #endregion

    #region Input

    public void Try_Interact()
    {
        if (currentBtn == null || currentMouseBtn == null || isStarting || isInIntro || isInteractTweening)
            return; 

        TitleInputManager.instance.Play_MousePointerClick();

        string soundSfxName = "";

        if (currentBtn == startBtn)
        {
            soundSfxName = "Click_Approve";
            Play_Starting();
        }
        else if (currentBtn == optionBtn)
        {
            soundSfxName = "Click_01";
            SetOn_OptionPanel();
        }
        else if (currentBtn == optionUi.backBtn)
        {
            soundSfxName = "Click_Reject";
            SetOff_OptionPanel();
        }
        else if (currentBtn == optionUi.applyBtn)
        {
            soundSfxName = "Click_Approve";
            Set_OptionValueApply();
        }
        else if (currentBtn == quitBtn)
        {
            soundSfxName = "Click_Reject";
            Application.Quit();
        }

        else if (Is_Interact_OptionElement(optionUi.languagePanelEui)) return;
        else if (Is_Interact_OptionElement(optionUi.screenModePanelEui)) return;
        else if (Is_Interact_OptionElement(optionUi.resolutionPanelEui)) return;
        else if (Is_Interact_OptionElement(optionUi.bgmVolumePanelEui)) return;
        else if (Is_Interact_OptionElement(optionUi.sfxVolumePanelEui)) return;
        else if (Is_Interact_OptionElement(optionUi.fpsPanelEui)) return;

        else if (currentBtn == resetBtn)
        {
            Set_ResetPanel(true);
        }
        else if (currentBtn == resetSureNoBtn)
        {
            Set_ResetPanel(false);
        }
        else if (currentBtn == resetSureYesBtn)
        {
            SaveDataManager.instance.Reset_JsonData();
            SaveDataManager.instance.Load_JsonData();
            LoadingSceneManager.instance.Play_LoadScene("TitleLobby");
        }

        if (soundSfxName != "")
            SoundManager.instance.Play_2D_SFX_UI(soundSfxName);
    }

    public void Try_OutInteract()
    {
        if (isStarting || isInIntro || isInteractTweening)
            return;

        if (optionUi.isOn)
        {
            SetOff_OptionPanel();
        }
    }

    #endregion

    #region Btn

    public override void Set_CurrentBtn(TitleOwnBtnEUIController targetBtn)
    {
        if (currentBtn == targetBtn) return;

        base.Set_CurrentBtn(targetBtn);

        if (currentBtn != null && (currentBtn == startBtn || currentBtn == optionBtn || currentBtn == quitBtn))
        {
            DevTool.Set_KillTween(selectedRt);
            selectedRt.DOAnchorPosY(currentBtn.rt.anchoredPosition.y, 0.2f);
        }

        for (int i = 0; i < titleAllBtns.Count; i++)
        {
            if (currentBtn == titleAllBtns[i])
                titleAllBtns[i].Set_SelectOnThis(0.2f);
            else
                titleAllBtns[i].Set_SelectOffThis(0.2f);
        }
    }

    public void Set_CurrentMouseBtn(TitleOwnBtnEUIController targetBtn)
    {
        if (currentMouseBtn == targetBtn) return;

        currentMouseBtn = targetBtn;
    }

    #endregion

    #region Reset

    private void Set_ResetPanel(bool onOff)
    {
        resetPanelGo.gameObject.SetActive(onOff);
        currentBtn = null;
    }

    #endregion

    #region Play

    private void Play_Starting()
    {
        StartCoroutine(Play_Starting_Cor(2f));
    }

    private IEnumerator Play_Starting_Cor(float delayTime)
    {
        isStarting = true;

        TitleLobbyUIManager.instance.Get_JustFadeIn(delayTime);

        yield return new WaitForSeconds(delayTime + 0.2f);

        SetOff_Play();

        LoadingSceneManager.instance.Play_LoadScene("MainGame");
    }

    #endregion

    #region Option

    #region Set (Option)

    private void SetOn_OptionPanel()
    {
        isInteractTweening = true;

        optionUi.SetOn_Panel();

        Sequence seq = DOTween.Sequence();

        seq.Append(optionUi.panelRt.DOAnchorPosX(440, 0.2f));
        seq.Join(optionUi.bgImg.DOFade(0.7f, 0.2f));
        seq.OnComplete(() =>
        {
            isInteractTweening = false;
        });
    }

    private void SetOff_OptionPanel()
    {
        isInteractTweening = true;

        Sequence seq = DOTween.Sequence();

        seq.Append(optionUi.panelRt.DOAnchorPosX(2500, 0.2f));
        seq.Join(optionUi.bgImg.DOFade(0f, 0.2f));
        seq.OnComplete(() =>
        {
            isInteractTweening = false;
            optionUi.SetOff_Panel();
        });
    }

    // Element
    private bool Is_Interact_OptionElement(TitleLRSlidingItemEUIController lrSlidingEui)
    {
        if (currentBtn == lrSlidingEui.leftBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            lrSlidingEui.Change_Left();
            optionUi.warningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (currentBtn == lrSlidingEui.rightBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            lrSlidingEui.Change_Right();
            optionUi.warningTxt.gameObject.SetActive(true);
            return true;
        }

            

        return false;
    }

    private bool Is_Interact_OptionElement(TitleFillScrollbarEUIController scrollEui)
    {
        if (currentBtn == scrollEui.leftBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            scrollEui.Dec();
            optionUi.warningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (currentBtn == scrollEui.rightBtn)
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            scrollEui.Inc();
            optionUi.warningTxt.gameObject.SetActive(true);
            return true;
        }

        return false;
    }


    // Apply
    private void Set_OptionValueApply()
    {
        if (!optionUi.warningTxt.gameObject.activeSelf) return;

        optionUi.warningTxt.gameObject.SetActive(false);

        ResourceManager.instance.Set_LanguageFont(optionUi.languagePanelEui.Get_CurrentIndex());
        GameManager.instance.Set_Screen(
            (eResolution)optionUi.resolutionPanelEui.Get_CurrentIndex(),
            (eScreenMode)optionUi.screenModePanelEui.Get_CurrentIndex());
        GameManager.instance.Set_FPS((eFPS)optionUi.fpsPanelEui.Get_CurrentIndex());
        SoundManager.instance.Set_BgmVolume(optionUi.bgmVolumePanelEui.Get_Value());
        SoundManager.instance.Set_SfxVolume(optionUi.sfxVolumePanelEui.Get_Value());

        SaveDataManager.instance.Save_OptionJsonData();
    }


    #endregion

    #endregion

    #region Language

    public override void Set_LanguageTxt()
    {
        startTxt.text = ResourceManager.instance.Get_StaticWord(89);
        optionTxt.text = ResourceManager.instance.Get_StaticWord(20);
        quitTxt.text = ResourceManager.instance.Get_StaticWord(21);

        optionUi.Set_LanguageTxt();
    }

    #endregion
}
