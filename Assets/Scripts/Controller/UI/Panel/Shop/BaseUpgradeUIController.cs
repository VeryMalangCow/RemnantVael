using UnityEngine;
using UniRx;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class BaseUpgradeUIController : PanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Base Upgrade Shop")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] private TMP_Text LabelTxt;

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] public DurablityEUIController ThisDurEUI;

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public TMP_Text BCTxt;
    [SerializeField] public TMP_Text ECTxt;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] private DescBUEUIController ThisDescPanel;

    [Space(10)]
    [Header("=== Close")]
    [SerializeField] private OwnBtnEUIController CloseBtn;

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
    [SerializeField] private BUShopData<float> DecEnergyPointMultipleShop;
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

    // String
    [HideInInspector] public static string LabelName = "BASE UPGRADE SHOP";

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
    }

    private void Offset_Basic()
    {
        // Tab
        foreach (TabEUIController MET in ThisPanelTabList)
        {
            MET.Offset();
            MET.ThisTabBtn.OwnerUIController = this;
        }

        // Label
        LabelTxt.text = LabelName;

        // Dur
        ThisDurEUI.Offset();

        // Desc
        ThisDescPanel.Offset();

        // Close
        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;
    }

    private void Offset_BUShop()
    {
        #region Each Offset

        PlayerController pc = PlayerManager.Instance.PlayerController;
        PlayerWeaponController pwc = pc.BaseWeapon;
        SkillWeaponController pswc = pc.SkillWeapon;
        BaseUpgradeManager bm = BaseUpgradeManager.Instance;

        DamageShop.Offset(pwc.BaseDamage, bm.BaseDamage_BUData, AllBUData_Float, this);
        ROFShop.Offset(pwc.ROF, bm.BaseROF_BUData, AllBUData_Float, this);
        CCShop.Offset(pwc.CC, bm.BaseCC_BUData, AllBUData_Float, this);
        CDShop.Offset(pwc.CD, bm.BaseCD_BUData, AllBUData_Float, this);
        MuzzleShop.Offset(pwc.MuzzleSpeed, bm.BaseMuzzleSpeed_BUData, AllBUData_Float, this);
        AccuracyRateShop.Offset(pwc.AccuracyRate, bm.BaseAccuracyRate_BUData, AllBUData_Float, this);
        KnockbackShop.Offset(pwc.KnockbackPower, bm.Knockback_BUData, AllBUData_Float, this);

        MaxEPShop.Offset(pc.MaxEP, bm.BaseMaxEP_BUData, AllBUData_Float, this);
        SpawnESMultipleShop.Offset(pc.SpawnESMultiple, bm.BaseSpawnESMultiple_BUData, AllBUData_Float, this);
        NeedEP_ForSkillMultipleShop.Offset(pc.NeedEP_ForSkillMultiple, bm.BaseNeedEP_ForSkillMultiple_BUData, AllBUData_Float, this);
        DecEnergyPointMultipleShop.Offset(pc.DecEnergyPointMultiple, bm.BaseDecEnergyPointMultiple_BUData, AllBUData_Float, this);
        ResistShop.Offset(pc.TakingDmgMultiple, bm.BaseResist_BUData, AllBUData_Float, this);

        WalkSpeedShop.Offset(pc.WalkSpeed, bm.BaseWalkSpeed_BUData, AllBUData_Float, this);
        WalkSpeedWhenShotMultipleShop.Offset(pc.WalkSpeedWhenShotMultiple, bm.BaseWalkSpeedWhenShotMultiple_BUData, AllBUData_Float, this);
        WalkAvoidChance.Offset(pc.AvoidChance, bm.BaseAvoidChance_BUData, AllBUData_Float, this);
        DashSpeedShop.Offset(pc.DashController.DashSpeed, bm.BaseDashSpeed_BUData, AllBUData_Float, this);

        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            SkillShopList[i].Skill_CooltimeShop.Offset(pswc.SkillList[i].MaxCooltime, bm.Skill_BUDataList[i].Skill_Cooltime_BUData, AllBUData_Float, this);
            SkillShopList[i].Skill_PowerShop.Offset(pswc.SkillList[i].Power, bm.Skill_BUDataList[i].Skill_Power_BUData, AllBUData_Float, this);
            SkillShopList[i].Skill_TierShop.Offset(pswc.SkillList[i].Tier, bm.Skill_BUDataList[i].Skill_Tier_BUData, AllBUData_Int, this);
        }

        #endregion
    }

    private void Offset_Subscribe()
    {
        PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState
            .Subscribe(_Value =>
            {
                for (int i = 0; i < DevTool.SkillAmount; i++)
                {
                    MainGameUIManager.Instance.PlayerHUD_UIController.SkillList[i].Set_CostText(
                        _Value * PlayerManager.Instance.PlayerController.SkillWeapon.SkillList[i].NeedEP.Value);
                }
            });

        PlayerManager.Instance.PlayerController.CurrentBC
            .Subscribe(value =>
            {
                BCTxt.text = value.ToString();
            });

        PlayerManager.Instance.PlayerController.CurrentEC
            .Subscribe(value =>
            {
                ECTxt.text = value.ToString();
            });
    }

    private void Offset_ColorComp()
    {
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
            MainColorCompList.Add(_BUShop.UpgradeEUI.SkillNameTxt);
            MainColorCompList.Add(_BUShop.UpgradeEUI.CostImg.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
            MainColorCompList.Add(_BUShop.UpgradeEUI.DescTxt);
            MainColorCompList.Add(_BUShop.UpgradeEUI.BuyBtn.ThisBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());

            SubColorCompList.Add(_BUShop.UpgradeEUI.SkillLvTxt);
            SubColorCompList.AddRange(_BUShop.UpgradeEUI.ThisImgTxtAmountEUI.AmountImgs);
            SubColorCompList.AddRange(_BUShop.UpgradeEUI.InnerImgList);
        }

        // Desc
        MainColorCompList.AddRange(ThisDescPanel.Get_MainColorList());
        SubColorCompList.AddRange(ThisDescPanel.Get_SubColorList());

        // Label
        MainColorCompList.Add(LabelTxt);

        SubColorCompList.Add(FrameInnerImg);

        SubColorCompList.Add(CloseBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());

        // Tab Btn => Txt & LightImg
        MainColorCompList.AddRange(Get_AllTabBtn_Txt());
        SubColorCompList.AddRange(Get_AllTabBtn_Img());


        Color mainClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }


    #endregion

    #region Framework

    private void OnEnable()
    {
        foreach(TabEUIController MET in ThisPanelTabList)
        {
            MET.Reset_ScrollBar();
        }
    }

    #endregion

    #region Set Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        Play_OnTween();

        // Dur
        ThisDurEUI.Set_Dur(BaseUpgradeController.UsingShop.CurrentDur);
    }

    public override void SetOff_ThisPanel()
    {
        base.SetOff_ThisPanel();

        BaseUpgradeController.UsingShop = null;
    }

    #endregion

    #region Input

    public void Try_Interact()
    {
        if (CurrentBtn == null || BaseUpgradeController.UsingShop == null)
        { return; }

        if (Is_Interact_Buy_Float()) return;
        if (Is_Interact_Buy_Int()) return;
        if (Is_Interact_CloseBtn()) return;
        if (Is_Interact_TabPanel()) return;
    }


    private bool Is_Interact_Buy_Float()
    {
        // 备概 内靛 (float)
        for (int i = 0; i < AllBUData_Float.Count; i++)
        {
            if (AllBUData_Float[i].UpgradeEUI.BuyBtn == CurrentBtn &&
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
            if (AllBUData_Int[i].UpgradeEUI.BuyBtn == CurrentBtn &&
                CurrentBtn.ThisBtn.interactable)
            {
                AllBUData_Int[i].Try_Buy();
                return true;
            }
        }
        return false;
    }

    private bool Is_Interact_CloseBtn()
    {
        if (CurrentBtn == CloseBtn)
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.SetOff_ThisPanel();
            return true;
        }
        return false;
    }

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

    #region Desc

    public void SetOn_Desc(TxtAmountForBuyEUIController _MTAFB)
    {
        BUState<float> baseUpgradeState_Float = DevTool.Get_ThisData(AllBUData_Float, _MTAFB);
        if (baseUpgradeState_Float != null)
        {  ThisDescPanel.SetOn_Desc<float>(baseUpgradeState_Float); }

        BUState<int> baseUpgradeState_Int = DevTool.Get_ThisData(AllBUData_Int, _MTAFB);
        if (baseUpgradeState_Int != null)
        { ThisDescPanel.SetOn_Desc<int>(baseUpgradeState_Int); }
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
}