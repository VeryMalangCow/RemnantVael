using UnityEngine;
using UniRx;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PlayerHUDController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player HUD")]

    [Space(10)]
    [Header("=== Tab")]
    [SerializeField] public bool IsTabInteracted = false;
    [SerializeField] public bool IsTabInputed = false;
    [SerializeField] private static float TabInputedMaxTime = 0.25f;
    [SerializeField] private float TabInputedCurrentTime = 0f;
    [HideInInspector] private static float TabInteractDurTime = 0.25f;

    [Header("-- All")]
    [SerializeField] private List<CanvasGroup> ParentCGList;

    [Header("-- Modules")]
    [SerializeField] private RectTransform ModuleListParentRT;
    [HideInInspector] private float DefaultModuleRectX;
    [HideInInspector] public List<InventorySlotEUIController> MEISList;

    [Header("-- Player States")]
    [SerializeField] private RectTransform PlayerStatesCostParentRT;
    [HideInInspector] private float DefaultPlayerStatesRectX;
    [SerializeField] private List<string> PlayerStatesStringList;
    [SerializeField] private TMP_Text PlayerStatesTxt;

    [Header("-- Skill State")]
    [SerializeField] private RectTransform SkillStatesParentRT;
    [HideInInspector] private float DefaultSkillStatesRectY;
    [SerializeField] private List<string> SkillStatesStringList;
    [SerializeField] private List<TMP_Text> SkillStatesTxtList;


    // Tab
    [HideInInspector] private Sequence TabSeq;

    [Space(10)]
    [Header("=== Energy")]
    [SerializeField] private ProgressBarEUIController EP;
    [SerializeField] private List<Image> EPInnerImgList;
    [SerializeField] private RectTransform EP_FlowRT;

    [Space(10)]
    [Header("=== Shield")]
    [SerializeField] private RectTransform ShieldRT;
    [SerializeField] private TMP_Text ShieldTxt;

    [Space(10)]
    [Header("=== Bettery")]
    [Header("-- Current")]
    [SerializeField] public ChargeSpriteEUIController CurrentEmptyBC;

    [Header("-- Bettery (Image Amount Class)")]
    [SerializeField] private ImgTxtAmountEUIController EmptyBC;
    [SerializeField] private ImgTxtAmountEUIController FullEC;
    [SerializeField] private Image ECCostArrowImg;
    [SerializeField] private TMP_Text ECCostTxt;

    [Space(10)]
    [Header("=== Other Item")]
    [SerializeField] private RectTransform MSRT;
    [SerializeField] private Image MS_InnerImg;
    [SerializeField] private TMP_Text MS_AmountTxt;

    [Space(10)]
    [Header("=== Boost")]
    [SerializeField] private RectTransform BoostRT;
    [HideInInspector] private float DefaultBoostRectY;
    [SerializeField] private TMP_Text BoostLv;
    [SerializeField] private GameObject[] BoostLightArr;
    [SerializeField] private GameObject[] BoostLightWheelArr;
    [SerializeField] List<Image> BoostInnerList;

    

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] public List<SkillEUIController> SkillList;

    [Space(10)]
    [Header("=== Minimap")]
    [SerializeField] public MinimapEUIController ThisMinimap;

    [Space(10)]
    [Header("=== Map Anno")]
    [SerializeField] private TMP_Text StageNameTxt;
    [SerializeField] private TMP_Text StageDescriptionTxt;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private TMP_Text InteractOnOffTxt;
    [SerializeField] private TMP_Text InteractDesctiptionTxt;

    [SerializeField] private Image InnerImg;
    [SerializeField] private Image UsingInnerImg;

    [HideInInspector] private bool IsActingInteractUI = false;

    [Header("=== Color Or Icon")]
    [Header("-- Icon")]
    [SerializeField] private List<Image> ESImgList;
    [SerializeField] private List<Image> SkillImgList;

    [Header("-- Buff")]
    [SerializeField] private Transform BuffParentTF;
    [SerializeField] private List<BuffIconEUIController> AllBuffIconUI;
    [SerializeField] private float BuffUI_XInterval = 12;

    [Header("-- Screen")]
    [SerializeField] private Image HittedScreen;


    [Header("-- MainColor")]
    [HideInInspector] public List<Component> MainColorCompList;

    [Header("-- SubColor")]
    [HideInInspector] public List<Component> SubColorCompList;


    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        EP.Offset();
        CurrentEmptyBC.Offset();
        EmptyBC.Offset();
        FullEC.Offset();

        for (int i = 0; i < SkillList.Count; i++)
        {
            SkillList[i].Offset();
        }
        ThisMinimap.Offset();
    }

    protected override void Offset_UI()
    {
        #region Reactive

        PlayerManager.Instance.PlayerController.MaxEP.ActualState
            .Subscribe(_MaxEP =>
            {
                EP.Set_MaxFillRT(_MaxEP * 3);

                EP.Set_FillImgSmooth(
                    PlayerManager.Instance.PlayerController.Get_CurrentEP().Value,
                    PlayerManager.Instance.PlayerController.MaxEP.ActualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.Get_CurrentEP()
            .Subscribe(_CurrentEP =>
            {
                EP.Set_FillImgSmooth(
                    PlayerManager.Instance.PlayerController.Get_CurrentEP().Value,
                    PlayerManager.Instance.PlayerController.MaxEP.ActualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBS
            .Subscribe(_CurrentBS =>
            {
                if (PlayerManager.Instance.PlayerController.CurrentBS.Value < PlayerManager.Instance.PlayerController.NeedBS_ForMakeBC)
                {
                    CurrentEmptyBC.Change_Sprite(_CurrentBS);
                }
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBC
            .Subscribe(_CurrentBC =>
            {
                EmptyBC.Set_Amount(_CurrentBC, 0.5f);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentEC
            .Subscribe(_CurrentEC =>
            {
                FullEC.Set_Amount(_CurrentEC, 0.5f);

                DOTween.Kill(ECCostArrowImg);
                ECCostArrowImg.DOFade(1f, 0.2f)
                    .OnComplete(() =>
                    {
                        ECCostArrowImg.DOFade(0.25f, 0.2f);
                    });
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentMS
            .Subscribe(_CurrentMS =>
            {
                Set_MSAmount(_CurrentMS);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBoostLv
            .Subscribe(_BoostLevel =>
            {
                Debug.Assert(_BoostLevel >= 0 && _BoostLevel <= 4, "Boost Range Out!");
                Set_TextOfBoost(_BoostLevel);

                Set_ActiveAmountBoost(BoostLightArr, _BoostLevel);
                Set_ActiveAmountBoost(BoostLightWheelArr, _BoostLevel);

                Set_RollAmountBoost(BoostLightWheelArr, _BoostLevel);
            })
            .AddTo(gameObject);

        #endregion

        #region Other Offset

        EP_FlowRT.DOAnchorPos(new Vector2(1920, 0), 2f, false)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        ECCostTxt.text = PlayerManager.Instance.PlayerController.NeedEP_ForMakeEC.ToString();

        MEISList = new List<InventorySlotEUIController>();
        for (int i = 0; i < ModuleListParentRT.transform.childCount; i++)
        {
            if (ModuleListParentRT.transform.GetChild(i).gameObject.TryGetComponent(out InventorySlotEUIController MEIS))
            {
                MEISList.Add(MEIS);
                MEIS.Offset();
            }
        }

        if (ModuleListParentRT.TryGetComponent(out RectTransform M_Rt))
        {
            DefaultModuleRectX = M_Rt.anchoredPosition.x;
        }
        if (PlayerStatesCostParentRT.TryGetComponent(out RectTransform PS_Rt))
        {
            DefaultPlayerStatesRectX = PS_Rt.anchoredPosition.x;
        }
        if (SkillStatesParentRT.TryGetComponent(out RectTransform SS_Rt))
        {
            DefaultSkillStatesRectY = SS_Rt.anchoredPosition.y;
        }

        for (int i = 0; i < ParentCGList.Count; i++)
        {
            ParentCGList[i].alpha = 0f;
        }

        DefaultBoostRectY = BoostRT.anchoredPosition.y;

        StageNameTxt.color = GetFullAlphaColor(StageNameTxt, 1);
        StageDescriptionTxt.color = GetFullAlphaColor(StageDescriptionTxt, 0);

        Color GetFullAlphaColor(TMP_Text _Txt, float _A)
        {
            Color clr = _Txt.color;
            clr.a = _A;
            return clr;
        }

        #endregion

        #region Set Img

        // Set Img

        for (int i = 0; i < ESImgList.Count; i++)
        {
            ESImgList[i].sprite = PlayerManager.Instance.PlayerController.ES_Sprite;
            ESImgList[i].SetNativeSize();
        }

        for (int i = 0; i < SkillImgList.Count; i++)
        {
            SkillImgList[i].sprite = PlayerManager.Instance.PlayerController.SkillWeapon.SkillList[i].ThisSkillUISprite;
        }

        #endregion

        #region Set Color

        // Main
        MainColorCompList.Add(EP.AfterImg.gameObject.transform.GetChild(0).GetComponent<Image>());
        MainColorCompList.Add(EP.ActualImg.gameObject.transform.GetChild(0).GetComponent<Image>());
        MainColorCompList.Add(EP.ActualImgLiner.gameObject.transform.GetComponent<Image>());

        MainColorCompList.Add(BoostLv);
        for (int i = 0; i < BoostLightArr.Length; i++)
        { MainColorCompList.Add(BoostLightArr[i].GetComponent<Image>()); MainColorCompList.Add(BoostLightWheelArr[i].GetComponent<Image>()); }

        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            MainColorCompList.Add(SkillList[i].SkillCostTxt);
            MainColorCompList.Add(SkillList[i].SkillErrorTxt);
            MainColorCompList.Add(SkillStatesTxtList[i]);

            SubColorCompList.Add(SkillList[i].SkillInnerImg);
        }

        MainColorCompList.Add(PlayerStatesTxt);

        MainColorCompList.Add(StageNameTxt);
        MainColorCompList.Add(StageDescriptionTxt);

        MainColorCompList.Add(ECCostTxt);
        MainColorCompList.Add(MS_AmountTxt);

        MainColorCompList.Add(EmptyBC.Txt_ExtraAmount);
        MainColorCompList.Add(FullEC.Txt_ExtraAmount);

        MainColorCompList.Add(InteractOnOffTxt);

        // Sub
        SubColorCompList.AddRange(EPInnerImgList);
        SubColorCompList.AddRange(BoostInnerList);


        SubColorCompList.Add(ThisMinimap.InnerImg);
        SubColorCompList.Add(CurrentEmptyBC.LightInner);

        SubColorCompList.Add(ECCostArrowImg);
        SubColorCompList.Add(EmptyBC.InnerImg);
        SubColorCompList.Add(FullEC.InnerImg);
        SubColorCompList.Add(MS_InnerImg);

        SubColorCompList.Add(InnerImg);
        SubColorCompList.Add(UsingInnerImg);

        // Color Set
        Color mainClr = PlayerManager.Instance.PlayerController.Get_Color_CorrectHitted(eDamageType.Energy, false);
        Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.Get_Color_CorrectHitted(eDamageType.Energy, true);
        Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
        #endregion

        #region Buff

        PoolingManager.Instance.BuffIcons.ParentTF = BuffParentTF;

        #endregion

    }

    #endregion

    #region Set Tab

    private void Reset_Tab()
    {
        PlayerController pc = PlayerManager.Instance.PlayerController;
        PlayerWeaponController pwc = pc.BaseWeapon;
        SkillWeaponController pswc = pc.SkillWeapon;

        SetPlayerState();

        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            SetSkillState(SkillStatesTxtList[i], pswc.SkillList[i]);
        }

        void SetPlayerState()
        {
            string playerStateTotalString = "";
            List<string> playerActualDataList = new List<string>()
            {
                pc.MaxEP.ActualState.Value.ToString(),
                pc.WalkSpeed.ActualState.Value.ToString(),
                pc.DashController.DashSpeed.ActualState.Value.ToString(),
                pc.DashController.NeedEP_ForDash.ToString(),
                pwc.BaseDamage.ActualState.Value.ToString(),
                pwc.ROF.ActualState.Value.ToString(),
                pwc.AccuracyRate.ActualState.Value.ToString(),
                pwc.CC.ActualState.Value.ToString(),
                pwc.CD.ActualState.Value.ToString()

            };
            for (int i = 0; i < PlayerStatesStringList.Count; i++)
            {
                playerStateTotalString += "<size=70%>" + PlayerStatesStringList[i] + ": </size>";
                playerStateTotalString += "<b>" + playerActualDataList[i] + "</b>\n";
            }
            PlayerStatesTxt.text = playerStateTotalString;
        }

        void SetSkillState(TMP_Text _TxtComp, ActiveSkillController _SkillController)
        {
            string skillStateTotalString = "";
            List<string> skillActualDataList = new List<string>()
            {
                _SkillController.Tier.ActualState.Value.ToString(),
                _SkillController.Power.ActualState.Value.ToString()
            };
            for (int i = 0; i < SkillStatesStringList.Count; i++)
            {
                skillStateTotalString += "<size=70%>" + SkillStatesStringList[i] + ": </size>\n";
                skillStateTotalString += "<b>" + skillActualDataList[i] + "</b>\n";
            }
            _TxtComp.text = skillStateTotalString;
        }
    }

    #endregion

    #region Framework

    private void Update()
    {
        Caculate_TabInput();
    }

    #endregion

    #region Item

    public void Set_MSAmount(int _Amount)
    {
        DOTween.Kill(MSRT);
        DOTween.Kill(MS_InnerImg);

        MS_InnerImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                MS_InnerImg.DOFade(0.25f, 0.2f);
            });

        MSRT.DOScale(1.2f, 0.2f)
            .OnComplete(() =>
            {
                MSRT.DOScale(1.0f, 0.2f);
            });
        MS_AmountTxt.text = _Amount.ToString();
    }

    #endregion

    #region Shield

    public void Set_ShieldGage(float _TotalShield)
    {
        DOTween.Kill(ShieldRT);
        ShieldRT.DOSizeDelta(new Vector2(8 + (_TotalShield * 3), ShieldRT.sizeDelta.y), 1f);
        ShieldTxt.text = "<size=75%>( </size>" + Mathf.Round(_TotalShield).ToString() + "<size=75%> )</size>";
    }

    #endregion

    #region Boost

    private void Set_TextOfBoost(int _CurrentLv)
    {
        BoostLv.text = _CurrentLv.ToString();
        Color clr = BoostLv.color;
        clr.a = 0.25f * (_CurrentLv + 1);
        BoostLv.color = clr;
        
    }

    private void Set_ActiveAmountBoost(GameObject[] _Arr, int _CurrentLv)
    {
        for (int i = 0; i < _Arr.Length; i++)
        {
            if (_Arr[i].TryGetComponent(out Image img))
            {
                if (DOTween.IsTweening(img))
                { DOTween.Kill(img); }

                if (i < _CurrentLv)
                {
                    _Arr[i].SetActive(true);
                    img.DOFade(1f, 0.2f);
                }
                else
                {
                    img.DOFade(0f, 0.2f)
                        .OnComplete(() =>
                        {
                            _Arr[i].SetActive(false);
                        });
                }
            }
        }
    }

    private void Set_RollAmountBoost(GameObject[] _Arr, int _CurrentLv)
    {
        for (int i = 0; i < _Arr.Length; i++)
        {
            _Arr[i].TryGetComponent(out RectTransform rt);
            
            if (i < _CurrentLv)
            {
                rt.DOLocalRotate(new Vector3(0, 0, 360), 0.2f, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        rt.DOLocalRotate(new Vector3(0, 0, 360), 3f / (i + 1f), RotateMode.LocalAxisAdd)
                            .SetEase(Ease.Linear)
                            .SetLoops(-1, LoopType.Restart);
                    });
            }
            else
            {
                DOTween.Kill(rt);
            }
        }
    }

    #endregion

    #region Description

    public void Set_StateInteractUI()
    {
        if (IsActingInteractUI)
        { return; }

        IInteract ii = PlayerManager.Instance.PlayerController.CurrentInteractable.Value;
        string txt = Get_KindOfCaseString(ii);

        if (ii != null && txt != "")
        {
            Set_EnableInteract(txt);
        }
        else
        {
            Set_DisableInteract();
        }
    }


    private void Set_DisableInteract()
    {
        DOTween.Kill(InteractOnOffTxt);
        InteractOnOffTxt.DOFade(0.25f, 0.5f);
        InteractOnOffTxt.text = "-DISABLE-";

        DOTween.Kill(InteractDesctiptionTxt);
        InteractDesctiptionTxt.DOFade(0.25f, 0.5f);
        InteractDesctiptionTxt.text = "< NONE >";
    }

    private void Set_EnableInteract(string _Interactable)
    {
        DOTween.Kill(InteractOnOffTxt);
        InteractOnOffTxt.DOFade(1f, 0.5f);
        InteractOnOffTxt.text = "-ENABLE-";

        DOTween.Kill(InteractDesctiptionTxt);
        InteractDesctiptionTxt.DOFade(1f, 0.5f);
        InteractDesctiptionTxt.text = "< " + _Interactable + " >";
    }


    public void Set_UseInteractUI()
    {
        IsActingInteractUI = true;

        DOTween.Kill(UsingInnerImg);
        UsingInnerImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                UsingInnerImg.DOFade(0.25f, 0.2f)
                .OnComplete(() =>
                {
                    IsActingInteractUI = false;
                    Set_StateInteractUI();
                });
            });
    }


    public void Set_StageDescription(string _StageName, string _StageDescription)
    {
        StageNameTxt.DOText(_StageName, 0.5f)
            .OnPlay(() =>
            {
                StageNameTxt.text = "";
            });
        StageDescriptionTxt.DOText(_StageDescription, 0.5f)
            .OnPlay(() =>
            {
                StageDescriptionTxt.text = "";
            });
    }

    #endregion

    #region Tab

    private void Caculate_TabInput()
    {
        if (IsTabInputed) // ¿Œ«≤ O
        {
            if (TabInputedMaxTime > TabInputedCurrentTime) // ¿Œ«≤ Ω√∞£ ∞ËªÍ
            {
                TabInputedCurrentTime += Time.deltaTime;
            }
            else // ¿Œ«≤ Ω√∞£ √Ê∫– ªÛ≈¬
            {
                if (!IsTabInteracted)
                {
                    SetOn_TabInteract();
                }
            }
        }
        else // ¿Œ«≤ X
        {
            if (TabInputedCurrentTime != 0)
            {
                TabInputedCurrentTime = 0f;
            }
            if (IsTabInteracted)
            {
                SetOff_TabInteract();
            }
        }
    }

    public void SetOn_TabInteract()
    {
        if (IsTabInteracted)
        { return; }
        IsTabInteracted = true;

        if (TabSeq != null && DOTween.IsTweening(TabSeq))
        { DOTween.Kill(TabSeq); }
        TabSeq = DOTween.Sequence();

        Reset_Tab();

        TabSeq.Join(ModuleListParentRT.DOAnchorPosX(0f, TabInteractDurTime));
        TabSeq.Join(PlayerStatesCostParentRT.DOAnchorPosX(0f, TabInteractDurTime));
        TabSeq.Join(SkillStatesParentRT.DOAnchorPosY(0f, TabInteractDurTime));
        TabSeq.Join(BoostRT.DOAnchorPosY(0f, TabInteractDurTime));
        TabSeq.Join(StageNameTxt.DOFade(0f, TabInteractDurTime));
        TabSeq.Join(StageDescriptionTxt.DOFade(1f, TabInteractDurTime));

        TabSeq.SetEase(Ease.OutCubic);
        for (int i = 0; i < ParentCGList.Count; i++)
        {
            ParentCGList[i].DOFade(1, TabInteractDurTime);
        }

        ThisMinimap.SetOn_TabInteract(TabInteractDurTime);
    }

    public void SetOff_TabInteract()
    {
        if (!IsTabInteracted)
        { return; }
        IsTabInteracted = false;
        TabInputedCurrentTime = 0f;

        if (TabSeq != null && DOTween.IsTweening(TabSeq))
        { DOTween.Kill(TabSeq); }
        TabSeq = DOTween.Sequence();

        TabSeq.Join(ModuleListParentRT.DOAnchorPosX(DefaultModuleRectX, TabInteractDurTime));
        TabSeq.Join(PlayerStatesCostParentRT.DOAnchorPosX(DefaultPlayerStatesRectX, TabInteractDurTime));
        TabSeq.Join(SkillStatesParentRT.DOAnchorPosY(DefaultSkillStatesRectY, TabInteractDurTime));
        TabSeq.Join(BoostRT.DOAnchorPosY(DefaultBoostRectY, TabInteractDurTime));
        TabSeq.Join(StageNameTxt.DOFade(1f, TabInteractDurTime));
        TabSeq.Join(StageDescriptionTxt.DOFade(0f, TabInteractDurTime));

        TabSeq.SetEase(Ease.InCubic);
        for (int i = 0; i < ParentCGList.Count; i++)
        {
            ParentCGList[i].DOFade(0, TabInteractDurTime);
        }

        ThisMinimap.SetOff_TabInteract(TabInteractDurTime);
    }

    #endregion

    #region Buff

    public void Set_GainBuffUI(BuffIconEUIController _MBI)
    {
        if (!AllBuffIconUI.Contains(_MBI))
        {
            AllBuffIconUI.Add(_MBI);
        }
        Set_BuffPosUI();
    }

    public void Set_ReductBuffUI(BuffIconEUIController _MBI)
    {
        Set_BuffPosUI();
    }

    public void Set_EndBuffUI(BuffIconEUIController _MBI)
    {
        if (AllBuffIconUI.Contains(_MBI))
        {
            AllBuffIconUI.Remove(_MBI);
        }
        Set_BuffPosUI();
    }

    private void Set_BuffPosUI()
    {
        for (int i = 0; i < AllBuffIconUI.Count; i++)
        {
            AllBuffIconUI[i].ThisRT.anchoredPosition = new Vector2(i * (AllBuffIconUI[i].ThisRT.rect.width + BuffUI_XInterval), 0);
        }
    }

    #endregion

    #region Screen

    public void Play_HittedPlayScreen(float _Dmg)
    {
        if (DOTween.IsTweening(HittedScreen))
        { DOTween.Kill(HittedScreen); }

        _Dmg = Math.Min(100, _Dmg) / 100;


        Sequence seq = DOTween.Sequence();
        seq.Append(HittedScreen.DOFade(_Dmg, 0.1f));
        seq.Append(HittedScreen.DOFade(0, 0.1f));
    }

    #endregion
}

