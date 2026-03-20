using UnityEngine;
using UniRx;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class BaseUpgradeUIController : PlayerShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Base Upgrade Shop")]

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public TMP_Text BCTxt;
    [SerializeField] public TMP_Text ECTxt;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] protected DescBUEUIController ThisDescPanel;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] public Image FrameInnerImg;

    #region - BU State

    [Space(10)]
    [Header("=== BU Data")]

    [Space(5)]
    [Header("-- Attack")]
    [SerializeField] private BUShopData<float> DamageShop;
    [SerializeField] private BUShopData<float> ROFShop;
    [SerializeField] private BUShopData<float> CCShop;
    [SerializeField] private BUShopData<float> CDShop;
    [SerializeField] private BUShopData<float> MuzzleShop;
    [SerializeField] private BUShopData<float> AccuracyRateShop;
    [SerializeField] private BUShopData<float> KnockbackShop;

    [Space(5)]
    [Header("-- EP")]
    [SerializeField] private BUShopData<float> MaxEPShop;
    [SerializeField] private BUShopData<float> SpawnESMultipleShop;
    [SerializeField] private BUShopData<float> NeedEP_ForSkillMultipleShop;
    //[SerializeField] private BUShopData<float> DecEnergyPointMultipleShop;
    [SerializeField] private BUShopData<float> ResistShop;

    [Space(5)]
    [Header("-- Movement")]
    [SerializeField] private BUShopData<float> WalkSpeedShop;
    [SerializeField] private BUShopData<float> WalkSpeedWhenShotMultipleShop;
    [SerializeField] private BUShopData<float> DashSpeedShop;
    [SerializeField] private BUShopData<float> WalkAvoidChance;

    [Space(5)]
    [Header("-- Skill")]
    [SerializeField] private List<BUShopSkillData<float, int>> SkillShopList;

    #endregion

    #endregion

    #region - Hide

    // BU Stata Data -> List
    [HideInInspector] public List<BUShopData<float>> AllBUData_Float = new List<BUShopData<float>>();
    [HideInInspector] public List<BUShopData<int>> AllBUData_Int = new List<BUShopData<int>>();

    #endregion

    #endregion


    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_Basic();
        Offset_BUShop();
        Offset_Subscribe();
        Offset_ColorComp();

        Set_LanguageTxt();
    }

    private void Offset_Basic()
    {
        // Desc
        ThisDescPanel.Offset();

    }

    private void Offset_BUShop()
    {
        #region Each Offset

        PlayerController pc = PlayerManager.instance.playerController;
        PlayerWeaponController pwc = pc.BaseWeapon;
        SkillWeaponController pswc = pc.SkillWeapon;
        BaseUpgradeManager bm = BaseUpgradeManager.instance;
        
        MaxEPShop.Offset(pc.MaxEP, bm.baseMaxEP_BUData, AllBUData_Float, this);
        SpawnESMultipleShop.Offset(pc.SpawnESMultiple, bm.baseSpawnESMultiple_BUData, AllBUData_Float, this);
        NeedEP_ForSkillMultipleShop.Offset(pc.NeedEP_ForSkillMultiple, bm.baseNeedEP_ForSkillMultiple_BUData, AllBUData_Float, this);
        ResistShop.Offset(pc.TakingDmgMultiple, bm.baseResist_BUData, AllBUData_Float, this);

        WalkSpeedShop.Offset(pc.WalkSpeed, bm.baseWalkSpeed_BUData, AllBUData_Float, this);
        WalkSpeedWhenShotMultipleShop.Offset(pc.WalkSpeedWhenShotMultiple, bm.baseWalkSpeedWhenShotMultiple_BUData, AllBUData_Float, this);
        WalkAvoidChance.Offset(pc.AvoidChance, bm.baseAvoidChance_BUData, AllBUData_Float, this);
        DashSpeedShop.Offset(pc.DashController.DashSpeed, bm.baseDashSpeed_BUData, AllBUData_Float, this);

        DamageShop.Offset(pwc.baseDamage, bm.baseDamage_BUData, AllBUData_Float, this);
        ROFShop.Offset(pwc.rof, bm.baseROF_BUData, AllBUData_Float, this);
        CCShop.Offset(pwc.cc, bm.baseCC_BUData, AllBUData_Float, this);
        CDShop.Offset(pwc.cd, bm.baseCD_BUData, AllBUData_Float, this);
        MuzzleShop.Offset(pwc.muzzleSpeed, bm.baseMuzzleSpeed_BUData, AllBUData_Float, this);
        AccuracyRateShop.Offset(pwc.accRate, bm.baseAccuracyRate_BUData, AllBUData_Float, this);
        KnockbackShop.Offset(pwc.kbPower, bm.knockback_BUData, AllBUData_Float, this);

        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            SkillShopList[i].skill_CooltimeShop.Offset(pswc.skillList[i].maxCooltime, bm.skill_BUDataList[i].skill_Cooltime_BUData, AllBUData_Float, this);
            SkillShopList[i].skill_PowerShop.Offset(pswc.skillList[i].power, bm.skill_BUDataList[i].skill_Power_BUData, AllBUData_Float, this);
            SkillShopList[i].skill_TierShop.Offset(pswc.skillList[i].tier, bm.skill_BUDataList[i].skill_Tier_BUData, AllBUData_Int, this);
        }

        #endregion
    }

    private void Offset_Subscribe()
    {
        PlayerManager.instance.playerController.NeedEP_ForSkillMultiple.actualState
            .Subscribe(_Value =>
            {
                for (int i = 0; i < DevTool.skillAmount; i++)
                {
                    MainGameUIManager.instance.playerHUD_UIController.SkillList[i].Set_CostText(
                        _Value * PlayerManager.instance.playerController.SkillWeapon.skillList[i].needEP.Value);
                }
            });

        PlayerManager.instance.playerController.CurrentBettery
            .Subscribe(value =>
            {
                BCTxt.text = value.ToString();
            });

        PlayerManager.instance.playerController.CurrentChargedBettery
            .Subscribe(value =>
            {
                ECTxt.text = value.ToString();
            });
    }

    public void Offset_ColorComp()
    {
        MainColorCompList = new List<Component>();
        SubColorCompList = new List<Component>();

        // BUShop
        for (int i = 0; i < AllBUData_Float.Count; i++)
        {
            Offset_ColorComp(AllBUData_Float[i]);
        }
        for (int i = 0; i < AllBUData_Int.Count; i++)
        {
            Offset_ColorComp(AllBUData_Int[i]);
        }

        void Offset_ColorComp<T>(BUShopData<T> _BUShop)
        {
            MainColorCompList.Add(_BUShop.upgradeEUI.SkillNameTxt);
            MainColorCompList.Add(_BUShop.upgradeEUI.CostImg.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
            MainColorCompList.Add(_BUShop.upgradeEUI.DescTxt);
            MainColorCompList.Add(_BUShop.upgradeEUI.BuyBtn.ThisBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());

            SubColorCompList.Add(_BUShop.upgradeEUI.SkillLvTxt);
            SubColorCompList.AddRange(_BUShop.upgradeEUI.ThisImgTxtAmountEUI.AmountImgs);
            SubColorCompList.AddRange(_BUShop.upgradeEUI.InnerImgList);
        }

        // Desc
        MainColorCompList.AddRange(ThisDescPanel.Get_MainColorList());
        SubColorCompList.AddRange(ThisDescPanel.Get_SubColorList());

        SubColorCompList.Add(FrameInnerImg);

        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }


    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        ThisMsgEUI.Reset_Data();
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        Play_OnTween();

        // Dur
        ThisDurEUI.Set_Dur(BaseUpgradeController.UsingShop.CurrentDur);
    }

    public override void SetOff_ThisPanel()
    {
        if (Is_Interact_Msg()) return;

        base.SetOff_ThisPanel();

        BaseUpgradeController.UsingShop = null;
    }

    #endregion

    #region Input

    public void Try_Interact()
    {
        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_Msg()) return;

        if (CurrentBtn == null || BaseUpgradeController.UsingShop == null) return;

        if (Is_Interact_Buy_Float()) return;
        if (Is_Interact_Buy_Int()) return;
        if (Is_Interact_CloseBtn()) return;
        if (Is_Interact_TabPanel()) return;
    }

    #region Buy

    private bool Is_Interact_Buy_Float()
    {
        // 备概 内靛 (float)
        for (int i = 0; i < AllBUData_Float.Count; i++)
        {
            if (AllBUData_Float[i].upgradeEUI.BuyBtn == CurrentBtn &&
                CurrentBtn.ThisBtn.interactable)
            {
                AllBUData_Float[i].Try_Buy();
                return true;
            }
        }
        return false;
    }

    private bool Is_Interact_Buy_Int()
    {
        // 备概 内靛 (int)
        for (int i = 0; i < AllBUData_Int.Count; i++)
        {
            if (AllBUData_Int[i].upgradeEUI.BuyBtn == CurrentBtn &&
                CurrentBtn.ThisBtn.interactable)
            {
                AllBUData_Int[i].Try_Buy();
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Interact

    private bool Is_Interact_TabPanel()
    {
        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            if (ThisPanelTabList[i].ThisTabBtn == CurrentBtn)
            {
                Change_ThisPanel(i);
                return true;
            }
        }
        return false;
    }

    #endregion

    #endregion

    #region Desc

    public void SetOn_Desc(TxtAmountForBuyEUIController _MTAFB, string _Name)
    {
        BUState<float> baseUpgradeState_Float = DevTool.Get_ThisData(AllBUData_Float, _MTAFB);
        if (baseUpgradeState_Float != null)
        { ThisDescPanel.SetOn_Desc<float>(baseUpgradeState_Float, _Name); }

        BUState<int> baseUpgradeState_Int = DevTool.Get_ThisData(AllBUData_Int, _MTAFB);
        if (baseUpgradeState_Int != null)
        { ThisDescPanel.SetOn_Desc<int>(baseUpgradeState_Int, _Name); }
    }

    public void SetOff_Desc()
    {
        ThisDescPanel.SetOff_Desc();
    }

    #endregion

    #region Tween

    private void Play_OnTween()
    {
        DevTool.Set_CompleteTween(FrameInnerImg);

        Sequence seq = DOTween.Sequence();
        seq.Append(FrameInnerImg.DOFade(1, 0.5f));
        seq.Append(FrameInnerImg.DOFade(0.5f, 0.5f));
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Label
        LabelName = ResourceManager.instance.Get_StaticWord(26) + " " + ResourceManager.instance.Get_StaticWord(2);
        LabelTxt.text = LabelName;

        // Tab
        TabBtnTxtList = new List<string>
        {
            ResourceManager.instance.Get_StaticWord(29),
            ResourceManager.instance.Get_StaticWord(30),
            ResourceManager.instance.Get_StaticWord(31),
            ResourceManager.instance.Get_SkillName(PlayerManager.instance.playerController.Get_ID(), 0),
            ResourceManager.instance.Get_SkillName(PlayerManager.instance.playerController.Get_ID(), 1)
        };

        // Shop
        MaxEPShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(8), ResourceManager.instance.Get_StaticDesc(0));
        SpawnESMultipleShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(38), ResourceManager.instance.Get_StaticDesc(1));
        NeedEP_ForSkillMultipleShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(39), ResourceManager.instance.Get_StaticDesc(2));
        ResistShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(37), ResourceManager.instance.Get_StaticDesc(4));

        WalkSpeedShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(40), ResourceManager.instance.Get_StaticDesc(5));
        WalkSpeedWhenShotMultipleShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(41), ResourceManager.instance.Get_StaticDesc(6));
        WalkAvoidChance.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(36), ResourceManager.instance.Get_StaticDesc(7));
        DashSpeedShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(10), ResourceManager.instance.Get_StaticDesc(8));

        DamageShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(12), ResourceManager.instance.Get_StaticDesc(9));
        ROFShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(13), ResourceManager.instance.Get_StaticDesc(10));
        CCShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(15), ResourceManager.instance.Get_StaticDesc(11));
        CDShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(16), ResourceManager.instance.Get_StaticDesc(12));
        MuzzleShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(43), ResourceManager.instance.Get_StaticDesc(13));
        AccuracyRateShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(14), ResourceManager.instance.Get_StaticDesc(14));
        KnockbackShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(44), ResourceManager.instance.Get_StaticDesc(15));

        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            SkillShopList[i].skill_CooltimeShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(45), ResourceManager.instance.Get_SkillDesc(PlayerManager.instance.playerController.Get_ID(), (i * (DevTool.skillAmount + 1)) + 0));
            SkillShopList[i].skill_PowerShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(18), ResourceManager.instance.Get_SkillDesc(PlayerManager.instance.playerController.Get_ID(), (i * (DevTool.skillAmount + 1)) + 1));
            SkillShopList[i].skill_TierShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(17), ResourceManager.instance.Get_SkillDesc(PlayerManager.instance.playerController.Get_ID(), (i * (DevTool.skillAmount + 1)) + 2));
        }

        // Desc
        ThisDescPanel.Set_LanguageTxt();

        base.Set_LanguageTxt();
    }

    #endregion
}