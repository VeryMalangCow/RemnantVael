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

    [Space(10)]
    [Header("=== View")]
    [SerializeField] private HudEpView epView;
    [SerializeField] private HudTabStateView tabStateView;
    [SerializeField] private HudTabModuleView tabModuleView;
    [SerializeField] private HudSkillView skillView;
    [SerializeField] private HudAllyStateView allyStateView;
    [SerializeField] private HudBetteryShardView betteryShardView;
    [SerializeField] private HudBetteryView betteriesView;
    [SerializeField] private HudLootableItemView lootableItemsView;
    [SerializeField] private HudKeyView keyView;

    public HudEpView EpView { get { return epView; } }
    public HudTabStateView TabStateView { get { return TabStateView; } }
    public HudTabModuleView TabModuleView { get { return tabModuleView; } }
    public HudSkillView SkillView { get { return skillView; } }
    public HudAllyStateView AllyStateView { get { return allyStateView; } }
    public HudBetteryShardView BetteryShardView { get { return betteryShardView; } }
    public HudBetteryView BetteriesView { get { return betteriesView; } }
    public HudLootableItemView LootableItemsView { get { return lootableItemsView; } }
    public HudKeyView KeyView { get { return keyView; } }


    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public CanvasGroup cg;


    public IEnumerator Init(Color mainClr, Color subClr)
    {
        PlayerController player = PlayerManager.instance.playerController;
        
        yield return epView.Init(mainClr, subClr);
        yield return tabStateView.Init(mainClr);
        yield return tabModuleView.Init();
        yield return allyStateView.Init();
        yield return skillView.Init(player.skillWeapon, mainClr, subClr);
        yield return betteryShardView.Init(subClr);
        yield return betteriesView.Init(player, mainClr, subClr);
        yield return lootableItemsView.Init();
        yield return keyView.Init();
    }


    #region Value

    #region - Inspector


    [Space(10)]
    [Header("=== Tab")]
    [SerializeField] public ReactiveProperty<bool> isTabInteracted = new ReactiveProperty<bool>();
    [SerializeField] public bool isTabInputed = false;
    [SerializeField] private static float tabInputedMaxTime = 0.25f;
    [SerializeField] private float tabInputedCurrentTime = 0f;

    [Space(10)]
    [Header("=== Boost")]
    [SerializeField] private RectTransform boostRt;
    [HideInInspector] private float defaultBoostRectY;
    [SerializeField] private TMP_Text boostLv;
    [SerializeField] private GameObject[] boostLightArr;
    [SerializeField] private GameObject[] boostLightWheelArr;
    [SerializeField] List<Image> boostInnerList;

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

    // Comp

    // Tab
    [HideInInspector] private static float tabInteractDurTime = 0.25f;

    // Tab -> Skill State

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
        
        // 버프
        //PoolingManager.Instance.BuffIcons.ParentTF = BuffParentTF;

        offsetXPos = hittedInfoRt.anchoredPosition.x;

        isTabInteracted.Value = false;

    }

    private void Offset_RectPosData()
    {
        // 기본 위치

        // 부스트
        defaultBoostRectY = boostRt.anchoredPosition.y;

        // High Lv Item
        defaultHighLvItemRectX = highLvItemRt.anchoredPosition.x;
    }

    private void Offset_Subscribe()
    {
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
        paneltyAnnoCg.alpha = 0f;
        DevTool.SetColor(uninteractableClr, paneltyAnnoNameTxt);
        paneltyAnnoNameTxt.text = "";
        DevTool.SetColor(uninteractableClr, paneltyAnnoDescTxt);
        paneltyAnnoDescTxt.text = "";
        paneltyAnnoCg.gameObject.SetActive(false);
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

        tabStateView.ResetTab(player);
        skillView.ResetTab(player.skillWeapon);
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

        tabStateView.TabOn(tabInteractDurTime);
        tabModuleView.TabOn(tabInteractDurTime);


        tabSeq = Play_SeqInteract(
            boostRtY: 0f,
            highLvItemRtX: 0f,
            stageNameAlpha: 0f,
            stageDescAlpha: 1f,
            tabInteractDurTime, Ease.OutCubic);

        minimapEui.SetOn_TabInteract(tabInteractDurTime);
    }

    public void SetOff_TabInteract()
    {
        if (!isTabInteracted.Value) return; 
        isTabInteracted.Value = false;

        DevTool.Set_KillTween(tabSeq);

        tabStateView.TabOff(tabInteractDurTime);
        tabModuleView.TabOff(tabInteractDurTime);

        tabSeq = Play_SeqInteract(
            defaultBoostRectY,
            defaultHighLvItemRectX,
            stageNameAlpha: 1f,
            stageDescAlpha: 0f, 
            tabInteractDurTime, Ease.InCubic);

        minimapEui.SetOff_TabInteract(tabInteractDurTime);
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
    private Sequence Play_SeqInteract(float boostRtY, float highLvItemRtX,
        float stageNameAlpha, float stageDescAlpha,
        float durTime, Ease ease)
    {
        Sequence seq = DOTween.Sequence();
        //seq.Join(moduleListParentRt.DOAnchorPosX(moduleRtX, durTime));
        //seq.Join(allyStateParentRt.DOAnchorPosX(allyStateRtX, durTime));
        //seq.Join(playerStatesCostParentRt.DOAnchorPosX(costRtX, durTime));
        //seq.Join(skillStatesParentRt.DOAnchorPosY(skillRtY, durTime));
        seq.Join(boostRt.DOAnchorPosY(boostRtY, durTime));
        seq.Join(highLvItemRt.DOAnchorPosX(highLvItemRtX, durTime));

        seq.Join(stageNameTxt.DOFade(stageNameAlpha, durTime));
        seq.Join(stageDescTxt.DOFade(stageDescAlpha, durTime));
        seq.SetEase(ease);
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

            // 스테이지
            stageNameTxt, stageDescTxt,

            // 상호작용
            interactOnOffTxt

        };

        // 부스트
        result.AddRange(DevTool.Get_ComponentTTypeList<Image>(boostLightArr.ToList()));
        result.AddRange(DevTool.Get_ComponentTTypeList<Image>(boostLightWheelArr.ToList()));



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

            // 상호작용
            innerImg, usingInnerImg
        };

        // 부스트 Inner
        subClrCompList.AddRange(boostInnerList);

        // Ally
        for (int i = 0; i < allAllyPresence.Count; i++)
            subClrCompList.Add(allAllyPresence[i].innerImg);
        
        return result;
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

        tabStateView.SetLanguage();
        skillView.SetLanguage();

        for (int i = 0; i < allAllyPresence.Count; i++)
            allAllyPresence[i].presenceLangTxt.text = $"{ResourceManager.instance.Get_StaticWord(i + 61)}<size=85%> {ResourceManager.instance.Get_StaticWord(70)}</size>";

        Set_InteractUI();
        Set_StageDescription();
    }

    #endregion
}

