using UnityEngine;
using UniRx;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using LeTai.TrueShadow;
using System.Collections.Generic;

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
    [HideInInspector] public List<ModifyEachInventorySlot> MEISList;

    [Header("-- Player States")]
    [SerializeField] private RectTransform PlayerStatesCostParentRT;
    [HideInInspector] private float DefaultPlayerStatesRectX;
    [SerializeField] private List<string> PlayerStatesStringList;
    [SerializeField] private TMP_Text PlayerStatesTxt;

    [Header("-- Skill State")]
    [SerializeField] private RectTransform SkillStatesParentRT;
    [HideInInspector] private float DefaultSkillStatesRectY;
    [SerializeField] private List<string> SkillStatesStringList;
    [SerializeField] private TMP_Text Skill0StatesTxt;
    [SerializeField] private TMP_Text Skill1StatesTxt;

    // Tab
    [HideInInspector] private Sequence TabSeq;

    [Space(10)]
    [Header("=== Energy")]
    [SerializeField] private ModifyReductionFocusProgressBar EP;
    [SerializeField] private RectTransform EP_FlowRT;

    [Space(10)]
    [Header("=== Bettery")]
    [Header("-- Current")]
    [SerializeField] public ModifySprite CurrentEmptyBC;

    [Header("-- Bettery (Image Amount Class)")]
    [SerializeField] private ModifyImgAmountAndTxt EmptyBC;
    [SerializeField] private ModifyImgAmountAndTxt FullEC;
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

    

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] public ModifySkillSet Skill0;
    [SerializeField] public ModifySkillSet Skill1;

    [Space(10)]
    [Header("=== Minimap")]
    [SerializeField] public ModifyMinimap ThisMinimap;

    [Space(10)]
    [Header("=== Discription")]
    [SerializeField] private TMP_Text InteractOnOffTxt;
    [SerializeField] private TMP_Text InteractDesctiptionTxt;
    [SerializeField] private TMP_Text StageDesctiptionTxt;

    [SerializeField] private Image InnerImg;
    [SerializeField] private Image UsingInnerImg;

    [HideInInspector] private bool IsActingInteractUI = false;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        EP.Offset();
        CurrentEmptyBC.Offset();
        EmptyBC.Offset();
        FullEC.Offset();

        Skill0.Offset();
        Skill1.Offset();
        ThisMinimap.Offset();
    }

    protected override void Offset_UI()
    {
        PlayerManager.Instance.PlayerController.MaxEP.ActualState
            .Subscribe(_MaxEP =>
            {
                EP.SetMaxFillRT(_MaxEP * 3);

                EP.SetFillImgSmooth(
                    PlayerManager.Instance.PlayerController.CurrentEP.Value,
                    PlayerManager.Instance.PlayerController.MaxEP.ActualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentEP
            .Subscribe(_CurrentEP =>
            {
                EP.SetFillImgSmooth(
                    PlayerManager.Instance.PlayerController.CurrentEP.Value,
                    PlayerManager.Instance.PlayerController.MaxEP.ActualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBS
            .Subscribe(_CurrentBS =>
            {
                if (PlayerManager.Instance.PlayerController.CurrentBS.Value < PlayerManager.Instance.PlayerController.NeedBS_ForMakeBC)
                {
                    CurrentEmptyBC.Modify_Sprite(_CurrentBS);
                }
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBC
            .Subscribe(_CurrentBC =>
            {
                EmptyBC.SetAmount(_CurrentBC, 0.5f);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentEC
            .Subscribe(_CurrentEC =>
            {
                FullEC.SetAmount(_CurrentEC, 0.5f);

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
                SetMSAmount(_CurrentMS);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBoostLv
            .Subscribe(_BoostLevel =>
            {
                SetTextOfBoost(_BoostLevel);

                SetActiveAmountBoost(BoostLightArr, _BoostLevel);
                SetActiveAmountBoost(BoostLightWheelArr, _BoostLevel);

                SetRollAmountBoost(BoostLightWheelArr, _BoostLevel);
            })
            .AddTo(gameObject);

        EP_FlowRT.DOAnchorPos(new Vector2(1920, 0), 2f, false)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        Transform[] allChildren = this.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if(child.gameObject.TryGetComponent(out TrueShadow ts))
            {
                float sizeDefault = ts.Size * 1.5f;
                DOTween.To(() => ts.Size, x => ts.Size = x, sizeDefault, 1)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        ECCostTxt.text = PlayerManager.Instance.PlayerController.NeedEP_ForMakeEC.ToString();

        MEISList = new List<ModifyEachInventorySlot>();
        for (int i = 0; i < ModuleListParentRT.transform.childCount; i++)
        {
            if (ModuleListParentRT.transform.GetChild(i).gameObject.TryGetComponent(out ModifyEachInventorySlot MEIS))
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
    }

    #endregion

    #region Set Tab

    private void ResetTab()
    {
        PlayerController pc = PlayerManager.Instance.PlayerController;
        PlayerWeaponController pwc = pc.BaseWeapon;
        SkillWeaponController pswc = pc.SkillWeapon;

        SetPlayerState();

        SetSkillState(Skill0StatesTxt, pswc.Skill_0);
        SetSkillState(Skill1StatesTxt, pswc.Skill_1);

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
        CaculateTabInput();
    }

    #endregion

    #region Item

    public void SetMSAmount(int _Amount)
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

    #region Boost

    private void SetTextOfBoost(int _CurrentLv)
    {
        BoostLv.text = _CurrentLv.ToString();
        Color clr = BoostLv.color;
        clr.a = 0.25f * (_CurrentLv + 1);
        BoostLv.color = clr;
        
    }

    private void SetActiveAmountBoost(GameObject[] _Arr, int _CurrentLv)
    {
        for (int i = 0; i < _Arr.Length; i++)
        {
            _Arr[i].TryGetComponent(out Image img);
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

    private void SetRollAmountBoost(GameObject[] _Arr, int _CurrentLv)
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
    
    public void SetStateInteractUI()
    {
        if (IsActingInteractUI)
        { return; }

        IInteract ii = PlayerManager.Instance.PlayerController.CurrentInteractable.Value;
        string txt = GetKindOfCaseString(ii);

        if (ii != null && txt != "")
        {
            SetEnableInteract(txt);
        }
        else
        {
            SetDisableInteract();
        }
    }


    private void SetDisableInteract()
    {
        DOTween.Kill(InteractOnOffTxt);
        InteractOnOffTxt.DOFade(0.25f, 0.5f);
        InteractOnOffTxt.text = "-DISABLE-";

        DOTween.Kill(InteractDesctiptionTxt);
        InteractDesctiptionTxt.DOFade(0.25f, 0.5f);
        InteractDesctiptionTxt.text = "< NONE >";
    }

    private void SetEnableInteract(string _Interactable)
    {
        DOTween.Kill(InteractOnOffTxt);
        InteractOnOffTxt.DOFade(1f, 0.5f);
        InteractOnOffTxt.text = "-ENABLE-";

        DOTween.Kill(InteractDesctiptionTxt);
        InteractDesctiptionTxt.DOFade(1f, 0.5f);
        InteractDesctiptionTxt.text = "< " + _Interactable + " >";
    }


    public void SetUseInteractUI()
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
                    SetStateInteractUI();
                });
            });
    }


    public void SetStageDescription(string _StageName)
    {
        StageDesctiptionTxt.DOText(_StageName, 0.5f)
            .OnPlay(() =>
            {
                StageDesctiptionTxt.text = "";
            });
    }

    #endregion

    #region Tab

    private void CaculateTabInput()
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
                    OnTabInteract();
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
                OffTabInteract();
            }
        }
    }

    public void OnTabInteract()
    {
        if (IsTabInteracted)
        { return; }
        IsTabInteracted = true;

        if (TabSeq != null && DOTween.IsTweening(TabSeq))
        { DOTween.Kill(TabSeq); }
        TabSeq = DOTween.Sequence();

        ResetTab();

        TabSeq.Join(ModuleListParentRT.DOAnchorPosX(0f, TabInteractDurTime));
        TabSeq.Join(PlayerStatesCostParentRT.DOAnchorPosX(0f, TabInteractDurTime));
        TabSeq.Join(SkillStatesParentRT.DOAnchorPosY(0f, TabInteractDurTime));
        TabSeq.Join(BoostRT.DOAnchorPosY(0f, TabInteractDurTime));

        TabSeq.SetEase(Ease.OutCubic);
        for (int i = 0; i < ParentCGList.Count; i++)
        {
            ParentCGList[i].DOFade(1, TabInteractDurTime);
        }

        ThisMinimap.OnTabInteract(TabInteractDurTime);
    }

    public void OffTabInteract()
    {
        if (!IsTabInteracted)
        { return; }
        IsTabInteracted = false;

        if (TabSeq != null && DOTween.IsTweening(TabSeq))
        { DOTween.Kill(TabSeq); }
        TabSeq = DOTween.Sequence();

        TabSeq.Join(ModuleListParentRT.DOAnchorPosX(DefaultModuleRectX, TabInteractDurTime));
        TabSeq.Join(PlayerStatesCostParentRT.DOAnchorPosX(DefaultPlayerStatesRectX, TabInteractDurTime));
        TabSeq.Join(SkillStatesParentRT.DOAnchorPosY(DefaultSkillStatesRectY, TabInteractDurTime));
        TabSeq.Join(BoostRT.DOAnchorPosY(DefaultBoostRectY, TabInteractDurTime));

        TabSeq.SetEase(Ease.InCubic);
        for (int i = 0; i < ParentCGList.Count; i++)
        {
            ParentCGList[i].DOFade(0, TabInteractDurTime);
        }

        ThisMinimap.OffTabInteract(TabInteractDurTime);
    }

    #endregion
}

