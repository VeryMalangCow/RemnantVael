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
    [SerializeField] private Transform InnerParentTF;
    [SerializeField] private Transform BaseInteractingPanelInnerParentTF;
    [SerializeField] private Image[] InnerImgArr;

    [Space(10)]
    [Header("=== Btn")]
    [SerializeField] private OwnBtnEUIController ResumeBtn;
    [SerializeField] private OwnBtnEUIController StateBtn;
    [SerializeField] private OwnBtnEUIController OptionBtn;
    [SerializeField] private OwnBtnEUIController InfoBtn;
    [SerializeField] private OwnBtnEUIController ReturnBtn;
    [SerializeField] private OwnBtnEUIController QuitBtn;

    [Space(10)]
    [Header("=== Img")]
    [SerializeField] private Image BasePanelBtnImg;

    [Space(20)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text BasePanelBtnTxt;
    [SerializeField] private TMP_Text BaseInteractingPanelTxt;

    [Space(10)]
    [Header("=== Base")]
    [SerializeField] private RectTransform BasePanelRT;
    [SerializeField] private CanvasGroup BaseInteractingPanelCG;

    #endregion

    #region - State

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private StateUIController StateUI;
    [Serializable] private class StateUIController
    {
        #region Value

        [SerializeField] public RectTransform PanelRT;
        [SerializeField] public OwnBtnEUIController BackBtn;
        [SerializeField] public OwnBtnEUIController ChangeTypeBtn;
        [SerializeField] public bool IsBUPanelOn = true;

        [SerializeField] private TMP_Text PlayerStateNameTxt;
        [SerializeField] private TMP_Text AllyStateNameTxt;
        [SerializeField] private Image[] InnerImgArr;

        #endregion

        #region Value - Player

        // DMG, ROF, CC, CD, MS, AR, KB
        // MaxEP, ESValue, SkillCost, Resist
        // WalkS, WalkSWhileS, DashP, AvoidC
        // Skill 00: Cooltime, Power, Tier
        // Skill 01: Cooltime, Power, Tier
        [Space(10)]
        [Header("=== Player - BU")]
        [SerializeField] private GameObject PlayerBUPanelGO;
        [SerializeField] private ScrollPanelEUIController PlayerBUScrollEUI;
        [SerializeField] private StandbyPlayerBUEUIController[] BUEUIArr;

        [Space(10)]
        [Header("=== Player - MU")]
        [SerializeField] private GameObject PlayerMUPanelGO;
        [SerializeField] private ScrollPanelEUIController PlayerMUScrollEUI;
        [SerializeField] private StandbyPlayerMUEUIController[] MUEUIArr;
        [SerializeField] private StandbyPlayerSynergyEUIController[] SynergyEUIArr;

        #endregion

        #region Value - Ally

        [Space(10)]
        [Header("=== Ally - BU")]
        [SerializeField] private ScrollPanelEUIController AllyScrollEUI;
        [SerializeField] private GameObject AllyBUEUIPrefab;
        [HideInInspector] private List<StandbyAllyBUEUIController> AllyBUEUIList = new List<StandbyAllyBUEUIController>();

        #endregion

        #region Offset

        public void Offset(OutMainGameUIController _UIController)
        {
            BackBtn.OwnerUIController = _UIController;
            BackBtn.Offset();

            ChangeTypeBtn.OwnerUIController = _UIController;
            ChangeTypeBtn.Offset();

            PlayerBUScrollEUI.Offset();
            PlayerMUScrollEUI.Offset();

            AllyScrollEUI.Offset();

            for (int i = 0; i < BUEUIArr.Length; i++)
                BUEUIArr[i].Offset();

            for (int i = 0; i < MUEUIArr.Length; i++)
                MUEUIArr[i].Offset();

            for (int i = 0; i < SynergyEUIArr.Length; i++)
                SynergyEUIArr[i].Offset();

            Set_Panel(true);
        }


        #endregion

        #region Panel

        public void Change_Panel()
        {
            SoundManager.Instance.Play_2D_SFX_UI("Click_01");
            Set_Panel(!IsBUPanelOn);
        }

        public void Set_Panel(bool _IsBUPanelOn)
        {
            IsBUPanelOn = _IsBUPanelOn;

            PlayerBUPanelGO.gameObject.SetActive(_IsBUPanelOn);
            PlayerMUPanelGO.gameObject.SetActive(!_IsBUPanelOn);

            for (int i = 0; i < AllyBUEUIList.Count; i++)
            {
                AllyBUEUIList[i].Set_Panel(_IsBUPanelOn);
            }
        }

        #endregion

        #region Set

        string Get(int _Index) => ResourceManager.Instance.Get_StaticWord(_Index);

        public void Set_Color(Color _ImgClr, Color _TxtClr)
        {
            for (int i = 0; i < InnerImgArr.Length; i++)
                InnerImgArr[i].color = _ImgClr;

            #region Player

            PlayerStateNameTxt.color = _ImgClr;

            // BU
            for (int i = 0; i < BUEUIArr.Length; i++)
                BUEUIArr[i].Set_Color(_ImgClr, _TxtClr);

            // MU
            for (int i = 0; i < MUEUIArr.Length; i++)
                MUEUIArr[i].Set_Color(_TxtClr);

            for (int i = 0; i < SynergyEUIArr.Length; i++)
                SynergyEUIArr[i].Set_Color(_ImgClr, _TxtClr);

            #endregion

            #region Ally

            AllyStateNameTxt.color = _ImgClr;

            for (int i = 0; i < AllyBUEUIList.Count; i++)
                AllyBUEUIList[i].Set_Color();

            #endregion
        }

        public void Set_LanguageTxt()
        {
            #region Player

            PlayerStateNameTxt.text = $"<size=70%><color=#808080>{Get(102)} - </color></size><b>[{Get(113)}]</b>";

            // DMG, ROF, CC, CD, MS, AR, KB
            BUEUIArr[0].Set_LanguageTxt(Get(12));
            BUEUIArr[1].Set_LanguageTxt(Get(13));
            BUEUIArr[2].Set_LanguageTxt(Get(15));
            BUEUIArr[3].Set_LanguageTxt(Get(16));
            BUEUIArr[4].Set_LanguageTxt(Get(43));
            BUEUIArr[5].Set_LanguageTxt(Get(14));
            BUEUIArr[6].Set_LanguageTxt(Get(44));

            // MaxEP, ESValue, SkillCost, Resist
            BUEUIArr[7].Set_LanguageTxt(Get(8));
            BUEUIArr[8].Set_LanguageTxt(Get(38));
            BUEUIArr[9].Set_LanguageTxt(Get(39));
            BUEUIArr[10].Set_LanguageTxt(Get(37));

            // WalkS, WalkSWhileS, DashP, AvoidC
            BUEUIArr[11].Set_LanguageTxt(Get(40));
            BUEUIArr[12].Set_LanguageTxt(Get(41));
            BUEUIArr[13].Set_LanguageTxt(Get(10));
            BUEUIArr[14].Set_LanguageTxt(Get(36));

            // Skill 00: Cooltime, Power, Tier
            BUEUIArr[15].Set_LanguageTxt(Get(45));
            BUEUIArr[16].Set_LanguageTxt(Get(18));
            BUEUIArr[17].Set_LanguageTxt(Get(17));

            // Skill 01: Cooltime, Power, Tier
            BUEUIArr[18].Set_LanguageTxt(Get(45));
            BUEUIArr[19].Set_LanguageTxt(Get(18));
            BUEUIArr[20].Set_LanguageTxt(Get(17));

            #endregion

            #region Ally

            // Ally - BU
            AllyStateNameTxt.text = $"<size=70%><color=#808080>{Get(102)} - </color><color=#FFFFFF></size><b>[{Get(95)}]</b></color>";

            #endregion
        }

        public void Set_State(List<AllyController> _AllAlly)
        {
            #region Player

            PlayerController pc = PlayerManager.Instance.playerController;
            PlayerWeaponController pwc = pc.BaseWeapon;
            SkillWeaponController swc = pc.SkillWeapon;
            PlayerDashController pdc = pc.DashController;

            #region Player BU

            // DMG, ROF, CC, CD, MS, AR, KB
            BUEUIArr[0].Set(pwc.BaseDamage.CurrentLevel.Value);
            BUEUIArr[1].Set(pwc.ROF.CurrentLevel.Value);
            BUEUIArr[2].Set(pwc.CC.CurrentLevel.Value);
            BUEUIArr[3].Set(pwc.CD.CurrentLevel.Value);
            BUEUIArr[4].Set(pwc.MuzzleSpeed.CurrentLevel.Value);
            BUEUIArr[5].Set(pwc.AccuracyRate.CurrentLevel.Value);
            BUEUIArr[6].Set(pwc.KnockbackPower.CurrentLevel.Value);
            // MaxEP, ESValue, SkillCost, Resist
            BUEUIArr[7].Set(pc.MaxEP.CurrentLevel.Value);
            BUEUIArr[8].Set(pc.SpawnESMultiple.CurrentLevel.Value);
            BUEUIArr[9].Set(pc.NeedEP_ForSkillMultiple.CurrentLevel.Value);
            BUEUIArr[10].Set(pc.TakingDmgMultiple.CurrentLevel.Value);
            // WalkS, WalkSWhileS, DashP, AvoidC
            BUEUIArr[11].Set(pc.WalkSpeed.CurrentLevel.Value);
            BUEUIArr[12].Set(pc.WalkSpeedWhenShotMultiple.CurrentLevel.Value);
            BUEUIArr[13].Set(pdc.DashSpeed.CurrentLevel.Value);
            BUEUIArr[14].Set(pc.AvoidChance.CurrentLevel.Value);
            // Skill 00: Cooltime, Power, Tier
            BUEUIArr[15].Set(swc.SkillList[0].MaxCooltime.CurrentLevel.Value);
            BUEUIArr[16].Set(swc.SkillList[0].Power.CurrentLevel.Value);
            BUEUIArr[17].Set(swc.SkillList[0].Tier.CurrentLevel.Value);
            // Skill 01: Cooltime, Power, Tier
            BUEUIArr[18].Set(swc.SkillList[1].MaxCooltime.CurrentLevel.Value);
            BUEUIArr[19].Set(swc.SkillList[1].Power.CurrentLevel.Value);
            BUEUIArr[20].Set(swc.SkillList[1].Tier.CurrentLevel.Value);

            #endregion

            #region Player MU

            // Module
            List<CopyModuleState> states = ModuleItemManager.Instance.Get_EquippedModuleState();
            for (int i = 0; i < MUEUIArr.Length; i++)
            {
                
                if (i < states.Count)
                    MUEUIArr[i].SetOn(states[i].MS);
                else
                    MUEUIArr[i].SetOff();
            }

            // Sync
            for (int i = 0; i < SynergyEUIArr.Length; i++)
                SynergyEUIArr[i].SetOff();

            int index = 0;
            Dictionary<int, int> syncData = ModuleItemManager.Instance.Get_CurrentMainChipData();
            foreach (KeyValuePair<int, int> data in syncData)
            {
                SynergyEUIArr[index].SetOn(data.Key, data.Value);
                index++;
            }

            #endregion

            #endregion

            #region Ally

            TryGen_AllyStateEUI(_AllAlly.Count);

            for (int i = 0; i < AllyBUEUIList.Count; i++)
            {
                if (_AllAlly.Count > i)
                {
                    AllyBUEUIList[i].gameObject.SetActive(true);

                    #region Ally BU

                    AllyBUEUIList[i].Set_Data(_AllAlly[i]);

                    #endregion
                }
                else
                {
                    AllyBUEUIList[i].gameObject.SetActive(false);
                }
            }

            #endregion
        }

        private void TryGen_AllyStateEUI(int _TargetAmount)
        {
            if (AllyBUEUIList.Count >= _TargetAmount) return;

            float baseX = -16;
            float baseY = -32;
            float intervalY = -180;

            int needAmount = _TargetAmount - AllyBUEUIList.Count;
            for (int i = 0; i < needAmount; i++)
            {
                Instantiate(AllyBUEUIPrefab, AllyScrollEUI.ActualMovableRT).TryGetComponent(out StandbyAllyBUEUIController eui);
                eui.Offset();
                eui.Set_Pos(new Vector2(baseX, baseY + (intervalY * AllyBUEUIList.Count)));
                AllyBUEUIList.Add(eui);
            }

            AllyScrollEUI.ActualMovableRT.sizeDelta = new Vector2(AllyScrollEUI.ActualMovableRT.sizeDelta.x,
                -((baseY * 1.5f) + (intervalY * AllyBUEUIList.Count)));
            AllyScrollEUI.Set_ScrollPanel();
        }

        #endregion
    }


    #endregion

    #region - Option

    [Space(10)]
    [Header("=== Option")]
    [SerializeField] private OptionUIController OptionUI; 
    [Serializable] public class OptionUIController
    {
        [SerializeField] public RectTransform PanelRT;
        [SerializeField] public OwnBtnEUIController BackBtn;
        [SerializeField] public OwnBtnEUIController ApplyBtn;
        [SerializeField] public TMP_Text WarningTxt;

        [Space(10)]
        [SerializeField] public LRSlidingItemEUIController LanguagePanelEUI;
        [SerializeField] public LRSlidingItemEUIController ScreenModePanelEUI;
        [SerializeField] public LRSlidingItemEUIController ResolutionPanelEUI;
        [SerializeField] public LRSlidingItemEUIController FPSPanelEUI;
        [SerializeField] public FillScrollbarEUIController BGMVolumePanelEUI;
        [SerializeField] public FillScrollbarEUIController SFXVolumePanelEUI;

        public void Offset(OutMainGameUIController _UIController)
        {
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

        public void Set_Panel()
        {
            PanelRT.gameObject.SetActive(true);
            WarningTxt.gameObject.SetActive(false);
            LanguagePanelEUI.Set_Item(GameManager.languageID);
            ScreenModePanelEUI.Set_Item((int)GameManager.screenMode);
            ResolutionPanelEUI.Set_Item((int)GameManager.resolutionMode);
            FPSPanelEUI.Set_Item((int)GameManager.fps);
            BGMVolumePanelEUI.Set_Value(SoundManager.Instance.BVolume);
            SFXVolumePanelEUI.Set_Value(SoundManager.Instance.SVolume);
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

        public List<Component> Get_MainColorComps()
        {
            List<Component> result = new List<Component>();

            result.AddRange(LanguagePanelEUI.Get_InnerMainColorList());
            result.AddRange(ScreenModePanelEUI.Get_InnerMainColorList());
            result.AddRange(ResolutionPanelEUI.Get_InnerMainColorList());
            result.AddRange(FPSPanelEUI.Get_InnerMainColorList());
            result.AddRange(BGMVolumePanelEUI.Get_InnerMainColorList());
            result.AddRange(SFXVolumePanelEUI.Get_InnerMainColorList());

            return result;            
        }
    }

    #endregion

    #region - Info

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private InfoUIController InfoUI;
    [Serializable] private class InfoUIController
    {
        [Header("=== Comp")]
        [SerializeField] public TMP_Text ListTxt;
        [SerializeField] public TMP_Text DetailTxt;

        [SerializeField] public RectTransform PanelRT;
        [SerializeField] public OwnBtnEUIController BackBtn;
        [SerializeField] private ScrollPanelEUIController ListScrollPanelEUI;

        [Header("=== Element")]
        [SerializeField] private Transform ListElementParentTF;
        [HideInInspector] private InfoEUIController[] ListEUIArr;
        
        [SerializeField] private Transform DetailElementParentTF;
        [HideInInspector] private InfoDetailEUIController[] DetailEUIArr;
        [HideInInspector] private InfoDetailEUIController CurrentDetailEUI;

        public void Offset(OutMainGameUIController _OwnerUI)
        {
            BackBtn.Offset();
            BackBtn.OwnerUIController = _OwnerUI;

            ListScrollPanelEUI.Offset();

            int amount = ListElementParentTF.childCount;
            
            ListEUIArr = new InfoEUIController[amount];
            DetailEUIArr = new InfoDetailEUIController[amount];
            for (int i = 0; i < amount; i++)
            {
                ListEUIArr[i] = ListElementParentTF.GetChild(i).TryGetComponent(out InfoEUIController infoListEUI) ? infoListEUI : null;
                ListEUIArr[i].OwnerUIController = _OwnerUI;
                ListEUIArr[i].Offset();

                DetailEUIArr[i] = DetailElementParentTF.GetChild(i).TryGetComponent(out InfoDetailEUIController infoDetailEUI) ? infoDetailEUI : null;
                DetailEUIArr[i].Offset();
            }

            Set_LanguageTxt();
            Set_List();
        }

        public void Set_List()
        {
            List<EachInfoJsonData> data = SaveDataManager.Instance.jsonData.InfoData;

            List<InfoEUIController> visibleEUIs = new List<InfoEUIController>();
            for (int i = 0; i < ListEUIArr.Length; i++)
            {
                if (data[i].CanVisible)
                {
                    ListEUIArr[i].gameObject.SetActive(true);
                    ListEUIArr[i].ThisRT.anchoredPosition = new Vector2(ListEUIArr[i].ThisRT.anchoredPosition.x, -20 + (-140 * (visibleEUIs.Count)));
                    visibleEUIs.Add(ListEUIArr[i]);
                }
                else
                {
                    ListEUIArr[i].gameObject.SetActive(false);
                }
            }

            float y = (140 * visibleEUIs.Count) + 20;
            ListScrollPanelEUI.Set_ScrollHeight(y);
        }

        public void Set_Panel(bool _OnOff)
        {
            PanelRT.gameObject.SetActive(_OnOff);

            if (!_OnOff)
            {
                SetOff_DetailWindow();
            }
        }

        public bool Is_ListBtn(OwnBtnEUIController _Btn)
        {
            return ListEUIArr.Contains((InfoEUIController)_Btn);
        }

        public void SetOn_DetailWindow(InfoEUIController _ListBtn)
        {
            SetOff_DetailWindow();

            int index = Array.IndexOf(ListEUIArr, _ListBtn);
            CurrentDetailEUI = DetailEUIArr[index];
            CurrentDetailEUI.gameObject.SetActive(true);
        }

        private void SetOff_DetailWindow()
        {
            if (CurrentDetailEUI != null)
            {
                CurrentDetailEUI.gameObject.SetActive(false);
                CurrentDetailEUI = null;
            }
        }

        public void Set_LanguageTxt()
        {
            ListTxt.text = ResourceManager.Instance.Get_StaticWord(145);
            DetailTxt.text = ResourceManager.Instance.Get_StaticWord(144);

            for (int i = 0; i < ListEUIArr.Length; i++)
                ListEUIArr[i].Set_LanguageTxt();

            for (int i = 0; i < DetailEUIArr.Length; i++)
                DetailEUIArr[i].Set_LanguageTxt();
        }
    }


    #endregion

    #region - Hide

    // Inner
    [HideInInspector] private List<Image> InnerImgs;

    // Panel
    [HideInInspector] private float InteractBasePanelPosX;
    [HideInInspector] public float InteractPanelPosX;

    [HideInInspector] private bool IsInteractTweening = false;
    [HideInInspector] private OutMainGameUIType CurrentType = OutMainGameUIType.BasePanel;


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
        OptionUI.Offset(this);
        StateUI.Offset(this);
        InfoUI.Offset(this);
    }

    private void Offset_Btn()
    {
        ResumeBtn.OwnerUIController = this;
        InfoBtn.OwnerUIController = this;
        OptionBtn.OwnerUIController = this;
        StateBtn.OwnerUIController = this;
        ReturnBtn.OwnerUIController = this;
        QuitBtn.OwnerUIController = this;

        ResumeBtn.Offset();
        StateBtn.Offset();
        OptionBtn.Offset();
        InfoBtn.Offset();
        ReturnBtn.Offset();
        QuitBtn.Offset();

        InnerImgs = DevTool.Get_ChildList<Image>(InnerParentTF);
    }

    private void Offset_Txt()
    {
        Set_LanguageTxt();
    }

    public void Offset_ColorComp()
    {
        MainColorCompList.Add(BasePanelBtnTxt);
        SubColorCompList.Add(BasePanelBtnImg);

        MainColorCompList.AddRange(DevTool.Get_ChildList<Image>(BaseInteractingPanelInnerParentTF));
        SubColorCompList.AddRange(InnerImgs);

        MainColorCompList.AddRange(OptionUI.Get_MainColorComps());

        Color clr = new Color(1, 1, 1, 0.1f);
        for (int i = 0; i < InnerImgArr.Length; i++)
            InnerImgArr[i].color = clr;

        MainColorCompList.AddRange(InnerImgArr);

        Color mainClr = PlayerManager.Instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;

        StateUI.Set_Color(mainClr, subClr);
    }

    private void Offset_PosValue()
    {
        InteractBasePanelPosX = 1000f;
        InteractPanelPosX = OptionUI.PanelRT.rect.width;
        OptionUI.PanelRT.gameObject.SetActive(false);

        BasePanelRT.anchoredPosition = Vector2.zero;
        OptionUI.PanelRT.anchoredPosition = Vector2.zero;

        BaseInteractingPanelCG.alpha = 0f;
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        SoundManager.Instance.Play_2D_SFX_UI("Click_Reject");
        PlayerManager.Instance.cameraController.Stop_SlowMotion();
        Time.timeScale = 0f;
    }

    public override void SetOff_ThisPanel()
    {
        base.SetOff_ThisPanel();

        SoundManager.Instance.Play_2D_SFX_UI("Click_Approve");
        Time.timeScale = 1f;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        InputManager.Instance.Play_MousePointerClick();

        if (Is_Interact_BasePanel()) return;
        if (Is_Interact_OptionPanel()) return;
        if (Is_Interact_StatePanel()) return;
        if (Is_Interact_InfoPanel()) return;
    }

    public void Try_InteractBack()
    {
        if (IsInteractTweening) return;

        switch (CurrentType)
        {
            case OutMainGameUIType.BasePanel:
                SetOff_ThisPanel();
                break;
            case OutMainGameUIType.OptionPanel:
                SetOff_Panel(OptionUI.PanelRT);
                break;

            case OutMainGameUIType.StatePanel:
                SetOff_Panel(StateUI.PanelRT);
                break;

            case OutMainGameUIType.InfoPanel:
                SetOff_Panel(InfoUI.PanelRT);
                InfoUI.Set_Panel(false);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Interact (Base Panel)

    private bool Is_Interact_BasePanel()
    {
        if (CurrentType != OutMainGameUIType.BasePanel) return false;

        // Base Btns
        if (CurrentBtn == ResumeBtn)
        {
            SetOff_ThisPanel();
        }
        else if (CurrentBtn == StateBtn)
        {
            SetOn_StatePanel();
        }
        else if (CurrentBtn == OptionBtn)
        {
            SetOn_OptionPanel();
        }
        else if (CurrentBtn == InfoBtn)
        {
            SetOn_InfoPanel();
        }
        else if (CurrentBtn == ReturnBtn)
        {
            SoundManager.Instance.Play_2D_SFX_UI("Click_Reject");
            EventManager.Instance.Set_Input(false);
            LoadingSceneManager.Instance.Play_LoadScene("MainGame");
        }
        else if (CurrentBtn == QuitBtn)
        {
            SoundManager.Instance.Play_2D_SFX_UI("Click_Reject");
            EventManager.Instance.Set_Input(false);
            LoadingSceneManager.Instance.Play_LoadScene("TitleLobby");
        }

        return true;
    }

    #endregion

    #region Interact (Option Panel)

    private bool Is_Interact_OptionPanel()
    {
        if (CurrentType != OutMainGameUIType.OptionPanel) return false;

        if (CurrentBtn == OptionUI.BackBtn)
        {
            SetOff_Panel(OptionUI.PanelRT);
            return true;
        }
        else if (CurrentBtn == OptionUI.ApplyBtn)
        {
            Set_OptionValueApply();
            OptionUI.WarningTxt.gameObject.SetActive(false);
            return true;
        }
        else if (Is_Interact_OptionElement(OptionUI.LanguagePanelEUI)) return true;
        else if (Is_Interact_OptionElement(OptionUI.ScreenModePanelEUI)) return true;
        else if (Is_Interact_OptionElement(OptionUI.ResolutionPanelEUI)) return true;
        else if (Is_Interact_OptionElement(OptionUI.BGMVolumePanelEUI)) return true;
        else if (Is_Interact_OptionElement(OptionUI.SFXVolumePanelEUI)) return true;
        else if (Is_Interact_OptionElement(OptionUI.FPSPanelEUI)) return true;

        return false;
    }

    private bool Is_Interact_OptionElement(LRSlidingItemEUIController _LRSlidingEUI)
    {
        if (CurrentBtn == _LRSlidingEUI.LeftBtn)
        {
            SoundManager.Instance.Play_2D_SFX_UI("Click_01");
            _LRSlidingEUI.Change_Left();
            OptionUI.WarningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (CurrentBtn == _LRSlidingEUI.RightBtn)
        {
            SoundManager.Instance.Play_2D_SFX_UI("Click_01");
            _LRSlidingEUI.Change_Right();
            OptionUI.WarningTxt.gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    private bool Is_Interact_OptionElement(FillScrollbarEUIController _ScrollEUI)
    {
        if (CurrentBtn == _ScrollEUI.LeftBtn)
        {
            SoundManager.Instance.Play_2D_SFX_UI("Click_01");
            _ScrollEUI.Dec();
            OptionUI.WarningTxt.gameObject.SetActive(true);
            return true;
        }
        else if (CurrentBtn == _ScrollEUI.RightBtn)
        {
            SoundManager.Instance.Play_2D_SFX_UI("Click_01");
            _ScrollEUI.Inc();
            OptionUI.WarningTxt.gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    #endregion

    #region Interact (State Panel)

    private bool Is_Interact_StatePanel()
    {
        if (CurrentType != OutMainGameUIType.StatePanel) return false;

        if (CurrentBtn == StateUI.BackBtn)
        {
            SetOff_Panel(StateUI.PanelRT);
            return true;
        }
        if (CurrentBtn == StateUI.ChangeTypeBtn)
        {
            StateUI.Change_Panel();
            return true;
        }

        return false;
    }

    #endregion

    #region Interact (Info Panel)

    private bool Is_Interact_InfoPanel()
    {
        if (CurrentType != OutMainGameUIType.InfoPanel) return false;

        if (CurrentBtn == InfoUI.BackBtn)
        {
            SetOff_Panel(InfoUI.PanelRT);
            InfoUI.Set_Panel(false);
            return true;
        }
        if (InfoUI.Is_ListBtn(CurrentBtn))
        {
            InfoUI.SetOn_DetailWindow((InfoEUIController)CurrentBtn);
            return true;
        }

        return false;
    }

    #endregion

    #region Set (Option)

    private void Set_OptionValueApply()
    {
        if (!OptionUI.WarningTxt.gameObject.activeSelf) return;

        SoundManager.Instance.Play_2D_SFX_UI("Click_Approve");

        ResourceManager.Instance.Set_LanguageFont(OptionUI.LanguagePanelEUI.Get_CurrentIndex());
        GameManager.Instance.Set_Screen(
            (eResolution)OptionUI.ResolutionPanelEUI.Get_CurrentIndex(),
            (eScreenMode)OptionUI.ScreenModePanelEUI.Get_CurrentIndex());
        GameManager.Instance.Set_FPS((eFPS)OptionUI.FPSPanelEUI.Get_CurrentIndex());
        SoundManager.Instance.Set_BgmVolume(OptionUI.BGMVolumePanelEUI.Get_Value());
        SoundManager.Instance.Set_SfxVolume(OptionUI.SFXVolumePanelEUI.Get_Value());

        SaveDataManager.Instance.Save_OptionJsonData();
    }

    private void SetOn_OptionPanel()
    {
        if (IsInteractTweening) return;

        BaseInteractingPanelTxt.text = ResourceManager.Instance.Get_StaticWord(20);
        SetOn_Panel(OutMainGameUIType.OptionPanel, OptionUI.PanelRT);

        OptionUI.Set_Panel();
    }

    #endregion

    #region Set (State)

    private void SetOn_StatePanel()
    {
        if (IsInteractTweening) return;

        BaseInteractingPanelTxt.text = ResourceManager.Instance.Get_StaticWord(102);
        SetOn_Panel(OutMainGameUIType.StatePanel, StateUI.PanelRT);

        StateUI.Set_Panel(true);
        StateUI.Set_State(AllyManager.Instance.AllAlly);
    }

    #endregion

    #region Set (Info)

    private void SetOn_InfoPanel()
    {
        if (IsInteractTweening) return;

        BaseInteractingPanelTxt.text = ResourceManager.Instance.Get_StaticWord(144);
        SetOn_Panel(OutMainGameUIType.InfoPanel, InfoUI.PanelRT);

        InfoUI.Set_Panel(true);
    }

    #endregion

    #region Set (Capsule)

    private void SetOn_Panel(OutMainGameUIType _Type, RectTransform _RT)
    {
        SoundManager.Instance.Play_2D_SFX_UI("Click_01");

        IsInteractTweening = true;
        CurrentType = _Type;
        _RT.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        seq.Join(BasePanelRT.DOAnchorPosX(-InteractBasePanelPosX, 0.2f));
        seq.Join(_RT.DOAnchorPosX(-InteractPanelPosX, 0.2f));
        seq.Join(BaseInteractingPanelCG.DOFade(1f, 0.2f));
        seq.OnComplete(() =>
        {
            IsInteractTweening = false;
        });
        seq.SetUpdate(true);
    }

    private void SetOff_Panel(RectTransform _RT)
    {
        SoundManager.Instance.Play_2D_SFX_UI("Click_Reject");

        if (IsInteractTweening) return;
        IsInteractTweening = true;
        CurrentType = OutMainGameUIType.BasePanel;

        Sequence seq = DOTween.Sequence();

        seq.Join(BasePanelRT.DOAnchorPosX(0f, 0.2f));
        seq.Join(_RT.DOAnchorPosX(100f, 0.2f));
        seq.Join(BaseInteractingPanelCG.DOFade(0f, 0.2f));
        seq.OnComplete(() =>
        {
            IsInteractTweening = false;
            _RT.gameObject.SetActive(false);
        });
        seq.SetUpdate(true);
    }

    #endregion

    #region Set (LanguageTxt)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        BasePanelBtnTxt.text = ResourceManager.Instance.Get_StaticWord(22);
        DevTool.Get_ComponentTType<TMP_Text>(ResumeBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(ResumeBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(19);
        DevTool.Get_ComponentTType<TMP_Text>(StateBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(StateBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(102);
        DevTool.Get_ComponentTType<TMP_Text>(OptionBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(OptionBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(20);
        DevTool.Get_ComponentTType<TMP_Text>(InfoBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(InfoBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(144);
        DevTool.Get_ComponentTType<TMP_Text>(ReturnBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(ReturnBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(143);
        DevTool.Get_ComponentTType<TMP_Text>(QuitBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(QuitBtn, 0)).gameObject).text = ResourceManager.Instance.Get_StaticWord(21);

        OptionUI.Set_LanguageTxt();
        StateUI.Set_LanguageTxt();
        InfoUI.Set_LanguageTxt();

        int langId = 0;
        switch (CurrentType)
        {
            case OutMainGameUIType.StatePanel: langId = 102; break;
            case OutMainGameUIType.OptionPanel: langId = 20; break;
            case OutMainGameUIType.InfoPanel: langId = 144; break;

            default: break;
        }
        BaseInteractingPanelTxt.text = ResourceManager.Instance.Get_StaticWord(langId);
    }

    #endregion
}
