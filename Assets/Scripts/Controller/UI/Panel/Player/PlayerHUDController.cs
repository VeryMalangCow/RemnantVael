using UnityEngine;
using UniRx;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerHUDController : UIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Player HUD")]

    [Space(10)]
    [Header("=== Tab")]
    [SerializeField] public bool IsTabInteracted = false;
    [SerializeField] public bool IsTabInputed = false;
    [SerializeField] private static float TabInputedMaxTime = 0.25f;
    [SerializeField] private float TabInputedCurrentTime = 0f;

    [Header("-- All")]
    [SerializeField] private List<CanvasGroup> ParentCGList;

    [Header("-- Modules")]
    [SerializeField] private RectTransform ModuleListParentRT;

    [Header("-- Player States")]
    [SerializeField] private RectTransform PlayerStatesCostParentRT;
    [SerializeField] private TMP_Text PlayerStatesTxt;

    [Header("-- Skill State")]
    [SerializeField] private RectTransform SkillStatesParentRT;
    [SerializeField] private List<TMP_Text> SkillStatesTxtList;

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
    [SerializeField] private LootableItemEUIController Credit_EUI;
    [SerializeField] private LootableItemEUIController Overrider_EUI;
    [SerializeField] private LootableItemEUIController MS_EUI;

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
    [SerializeField] private Color UninteractableColor;

    [Space(10)]
    [Header("=== Ally")]
    [SerializeField] private AllyPresenceEUIController ST_AllyPresence;
    [SerializeField] private AllyPresenceEUIController UT_AllyPresence;
    [SerializeField] private AllyPresenceEUIController NT_AllyPresence;

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] private Transform BuffParentTF;
    [SerializeField] public List<BuffIconEUIController> AllBuffIconUI;
    [SerializeField] private float BuffUI_XInterval = 12;

    [Space(10)]
    [Header("=== Screen")]
    [SerializeField] private Image HittedScreen;
    [SerializeField] private Transform HittedInfoPivotTF;
    [SerializeField] private RectTransform HittedInfoRT;
    [SerializeField] private TMP_Text HittedDmgTxt;
    [SerializeField] private CanvasGroup PaneltyAnnoCG;
    [SerializeField] private TMP_Text PaneltyAnnoNameTxt;
    [SerializeField] private TMP_Text PaneltyAnnoDescTxt;

    #endregion

    #region - Hide

    // String
    [HideInInspector] private static string InteractEnableString;
    [HideInInspector] private static string InteractDisableString;
    [HideInInspector] private static string InteracInoperableString;
    [HideInInspector] private static string InteractNoneString;
    [HideInInspector] private List<string> PlayerStatesStringList = new List<string>();
    [HideInInspector] private List<string> SkillStatesStringList = new List<string>();

    // Comp
    [HideInInspector] public CanvasGroup ThisCG;

    // Tab
    [HideInInspector] private static float TabInteractDurTime = 0.25f;

    // Tab -> Module
    [HideInInspector] private float DefaultModuleRectX;
    [HideInInspector] public List<InventorySlotEUIController> ModuleSlots = new List<InventorySlotEUIController>();

    // Tab -> Skill State
    [HideInInspector] private List<Image> SkillImgList = new List<Image>();
    [HideInInspector] private float DefaultPlayerStatesRectX;
    [HideInInspector] private float DefaultSkillStatesRectY;

    // Color
    [HideInInspector] public List<Component> MainColorCompList;
    [HideInInspector] public List<Component> SubColorCompList;
    [HideInInspector] private Color InteractableColor;

    // Tab
    [HideInInspector] private Sequence TabSeq;

    // Interact
    [HideInInspector] private bool IsActingInteractUI = false;

    // Ally
    [HideInInspector] private List<AllyPresenceEUIController> AllAllyPresence = new List<AllyPresenceEUIController>();

    // Hitted
    [HideInInspector] private static float OffsetXPos;
    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();
        
        Offset_Basic();
        Offset_RectPosData();
        Offset_Subscribe();
        Offset_Img();
        Offset_ColorComp();
        Offset_AfterColorSet();
        Set_LanguageTxt();
    }

    private void Offset_Basic()
    {
        for (int i = 0; i < AllAllyPresence.Count; i++)
            AllAllyPresence[i].Offset();
        
        AllAllyPresence = new List<AllyPresenceEUIController> { ST_AllyPresence, UT_AllyPresence, NT_AllyPresence };

        EP.Offset();
        CurrentEmptyBC.Offset();
        EmptyBC.Offset();
        FullEC.Offset();

        // EP Txt
        ECCostTxt.text = PlayerManager.Instance.PlayerController.NeedEP_ForMakeEC.ToString();

        // 스킬
        for (int i = 0; i < SkillList.Count; i++) SkillList[i].Offset();

        // 모듈 아이템
        ModuleSlots = DevTool.Get_ChildList<InventorySlotEUIController>(ModuleListParentRT);
        for (int i = 0; i < ModuleSlots.Count; i++)
        {
            ModuleSlots[i].Offset();
            ModuleSlots[i].ThisItem.Offset();
            ModuleSlots[i].Set_EquipedTxt(true, i);
        }

        // 버프
        PoolingManager.Instance.BuffIcons.ParentTF = BuffParentTF;

        ThisCG = DevTool.Get_ComponentTType(gameObject, out CanvasGroup cg) ? cg : null;

        OffsetXPos = HittedInfoRT.anchoredPosition.x;

    }

    private void Offset_RectPosData()
    {
        // 기본 위치
        // 모듈
        DefaultModuleRectX = DevTool.Get_ComponentTType(
            ModuleListParentRT.gameObject, out RectTransform module_Rt) ?
                module_Rt.anchoredPosition.x : 0f;

        // 플레이어 스탯
        DefaultPlayerStatesRectX = DevTool.Get_ComponentTType(
            PlayerStatesCostParentRT.gameObject, out RectTransform state_Rt) ?
                state_Rt.anchoredPosition.x : 0f;

        // 스킬
        DefaultSkillStatesRectY = DevTool.Get_ComponentTType(
            SkillStatesParentRT.gameObject, out RectTransform ss_Rt) ?
                ss_Rt.anchoredPosition.y : 0f;

        // CG 값
        for (int i = 0; i < ParentCGList.Count; i++) ParentCGList[i].alpha = 0f;

        // 부스트
        DefaultBoostRectY = BoostRT.anchoredPosition.y;
    }

    private void Offset_Subscribe()
    {
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

        PlayerManager.Instance.PlayerController.CurrentBetteryShard
            .Subscribe(_CurrentBS =>
            {
                if (PlayerManager.Instance.PlayerController.CurrentBetteryShard.Value < PlayerManager.Instance.PlayerController.NeedBS_ForMakeBC)
                {
                    CurrentEmptyBC.Change_Sprite(_CurrentBS);
                }
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBettery
            .Subscribe(_CurrentBC =>
            {
                EmptyBC.Set_Amount(_CurrentBC, 0.5f);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentChargedBettery
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


        PlayerManager.Instance.PlayerController.CurrentCredit
            .Subscribe(_CurrentCredit =>
            {
                Credit_EUI.Play_Amount(_CurrentCredit);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentOverrider
            .Subscribe(_CurrentOverrider =>
            {
                Overrider_EUI.Play_Amount(_CurrentOverrider);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentModuleShard
            .Subscribe(_CurrentMS =>
            {
                MS_EUI.Play_Amount(_CurrentMS);
            })
            .AddTo(gameObject);


        PlayerManager.Instance.PlayerController.CurrentBoostLv
            .Subscribe(_BoostLevel =>
            {
                Set_TextOfBoost(_BoostLevel);

                Play_ActiveBoost(BoostLightArr, _BoostLevel);
                Play_ActiveBoost(BoostLightWheelArr, _BoostLevel);

                Play_RollBoost(BoostLightWheelArr, _BoostLevel);
            })
            .AddTo(gameObject);


        PlayerManager.Instance.PlayerController.StrikeTeamPresence
            .Subscribe(_presence =>
            {
                ST_AllyPresence.Play_Amount(PlayerManager.Instance.PlayerController.StrikeTeamPresence.Value, PlayerManager.Instance.PlayerController.NeedStrikeTeamPresence.Value);
            });
        PlayerManager.Instance.PlayerController.UplinkTeamPresence
            .Subscribe(_presence =>
            {
                UT_AllyPresence.Play_Amount(PlayerManager.Instance.PlayerController.UplinkTeamPresence.Value, PlayerManager.Instance.PlayerController.NeedUplinkTeamPresence.Value);
            });
        PlayerManager.Instance.PlayerController.NeoTeamPresence
            .Subscribe(_presence =>
            {
                NT_AllyPresence.Play_Amount(PlayerManager.Instance.PlayerController.NeoTeamPresence.Value, PlayerManager.Instance.PlayerController.NeedNeoTeamPresence.Value);
            });

        PlayerManager.Instance.PlayerController.NeedStrikeTeamPresence
            .Subscribe(_needPresence =>
            {
                ST_AllyPresence.Play_Amount(PlayerManager.Instance.PlayerController.StrikeTeamPresence.Value, PlayerManager.Instance.PlayerController.NeedStrikeTeamPresence.Value);
            });
        PlayerManager.Instance.PlayerController.NeedUplinkTeamPresence
            .Subscribe(_needPresence =>
            {
                UT_AllyPresence.Play_Amount(PlayerManager.Instance.PlayerController.UplinkTeamPresence.Value, PlayerManager.Instance.PlayerController.NeedUplinkTeamPresence.Value);
            });
        PlayerManager.Instance.PlayerController.NeedNeoTeamPresence
            .Subscribe(_needPresence =>
            {
                NT_AllyPresence.Play_Amount(PlayerManager.Instance.PlayerController.NeoTeamPresence.Value, PlayerManager.Instance.PlayerController.NeedNeoTeamPresence.Value);
            });
    }

    private void Offset_Img()
    {
        // EP 지나가는 효과 이미지
        EP_FlowRT.DOAnchorPos(new Vector2(1920, 0), 2f, false)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        // 스킬 이미지
        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            SkillImgList.Add(DevTool.Get_ComponentTType<Image>(SkillList[i].gameObject));
            SkillImgList[i].sprite = PlayerManager.Instance.PlayerController.SkillWeapon.SkillList[i].ThisIcon;
        }

        Credit_EUI.Offset();
        Overrider_EUI.Offset();
        MS_EUI.Offset();

        PaneltyAnnoCG.alpha = 0f;
        DevTool.Set_Color(UninteractableColor, PaneltyAnnoNameTxt);
        PaneltyAnnoNameTxt.text = "";
        DevTool.Set_Color(UninteractableColor, PaneltyAnnoDescTxt);
        PaneltyAnnoDescTxt.text = "";
        PaneltyAnnoCG.gameObject.SetActive(false);
    }

    private void Offset_ColorComp()
    {
        DevTool.Set_AlphaColor(StageNameTxt, 1);
        DevTool.Set_AlphaColor(StageDescriptionTxt, 0);

        MainColorCompList.AddRange(Get_MainColorComp());
        SubColorCompList.AddRange(Get_SubColorTxt());

        // Color Set
        Color mainClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;

        InteractableColor = InteractOnOffTxt.color;
    }

    private void Offset_AfterColorSet()
    {
        ThisMinimap.Offset();
    }

    #endregion

    #region Reset

    private void Reset_Tab()
    {
        PlayerController player = PlayerManager.Instance.PlayerController;
        PlayerWeaponController weapon = player.BaseWeapon;
        SkillWeaponController skill = player.SkillWeapon;

        PlayerStatesTxt.text = Get_PlayerStateTxt(player, weapon);

        for (int i = 0; i < DevTool.SkillAmount; i++)
            SkillStatesTxtList[i].text = Get_SkillStateTxt(skill.SkillList[i]);
    }

    #endregion

    #region Framework

    private void LateUpdate()
    {
        Caculate_TabInput(Time.deltaTime);
    }

    #endregion

    #region Caculate

    private void Caculate_TabInput(float _DeltaTime)
    {
        if (IsTabInputed)  
            Caculate_WhenTabInputOn(_DeltaTime);
        else  
            Caculate_WhenTabInputOff(_DeltaTime);
    }

    private void Caculate_WhenTabInputOn(float _DeltaTime)
    {
        if (TabInputedMaxTime > TabInputedCurrentTime) // 인풋 시간 계산
        {
            TabInputedCurrentTime += _DeltaTime;
        }
        else // 인풋 시간 충분 상태
        {
            if (!IsTabInteracted)
            {
                SetOn_TabInteract();
            }
        }
    }

    private void Caculate_WhenTabInputOff(float _DeltaTime)
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

    #endregion

    #region Interact

    public void Set_InteractUI()
    {
        if (IsActingInteractUI) return; 

        IInteract ii = PlayerManager.Instance.PlayerController.CurrentInteractable.Value;
        string txt = DevTool.Get_InteractingAnnoTxt(ii, out bool canInteract);

        if (ii != null && txt != "")
        {
            if (canInteract)
            {
                Set_InteractTxt($"-{InteractEnableString}-", txt, InteractableColor);
                Set_InteractFade(1f, 0.5f);
            }
            else
            {
                Set_InteractTxt($"-{InteracInoperableString}-", txt, UninteractableColor);
                Set_InteractFade(1f, 0.5f);
            }
        }
        else
        {
            Set_InteractTxt($"-{InteractDisableString}-", $"< {InteractNoneString} >", new Color(1, 1, 1, InteractOnOffTxt.color.a));
            Set_InteractFade(0.25f, 0.5f);
        }
    }

    private void Set_InteractTxt(string _OnOffTxt, string _InteractableTxt, Color _OnOffTxtColor)
    {
        InteractOnOffTxt.text = _OnOffTxt;
        InteractOnOffTxt.color = _OnOffTxtColor;
        InteractDesctiptionTxt.text = _InteractableTxt;
    }

    private void Set_InteractFade(float _Alpha, float _DurTime)
    {
        DevTool.Set_KillTween(InteractOnOffTxt);
        DevTool.Set_KillTween(InteractDesctiptionTxt);

        InteractOnOffTxt.DOFade(_Alpha, _DurTime);
        InteractDesctiptionTxt.DOFade(_Alpha, _DurTime);
    }

    #endregion

    #region Stage

    public void Set_StageDescription()
    {
        StageNameTxt.DOText(
            CSVManager.Instance.Get_MapName(
                StageManager.Instance.Get_CollectStageData(
                    StageManager.Instance.TargetStageID).InfoData.StageID), 0.5f)
            .OnPlay(() => { StageNameTxt.text = ""; });
        StageDescriptionTxt.DOText(
            CSVManager.Instance.Get_MapDesc(
                StageManager.Instance.Get_CollectStageData(
                    StageManager.Instance.TargetStageID).InfoData.StageID), 0.5f)
            .OnPlay(() => { StageDescriptionTxt.text = ""; });
    }

    #endregion

    #region Tab

    public void SetOn_TabInteract()
    {
        if (IsTabInteracted) return;
        IsTabInteracted = true;

        Reset_Tab();

        DevTool.Set_KillTween(TabSeq);

        TabSeq = Play_SeqInteract(
            0f, 0f, 0f, 0f,
            0f, 1f, TabInteractDurTime, Ease.OutCubic);

        TabSeq.Join(Play_FadeCGs(1, TabInteractDurTime));

        ThisMinimap.SetOn_TabInteract(TabInteractDurTime);
    }

    public void SetOff_TabInteract()
    {
        if (!IsTabInteracted) return; 
        IsTabInteracted = false;

        DevTool.Set_KillTween(TabSeq);

        TabSeq = Play_SeqInteract(
            DefaultModuleRectX, DefaultPlayerStatesRectX, DefaultSkillStatesRectY, DefaultBoostRectY,
            1f, 0f, TabInteractDurTime, Ease.InCubic);

        TabSeq.Join(Play_FadeCGs(0, TabInteractDurTime));

        ThisMinimap.SetOff_TabInteract(TabInteractDurTime);
    }

    #endregion

    #region Shield

    public void Set_ShieldGage(float _TotalShield)
    {
        DevTool.Set_KillTween(ShieldRT);

        ShieldRT.DOSizeDelta(new Vector2(Get_ShieldGageX(_TotalShield), ShieldRT.sizeDelta.y), 1f);
        ShieldTxt.text = "<size=75%>( </size>" + Mathf.Round(_TotalShield).ToString() + "<size=75%> )</size>";
    }

    #endregion

    #region Boost

    private void Set_TextOfBoost(int _CurrentLv)
    {
        BoostLv.text = _CurrentLv.ToString();
        DevTool.Set_AlphaColor(BoostLv, _CurrentLv == 0 ? 0.1f : 0.25f * (_CurrentLv));
    }

    #endregion

    #region Buff

    public void Set_BuffPosUI()
    {
        for (int i = 0; i < AllBuffIconUI.Count; i++)
            AllBuffIconUI[i].ThisRT.anchoredPosition = new Vector2(i * (AllBuffIconUI[i].ThisRT.rect.width + BuffUI_XInterval), 0);
    }

    #endregion

    #region Tween

    #region Panelty

    public void Play_PrisonPanelty()
    {
        Play_HittedPlayScreen(20, 1f);

        string title = $"< {CSVManager.Instance.Get_StaticDesc(36).Replace("\\n", "\n")} >";
        string desc = CSVManager.Instance.Get_StaticDesc(37).Replace("\\n", "\n");
        PaneltyAnnoNameTxt.text = "";
        PaneltyAnnoDescTxt.text = "";

        Sequence seq = DOTween.Sequence();
        PaneltyAnnoCG.gameObject.SetActive(true);

        seq.Append(PaneltyAnnoCG.DOFade(1f, 0.5f));
        seq.Join(PaneltyAnnoNameTxt.DOText(title, 0.5f));
        seq.Join(PaneltyAnnoDescTxt.DOText(desc, 0.5f));
        seq.AppendInterval(1f);
        seq.Append(PaneltyAnnoCG.DOFade(0f, 2f));

        seq.OnComplete(() => { PaneltyAnnoCG.gameObject.SetActive(false); });
    }

    #endregion

    #region Hitted

    // 피격 시 효과
    public void Play_HittedPlayScreen(float _Dmg, float _DurTime)
    {
        DevTool.Set_KillTween(HittedScreen);

        Sequence seq = DOTween.Sequence();
        seq.Append(HittedScreen.DOFade((Math.Min(100, _Dmg) * 0.01f), _DurTime));
        seq.Append(HittedScreen.DOFade(0, _DurTime));
    }


    // 피격 정보
    public void Play_HittedPlayInfo(float _Dmg, float _DurTime)
    {
        HittedDmgTxt.text = $"<size=75%>{CSVManager.Instance.Get_StaticWord(74)}:</size> {_Dmg.ToString("0.0")}";
        HittedDmgTxt.color = UninteractableColor;

        Play_Info(_DurTime);
    }

    // 회피 정보
    public void Play_AvoidPlayInfo(float _DurTime)
    {
        HittedDmgTxt.text = $"{CSVManager.Instance.Get_StaticWord(75)}";
        HittedDmgTxt.color = Color.white;

        Play_Info(_DurTime);
    }

    // 정보
    private void Play_Info(float _DurTime)
    {
        HittedInfoPivotTF.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-5f, 5f));

        DevTool.Set_KillTween(HittedInfoRT);

        Sequence seq = DOTween.Sequence();

        HittedInfoRT.anchoredPosition = new Vector2(OffsetXPos, 0);
        seq.Append(HittedInfoRT.DOAnchorPosX(-50f, _DurTime * 0.2f).SetEase(Ease.Linear));
        seq.Append(HittedInfoRT.DOAnchorPosX(50f, _DurTime * 0.6f).SetEase(Ease.Linear));
        seq.Append(HittedInfoRT.DOAnchorPosX(-OffsetXPos, _DurTime * 0.2f).SetEase(Ease.Linear));
    }

    #endregion

    #region Boost

    // 부스트 키기
    private void Play_ActiveBoost(GameObject[] _Arr, int _CurrentLv)
    {
        for (int i = 0; i < _Arr.Length; i++)
        {
            if (DevTool.Get_ComponentTType(_Arr[i].gameObject, out Image img))
            {
                DevTool.Set_KillTween(img);

                if (i < _CurrentLv) img.DOFade(1f, 0.2f);
                else img.DOFade(0f, 0.2f);
            }
        }
    }

    // 부스트 돌리기
    private void Play_RollBoost(GameObject[] _Arr, int _CurrentLv)
    {
        for (int i = 0; i < _Arr.Length; i++)
        {
            if (DevTool.Get_ComponentTType(_Arr[i].gameObject, out RectTransform rt))
            {
                if (i < _CurrentLv) Play_EachRollBoost(rt, i);
                else DevTool.Set_KillTween(rt);
            }
        }
    }

    // 부스트 하나씩 돌리기
    private void Play_EachRollBoost(RectTransform _RT, int _Index)
    {
        _RT.DOLocalRotate(new Vector3(0, 0, 360), 0.2f, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        _RT.DOLocalRotate(new Vector3(0, 0, 360), 3f / (_Index + 1f), RotateMode.LocalAxisAdd)
                            .SetEase(Ease.Linear)
                            .SetLoops(-1, LoopType.Restart);
                    });
    }

    #endregion

    #region Tab Interact

    // Tab 이동
    private Sequence Play_SeqInteract(
        float _ModuleRtX, float _CostRtX, float _SkillRtY, float _BoostRtY,
        float _StageNameAlpha, float _StageDescAlpha,
        float _DurTime, Ease _Ease)
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(ModuleListParentRT.DOAnchorPosX(_ModuleRtX, _DurTime));
        seq.Join(PlayerStatesCostParentRT.DOAnchorPosX(_CostRtX, _DurTime));
        seq.Join(SkillStatesParentRT.DOAnchorPosY(_SkillRtY, _DurTime));
        seq.Join(BoostRT.DOAnchorPosY(_BoostRtY, _DurTime));
        seq.Join(StageNameTxt.DOFade(_StageNameAlpha, _DurTime));
        seq.Join(StageDescriptionTxt.DOFade(_StageDescAlpha, _DurTime));
        seq.SetEase(_Ease);
        return seq;
    }

    // Tab 투명도
    private Sequence Play_FadeCGs(float _Alpha, float _DurTime)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < ParentCGList.Count; i++)
            seq.Join(ParentCGList[i].DOFade(_Alpha, _DurTime));
        return seq;
    }



    // 상호작용 시 발생
    public void Play_UseInteractUI()
    {
        IsActingInteractUI = true;

        DevTool.Set_KillTween(UsingInnerImg);

        UsingInnerImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                UsingInnerImg.DOFade(0.25f, 0.2f)
                .OnComplete(() =>
                {
                    IsActingInteractUI = false;
                    Set_InteractUI();
                });
            });
    }
    #endregion

    #endregion

    #region Get

    private List<Component> Get_MainColorComp()
    {
        List<Component> result = new List<Component>
        {
            // EP 게이지
            DevTool.Get_ComponentTType<Image>(EP.AfterImg.gameObject.transform.GetChild(0).gameObject),
            DevTool.Get_ComponentTType<Image>(EP.ActualImg.gameObject.transform.GetChild(0).gameObject),
            DevTool.Get_ComponentTType<Image>(EP.ActualImgLiner.gameObject),

            // 부스트
            BoostLv,

            // 플레이어 스탯
            PlayerStatesTxt,

            // 스테이지
            StageNameTxt, StageDescriptionTxt,

            // 아이템
            ECCostTxt, EmptyBC.AmountTxt, FullEC.AmountTxt,

            // 상호작용
            InteractOnOffTxt

        };

        // 부스트
        result.AddRange(DevTool.Get_ComponentTTypeList<Image>(BoostLightArr.ToList()));
        result.AddRange(DevTool.Get_ComponentTTypeList<Image>(BoostLightWheelArr.ToList()));

        // 스킬
        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            result.Add(SkillList[i].SkillCostTxt);
            result.Add(SkillList[i].SkillErrorTxt);
            result.Add(SkillStatesTxtList[i]);
        }

        // Ally
        for (int i = 0; i < AllAllyPresence.Count; i++)
            MainColorCompList.AddRange(DevTool.Get_ChildList<Image>(AllAllyPresence[i].CapMiddleRT.transform));
        
        return result;
    }

    private List<Component> Get_SubColorTxt()
    {
        List<Component> result = new List<Component>
        {
            // 미니맵
            ThisMinimap.InnerImg, 

            // 아이템
            CurrentEmptyBC.LightInner, ECCostArrowImg, EmptyBC.InnerImg, FullEC.InnerImg,
            
            // 상호작용
            InnerImg, UsingInnerImg
        };

        // EP 게이지 Inner
        SubColorCompList.AddRange(EPInnerImgList);

        // 부스트 Inner
        SubColorCompList.AddRange(BoostInnerList);

        // 스킬
        for (int i = 0; i < DevTool.SkillAmount; i++)
            SubColorCompList.Add(SkillList[i].SkillInnerImg);

        // Ally
        for (int i = 0; i < AllAllyPresence.Count; i++)
            SubColorCompList.Add(AllAllyPresence[i].InnerImg);
        
        return result;
    }

    private float Get_ShieldGageX(float _ShieldValue)
    {
        return 8 + (_ShieldValue * 3);
    }

    // Tab 플레이어 스탯의 엘레먼트
    private List<string> Get_PlayerStateStrings(PlayerController _Player, PlayerWeaponController _Weapon)
    {
        return new List<string>()
        {
            _Player.MaxEP.ActualState.Value.ToString(),
            _Player.WalkSpeed.ActualState.Value.ToString(),
            _Player.DashController.DashSpeed.ActualState.Value.ToString(),
            _Player.DashController.Get_ActualNeedEP().ToString(),
            _Weapon.BaseDamage.ActualState.Value.ToString(),
            _Weapon.ROF.ActualState.Value.ToString(),
            _Weapon.AccuracyRate.ActualState.Value.ToString(),
            _Weapon.CC.ActualState.Value.ToString(),
            _Weapon.CD.ActualState.Value.ToString()
        };
    }

    // Tab 플레이어 스탯 Txt 
    private string Get_PlayerStateTxt(PlayerController _Player, PlayerWeaponController _Weapon)
    {
        string result = "";
        List<string> strings = Get_PlayerStateStrings(_Player, _Weapon);

        for (int i = 0; i < PlayerStatesStringList.Count; i++)
        {
            result += "<size=70%>" + PlayerStatesStringList[i] + ": </size>";
            result += "<b>" + strings[i] + "</b>\n";
        }
        return result;
    }

    // Tab 스킬 스탯의 엘레먼트
    private List<string> Get_SkillStateStrings(ActiveSkillController _Skill)
    {
        return new List<string>()
        {
            _Skill.Tier.ActualState.Value.ToString(),
            _Skill.Power.ActualState.Value.ToString()
        };
    }

    // Tab 스킬 스탯 Txt
    private string Get_SkillStateTxt(ActiveSkillController _Skill)
    {
        string result = "";
        List<string> strings = Get_SkillStateStrings(_Skill);

        for (int i = 0; i < SkillStatesStringList.Count; i++)
        {
            result += "<size=70%>" + SkillStatesStringList[i] + ": </size>\n";
            result += "<b>" + strings[i] + "</b>\n";
        }
        return result;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        InteractEnableString = CSVManager.Instance.Get_StaticWord(4);
        InteractDisableString = CSVManager.Instance.Get_StaticWord(5);
        InteracInoperableString = CSVManager.Instance.Get_StaticWord(6);
        InteractNoneString = CSVManager.Instance.Get_StaticWord(7);

        PlayerStatesStringList.Clear();
        for (int i = 8; i <= 16; i++)
            PlayerStatesStringList.Add(CSVManager.Instance.Get_StaticWord(i));

        SkillStatesStringList.Clear();
        for (int i = 17; i <= 18; i++)
            SkillStatesStringList.Add(CSVManager.Instance.Get_StaticWord(i));

        for (int i = 0; i < AllAllyPresence.Count; i++)
            AllAllyPresence[i].PresenceLangTxt.text = $"{CSVManager.Instance.Get_StaticWord(i + 61)}<size=85%> {CSVManager.Instance.Get_StaticWord(70)}</size>";

        Set_InteractUI();
        Set_StageDescription();
    }

    #endregion
}

