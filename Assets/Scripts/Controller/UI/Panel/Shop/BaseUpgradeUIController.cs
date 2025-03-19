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

    #region BU State

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
    [HideInInspector] public List<BUShopData<float>> AllUpgradeDataList_Float;
    [HideInInspector] public List<BUShopData<int>> AllUpgradeDataList_Int;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        DamageShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.BaseDamage, BaseUpgradeManager.Instance.BaseDamage_BUData, this);
        ROFShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.ROF, BaseUpgradeManager.Instance.BaseROF_BUData, this);
        CCShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.CC, BaseUpgradeManager.Instance.BaseCC_BUData, this);
        CDShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.CD, BaseUpgradeManager.Instance.BaseCD_BUData, this);
        MuzzleShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.MuzzleSpeed, BaseUpgradeManager.Instance.BaseMuzzleSpeed_BUData, this);
        AccuracyRateShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.AccuracyRate, BaseUpgradeManager.Instance.BaseAccuracyRate_BUData, this);
        KnockbackShop.Offset(PlayerManager.Instance.PlayerController.BaseWeapon.KnockbackPower, BaseUpgradeManager.Instance.Knockback_BUData, this);

        MaxEPShop.Offset(PlayerManager.Instance.PlayerController.MaxEP, BaseUpgradeManager.Instance.BaseMaxEP_BUData, this);
        SpawnESMultipleShop.Offset(PlayerManager.Instance.PlayerController.SpawnESMultiple, BaseUpgradeManager.Instance.BaseSpawnESMultiple_BUData, this);
        NeedEP_ForSkillMultipleShop.Offset(PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple, BaseUpgradeManager.Instance.BaseNeedEP_ForSkillMultiple_BUData, this);
        DecEnergyPointMultipleShop.Offset(PlayerManager.Instance.PlayerController.DecEnergyPointMultiple, BaseUpgradeManager.Instance.BaseDecEnergyPointMultiple_BUData, this);
        ResistShop.Offset(PlayerManager.Instance.PlayerController.TakingDmgMultiple, BaseUpgradeManager.Instance.BaseResist_BUData, this);

        WalkSpeedShop.Offset(PlayerManager.Instance.PlayerController.WalkSpeed, BaseUpgradeManager.Instance.BaseWalkSpeed_BUData, this);
        WalkSpeedWhenShotMultipleShop.Offset(PlayerManager.Instance.PlayerController.WalkSpeedWhenShotMultiple, BaseUpgradeManager.Instance.BaseWalkSpeedWhenShotMultiple_BUData, this);
        WalkAvoidChance.Offset(PlayerManager.Instance.PlayerController.AvoidChance, BaseUpgradeManager.Instance.BaseAvoidChance_BUData, this);
        DashSpeedShop.Offset(PlayerManager.Instance.PlayerController.DashController.DashSpeed, BaseUpgradeManager.Instance.BaseDashSpeed_BUData, this);
        
        
        for (int i = 0; i < DevTool.SkillAmount; i++)
        {
            SkillShopList[i].Skill_CooltimeShop.Offset(PlayerManager.Instance.PlayerController.SkillWeapon.SkillList[i].MaxCooltime, BaseUpgradeManager.Instance.Skill_BUDataList[i].Skill_Cooltime_BUData, this);
            SkillShopList[i].Skill_PowerShop.Offset(PlayerManager.Instance.PlayerController.SkillWeapon.SkillList[i].Power, BaseUpgradeManager.Instance.Skill_BUDataList[i].Skill_Power_BUData, this);
            SkillShopList[i].Skill_TierShop.Offset(PlayerManager.Instance.PlayerController.SkillWeapon.SkillList[i].Tier, BaseUpgradeManager.Instance.Skill_BUDataList[i].Skill_Tier_BUData, this);
        }

        AllUpgradeDataList_Float = new List<BUShopData<float>>()
        {
            DamageShop, ROFShop, CCShop, CDShop, MuzzleShop, AccuracyRateShop, KnockbackShop,
            MaxEPShop,SpawnESMultipleShop, NeedEP_ForSkillMultipleShop, DecEnergyPointMultipleShop, ResistShop,
            WalkSpeedShop, WalkSpeedWhenShotMultipleShop, WalkAvoidChance, DashSpeedShop,
            SkillShopList[0].Skill_CooltimeShop, SkillShopList[0].Skill_PowerShop,
            SkillShopList[1].Skill_CooltimeShop, SkillShopList[1].Skill_PowerShop
        };

        for (int i = 0; i < AllUpgradeDataList_Float.Count; i++)
        {
            MainColorCompList.Add(AllUpgradeDataList_Float[i].UpgradeEUI.SkillNameTxt);
            SubColorCompList.Add(AllUpgradeDataList_Float[i].UpgradeEUI.SkillLvTxt);
            SubColorCompList.AddRange(AllUpgradeDataList_Float[i].UpgradeEUI.ThisMIAAT.Img_List);
            SubColorCompList.AddRange(AllUpgradeDataList_Float[i].UpgradeEUI.InnerImgList);
            MainColorCompList.Add(AllUpgradeDataList_Float[i].UpgradeEUI.CostImg.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
            MainColorCompList.Add(AllUpgradeDataList_Float[i].UpgradeEUI.SimpleDescTxt);
            MainColorCompList.Add(AllUpgradeDataList_Float[i].UpgradeEUI.BuyBtn.ThisBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
        }

        AllUpgradeDataList_Int = new List<BUShopData<int>>()
        {
            SkillShopList[0].Skill_TierShop,
            SkillShopList[1].Skill_TierShop
        };

        for (int i = 0; i < AllUpgradeDataList_Int.Count; i++)
        {
            MainColorCompList.Add(AllUpgradeDataList_Int[i].UpgradeEUI.SkillNameTxt);
            SubColorCompList.Add(AllUpgradeDataList_Int[i].UpgradeEUI.SkillLvTxt);
            SubColorCompList.AddRange(AllUpgradeDataList_Int[i].UpgradeEUI.ThisMIAAT.Img_List);
            SubColorCompList.AddRange(AllUpgradeDataList_Int[i].UpgradeEUI.InnerImgList);
            MainColorCompList.Add(AllUpgradeDataList_Int[i].UpgradeEUI.CostImg.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
            MainColorCompList.Add(AllUpgradeDataList_Int[i].UpgradeEUI.SimpleDescTxt);
            MainColorCompList.Add(AllUpgradeDataList_Int[i].UpgradeEUI.BuyBtn.ThisBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
        }

        foreach (TabEUIController MET in ThisPanelTabList)
        {
            MET.Offset();
            MET.ThisTabBtn.OwnerUIController = this;
        }

        ThisDescPanel.Offset();

        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;

        PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Subscribe(_Value =>
        {
            for (int i = 0; i < DevTool.SkillAmount; i++)
            {
                MainGameUIManager.Instance.PlayerHUD_UIController.SkillList[i].Set_CostText(
                    _Value * PlayerManager.Instance.PlayerController.SkillWeapon.SkillList[i].NeedEP.Value);
            }
        });
        // BC // EC
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

        // Dur
        ThisDurEUI.Offset();

        // Desc
        if (ThisDescPanel.CurrentUpgradeGraphSpot.gameObject.TryGetComponent(out Image img))
        { MainColorCompList.Add(img); }

        MainColorCompList.Add(ThisDescPanel.CenterName);

        // Graph
        MainColorCompList.Add(ThisDescPanel.UpgradeGraphValueTxt);
        MainColorCompList.AddRange(ThisDescPanel.UpgradeGraphDetailState_TxtList);

        SubColorCompList.Add(ThisDescPanel.UpgradeGraphLVTxt);
        SubColorCompList.AddRange(ThisDescPanel.UpgradeGraphLV_TxtList);

        // Next
        MainColorCompList.Add(ThisDescPanel.NextLvTxt);
        MainColorCompList.Add(ThisDescPanel.NextStateTxt);
        MainColorCompList.Add(ThisDescPanel.UpgradeNextValueTxt);

        SubColorCompList.Add(ThisDescPanel.UpgradeNextLVTxt);

        // Comp
        MainColorCompList.Add(LabelTxt);
        LabelTxt.text = LabelName;
        SubColorCompList.Add(FrameInnerImg);
        SubColorCompList.Add(CloseBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());

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

    #region Input

    public void Try_Interact()
    {
        if (CurrentBtn == null || BaseUpgradeController.UsingShop == null)
        { return; }

        // 구매 코드 (float)
        for (int i = 0; i < AllUpgradeDataList_Float.Count; i++)
        {
            if (AllUpgradeDataList_Float[i].UpgradeEUI.BuyBtn == CurrentBtn &&
                CurrentBtn.ThisBtn.interactable)
            {
                AllUpgradeDataList_Float[i].Try_Buy();
                return;
            }
        }

        // 구매 코드 (int)
        for (int i = 0; i < AllUpgradeDataList_Int.Count; i++)
        {
            if (AllUpgradeDataList_Int[i].UpgradeEUI.BuyBtn == CurrentBtn &&
                CurrentBtn.ThisBtn.interactable)
            {
                AllUpgradeDataList_Int[i].Try_Buy();
                return;
            }
        }

        // 닫기
        if (CurrentBtn == CloseBtn)
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.SetOff_ThisPanel();
            return;
        }

        // 탭
        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            if (ThisPanelTabList[i].ThisTabBtn == CurrentBtn)
            {
                Change_ThisPanel(i);
                return;
            }
        }
    }

    #endregion

    #region Set Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        if (DOTween.IsTweening(FrameInnerImg))
        { DOTween.Complete(FrameInnerImg); }

        Sequence seq = DOTween.Sequence();
        seq.Append(FrameInnerImg.DOFade(1, 0.5f));
        seq.Append(FrameInnerImg.DOFade(0.5f, 0.5f));

        ThisDurEUI.Set_Dur(BaseUpgradeController.UsingShop.CurrentDur);
    }

    public override void SetOff_ThisPanel()
    {
        base.SetOff_ThisPanel();
        BaseUpgradeController.UsingShop = null;

    }

    #endregion

    #region Desc

    public void SetOn_Desc(TxtAmountForBuyEUIController _MTAFB)
    {
        BUState<float> baseUpgradeState_Float = DevTool.Get_ThisData(AllUpgradeDataList_Float, _MTAFB);
        if (baseUpgradeState_Float != null)
        {  ThisDescPanel.SetOn_Desc<float>(baseUpgradeState_Float); }

        BUState<int> baseUpgradeState_Int = DevTool.Get_ThisData(AllUpgradeDataList_Int, _MTAFB);
        if (baseUpgradeState_Int != null)
        { ThisDescPanel.SetOn_Desc<int>(baseUpgradeState_Int); }
    }

    public void SetOff_Desc()
    {
        ThisDescPanel.SetOff_Desc();
    }

    #endregion
}