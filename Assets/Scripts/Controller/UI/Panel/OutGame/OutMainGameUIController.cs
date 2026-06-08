using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
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

    #region - State

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private StateUIController stateUi;
    [Serializable] private class StateUIController
    {
        #region Value

        [SerializeField] public RectTransform panelRt;
        [SerializeField] public OwnBtnEUIController backBtn;
        [SerializeField] public OwnBtnEUIController changeTypeBtn;
        [SerializeField] public bool isBuPanelOn = true;

        [SerializeField] private TMP_Text playerStateNameTxt;
        [SerializeField] private TMP_Text allyStateNameTxt;
        [SerializeField] private Image[] innerImgArr;

        #endregion

        #region Value - Player

        // DMG, ROF, CC, CD, MS, AR, KB
        // MaxEP, ESValue, SkillCost, Resist
        // WalkS, WalkSWhileS, DashP, AvoidC
        // Skill 00: Cooltime, Power, Tier
        // Skill 01: Cooltime, Power, Tier
        [Space(10)]
        [Header("=== Player - BU")]
        [SerializeField] private GameObject playerBuPanelGo;
        [SerializeField] private ScrollPanelEUIController playerBuScrollEui;
        [SerializeField] private StandbyPlayerBUEUIController[] buEuiArr;

        [Space(10)]
        [Header("=== Player - MU")]
        [SerializeField] private GameObject playerMuPanelGo;
        [SerializeField] private ScrollPanelEUIController playerMuScrollEui;
        [SerializeField] private StandbyPlayerMUEUIController[] muEuiArr;
        [SerializeField] private StandbyPlayerSynergyEUIController[] synergyEuiArr;

        #endregion

        #region Value - Ally

        [Space(10)]
        [Header("=== Ally - BU")]
        [SerializeField] private ScrollPanelEUIController allyScrollEui;
        [SerializeField] private GameObject allyBuEuiPrefab;
        [HideInInspector] private List<StandbyAllyBUEUIController> allyBuEuiList = new List<StandbyAllyBUEUIController>();

        #endregion

        #region Offset

        public void Offset(OutMainGameUIController uiController)
        {
            backBtn.ownerUIController = uiController;
            backBtn.Offset();

            changeTypeBtn.ownerUIController = uiController;
            changeTypeBtn.Offset();

            playerBuScrollEui.Offset();
            playerMuScrollEui.Offset();

            allyScrollEui.Offset();

            for (int i = 0; i < buEuiArr.Length; i++)
                buEuiArr[i].Offset();

            for (int i = 0; i < muEuiArr.Length; i++)
                muEuiArr[i].Offset();

            for (int i = 0; i < synergyEuiArr.Length; i++)
                synergyEuiArr[i].Offset();

            Set_Panel(true);
        }


        #endregion

        #region Panel

        public void Change_Panel()
        {
            SoundManager.instance.Play_2D_SFX_UI("Click_01");
            Set_Panel(!isBuPanelOn);
        }

        public void Set_Panel(bool isBuPanelOn)
        {
            this.isBuPanelOn = isBuPanelOn;

            playerBuPanelGo.gameObject.SetActive(isBuPanelOn);
            playerMuPanelGo.gameObject.SetActive(!isBuPanelOn);

            for (int i = 0; i < allyBuEuiList.Count; i++)
            {
                allyBuEuiList[i].Set_Panel(isBuPanelOn);
            }
        }

        #endregion

        #region Set

        string Get(int index) => ResourceManager.instance.Get_StaticWord(index);

        public void Set_Color(Color imgClr, Color txtClr)
        {
            for (int i = 0; i < innerImgArr.Length; i++)
                innerImgArr[i].color = imgClr;

            #region Player

            playerStateNameTxt.color = imgClr;

            // BU
            for (int i = 0; i < buEuiArr.Length; i++)
                buEuiArr[i].Set_Color(imgClr, txtClr);

            // MU
            for (int i = 0; i < muEuiArr.Length; i++)
                muEuiArr[i].Set_Color(txtClr);

            for (int i = 0; i < synergyEuiArr.Length; i++)
                synergyEuiArr[i].Set_Color(imgClr, txtClr);

            #endregion

            #region Ally

            allyStateNameTxt.color = imgClr;

            for (int i = 0; i < allyBuEuiList.Count; i++)
                allyBuEuiList[i].Set_Color();

            #endregion
        }

        public void Set_LanguageTxt()
        {
            #region Player

            playerStateNameTxt.text = $"<size=70%><color=#808080>{Get(102)} - </color></size><b>[{Get(113)}]</b>";

            // DMG, ROF, CC, CD, MS, AR, KB
            buEuiArr[0].Set_LanguageTxt(Get(12));
            buEuiArr[1].Set_LanguageTxt(Get(13));
            buEuiArr[2].Set_LanguageTxt(Get(15));
            buEuiArr[3].Set_LanguageTxt(Get(16));
            buEuiArr[4].Set_LanguageTxt(Get(43));
            buEuiArr[5].Set_LanguageTxt(Get(14));
            buEuiArr[6].Set_LanguageTxt(Get(44));

            // MaxEP, ESValue, SkillCost, Resist
            buEuiArr[7].Set_LanguageTxt(Get(8));
            buEuiArr[8].Set_LanguageTxt(Get(38));
            buEuiArr[9].Set_LanguageTxt(Get(39));
            buEuiArr[10].Set_LanguageTxt(Get(37));

            // WalkS, WalkSWhileS, DashP, AvoidC
            buEuiArr[11].Set_LanguageTxt(Get(40));
            buEuiArr[12].Set_LanguageTxt(Get(41));
            buEuiArr[13].Set_LanguageTxt(Get(10));
            buEuiArr[14].Set_LanguageTxt(Get(36));

            // Skill 00: Cooltime, Power, Tier
            buEuiArr[15].Set_LanguageTxt(Get(45));
            buEuiArr[16].Set_LanguageTxt(Get(18));
            buEuiArr[17].Set_LanguageTxt(Get(17));

            // Skill 01: Cooltime, Power, Tier
            buEuiArr[18].Set_LanguageTxt(Get(45));
            buEuiArr[19].Set_LanguageTxt(Get(18));
            buEuiArr[20].Set_LanguageTxt(Get(17));

            #endregion

            #region Ally

            // Ally - BU
            allyStateNameTxt.text = $"<size=70%><color=#808080>{Get(102)} - </color><color=#FFFFFF></size><b>[{Get(95)}]</b></color>";

            #endregion
        }

        public void Set_State(List<AllyController> allAlly)
        {
            #region Player

            PlayerController pc = PlayerManager.instance.playerController;
            PlayerWeaponController pwc = pc.baseWeapon;
            SkillWeaponController swc = pc.skillWeapon;
            PlayerDashController pdc = pc.dash;

            #region Player BU

            // DMG, ROF, CC, CD, MS, AR, KB
            buEuiArr[0].Set(pwc.baseDamage.currentLevel.Value);
            buEuiArr[1].Set(pwc.rof.currentLevel.Value);
            buEuiArr[2].Set(pwc.cc.currentLevel.Value);
            buEuiArr[3].Set(pwc.cd.currentLevel.Value);
            buEuiArr[4].Set(pwc.muzzleSpeed.currentLevel.Value);
            buEuiArr[5].Set(pwc.accRate.currentLevel.Value);
            buEuiArr[6].Set(pwc.kbPower.currentLevel.Value);
            // MaxEP, ESValue, SkillCost, Resist
            buEuiArr[7].Set(pc.maxEP.currentLevel.Value);
            buEuiArr[8].Set(pc.spawnESMultiple.currentLevel.Value);
            buEuiArr[9].Set(pc.needEP_ForSkillMultiple.currentLevel.Value);
            buEuiArr[10].Set(pc.takingDmgMultiple.currentLevel.Value);
            // WalkS, WalkSWhileS, DashP, AvoidC
            buEuiArr[11].Set(pc.walkSpeed.currentLevel.Value);
            buEuiArr[12].Set(pc.walkSpeedWhenShotMultiple.currentLevel.Value);
            buEuiArr[13].Set(pdc.dashSpeed.currentLevel.Value);
            buEuiArr[14].Set(pc.avoidChance.currentLevel.Value);
            // Skill 00: Cooltime, Power, Tier
            buEuiArr[15].Set(swc.skillList[0].maxCooltime.currentLevel.Value);
            buEuiArr[16].Set(swc.skillList[0].power.currentLevel.Value);
            buEuiArr[17].Set(swc.skillList[0].tier.currentLevel.Value);
            // Skill 01: Cooltime, Power, Tier
            buEuiArr[18].Set(swc.skillList[1].maxCooltime.currentLevel.Value);
            buEuiArr[19].Set(swc.skillList[1].power.currentLevel.Value);
            buEuiArr[20].Set(swc.skillList[1].tier.currentLevel.Value);

            #endregion

            #region Player MU

            // Module
            List<CopyModuleState> states = ModuleItemManager.instance.Get_EquippedModuleState();
            for (int i = 0; i < muEuiArr.Length; i++)
            {
                
                if (i < states.Count)
                    muEuiArr[i].SetOn(states[i].state);
                else
                    muEuiArr[i].SetOff();
            }

            // Sync
            for (int i = 0; i < synergyEuiArr.Length; i++)
                synergyEuiArr[i].SetOff();

            int index = 0;
            Dictionary<int, int> syncData = ModuleItemManager.instance.Get_CurrentMainChipData();
            foreach (KeyValuePair<int, int> data in syncData)
            {
                synergyEuiArr[index].SetOn(data.Key, data.Value);
                index++;
            }

            #endregion

            #endregion

            #region Ally

            TryGen_AllyStateEUI(allAlly.Count);

            for (int i = 0; i < allyBuEuiList.Count; i++)
            {
                if (allAlly.Count > i)
                {
                    allyBuEuiList[i].gameObject.SetActive(true);

                    #region Ally BU

                    allyBuEuiList[i].Set_Data(allAlly[i]);

                    #endregion
                }
                else
                {
                    allyBuEuiList[i].gameObject.SetActive(false);
                }
            }

            #endregion
        }

        private void TryGen_AllyStateEUI(int targetAmount)
        {
            if (allyBuEuiList.Count >= targetAmount) return;

            float baseX = -16;
            float baseY = -32;
            float intervalY = -180;

            int needAmount = targetAmount - allyBuEuiList.Count;
            for (int i = 0; i < needAmount; i++)
            {
                Instantiate(allyBuEuiPrefab, allyScrollEui.actualMovableRt).TryGetComponent(out StandbyAllyBUEUIController eui);
                eui.Offset();
                eui.Set_Pos(new Vector2(baseX, baseY + (intervalY * allyBuEuiList.Count)));
                allyBuEuiList.Add(eui);
            }

            allyScrollEui.actualMovableRt.sizeDelta = new Vector2(allyScrollEui.actualMovableRt.sizeDelta.x,
                -((baseY * 1.5f) + (intervalY * allyBuEuiList.Count)));
            allyScrollEui.Set_ScrollPanel();
        }

        #endregion
    }


    #endregion

    #region - Option

    [Space(10)]
    [Header("=== Option")]
    [SerializeField] private OptionUIController optionUi; 
    [Serializable] public class OptionUIController
    {
        [SerializeField] public RectTransform panelRt;
        [SerializeField] public OwnBtnEUIController backBtn;
        [SerializeField] public OwnBtnEUIController applyBtn;
        [SerializeField] public TMP_Text warningTxt;

        [Space(10)]
        [SerializeField] public LRSlidingItemEUIController languagePanelEui;
        [SerializeField] public LRSlidingItemEUIController screenModePanelEui;
        [SerializeField] public LRSlidingItemEUIController resolutionPanelEui;
        [SerializeField] public LRSlidingItemEUIController fpsPanelEui;
        [SerializeField] public FillScrollbarEUIController bgmVolumePanelEui;
        [SerializeField] public FillScrollbarEUIController sfxVolumePanelEui;

        public void Offset(OutMainGameUIController uiController)
        {
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

        public void Set_Panel()
        {
            panelRt.gameObject.SetActive(true);
            warningTxt.gameObject.SetActive(false);
            languagePanelEui.Set_Item(GameManager.languageID);
            screenModePanelEui.Set_Item((int)GameManager.screenMode);
            resolutionPanelEui.Set_Item((int)GameManager.resolutionMode);
            fpsPanelEui.Set_Item((int)GameManager.fps);
            bgmVolumePanelEui.Set_Value(SoundManager.instance.bgmVolume);
            sfxVolumePanelEui.Set_Value(SoundManager.instance.sfxVolume);
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

        public List<Component> Get_MainColorComps()
        {
            List<Component> result = new List<Component>();

            result.AddRange(languagePanelEui.Get_InnerMainColorList());
            result.AddRange(screenModePanelEui.Get_InnerMainColorList());
            result.AddRange(resolutionPanelEui.Get_InnerMainColorList());
            result.AddRange(fpsPanelEui.Get_InnerMainColorList());
            result.AddRange(bgmVolumePanelEui.Get_InnerMainColorList());
            result.AddRange(sfxVolumePanelEui.Get_InnerMainColorList());

            return result;            
        }
    }

    #endregion

    #region - Info

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private InfoUIController infoUi;
    [Serializable] private class InfoUIController
    {
        [Header("=== Comp")]
        [SerializeField] public TMP_Text listTxt;
        [SerializeField] public TMP_Text detailTxt;

        [SerializeField] public RectTransform panelRt;
        [SerializeField] public OwnBtnEUIController backBtn;
        [SerializeField] private ScrollPanelEUIController listScrollPanelEui;

        [Header("=== Element")]
        [SerializeField] private Transform listElementParentTf;

        [HideInInspector] private InfoEUIController[] listEuiArr;
        
        [SerializeField] private Transform detailElementParentTf;

        [HideInInspector] private InfoDetailEUIController[] detailEuiArr;
        [HideInInspector] private InfoDetailEUIController currentDetailEui;

        public void Offset(OutMainGameUIController ownerUI)
        {
            backBtn.Offset();
            backBtn.ownerUIController = ownerUI;

            listScrollPanelEui.Offset();

            int amount = listElementParentTf.childCount;
            
            listEuiArr = new InfoEUIController[amount];
            detailEuiArr = new InfoDetailEUIController[amount];
            for (int i = 0; i < amount; i++)
            {
                listEuiArr[i] = listElementParentTf.GetChild(i).TryGetComponent(out InfoEUIController infoListEUI) ? infoListEUI : null;
                listEuiArr[i].ownerUIController = ownerUI;
                listEuiArr[i].Offset();

                detailEuiArr[i] = detailElementParentTf.GetChild(i).TryGetComponent(out InfoDetailEUIController infoDetailEUI) ? infoDetailEUI : null;
                detailEuiArr[i].Offset();
            }

            Set_LanguageTxt();
            Set_List();
        }

        public void Set_List()
        {
            List<EachInfoJsonData> data = SaveDataManager.instance.jsonData.infoData;

            List<InfoEUIController> visibleEUIs = new List<InfoEUIController>();
            for (int i = 0; i < listEuiArr.Length; i++)
            {
                if (data[i].canVisible)
                {
                    listEuiArr[i].gameObject.SetActive(true);
                    listEuiArr[i].rt.anchoredPosition = new Vector2(listEuiArr[i].rt.anchoredPosition.x, -20 + (-140 * (visibleEUIs.Count)));
                    visibleEUIs.Add(listEuiArr[i]);
                }
                else
                {
                    listEuiArr[i].gameObject.SetActive(false);
                }
            }

            float y = (140 * visibleEUIs.Count) + 20;
            listScrollPanelEui.Set_ScrollHeight(y);
        }

        public void Set_Panel(bool onOff)
        {
            panelRt.gameObject.SetActive(onOff);

            if (!onOff)
            {
                SetOff_DetailWindow();
            }
        }

        public bool Is_ListBtn(OwnBtnEUIController btn)
        {
            return listEuiArr.Contains((InfoEUIController)btn);
        }

        public void SetOn_DetailWindow(InfoEUIController listBtn)
        {
            SetOff_DetailWindow();

            int index = Array.IndexOf(listEuiArr, listBtn);
            currentDetailEui = detailEuiArr[index];
            currentDetailEui.gameObject.SetActive(true);
        }

        private void SetOff_DetailWindow()
        {
            if (currentDetailEui != null)
            {
                currentDetailEui.gameObject.SetActive(false);
                currentDetailEui = null;
            }
        }

        public void Set_LanguageTxt()
        {
            listTxt.text = ResourceManager.instance.Get_StaticWord(145);
            detailTxt.text = ResourceManager.instance.Get_StaticWord(144);

            for (int i = 0; i < listEuiArr.Length; i++)
                listEuiArr[i].Set_LanguageTxt();

            for (int i = 0; i < detailEuiArr.Length; i++)
                detailEuiArr[i].Set_LanguageTxt();
        }
    }


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
        optionUi.Offset(this);
        stateUi.Offset(this);
        infoUi.Offset(this);
    }

    private void Offset_Btn()
    {
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
    }

    private void Offset_Txt()
    {
        SetLanguageTxt();
    }

    public void Offset_ColorComp()
    {
        mainColorCompList.Add(basePanelBtnTxt);
        subColorCompList.Add(basePanelBtnImg);

        mainColorCompList.AddRange(DevTool.Get_ChildList<Image>(baseInteractingPanelInnerParentTf));
        subColorCompList.AddRange(innerImgs);

        mainColorCompList.AddRange(optionUi.Get_MainColorComps());

        Color clr = new Color(1, 1, 1, 0.1f);
        for (int i = 0; i < innerImgArr.Length; i++)
            innerImgArr[i].color = clr;

        mainColorCompList.AddRange(innerImgArr);

        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;

        stateUi.Set_Color(mainClr, subClr);
    }

    private void Offset_PosValue()
    {
        interactBasePanelPosX = 1000f;
        interactPanelPosX = optionUi.panelRt.rect.width;
        optionUi.panelRt.gameObject.SetActive(false);

        basePanelRt.anchoredPosition = Vector2.zero;
        optionUi.panelRt.anchoredPosition = Vector2.zero;

        baseInteractingPanelCg.alpha = 0f;
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
                SetOff_Panel(optionUi.panelRt);
                break;

            case OutMainGameUIType.StatePanel:
                SetOff_Panel(stateUi.panelRt);
                break;

            case OutMainGameUIType.InfoPanel:
                SetOff_Panel(infoUi.panelRt);
                infoUi.Set_Panel(false);
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

        if (currentBtn == optionUi.backBtn)
        {
            SetOff_Panel(optionUi.panelRt);
            return true;
        }
        else if (currentBtn == optionUi.applyBtn)
        {
            Set_OptionValueApply();
            optionUi.warningTxt.gameObject.SetActive(false);
            return true;
        }
        else if (Is_Interact_OptionElement(optionUi.languagePanelEui)) return true;
        else if (Is_Interact_OptionElement(optionUi.screenModePanelEui)) return true;
        else if (Is_Interact_OptionElement(optionUi.resolutionPanelEui)) return true;
        else if (Is_Interact_OptionElement(optionUi.bgmVolumePanelEui)) return true;
        else if (Is_Interact_OptionElement(optionUi.sfxVolumePanelEui)) return true;
        else if (Is_Interact_OptionElement(optionUi.fpsPanelEui)) return true;

        return false;
    }

    private bool Is_Interact_OptionElement(LRSlidingItemEUIController lrSlidingEui)
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

    private bool Is_Interact_OptionElement(FillScrollbarEUIController scrollEui)
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

    #endregion

    #region Interact (State Panel)

    private bool Is_Interact_StatePanel()
    {
        if (currentType != OutMainGameUIType.StatePanel) return false;

        if (currentBtn == stateUi.backBtn)
        {
            SetOff_Panel(stateUi.panelRt);
            return true;
        }
        if (currentBtn == stateUi.changeTypeBtn)
        {
            stateUi.Change_Panel();
            return true;
        }

        return false;
    }

    #endregion

    #region Interact (Info Panel)

    private bool Is_Interact_InfoPanel()
    {
        if (currentType != OutMainGameUIType.InfoPanel) return false;

        if (currentBtn == infoUi.backBtn)
        {
            SetOff_Panel(infoUi.panelRt);
            infoUi.Set_Panel(false);
            return true;
        }
        if (infoUi.Is_ListBtn(currentBtn))
        {
            infoUi.SetOn_DetailWindow((InfoEUIController)currentBtn);
            return true;
        }

        return false;
    }

    #endregion

    #region Set (Option)

    private void Set_OptionValueApply()
    {
        if (!optionUi.warningTxt.gameObject.activeSelf) return;

        SoundManager.instance.Play_2D_SFX_UI("Click_Approve");

        ResourceManager.instance.Set_LanguageFont(optionUi.languagePanelEui.Get_CurrentIndex());
        GameManager.instance.Set_Screen(
            (eResolution)optionUi.resolutionPanelEui.Get_CurrentIndex(),
            (eScreenMode)optionUi.screenModePanelEui.Get_CurrentIndex());
        GameManager.instance.Set_FPS((eFPS)optionUi.fpsPanelEui.Get_CurrentIndex());
        SoundManager.instance.Set_BgmVolume(optionUi.bgmVolumePanelEui.Get_Value());
        SoundManager.instance.Set_SfxVolume(optionUi.sfxVolumePanelEui.Get_Value());

        SaveDataManager.instance.Save_OptionJsonData();
    }

    private void SetOn_OptionPanel()
    {
        if (isInteractTweening) return;

        baseInteractingPanelTxt.text = ResourceManager.instance.Get_StaticWord(20);
        SetOn_Panel(OutMainGameUIType.OptionPanel, optionUi.panelRt);

        optionUi.Set_Panel();
    }

    #endregion

    #region Set (State)

    private void SetOn_StatePanel()
    {
        if (isInteractTweening) return;

        baseInteractingPanelTxt.text = ResourceManager.instance.Get_StaticWord(102);
        SetOn_Panel(OutMainGameUIType.StatePanel, stateUi.panelRt);

        stateUi.Set_Panel(true);
        stateUi.Set_State(AllyManager.instance.allAlly);
    }

    #endregion

    #region Set (Info)

    private void SetOn_InfoPanel()
    {
        if (isInteractTweening) return;

        baseInteractingPanelTxt.text = ResourceManager.instance.Get_StaticWord(144);
        SetOn_Panel(OutMainGameUIType.InfoPanel, infoUi.panelRt);

        infoUi.Set_Panel(true);
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

        optionUi.Set_LanguageTxt();
        stateUi.Set_LanguageTxt();
        infoUi.Set_LanguageTxt();

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
