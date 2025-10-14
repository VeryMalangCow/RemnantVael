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
    [SerializeField] private RectTransform BtnsRT;
    [SerializeField] private CanvasGroup BGCG;

    [Space(10)]
    [Header("=== Element")]
    [SerializeField] private List<TitleElement> AllTitleElementUI;
    [SerializeField] private List<TitleTSElement> AllTitleTSElementUI;

    [Space(5)]
    [Header("-- Smoke")]
    [SerializeField] private List<TitleSmokeEUIController> AllTitleSmokeEUI;

    [Space(5)]
    [Header("-- Cloud")]
    [SerializeField] private List<RectTransform> CloudRTList_BackMoon;
    [SerializeField] private List<RectTransform> CloudRTList_FrontMoon;
    [SerializeField] private float CloudMovingLimit = 3000f;
    [SerializeField] private float CloudMovingTime = 1f;

    [Space(10)]
    [Header("=== Prefab")]
    [SerializeField] public GameObject SmokeCellEUIPrefab;
    [SerializeField] public GameObject CloudCellEUIPrefab;
    
    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private TitleOwnBtnEUIController StartBtn;
    [SerializeField] private TMP_Text StartTxt;
    [SerializeField] private TitleOwnBtnEUIController OptionBtn;
    [SerializeField] private TMP_Text OptionTxt;
    [SerializeField] private TitleOwnBtnEUIController QuitBtn;
    [SerializeField] private TMP_Text QuitTxt;
    [SerializeField] private RectTransform SelectedRT;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float DurTime;

    [Space(10)]
    [Header("=== Interact")]    
    [SerializeField] private IInteract CurrentInteractable;

    [Space(10)]
    [Header("=== Option")]
    [SerializeField] private OptionUIController OptionUI;
    [Serializable]
    public class OptionUIController
    {
        [SerializeField] public RectTransform PanelRT;
        [SerializeField] public Image BGImg;
        [SerializeField] public TitleOwnBtnEUIController BackBtn;
        [SerializeField] public TitleOwnBtnEUIController ApplyBtn;
        [SerializeField] public TMP_Text WarningTxt;
        [SerializeField] public bool IsOn = false;

        [Space(10)]
        [SerializeField] public TitleLRSlidingItemEUIController LanguagePanelEUI;
        [SerializeField] public TitleLRSlidingItemEUIController ScreenModePanelEUI;
        [SerializeField] public TitleLRSlidingItemEUIController ResolutionPanelEUI;
        [SerializeField] public TitleLRSlidingItemEUIController FPSPanelEUI;
        [SerializeField] public TitleFillScrollbarEUIController BGMVolumePanelEUI;
        [SerializeField] public TitleFillScrollbarEUIController SFXVolumePanelEUI;

        public void Offset(TitleLobbyUIController _UIController)
        {
            SetOff_Panel();

            BackBtn.OwnerUIController = _UIController;
            BackBtn.Offset();

            ApplyBtn.OwnerUIController = _UIController;
            ApplyBtn.Offset();

            LanguagePanelEUI.Set_OwnerUIController(_UIController);
            LanguagePanelEUI.Offset();

            ScreenModePanelEUI.Set_OwnerUIController(_UIController);
            ScreenModePanelEUI.Offset();

            ResolutionPanelEUI.Set_OwnerUIController(_UIController);
            ResolutionPanelEUI.Offset();

            FPSPanelEUI.Set_OwnerUIController(_UIController);
            FPSPanelEUI.Offset();

            BGMVolumePanelEUI.Set_OwnerUIController(_UIController);
            BGMVolumePanelEUI.Offset();

            SFXVolumePanelEUI.Set_OwnerUIController(_UIController);
            SFXVolumePanelEUI.Offset();
        }

        public void SetOn_Panel()
        {
            PanelRT.gameObject.SetActive(true);
            BGImg.gameObject.SetActive(true);
            WarningTxt.gameObject.SetActive(false);
            LanguagePanelEUI.Set_Item(GameManager.LanguageID);
            ScreenModePanelEUI.Set_Item((int)GameManager.ScreenMode);
            ResolutionPanelEUI.Set_Item((int)GameManager.ResolutionMode);
            FPSPanelEUI.Set_Item((int)GameManager.FPS);
            BGMVolumePanelEUI.Set_Value(SoundManager.Instance.BVolume);
            SFXVolumePanelEUI.Set_Value(SoundManager.Instance.SVolume);

            IsOn = true;
        }

        public void SetOff_Panel()
        {
            PanelRT.gameObject.SetActive(false);
            BGImg.gameObject.SetActive(false);

            IsOn = false;
        }

        public void Set_LanguageTxt()
        {
            WarningTxt.text = ResourceManager.Instance.Get_StaticDesc(31);

            DevTool.Get_ComponentTType<TMP_Text>(ApplyBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(ApplyBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(91);
            LanguagePanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(92);
            ScreenModePanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(140);
            ResolutionPanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(137);
            FPSPanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(141);
            BGMVolumePanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(138);
            SFXVolumePanelEUI.HeaderTxt.text = ResourceManager.Instance.Get_StaticWord(139);
        }
    }

    #endregion

    #region - Hide

    // Value
    [HideInInspector] public bool IsInIntro = true;
    [HideInInspector] private bool IsStarting = false;
    [HideInInspector] private float UIElementMovingPowerMultiple = 0.002f;

    // Btn
    [HideInInspector] private List<TitleOwnBtnEUIController> TitleAllBtns;
    [HideInInspector] private TitleOwnBtnEUIController CurrentMouseBtn = null;

    [HideInInspector] private List<Tween> CouldTween = null;

    // Option
    [HideInInspector] private bool IsInteractTweening = false;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        DevTool.Get_ComponentTType<Canvas>(gameObject).planeDistance = 10;

        StartBtn.Offset();
        StartBtn.OwnerUIController = this;

        OptionBtn.Offset();
        OptionBtn.OwnerUIController = this;

        QuitBtn.Offset();
        QuitBtn.OwnerUIController = this;

        TitleAllBtns = new List<TitleOwnBtnEUIController>
        { StartBtn, OptionBtn, QuitBtn };

        BGCG.alpha = 1f;

        IsStarting = false;

        for (int i = 0; i < AllTitleSmokeEUI.Count; i++)
        {
            AllTitleSmokeEUI[i].OwnerUIController = this;
            AllTitleSmokeEUI[i].Offset();
        }

        CouldTween = new List<Tween>
        {
            Play_CloudMoving_FromLeft(CloudRTList_BackMoon[0], CloudMovingTime),
            Play_CloudMoving_FromLeft(CloudRTList_FrontMoon[0], CloudMovingTime),
            Play_CloudMoving_FromCenter(CloudRTList_BackMoon[1], CloudMovingTime),
            Play_CloudMoving_FromCenter(CloudRTList_FrontMoon[1], CloudMovingTime)
        };

        OptionUI.Offset(this);
        Set_LanguageTxt();
    }

    #endregion

    #region Play

    private void SetOff_Play()
    {
        for (int i = 0; i < AllTitleSmokeEUI.Count; i++)
            AllTitleSmokeEUI[i].Stop_VFX();

        for (int i = 0; i < CouldTween.Count; i++)
            DevTool.Set_KillTween(CouldTween[i]);

        CouldTween = null;
    }

    #endregion

    #region Cloud

    private Tween Play_CloudMoving_FromLeft(RectTransform _CloudRT, float _DurTime)
    {
        Tween tween = _CloudRT.DOAnchorPosX(CloudMovingLimit, _DurTime)
            .OnComplete(() =>
            {
                _CloudRT.anchoredPosition = new Vector2(-CloudMovingLimit, 0);
            })
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);


        return tween;
    }

    private Tween Play_CloudMoving_FromCenter(RectTransform _CloudRT, float _DurTime)
    {
        Tween tween = _CloudRT.DOAnchorPosX(CloudMovingLimit, _DurTime * 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _CloudRT.anchoredPosition = new Vector2(-CloudMovingLimit, 0);
                _CloudRT.DOAnchorPosX(CloudMovingLimit, _DurTime).OnComplete(() =>
                {
                    _CloudRT.anchoredPosition = new Vector2(-CloudMovingLimit, 0);
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

        Set_CurrentBtn(StartBtn);
    }

    private void LateUpdate()
    {
        Vector2 movingPower = Get_MovingPowerVec() * -1;
        Set_UIElementTF(movingPower);
    }


    #endregion

    #region UI Element (TS)

    private void Set_UIElementTS(bool _Loop)
    {
        for (int i = 0; i < AllTitleTSElementUI.Count; i++)
        {
            int index = i;
            TitleTSElement trueShadowElementSet = AllTitleTSElementUI[index];
            Get_EachUIElementTS(trueShadowElementSet.ThisTSList, trueShadowElementSet.Max, trueShadowElementSet.Min, trueShadowElementSet.DurTime, _Loop);
        }
    }


    private void Get_EachUIElementTS(List<TrueShadow> _TSList, float _Max, float _Min, float _DurTime, bool _Loop = true)
    {
        for (int i = 0; i < _TSList.Count; i++)
        {
            int index = i;

            _TSList[index].Size = _Min;

            Tween tween = DOTween.To(
                () => _TSList[index].Size,
                x => _TSList[index].Size = x,
                _Max, _DurTime * 0.5f)
                .SetEase(Ease.Linear);

            if (_Loop)
                tween.SetLoops(-1, LoopType.Yoyo);
        }
    }

    #endregion

    #region UI Element (Pos)

    private void Set_UIElementTF(Vector2 _MovingPower)
    {
        for (int i = 0; i < AllTitleElementUI.Count; i++)
        {
            Vector2 thisVec = new Vector2(AllTitleElementUI[i].MovingPowerX, AllTitleElementUI[i].MovingPowerY);
            Vector2 targetVec = thisVec * _MovingPower;
            Set_EachUIElementPos(AllTitleElementUI[i].MovingRT, targetVec);
        }
    }

    private void Set_EachUIElementPos(RectTransform _RT, Vector2 _TargetVec)
    {
        _RT.anchoredPosition = _TargetVec;
    }

    private Vector2 Get_MovingPowerVec()
    {
        return UIElementMovingPowerMultiple * TitleInputManager.Instance.Get_AnchorMousePos();
    }

    #endregion

    #region Input

    public void Try_Interact()
    {
        if (CurrentBtn == null || CurrentMouseBtn == null || IsStarting || IsInIntro || IsInteractTweening)
            return; 

        TitleInputManager.Instance.Play_MousePointerClick();

        if (CurrentBtn == StartBtn) StartCoroutine(Play_Starting_Cor(2f));
        else if (CurrentBtn == OptionBtn) SetOn_OptionPanel();
        else if (CurrentBtn == OptionUI.BackBtn) SetOff_OptionPanel();
        else if (CurrentBtn == OptionUI.ApplyBtn) Set_OptionValueApply();
        else if (CurrentBtn == QuitBtn) Application.Quit();

        else if (Is_Interact_OptionElement(OptionUI.LanguagePanelEUI)) return;
        else if (Is_Interact_OptionElement(OptionUI.ScreenModePanelEUI)) return;
        else if (Is_Interact_OptionElement(OptionUI.ResolutionPanelEUI)) return;
        else if (Is_Interact_OptionElement(OptionUI.BGMVolumePanelEUI)) return;
        else if (Is_Interact_OptionElement(OptionUI.SFXVolumePanelEUI)) return;
        else if (Is_Interact_OptionElement(OptionUI.FPSPanelEUI)) return;
    }

    public void Try_OutInteract()
    {
        if (IsStarting || IsInIntro || IsInteractTweening)
            return;

        if (OptionUI.IsOn)
        {
            SetOff_OptionPanel();
        }
    }

    #endregion

    #region Btn

    public override void Set_CurrentBtn(TitleOwnBtnEUIController _TargetBtn)
    {
        if (CurrentBtn == _TargetBtn) return;

        base.Set_CurrentBtn(_TargetBtn);

        if (CurrentBtn != null && (CurrentBtn == StartBtn || CurrentBtn == OptionBtn || CurrentBtn == QuitBtn))
        {
            DevTool.Set_KillTween(SelectedRT);
            SelectedRT.DOAnchorPosY(CurrentBtn.ThisRT.anchoredPosition.y, 0.2f);
        }

        for (int i = 0; i < TitleAllBtns.Count; i++)
        {
            if (CurrentBtn == TitleAllBtns[i])
                TitleAllBtns[i].Set_SelectOnThis(0.2f);
            else
                TitleAllBtns[i].Set_SelectOffThis(0.2f);
        }
    }

    public void Set_CurrentMouseBtn(TitleOwnBtnEUIController _TargetBtn)
    {
        if (CurrentMouseBtn == _TargetBtn) return;

        CurrentMouseBtn = _TargetBtn;
    }

    #endregion

    #region Play

    private IEnumerator Play_Starting_Cor(float _DelayTime)
    {
        IsStarting = true;

        TitleLobbyUIManager.Instance.Get_JustFadeIn(_DelayTime);

        yield return new WaitForSeconds(_DelayTime + 0.2f);

        SetOff_Play();

        LoadingSceneManager.Instance.Play_LoadScene("MainGame");
    }

    #endregion

    #region Option

    #region Set (Option)

    private void SetOn_OptionPanel()
    {
        IsInteractTweening = true;

        OptionUI.SetOn_Panel();

        Sequence seq = DOTween.Sequence();

        seq.Append(OptionUI.PanelRT.DOAnchorPosX(440, 0.2f));
        seq.Join(OptionUI.BGImg.DOFade(0.7f, 0.2f));
        seq.OnComplete(() =>
        {
            IsInteractTweening = false;
        });
    }

    private void SetOff_OptionPanel()
    {
        IsInteractTweening = true;

        Sequence seq = DOTween.Sequence();

        seq.Append(OptionUI.PanelRT.DOAnchorPosX(2500, 0.2f));
        seq.Join(OptionUI.BGImg.DOFade(0f, 0.2f));
        seq.OnComplete(() =>
        {
            IsInteractTweening = false;
            OptionUI.SetOff_Panel();
        });
    }

    // Element
    private bool Is_Interact_OptionElement(TitleLRSlidingItemEUIController _LRSlidingEUI)
    {
        if (CurrentBtn == _LRSlidingEUI.LeftBtn)
        {
            _LRSlidingEUI.Change_Left();
            OptionUI.WarningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (CurrentBtn == _LRSlidingEUI.RightBtn)
        {
            _LRSlidingEUI.Change_Right();
            OptionUI.WarningTxt.gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    private bool Is_Interact_OptionElement(TitleFillScrollbarEUIController _ScrollEUI)
    {
        if (CurrentBtn == _ScrollEUI.LeftBtn)
        {
            _ScrollEUI.Dec();
            OptionUI.WarningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (CurrentBtn == _ScrollEUI.RightBtn)
        {
            _ScrollEUI.Inc();
            OptionUI.WarningTxt.gameObject.SetActive(true);
            return true;
        }

        return false;
    }


    // Apply
    private void Set_OptionValueApply()
    {
        OptionUI.WarningTxt.gameObject.SetActive(false);

        ResourceManager.Instance.Set_LanguageFont(OptionUI.LanguagePanelEUI.Get_CurrentIndex());
        GameManager.Instance.Set_Screen(
            (eResolution)OptionUI.ResolutionPanelEUI.Get_CurrentIndex(),
            (eScreenMode)OptionUI.ScreenModePanelEUI.Get_CurrentIndex());
        GameManager.Instance.Set_FPS((eFPS)OptionUI.FPSPanelEUI.Get_CurrentIndex());
        SoundManager.Instance.Set_BgmVolume(OptionUI.BGMVolumePanelEUI.Get_Value());
        SoundManager.Instance.Set_SfxVolume(OptionUI.SFXVolumePanelEUI.Get_Value());
    }


    #endregion

    #endregion

    #region Language

    public override void Set_LanguageTxt()
    {
        StartTxt.text = ResourceManager.Instance.Get_StaticWord(89);
        OptionTxt.text = ResourceManager.Instance.Get_StaticWord(20);
        QuitTxt.text = ResourceManager.Instance.Get_StaticWord(21);

        OptionUI.Set_LanguageTxt();
    }

    #endregion
}
