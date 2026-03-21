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
    [SerializeField] public ReactiveProperty<bool> IsTabInteracted = new ReactiveProperty<bool>();
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

    [Header("-- Ally States")]
    [SerializeField] private RectTransform AllyStateParentRT;
    [SerializeField] private List<RectTransform> AllyHUDList = new List<RectTransform>();
    [SerializeField] private readonly static int MaxCol = 6;
    [SerializeField] private readonly static float ColInterval = -120;
    [SerializeField] private readonly static float RowInterval = 240;

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
    [SerializeField] public Image StageIcon;

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
    [SerializeField] private Image AllyReputationImg;
    [SerializeField] private TMP_Text AllyReputationTxt;

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

    [Space(10)]
    [Header("=== Key Item")]
    [SerializeField] private Image KeyItemVFXImg;
    [SerializeField] private List<Image> KeyItemImgList;

    [Space(10)]
    [Header("=== High Lv Item")]
    [SerializeField] private RectTransform HighLvItemRT;
    [SerializeField] private List<TMP_Text> HighLvItemAmountTxtList;

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
    [HideInInspector] private float DefaultAllyStateRectX;

    // Hitted
    [HideInInspector] private static float OffsetXPos;

    // KeyItem
    [HideInInspector] private List<TMP_Text> KeyItemAmountTxtList;

    // High Lv Item
    [HideInInspector] private float DefaultHighLvItemRectX;

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
        Offset_HighLvItem();
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
        ECCostTxt.text = PlayerManager.instance.playerController.needEP_ForMakeEC.ToString();

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
        //PoolingManager.Instance.BuffIcons.ParentTF = BuffParentTF;

        ThisCG = DevTool.Get_ComponentTType(gameObject, out CanvasGroup cg) ? cg : null;

        OffsetXPos = HittedInfoRT.anchoredPosition.x;

        IsTabInteracted.Value = false;

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

        // Ally
        DefaultAllyStateRectX = AllyStateParentRT.anchoredPosition.x;

        // High Lv Item
        DefaultHighLvItemRectX = HighLvItemRT.anchoredPosition.x;
    }

    private void Offset_Subscribe()
    {
        PlayerManager.instance.playerController.maxEP.actualState
            .Subscribe(_MaxEP =>
            {
                EP.Set_MaxFillRT(_MaxEP * 3);

                EP.Set_FillImgSmooth(
                    PlayerManager.instance.playerController.Get_CurrentEP().Value,
                    PlayerManager.instance.playerController.maxEP.actualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.Get_CurrentEP()
            .Subscribe(_CurrentEP =>
            {
                EP.Set_FillImgSmooth(
                    PlayerManager.instance.playerController.Get_CurrentEP().Value,
                    PlayerManager.instance.playerController.maxEP.actualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.currentBetteryShard
            .Subscribe(_CurrentBS =>
            {
                if (PlayerManager.instance.playerController.currentBetteryShard.Value < PlayerManager.instance.playerController.needBS_ForMakeBC)
                {
                    CurrentEmptyBC.Change_Sprite(_CurrentBS);
                }
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.currentBettery
            .Subscribe(_CurrentBC =>
            {
                EmptyBC.Set_Amount(_CurrentBC, 0.5f);
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.currentChargedBettery
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


        PlayerManager.instance.playerController.currentCredit
            .Subscribe(_CurrentCredit =>
            {
                Credit_EUI.Play_Amount(_CurrentCredit);
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.currentOverrider
            .Subscribe(_CurrentOverrider =>
            {
                Overrider_EUI.Play_Amount(_CurrentOverrider);
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.currentModuleShard
            .Subscribe(_CurrentMS =>
            {
                MS_EUI.Play_Amount(_CurrentMS);
            })
            .AddTo(gameObject);


        PlayerManager.instance.playerController.currentBoostLv
            .Subscribe(_BoostLevel =>
            {
                Set_TextOfBoost(_BoostLevel);

                Play_ActiveBoost(BoostLightArr, _BoostLevel);
                Play_ActiveBoost(BoostLightWheelArr, _BoostLevel);

                Play_RollBoost(BoostLightWheelArr, _BoostLevel);
            })
            .AddTo(gameObject);


        PlayerManager.instance.playerController.strikeTeamPresence
            .Subscribe(_presence =>
            {
                ST_AllyPresence.Play_Amount(PlayerManager.instance.playerController.strikeTeamPresence.Value, PlayerManager.instance.playerController.needStrikeTeamPresence.Value);
            });
        PlayerManager.instance.playerController.uplinkTeamPresence
            .Subscribe(_presence =>
            {
                UT_AllyPresence.Play_Amount(PlayerManager.instance.playerController.uplinkTeamPresence.Value, PlayerManager.instance.playerController.needUplinkTeamPresence.Value);
            });
        PlayerManager.instance.playerController.neoTeamPresence
            .Subscribe(_presence =>
            {
                NT_AllyPresence.Play_Amount(PlayerManager.instance.playerController.neoTeamPresence.Value, PlayerManager.instance.playerController.needNeoTeamPresence.Value);
            });

        PlayerManager.instance.playerController.needStrikeTeamPresence
            .Subscribe(_needPresence =>
            {
                ST_AllyPresence.Play_Amount(PlayerManager.instance.playerController.strikeTeamPresence.Value, PlayerManager.instance.playerController.needStrikeTeamPresence.Value);
            });
        PlayerManager.instance.playerController.needUplinkTeamPresence
            .Subscribe(_needPresence =>
            {
                UT_AllyPresence.Play_Amount(PlayerManager.instance.playerController.uplinkTeamPresence.Value, PlayerManager.instance.playerController.needUplinkTeamPresence.Value);
            });
        PlayerManager.instance.playerController.needNeoTeamPresence
            .Subscribe(_needPresence =>
            {
                NT_AllyPresence.Play_Amount(PlayerManager.instance.playerController.neoTeamPresence.Value, PlayerManager.instance.playerController.needNeoTeamPresence.Value);
            });
    }

    private void Offset_Img()
    {
        // EP 지나가는 효과 이미지
        EP_FlowRT.DOAnchorPos(new Vector2(1920, 0), 2f, false)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        // 스킬 이미지
        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            SkillImgList.Add(DevTool.Get_ComponentTType<Image>(SkillList[i].gameObject));
            SkillImgList[i].sprite = PlayerManager.instance.playerController.skillWeapon.skillList[i].icon;
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

        // Key
        KeyItemAmountTxtList = new List<TMP_Text>();
        for (int i = 0; i < KeyItemImgList.Count; i++)
        {
            KeyItemAmountTxtList.Add(DevTool.Get_ComponentTType<TMP_Text>(KeyItemImgList[i].gameObject.transform.GetChild(0).gameObject));
            KeyItemImgList[i].gameObject.SetActive(false);
        }

    }

    public void Offset_ColorComp()
    {
        DevTool.Set_AlphaColor(StageNameTxt, 1);
        DevTool.Set_AlphaColor(StageDescriptionTxt, 0);

        MainColorCompList.AddRange(Get_MainColorComp());
        SubColorCompList.AddRange(Get_SubColorTxt());

        // Color Set
        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;

        InteractableColor = InteractOnOffTxt.color;
    }

    private void Offset_AfterColorSet()
    {
        ThisMinimap.Offset();
    }

    private void Offset_HighLvItem()
    {
        Init_HighLvItemUI();
    }

    #endregion

    #region Reset

    private void Reset_Tab()
    {
        PlayerController player = PlayerManager.instance.playerController;
        PlayerWeaponController weapon = player.baseWeapon;
        SkillWeaponController skill = player.skillWeapon;

        PlayerStatesTxt.text = Get_PlayerStateTxt(player, weapon);

        for (int i = 0; i < DevTool.skillAmount; i++)
            SkillStatesTxtList[i].text = Get_SkillStateTxt(skill.skillList[i]);
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
            if (!IsTabInteracted.Value)
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
        if (IsTabInteracted.Value)
        {
            SetOff_TabInteract();
        }
    }

    #endregion

    #region Interact

    public void Set_InteractUI()
    {
        if (IsActingInteractUI) return; 

        IInteract ii = PlayerManager.instance.playerController.currentInteractable.Value;
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
            ResourceManager.instance.Get_MapName(
                StageManager.instance.Get_CurrentStageData().infoData.stageId), 0.5f)
            .OnPlay(() => { StageNameTxt.text = ""; });

        StageDescriptionTxt.DOText(
            ResourceManager.instance.Get_MapDesc(
                StageManager.instance.Get_CurrentStageData().infoData.stageId), 0.5f)
            .OnPlay(() => { StageDescriptionTxt.text = ""; });
    }

    #endregion

    #region Tab

    public void SetOn_TabInteract()
    {
        if (IsTabInteracted.Value) return;
        IsTabInteracted.Value = true;

        Reset_Tab();

        DevTool.Set_KillTween(TabSeq);

        TabSeq = Play_SeqInteract(
            _ModuleRtX: 0f,
            _AllyStateRtX: 700f,
            _CostRtX: 0f,
            _SkillRtY: 0f,
            _BoostRtY: 0f,
            _HighLvItemRtX: 0f,
            _StageNameAlpha: 0f,
            _StageDescAlpha: 1f,
            TabInteractDurTime, Ease.OutCubic);

        TabSeq.Join(Play_FadeCGs(1, TabInteractDurTime));

        ThisMinimap.SetOn_TabInteract(TabInteractDurTime);
    }

    public void SetOff_TabInteract()
    {
        if (!IsTabInteracted.Value) return; 
        IsTabInteracted.Value = false;

        DevTool.Set_KillTween(TabSeq);

        TabSeq = Play_SeqInteract(
            DefaultModuleRectX,
            DefaultAllyStateRectX, 
            DefaultPlayerStatesRectX,
            DefaultSkillStatesRectY, 
            DefaultBoostRectY,
            DefaultHighLvItemRectX,
            _StageNameAlpha: 1f,
            _StageDescAlpha: 0f, 
            TabInteractDurTime, Ease.InCubic);

        TabSeq.Join(Play_FadeCGs(0, TabInteractDurTime));

        ThisMinimap.SetOff_TabInteract(TabInteractDurTime);
    }

    #endregion

    #region Ally State

    public void Add_AllyState(AllyHUDController _HUD)
    {
        if (_HUD.gameObject.TryGetComponent(out RectTransform rt))
        {
            rt.gameObject.transform.SetParent(AllyStateParentRT);
            rt.gameObject.layer = LayerMask.NameToLayer("UI");

            rt.pivot = new Vector2(0, 1);
            rt.localScale = Vector3.one;

            int currentAmount = AllyHUDList.Count;
            float y = currentAmount == 0 ? 0 : (currentAmount % MaxCol) * ColInterval;
            float x = currentAmount == 0 ? 0 : (currentAmount / MaxCol) * RowInterval;
            rt.anchoredPosition3D = new Vector3(x, y, 0);

            AllyHUDList.Add(rt);
        }
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

    #region High Lv Item

    public void Init_HighLvItemUI()
    {
        List<EachItemJsonData> itemData = SaveDataManager.instance.jsonData.itemData;
        for (int i = 0; i < itemData.Count; i++)
        {
            HighLvItemAmountTxtList[i].text = itemData[i].amount.ToString();
        }
    }

    #endregion

    #region Tween

    #region Panelty

    public void Play_PrisonPanelty()
    {
        Play_HittedPlayScreen(20, 1f);

        string title = $"< {ResourceManager.instance.Get_StaticDesc(36).Replace("\\n", "\n")} >";
        string desc = ResourceManager.instance.Get_StaticDesc(37).Replace("\\n", "\n");
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
        HittedDmgTxt.text = $"<size=75%>{ResourceManager.instance.Get_StaticWord(74)}:</size> {_Dmg.ToString("0.0")}";
        HittedDmgTxt.color = UninteractableColor;

        Play_Info(_DurTime);
    }

    // 회피 정보
    public void Play_AvoidPlayInfo(float _DurTime)
    {
        HittedDmgTxt.text = $"{ResourceManager.instance.Get_StaticWord(75)}";
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
        float _ModuleRtX, float _AllyStateRtX, float _CostRtX, float _SkillRtY, float _BoostRtY, float _HighLvItemRtX,
        float _StageNameAlpha, float _StageDescAlpha,
        float _DurTime, Ease _Ease)
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(ModuleListParentRT.DOAnchorPosX(_ModuleRtX, _DurTime));
        seq.Join(AllyStateParentRT.DOAnchorPosX(_AllyStateRtX, _DurTime));
        seq.Join(PlayerStatesCostParentRT.DOAnchorPosX(_CostRtX, _DurTime));
        seq.Join(SkillStatesParentRT.DOAnchorPosY(_SkillRtY, _DurTime));
        seq.Join(BoostRT.DOAnchorPosY(_BoostRtY, _DurTime));
        seq.Join(HighLvItemRT.DOAnchorPosX(_HighLvItemRtX, _DurTime));

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
        for (int i = 0; i < DevTool.skillAmount; i++)
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
        for (int i = 0; i < DevTool.skillAmount; i++)
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
            _Player.maxEP.actualState.Value.ToString(),
            _Player.walkSpeed.actualState.Value.ToString(),
            _Player.dash.dashSpeed.actualState.Value.ToString(),
            _Player.dash.Get_ActualNeedEP().ToString(),
            _Weapon.baseDamage.actualState.Value.ToString(),
            _Weapon.rof.actualState.Value.ToString(),
            _Weapon.accRate.actualState.Value.ToString(),
            _Weapon.cc.actualState.Value.ToString(),
            _Weapon.cd.actualState.Value.ToString()
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
            _Skill.tier.actualState.Value.ToString(),
            _Skill.power.actualState.Value.ToString()
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

    #region Key Item

    public void Set_KeyItem(Dictionary<int, int> _KeyItemDict)
    {
        for (int i = 0; i < KeyItemImgList.Count; i++)
        {
            KeyItemImgList[i].gameObject.SetActive(false);
        }

        int index = 0;

        foreach(var keyItem in _KeyItemDict)
        {
            if (keyItem.Value == 0) continue;

            KeyItemImgList[index].sprite = ResourceManager.instance.Get_KeyCardSprite(keyItem.Key);
            KeyItemAmountTxtList[index].text = keyItem.Value.ToString();
            KeyItemImgList[index].gameObject.SetActive(true);

            index++;
        }
    }

    public void Effect_KeyIcon(int _ID)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < KeyItemImgList.Count; i++)
        {
            if (KeyItemImgList[i].gameObject.activeSelf && KeyItemImgList[i].sprite == ResourceManager.instance.Get_KeyCardSprite(_ID))
            {
                seq.Append(KeyItemImgList[i].transform.DOScale(1.3f, 0.1f));
                seq.Append(KeyItemImgList[i].transform.DOScale(1f, 0.3f));

                if (KeyItemVFXImg.TryGetComponent(out RectTransform vfxRt) &&
                    KeyItemImgList[i].TryGetComponent(out RectTransform imgRt))
                {
                    DevTool.Set_KillTween(vfxRt);
                    vfxRt.anchoredPosition = imgRt.anchoredPosition;
                    vfxRt.rotation = Quaternion.identity;
                    vfxRt.DORotate(Vector3.forward * 360, 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                    vfxRt.transform.localScale = Vector3.one;
                    vfxRt.DOScale(0.5f, 1f);
                }
                DevTool.Set_KillTween(KeyItemVFXImg);
                KeyItemVFXImg.color = new Color(1, 1, 1, 0.7f);
                KeyItemVFXImg.DOFade(0f, 1f);
            }
        }
    }

    #endregion

    #region Ally Reputation

    public void Set_AllyReputation(float _Value)
    {
        AllyReputationTxt.text = $"{_Value} %"; 
        AllyReputationImg.fillAmount = _Value * 0.01f;
        AllyReputationImg.color = Color.Lerp(new Color(0.8f, 1f, 0.8f, 1f), new Color(0.35f, 1f, 0.35f, 1f), AllyReputationImg.fillAmount);
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        InteractEnableString = ResourceManager.instance.Get_StaticWord(4);
        InteractDisableString = ResourceManager.instance.Get_StaticWord(5);
        InteracInoperableString = ResourceManager.instance.Get_StaticWord(6);
        InteractNoneString = ResourceManager.instance.Get_StaticWord(7);

        PlayerStatesStringList.Clear();
        for (int i = 8; i <= 16; i++)
            PlayerStatesStringList.Add(ResourceManager.instance.Get_StaticWord(i));

        SkillStatesStringList.Clear();
        for (int i = 17; i <= 18; i++)
            SkillStatesStringList.Add(ResourceManager.instance.Get_StaticWord(i));

        for (int i = 0; i < AllAllyPresence.Count; i++)
            AllAllyPresence[i].PresenceLangTxt.text = $"{ResourceManager.instance.Get_StaticWord(i + 61)}<size=85%> {ResourceManager.instance.Get_StaticWord(70)}</size>";

        Set_InteractUI();
        Set_StageDescription();
    }

    #endregion
}

