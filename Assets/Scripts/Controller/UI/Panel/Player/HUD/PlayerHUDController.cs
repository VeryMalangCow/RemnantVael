using UnityEngine;
using UniRx;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class PlayerHUDController : UIController
{
    // Value
    [Space(20)]
    [Header("<><><><><> Player HUD")]

    [SerializeField] private PlayerEpView epView;
    public PlayerEpView EpView { get { return epView; } }

    public IEnumerator Init(Color mainClr, Color subClr)
    {
        yield return EpView.Init(mainClr, subClr);
    }


    #region Value

    #region - Inspector


    [Space(10)]
    [Header("=== Energy")]


    [Space(10)]
    [Header("=== Tab")]
    [SerializeField] public ReactiveProperty<bool> isTabInteracted = new ReactiveProperty<bool>();
    [SerializeField] public bool isTabInputed = false;
    [SerializeField] private static float tabInputedMaxTime = 0.25f;
    [SerializeField] private float tabInputedCurrentTime = 0f;

    [Header("-- All")]
    [SerializeField] private List<CanvasGroup> parentCgList;

    [Header("-- Modules")]
    [SerializeField] private RectTransform moduleListParentRt;

    [Header("-- Player States")]
    [SerializeField] private RectTransform playerStatesCostParentRt;
    [SerializeField] private TMP_Text playerStatesTxt;

    [Header("-- Ally States")]
    [SerializeField] private RectTransform allyStateParentRt;
    [SerializeField] private List<RectTransform> allyHudList = new List<RectTransform>();
    [SerializeField] private readonly static int maxCol = 6;
    [SerializeField] private readonly static float colInterval = -120;
    [SerializeField] private readonly static float rowInterval = 240;

    [Header("-- Skill State")]
    [SerializeField] private RectTransform skillStatesParentRt;
    [SerializeField] private List<TMP_Text> skillStatesTxtList;



    [Space(10)]
    [Header("=== Bettery")]
    [Header("-- Current")]
    [SerializeField] public ChargeSpriteEUIController currentEmptyBc;

    [Header("-- Bettery (Image Amount Class)")]
    [SerializeField] private ImgTxtAmountEUIController emptyBc;
    [SerializeField] private ImgTxtAmountEUIController fullEc;
    [SerializeField] private Image ecCostArrowImg;
    [SerializeField] private TMP_Text ecCostTxt;

    [Space(10)]
    [Header("=== Other Item")]
    [SerializeField] private LootableItemEUIController creditEui;
    [SerializeField] private LootableItemEUIController overriderEui;
    [SerializeField] private LootableItemEUIController msEui;

    [Space(10)]
    [Header("=== Boost")]
    [SerializeField] private RectTransform boostRt;
    [HideInInspector] private float defaultBoostRectY;
    [SerializeField] private TMP_Text boostLv;
    [SerializeField] private GameObject[] boostLightArr;
    [SerializeField] private GameObject[] boostLightWheelArr;
    [SerializeField] List<Image> boostInnerList;

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] public List<SkillEUIController> skillList;

    [Space(10)]
    [Header("=== Minimap")]
    [SerializeField] public MinimapEUIController minimapEui;
    [SerializeField] public Image stageIcon;

    [Space(10)]
    [Header("=== Map Anno")]
    [SerializeField] private TMP_Text stageNameTxt;
    [SerializeField] private TMP_Text stageDescTxt;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private TMP_Text interactOnOffTxt;
    [SerializeField] private TMP_Text interactDescTxt;

    [SerializeField] private Image innerImg;
    [SerializeField] private Image usingInnerImg;
    [SerializeField] private Color uninteractableClr;

    [Space(10)]
    [Header("=== Ally")]
    [SerializeField] private AllyPresenceEUIController stAllyPresence;
    [SerializeField] private AllyPresenceEUIController utAllyPresence;
    [SerializeField] private AllyPresenceEUIController ntAllyPresence;
    [SerializeField] private Image allyReputationImg;
    [SerializeField] private TMP_Text allyReputationTxt;

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] private Transform buffParentTf;
    [SerializeField] public List<BuffIconEUIController> allBuffIconUi;
    [SerializeField] private float buffUiXInterval = 12;

    [Space(10)]
    [Header("=== Screen")]
    [SerializeField] private Image hittedScreen;
    [SerializeField] private Transform hittedInfoPivotTf;
    [SerializeField] private RectTransform hittedInfoRt;
    [SerializeField] private TMP_Text hittedDmgTxt;
    [SerializeField] private CanvasGroup paneltyAnnoCg;
    [SerializeField] private TMP_Text paneltyAnnoNameTxt;
    [SerializeField] private TMP_Text paneltyAnnoDescTxt;

    [Space(10)]
    [Header("=== Key Item")]
    [SerializeField] private Image keyItemVfxImg;
    [SerializeField] private List<Image> keyItemImgList;
    [SerializeField] private List<TMP_Text> keyItemAmountTxtList;

    [Space(10)]
    [Header("=== High Lv Item")]
    [SerializeField] private RectTransform highLvItemRt;
    [SerializeField] private List<TMP_Text> highLvItemAmountTxtList;

    #endregion

    #region - Hide

    // String
    [HideInInspector] private static string interactEnableString;
    [HideInInspector] private static string interactDisableString;
    [HideInInspector] private static string interacInoperableString;
    [HideInInspector] private static string interactNoneString;
    [HideInInspector] private List<string> playerStatesStringList = new List<string>();
    [HideInInspector] private List<string> skillStatesStringList = new List<string>();

    // Comp
    [HideInInspector] public CanvasGroup cg;

    // Tab
    [HideInInspector] private static float tabInteractDurTime = 0.25f;

    // Tab -> Module
    [HideInInspector] private float defaultModuleRectX;
    [HideInInspector] public List<InventorySlotEUIController> moduleSlots = new List<InventorySlotEUIController>();

    // Tab -> Skill State
    [HideInInspector] private List<Image> skillImgList = new List<Image>();
    [HideInInspector] private float defaultPlayerStatesRectX;
    [HideInInspector] private float defaultSkillStatesRectY;

    // Color
    [HideInInspector] public List<Component> mainClrCompList;
    [HideInInspector] public List<Component> subClrCompList;
    [HideInInspector] private Color interactableClr;

    // Tab
    [HideInInspector] private Sequence tabSeq;

    // Interact
    [HideInInspector] private bool isActingInteractUi = false;

    // Ally
    [HideInInspector] private List<AllyPresenceEUIController> allAllyPresence = new List<AllyPresenceEUIController>();
    [HideInInspector] private float defaultAllyStateRectX;

    // Hitted
    [HideInInspector] private static float offsetXPos;

    // KeyItem

    // High Lv Item
    [HideInInspector] private float defaultHighLvItemRectX;

    #endregion

    #endregion

    #region Offset

    public override void Offset(Camera camera)
    {
        base.Offset(camera);

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
        allAllyPresence = new List<AllyPresenceEUIController> { stAllyPresence, utAllyPresence, ntAllyPresence };

        for (int i = 0; i < allAllyPresence.Count; i++)
            allAllyPresence[i].Offset();
        
        currentEmptyBc.Offset();
        emptyBc.Offset();
        fullEc.Offset();

        // EP Txt
        ecCostTxt.text = PlayerManager.instance.playerController.needEP_ForMakeEC.ToString();

        // 스킬
        for (int i = 0; i < skillList.Count; i++) skillList[i].Offset();

        // 모듈 아이템
        moduleSlots = DevTool.Get_ChildList<InventorySlotEUIController>(moduleListParentRt);
        for (int i = 0; i < moduleSlots.Count; i++)
        {
            moduleSlots[i].Offset();
            moduleSlots[i].item.Offset();
            moduleSlots[i].Set_EquipedTxt(true, i);
        }

        // 버프
        //PoolingManager.Instance.BuffIcons.ParentTF = BuffParentTF;

        cg = DevTool.Get_ComponentTType(gameObject, out CanvasGroup _cg) ? _cg : null;

        offsetXPos = hittedInfoRt.anchoredPosition.x;

        isTabInteracted.Value = false;

    }

    private void Offset_RectPosData()
    {
        // 기본 위치
        // 모듈
        defaultModuleRectX = DevTool.Get_ComponentTType(
            moduleListParentRt.gameObject, out RectTransform module_Rt) ?
                module_Rt.anchoredPosition.x : 0f;

        // 플레이어 스탯
        defaultPlayerStatesRectX = DevTool.Get_ComponentTType(
            playerStatesCostParentRt.gameObject, out RectTransform state_Rt) ?
                state_Rt.anchoredPosition.x : 0f;

        // 스킬
        defaultSkillStatesRectY = DevTool.Get_ComponentTType(
            skillStatesParentRt.gameObject, out RectTransform ss_Rt) ?
                ss_Rt.anchoredPosition.y : 0f;

        // CG 값
        for (int i = 0; i < parentCgList.Count; i++) parentCgList[i].alpha = 0f;

        // 부스트
        defaultBoostRectY = boostRt.anchoredPosition.y;

        // Ally
        defaultAllyStateRectX = allyStateParentRt.anchoredPosition.x;

        // High Lv Item
        defaultHighLvItemRectX = highLvItemRt.anchoredPosition.x;
    }

    private void Offset_Subscribe()
    {
        PlayerManager.instance.playerController.currentBetteryShard
            .Subscribe(_currentBs =>
            {
                if (PlayerManager.instance.playerController.currentBetteryShard.Value < PlayerManager.instance.playerController.needBS_ForMakeBC)
                {
                    currentEmptyBc.Change_Sprite(_currentBs);
                }
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.currentBettery
            .Subscribe(_currentBc =>
            {
                emptyBc.Set_Amount(_currentBc, 0.5f);
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.currentChargedBettery
            .Subscribe(_currentEc =>
            {
                fullEc.Set_Amount(_currentEc, 0.5f);

                DOTween.Kill(ecCostArrowImg);
                ecCostArrowImg.DOFade(1f, 0.2f)
                    .OnComplete(() =>
                    {
                        ecCostArrowImg.DOFade(0.25f, 0.2f);
                    });
            })
            .AddTo(gameObject);


        PlayerManager.instance.playerController.currentCredit
            .Subscribe(_currentCredit =>
            {
                creditEui.Play_Amount(_currentCredit);
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.currentOverrider
            .Subscribe(_currentOverrider =>
            {
                overriderEui.Play_Amount(_currentOverrider);
            })
            .AddTo(gameObject);

        PlayerManager.instance.playerController.currentModuleShard
            .Subscribe(_currentMS =>
            {
                msEui.Play_Amount(_currentMS);
            })
            .AddTo(gameObject);


        PlayerManager.instance.playerController.currentBoostLv
            .Subscribe(_boostLevel =>
            {
                Set_TextOfBoost(_boostLevel);

                Play_ActiveBoost(boostLightArr, _boostLevel);
                Play_ActiveBoost(boostLightWheelArr, _boostLevel);

                Play_RollBoost(boostLightWheelArr, _boostLevel);
            })
            .AddTo(gameObject);


        PlayerManager.instance.playerController.strikeTeamPresence
            .Subscribe(_presence =>
            {
                stAllyPresence.Play_Amount(PlayerManager.instance.playerController.strikeTeamPresence.Value, PlayerManager.instance.playerController.needStrikeTeamPresence.Value);
            });
        PlayerManager.instance.playerController.uplinkTeamPresence
            .Subscribe(_presence =>
            {
                utAllyPresence.Play_Amount(PlayerManager.instance.playerController.uplinkTeamPresence.Value, PlayerManager.instance.playerController.needUplinkTeamPresence.Value);
            });
        PlayerManager.instance.playerController.neoTeamPresence
            .Subscribe(_presence =>
            {
                ntAllyPresence.Play_Amount(PlayerManager.instance.playerController.neoTeamPresence.Value, PlayerManager.instance.playerController.needNeoTeamPresence.Value);
            });

        PlayerManager.instance.playerController.needStrikeTeamPresence
            .Subscribe(_needPresence =>
            {
                stAllyPresence.Play_Amount(PlayerManager.instance.playerController.strikeTeamPresence.Value, PlayerManager.instance.playerController.needStrikeTeamPresence.Value);
            });
        PlayerManager.instance.playerController.needUplinkTeamPresence
            .Subscribe(_needPresence =>
            {
                utAllyPresence.Play_Amount(PlayerManager.instance.playerController.uplinkTeamPresence.Value, PlayerManager.instance.playerController.needUplinkTeamPresence.Value);
            });
        PlayerManager.instance.playerController.needNeoTeamPresence
            .Subscribe(_needPresence =>
            {
                ntAllyPresence.Play_Amount(PlayerManager.instance.playerController.neoTeamPresence.Value, PlayerManager.instance.playerController.needNeoTeamPresence.Value);
            });
    }

    private void Offset_Img()
    {
        // 스킬 이미지
        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            skillImgList.Add(DevTool.Get_ComponentTType<Image>(skillList[i].gameObject));
            skillImgList[i].sprite = PlayerManager.instance.playerController.skillWeapon.skillList[i].icon;
        }

        creditEui.Offset();
        overriderEui.Offset();
        msEui.Offset();

        paneltyAnnoCg.alpha = 0f;
        DevTool.SetColor(uninteractableClr, paneltyAnnoNameTxt);
        paneltyAnnoNameTxt.text = "";
        DevTool.SetColor(uninteractableClr, paneltyAnnoDescTxt);
        paneltyAnnoDescTxt.text = "";
        paneltyAnnoCg.gameObject.SetActive(false);

        // Key
        for (int i = 0; i < keyItemImgList.Count; i++)
        {
            keyItemImgList[i].gameObject.SetActive(false);
        }

    }

    public void Offset_ColorComp()
    {
        DevTool.Set_AlphaColor(stageNameTxt, 1);
        DevTool.Set_AlphaColor(stageDescTxt, 0);

        mainClrCompList.AddRange(Get_MainColorComp());
        subClrCompList.AddRange(Get_SubColorTxt());

        // Color Set
        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, mainClrCompList);
        mainClrCompList.Clear();
        mainClrCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, subClrCompList);
        subClrCompList.Clear();
        subClrCompList = null;

        interactableClr = interactOnOffTxt.color;
    }

    private void Offset_AfterColorSet()
    {
        minimapEui.Offset();
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

        playerStatesTxt.text = Get_PlayerStateTxt(player, weapon);

        for (int i = 0; i < DevTool.skillAmount; i++)
            skillStatesTxtList[i].text = Get_SkillStateTxt(skill.skillList[i]);
    }

    #endregion

    #region Framework

    private void LateUpdate()
    {
        Caculate_TabInput(Time.deltaTime);
    }

    #endregion

    #region Caculate

    private void Caculate_TabInput(float deltaTime)
    {
        if (isTabInputed)  
            Caculate_WhenTabInputOn(deltaTime);
        else  
            Caculate_WhenTabInputOff(deltaTime);
    }

    private void Caculate_WhenTabInputOn(float deltaTime)
    {
        if (tabInputedMaxTime > tabInputedCurrentTime) // 인풋 시간 계산
        {
            tabInputedCurrentTime += deltaTime;
        }
        else // 인풋 시간 충분 상태
        {
            if (!isTabInteracted.Value)
            {
                SetOn_TabInteract();
            }
        }
    }

    private void Caculate_WhenTabInputOff(float deltaTime)
    {
        if (tabInputedCurrentTime != 0)
        {
            tabInputedCurrentTime = 0f;
        }
        if (isTabInteracted.Value)
        {
            SetOff_TabInteract();
        }
    }

    #endregion

    #region Interact

    public void Set_InteractUI()
    {
        if (isActingInteractUi) return; 

        IInteract ii = PlayerManager.instance.playerController.currentInteractable.Value;
        string txt = DevTool.Get_InteractingAnnoTxt(ii, out bool canInteract);

        if (ii != null && txt != "")
        {
            if (canInteract)
            {
                Set_InteractTxt($"-{interactEnableString}-", txt, interactableClr);
                Set_InteractFade(1f, 0.5f);
            }
            else
            {
                Set_InteractTxt($"-{interacInoperableString}-", txt, uninteractableClr);
                Set_InteractFade(1f, 0.5f);
            }
        }
        else
        {
            Set_InteractTxt($"-{interactDisableString}-", $"< {interactNoneString} >", new Color(1, 1, 1, interactOnOffTxt.color.a));
            Set_InteractFade(0.25f, 0.5f);
        }
    }


    private void Set_InteractTxt(string onOffTxt, string interactableTxt, Color onOffTxtColor)
    {
        interactOnOffTxt.text = onOffTxt;
        interactOnOffTxt.color = onOffTxtColor;
        interactDescTxt.text = interactableTxt;
    }

    private void Set_InteractFade(float alpha, float durTime)
    {
        DevTool.SetKillTween(interactOnOffTxt);
        DevTool.SetKillTween(interactDescTxt);

        interactOnOffTxt.DOFade(alpha, durTime);
        interactDescTxt.DOFade(alpha, durTime);
    }

    #endregion

    #region Stage

    public void Set_StageDescription()
    {
        stageNameTxt.DOText(
            ResourceManager.instance.Get_MapName(
                StageManager.instance.Get_CurrentStageData().infoData.stageId), 0.5f)
            .OnPlay(() => { stageNameTxt.text = ""; });

        stageDescTxt.DOText(
            ResourceManager.instance.Get_MapDesc(
                StageManager.instance.Get_CurrentStageData().infoData.stageId), 0.5f)
            .OnPlay(() => { stageDescTxt.text = ""; });
    }

    #endregion

    #region Tab

    public void SetOn_TabInteract()
    {
        if (isTabInteracted.Value) return;
        isTabInteracted.Value = true;

        Reset_Tab();

        DevTool.Set_KillTween(tabSeq);

        tabSeq = Play_SeqInteract(
            moduleRtX: 0f,
            allyStateRtX: 700f,
            costRtX: 0f,
            skillRtY: 0f,
            boostRtY: 0f,
            highLvItemRtX: 0f,
            stageNameAlpha: 0f,
            stageDescAlpha: 1f,
            tabInteractDurTime, Ease.OutCubic);

        tabSeq.Join(Play_FadeCGs(1, tabInteractDurTime));

        minimapEui.SetOn_TabInteract(tabInteractDurTime);
    }

    public void SetOff_TabInteract()
    {
        if (!isTabInteracted.Value) return; 
        isTabInteracted.Value = false;

        DevTool.Set_KillTween(tabSeq);

        tabSeq = Play_SeqInteract(
            defaultModuleRectX,
            defaultAllyStateRectX, 
            defaultPlayerStatesRectX,
            defaultSkillStatesRectY, 
            defaultBoostRectY,
            defaultHighLvItemRectX,
            stageNameAlpha: 1f,
            stageDescAlpha: 0f, 
            tabInteractDurTime, Ease.InCubic);

        tabSeq.Join(Play_FadeCGs(0, tabInteractDurTime));

        minimapEui.SetOff_TabInteract(tabInteractDurTime);
    }

    #endregion

    #region Ally State

    public void Add_AllyState(AllyHUDController hud)
    {
        if (hud.gameObject.TryGetComponent(out RectTransform rt))
        {
            rt.gameObject.transform.SetParent(allyStateParentRt);
            rt.gameObject.layer = LayerMask.NameToLayer("UI");

            rt.pivot = new Vector2(0, 1);
            rt.localScale = Vector3.one;

            int currentAmount = allyHudList.Count;
            float y = currentAmount == 0 ? 0 : (currentAmount % maxCol) * colInterval;
            float x = currentAmount == 0 ? 0 : (currentAmount / maxCol) * rowInterval;
            rt.anchoredPosition3D = new Vector3(x, y, 0);

            allyHudList.Add(rt);
        }
    }

    #endregion

    #region Boost

    private void Set_TextOfBoost(int currentLv)
    {
        boostLv.text = currentLv.ToString();
        DevTool.Set_AlphaColor(boostLv, currentLv == 0 ? 0.1f : 0.25f * (currentLv));
    }

    #endregion

    #region Buff

    public void Set_BuffPosUI()
    {
        for (int i = 0; i < allBuffIconUi.Count; i++)
            allBuffIconUi[i].rt.anchoredPosition = new Vector2(i * (allBuffIconUi[i].rt.rect.width + buffUiXInterval), 0);
    }

    #endregion

    #region High Lv Item

    public void Init_HighLvItemUI()
    {
        List<EachItemJsonData> itemData = SaveDataManager.instance.jsonData.itemData;
        for (int i = 0; i < itemData.Count; i++)
        {
            highLvItemAmountTxtList[i].text = itemData[i].amount.ToString();
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
        paneltyAnnoNameTxt.text = "";
        paneltyAnnoDescTxt.text = "";

        Sequence seq = DOTween.Sequence();
        paneltyAnnoCg.gameObject.SetActive(true);

        seq.Append(paneltyAnnoCg.DOFade(1f, 0.5f));
        seq.Join(paneltyAnnoNameTxt.DOText(title, 0.5f));
        seq.Join(paneltyAnnoDescTxt.DOText(desc, 0.5f));
        seq.AppendInterval(1f);
        seq.Append(paneltyAnnoCg.DOFade(0f, 2f));

        seq.OnComplete(() => { paneltyAnnoCg.gameObject.SetActive(false); });
    }

    #endregion

    #region Hitted

    // 피격 시 효과
    public void Play_HittedPlayScreen(float dmg, float durTime)
    {
        DevTool.SetKillTween(hittedScreen);

        Sequence seq = DOTween.Sequence();
        seq.Append(hittedScreen.DOFade((Math.Min(100, dmg) * 0.01f), durTime));
        seq.Append(hittedScreen.DOFade(0, durTime));
    }


    // 피격 정보
    public void Play_HittedPlayInfo(float dmg, float durTime)
    {
        hittedDmgTxt.text = $"<size=75%>{ResourceManager.instance.Get_StaticWord(74)}:</size> {dmg.ToString("0.0")}";
        hittedDmgTxt.color = uninteractableClr;

        Play_Info(durTime);
    }

    // 회피 정보
    public void Play_AvoidPlayInfo(float durTime)
    {
        hittedDmgTxt.text = $"{ResourceManager.instance.Get_StaticWord(75)}";
        hittedDmgTxt.color = Color.white;

        Play_Info(durTime);
    }

    // 정보
    private void Play_Info(float durTime)
    {
        hittedInfoPivotTf.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-5f, 5f));

        DevTool.SetKillTween(hittedInfoRt);

        Sequence seq = DOTween.Sequence();

        hittedInfoRt.anchoredPosition = new Vector2(offsetXPos, 0);
        seq.Append(hittedInfoRt.DOAnchorPosX(-50f, durTime * 0.2f).SetEase(Ease.Linear));
        seq.Append(hittedInfoRt.DOAnchorPosX(50f, durTime * 0.6f).SetEase(Ease.Linear));
        seq.Append(hittedInfoRt.DOAnchorPosX(-offsetXPos, durTime * 0.2f).SetEase(Ease.Linear));
    }

    #endregion

    #region Boost

    // 부스트 키기
    private void Play_ActiveBoost(GameObject[] arr, int currentLv)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (DevTool.Get_ComponentTType(arr[i].gameObject, out Image img))
            {
                DevTool.SetKillTween(img);

                if (i < currentLv) img.DOFade(1f, 0.2f);
                else img.DOFade(0f, 0.2f);
            }
        }
    }

    // 부스트 돌리기
    private void Play_RollBoost(GameObject[] arr, int currentLv)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (DevTool.Get_ComponentTType(arr[i].gameObject, out RectTransform rt))
            {
                if (i < currentLv) Play_EachRollBoost(rt, i);
                else DevTool.SetKillTween(rt);
            }
        }
    }

    // 부스트 하나씩 돌리기
    private void Play_EachRollBoost(RectTransform rt, int index)
    {
        rt.DOLocalRotate(new Vector3(0, 0, 360), 0.2f, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        rt.DOLocalRotate(new Vector3(0, 0, 360), 3f / (index + 1f), RotateMode.LocalAxisAdd)
                            .SetEase(Ease.Linear)
                            .SetLoops(-1, LoopType.Restart);
                    });
    }

    #endregion

    #region Tab Interact

    // Tab 이동
    private Sequence Play_SeqInteract(
        float moduleRtX, float allyStateRtX, float costRtX, float skillRtY, float boostRtY, float highLvItemRtX,
        float stageNameAlpha, float stageDescAlpha,
        float durTime, Ease ease)
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(moduleListParentRt.DOAnchorPosX(moduleRtX, durTime));
        seq.Join(allyStateParentRt.DOAnchorPosX(allyStateRtX, durTime));
        seq.Join(playerStatesCostParentRt.DOAnchorPosX(costRtX, durTime));
        seq.Join(skillStatesParentRt.DOAnchorPosY(skillRtY, durTime));
        seq.Join(boostRt.DOAnchorPosY(boostRtY, durTime));
        seq.Join(highLvItemRt.DOAnchorPosX(highLvItemRtX, durTime));

        seq.Join(stageNameTxt.DOFade(stageNameAlpha, durTime));
        seq.Join(stageDescTxt.DOFade(stageDescAlpha, durTime));
        seq.SetEase(ease);
        return seq;
    }

    // Tab 투명도
    private Sequence Play_FadeCGs(float alpha, float durTime)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < parentCgList.Count; i++)
            seq.Join(parentCgList[i].DOFade(alpha, durTime));
        return seq;
    }



    // 상호작용 시 발생
    public void Play_UseInteractUI()
    {
        isActingInteractUi = true;

        DevTool.SetKillTween(usingInnerImg);

        usingInnerImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                usingInnerImg.DOFade(0.25f, 0.2f)
                .OnComplete(() =>
                {
                    isActingInteractUi = false;
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
            // 부스트
            boostLv,

            // 플레이어 스탯
            playerStatesTxt,

            // 스테이지
            stageNameTxt, stageDescTxt,

            // 아이템
            ecCostTxt, emptyBc.amountTxt, fullEc.amountTxt,

            // 상호작용
            interactOnOffTxt

        };

        // 부스트
        result.AddRange(DevTool.Get_ComponentTTypeList<Image>(boostLightArr.ToList()));
        result.AddRange(DevTool.Get_ComponentTTypeList<Image>(boostLightWheelArr.ToList()));

        // 스킬
        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            result.Add(skillList[i].skillCostTxt);
            result.Add(skillList[i].skillErrorTxt);
            result.Add(skillStatesTxtList[i]);
        }

        // Ally
        for (int i = 0; i < allAllyPresence.Count; i++)
        {
            mainClrCompList.AddRange(DevTool.Get_ChildList<Image>(allAllyPresence[i].capMiddleRt.transform));
        }
        
        return result;
    }

    private List<Component> Get_SubColorTxt()
    {
        List<Component> result = new List<Component>
        {
            // 미니맵
            minimapEui.innerImg, 

            // 아이템
            currentEmptyBc.lightInner, ecCostArrowImg, emptyBc.innerImg, fullEc.innerImg,
            
            // 상호작용
            innerImg, usingInnerImg
        };

        // 부스트 Inner
        subClrCompList.AddRange(boostInnerList);

        // 스킬
        for (int i = 0; i < DevTool.skillAmount; i++)
            subClrCompList.Add(skillList[i].skillInnerImg);

        // Ally
        for (int i = 0; i < allAllyPresence.Count; i++)
            subClrCompList.Add(allAllyPresence[i].innerImg);
        
        return result;
    }


    // Tab 플레이어 스탯의 엘레먼트
    private List<string> Get_PlayerStateStrings(PlayerController player, PlayerWeaponController weapon)
    {
        return new List<string>()
        {
            player.maxEP.actualState.ToString(),
            player.walkSpeed.actualState.ToString(),
            player.dash.dashSpeed.actualState.ToString(),
            player.dash.Get_ActualNeedEP().ToString(),
            weapon.baseDamage.actualState.ToString(),
            weapon.rof.actualState.ToString(),
            weapon.accRate.actualState.ToString(),
            weapon.cc.actualState.ToString(),
            weapon.cd.actualState.ToString()
        };
    }

    // Tab 플레이어 스탯 Txt 
    private string Get_PlayerStateTxt(PlayerController player, PlayerWeaponController weapon)
    {
        string result = "";
        List<string> strings = Get_PlayerStateStrings(player, weapon);

        for (int i = 0; i < playerStatesStringList.Count; i++)
        {
            result += "<size=70%>" + playerStatesStringList[i] + ": </size>";
            result += "<b>" + strings[i] + "</b>\n";
        }
        return result;
    }

    // Tab 스킬 스탯의 엘레먼트
    private List<string> Get_SkillStateStrings(ActiveSkillController skill)
    {
        return new List<string>()
        {
            skill.tier.actualState.ToString(),
            skill.power.actualState.ToString()
        };
    }

    // Tab 스킬 스탯 Txt
    private string Get_SkillStateTxt(ActiveSkillController skill)
    {
        string result = "";
        List<string> strings = Get_SkillStateStrings(skill);

        for (int i = 0; i < skillStatesStringList.Count; i++)
        {
            result += "<size=70%>" + skillStatesStringList[i] + ": </size>\n";
            result += "<b>" + strings[i] + "</b>\n";
        }
        return result;
    }

    #endregion

    #region Key Item

    public void Set_KeyItem(Dictionary<int, int> keyItemDict)
    {
        for (int i = 0; i < keyItemImgList.Count; i++)
        {
            keyItemImgList[i].gameObject.SetActive(false);
        }

        int index = 0;

        foreach(var keyItem in keyItemDict)
        {
            if (keyItem.Value == 0) continue;

            keyItemImgList[index].sprite = ResourceManager.instance.Get_KeyCardSprite(keyItem.Key);
            keyItemAmountTxtList[index].text = keyItem.Value.ToString();
            keyItemImgList[index].gameObject.SetActive(true);

            index++;
        }
    }

    public void Effect_KeyIcon(int id)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < keyItemImgList.Count; i++)
        {
            if (keyItemImgList[i].gameObject.activeSelf && keyItemImgList[i].sprite == ResourceManager.instance.Get_KeyCardSprite(id))
            {
                seq.Append(keyItemImgList[i].transform.DOScale(1.3f, 0.1f));
                seq.Append(keyItemImgList[i].transform.DOScale(1f, 0.3f));

                if (keyItemVfxImg.TryGetComponent(out RectTransform vfxRt) &&
                    keyItemImgList[i].TryGetComponent(out RectTransform imgRt))
                {
                    DevTool.SetKillTween(vfxRt);
                    vfxRt.anchoredPosition = imgRt.anchoredPosition;
                    vfxRt.rotation = Quaternion.identity;
                    vfxRt.DORotate(Vector3.forward * 360, 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                    vfxRt.transform.localScale = Vector3.one;
                    vfxRt.DOScale(0.5f, 1f);
                }
                DevTool.SetKillTween(keyItemVfxImg);
                keyItemVfxImg.color = new Color(1, 1, 1, 0.7f);
                keyItemVfxImg.DOFade(0f, 1f);
            }
        }
    }

    #endregion

    #region Ally Reputation

    public void Set_AllyReputation(float value)
    {
        allyReputationTxt.text = $"{value} %"; 
        allyReputationImg.fillAmount = value * 0.01f;
        allyReputationImg.color = Color.Lerp(new Color(0.8f, 1f, 0.8f, 1f), new Color(0.35f, 1f, 0.35f, 1f), allyReputationImg.fillAmount);
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        interactEnableString = ResourceManager.instance.Get_StaticWord(4);
        interactDisableString = ResourceManager.instance.Get_StaticWord(5);
        interacInoperableString = ResourceManager.instance.Get_StaticWord(6);
        interactNoneString = ResourceManager.instance.Get_StaticWord(7);

        playerStatesStringList.Clear();
        for (int i = 8; i <= 16; i++)
            playerStatesStringList.Add(ResourceManager.instance.Get_StaticWord(i));

        skillStatesStringList.Clear();
        for (int i = 17; i <= 18; i++)
            skillStatesStringList.Add(ResourceManager.instance.Get_StaticWord(i));

        for (int i = 0; i < allAllyPresence.Count; i++)
            allAllyPresence[i].presenceLangTxt.text = $"{ResourceManager.instance.Get_StaticWord(i + 61)}<size=85%> {ResourceManager.instance.Get_StaticWord(70)}</size>";

        Set_InteractUI();
        Set_StageDescription();
    }

    #endregion
}

