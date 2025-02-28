using UnityEngine;
using UniRx;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class BaseUpgradeUIController : PanelUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Base Upgrade Shop")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] private TMP_Text LabelTxt;
    [SerializeField] private string LabelName;

    [Space(10)]
    [Header("=== BC, EC")]
    [SerializeField] public TMP_Text BCTxt;
    [SerializeField] public TMP_Text ECTxt;

    [Space(10)]
    [Header("=== Tab Element")]

    [Space(10)]
    [Header("-- Attack")]
    [SerializeField] private BUShopEachData<float> DamageShop;
    [SerializeField] private BUShopEachData<float> ROFShop;
    [SerializeField] private BUShopEachData<float> CCShop;
    [SerializeField] private BUShopEachData<float> CDShop;
    [SerializeField] private BUShopEachData<float> MuzzleShop;
    [SerializeField] private BUShopEachData<float> AccuracyRateShop;
    [SerializeField] private BUShopEachData<float> KnockbackShop;
    
    [Space(10)]
    [Header("-- EP")]
    [SerializeField] private BUShopEachData<float> MaxEPShop;
    [SerializeField] private BUShopEachData<float> SpawnESMultipleShop;
    [SerializeField] private BUShopEachData<float> NeedEP_ForSkillMultipleShop;
    [SerializeField] private BUShopEachData<float> DecEnergyPointMultipleShop;
    [SerializeField] private BUShopEachData<float> ResistShop;

    [Space(10)]
    [Header("-- Movement")]
    [SerializeField] private BUShopEachData<float> WalkSpeedShop;
    [SerializeField] private BUShopEachData<float> WalkSpeedWhenShotMultipleShop;
    [SerializeField] private BUShopEachData<float> DashSpeedShop;
    [SerializeField] private BUShopEachData<float> WalkAvoidChance;

    [Space(10)]
    [Header("-- Skill 0")]
    [SerializeField] private BUShopEachData<float> Skill0_CooltimeShop;
    [SerializeField] private BUShopEachData<float> Skill0_PowerShop;
    [SerializeField] private BUShopEachData<int> Skill0_TierShop;

    [Space(10)]
    [Header("-- Skill 1")]
    [SerializeField] private BUShopEachData<float> Skill1_CooltimeShop;
    [SerializeField] private BUShopEachData<float> Skill1_PowerShop;
    [SerializeField] private BUShopEachData<int> Skill1_TierShop;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] private ModifyDescPanel_ForBaseUpgrade ThisDescPanel;

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] private TMP_Text DurablityTxt;
    [SerializeField] private TMP_Text DurablityStateTxt;
    [SerializeField] private string DurablityStringTxt;
    [SerializeField] private Transform FillImgListParentTF;
    [HideInInspector] private List<Image> FillImgList;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private ModifyOwnEachBtn CloseBtn;
    [SerializeField] public Image FrameInnerImg;

    [Header("-- MainColor")]
    [SerializeField] public List<TMP_Text> TabTxtList;
    [HideInInspector] public List<Component> MainColorCompList;
    [Header("-- SubColor")]
    [SerializeField] public List<CanvasGroup> LightTabCGList;
    [HideInInspector] public List<Component> SubColorCompList;

    [HideInInspector] public List<BUShopEachData<float>> AllUpgradeDataList_Float;
    [HideInInspector] public List<BUShopEachData<int>> AllUpgradeDataList_Int;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
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

        Skill0_CooltimeShop.Offset(PlayerManager.Instance.PlayerController.SkillWeapon.Skill_0.MaxCooltime, BaseUpgradeManager.Instance.Skill0_Cooltime_BUData, this);
        Skill0_PowerShop.Offset(PlayerManager.Instance.PlayerController.SkillWeapon.Skill_0.Power, BaseUpgradeManager.Instance.Skill0_Power_BUData, this);
        Skill0_TierShop.Offset(PlayerManager.Instance.PlayerController.SkillWeapon.Skill_0.Tier, BaseUpgradeManager.Instance.Skill0_Tier_BUData, this);

        Skill1_CooltimeShop.Offset(PlayerManager.Instance.PlayerController.SkillWeapon.Skill_1.MaxCooltime, BaseUpgradeManager.Instance.Skill1_Cooltime_BUData, this);
        Skill1_PowerShop.Offset(PlayerManager.Instance.PlayerController.SkillWeapon.Skill_1.Power, BaseUpgradeManager.Instance.Skill1_Power_BUData, this);
        Skill1_TierShop.Offset(PlayerManager.Instance.PlayerController.SkillWeapon.Skill_1.Tier, BaseUpgradeManager.Instance.Skill1_Tier_BUData, this);


        AllUpgradeDataList_Float = new List<BUShopEachData<float>>()
        {
            DamageShop, ROFShop, CCShop, CDShop, MuzzleShop, AccuracyRateShop, KnockbackShop,
            MaxEPShop,SpawnESMultipleShop, NeedEP_ForSkillMultipleShop, DecEnergyPointMultipleShop, ResistShop,
            WalkSpeedShop, WalkSpeedWhenShotMultipleShop, WalkAvoidChance, DashSpeedShop,
            Skill0_CooltimeShop, Skill0_PowerShop,
            Skill1_CooltimeShop, Skill1_PowerShop
        };
        AllUpgradeDataList_Int = new List<BUShopEachData<int>>()
        {
            Skill0_TierShop,
            Skill1_TierShop
        };


        foreach (ModifyEachTab MET in ThisPanelTabList)
        {
            MET.Offset();
            MET.ThisTabBtn.OwnerUIController = this;
        }

        ThisDescPanel.Offset();

        CloseBtn.Offset();
        CloseBtn.OwnerUIController = this;

        PlayerManager.Instance.PlayerController.NeedEP_ForSkillMultiple.ActualState.Subscribe(_Value =>
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.Skill0.SetCostText(
                _Value * PlayerManager.Instance.PlayerController.SkillWeapon.Skill_0.NeedEP.Value);
            MainGameUIManager.Instance.PlayerHUD_UIController.Skill1.SetCostText(
                _Value * PlayerManager.Instance.PlayerController.SkillWeapon.Skill_1.NeedEP.Value);
        });

        
    }

    protected override void Offset_UI()
    {
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
        DurablityTxt.text = DurablityStringTxt + " :";

        FillImgList = new List<Image>();
        for (int i = 0; i < FillImgListParentTF.childCount; i++)
        {
            FillImgListParentTF.GetChild(i).gameObject.transform.GetChild(0).gameObject.TryGetComponent(out Image EmptyImg);
            FillImgList.Add(EmptyImg);
        }

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

        SetTabTxt(TabTxtList, MainColorCompList);
        TabTxtList.Clear(); TabTxtList = null;

        SetTabLightAlpha(0.1f, LightTabCGList, SubColorCompList);
        LightTabCGList.Clear(); LightTabCGList = null;

        Color mainClr = PlayerManager.Instance.PlayerController.GetCorrectHitted_C(eDamageType.Energy, false);
        SetColor(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.GetCorrectHitted_C(eDamageType.Energy, true);
        SetColor(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        foreach(ModifyEachTab MET in ThisPanelTabList)
        {
            MET.OnReset();
        }
    }

    #endregion

    #region Input

    public void TryInteractClick()
    {
        if (CurrentBtn == null || BaseUpgradeController.UsingShop == null)
        { return; }

        // 구매 코드 (float)
        for (int i = 0; i < AllUpgradeDataList_Float.Count; i++)
        {
            if (AllUpgradeDataList_Float[i].Upgrade_BuyBtn == CurrentBtn &&
                CurrentBtn.ThisBtn.interactable)
            {
                AllUpgradeDataList_Float[i].TryBuy();
                return;
            }
        }

        // 구매 코드 (int)
        for (int i = 0; i < AllUpgradeDataList_Int.Count; i++)
        {
            if (AllUpgradeDataList_Int[i].Upgrade_BuyBtn == CurrentBtn &&
                CurrentBtn.ThisBtn.interactable)
            {
                AllUpgradeDataList_Int[i].TryBuy();
                return;
            }
        }

        // 닫기
        if (CurrentBtn == CloseBtn)
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.CloseThisPanel();
            return;
        }

        // 탭
        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            if (ThisPanelTabList[i].ThisTabBtn == CurrentBtn)
            {
                ChangeThisPanel(i);
                return;
            }
        }
    }

    #endregion

    #region Set Panel

    public override void OpenThisPanel()
    {
        base.OpenThisPanel();

        if (DOTween.IsTweening(FrameInnerImg))
        { DOTween.Complete(FrameInnerImg); }

        Sequence seq = DOTween.Sequence();
        seq.Append(FrameInnerImg.DOFade(1, 0.5f));
        seq.Append(FrameInnerImg.DOFade(0.5f, 0.5f));

        MainGameUIManager.Instance.BaseUpgrade_UIController.SetDur(
            BaseUpgradeController.UsingShop.ThisDurablity);
    }

    public override void CloseThisPanel()
    {
        base.CloseThisPanel();
        BaseUpgradeController.UsingShop = null;

    }

    #endregion

    #region Desc

    public void SetDesc(ModifyTextAmountForBuy _MTAFB)
    {
        BUState<float> baseUpgradeState_Float = BUShopEachData<float>.GetThisData(AllUpgradeDataList_Float, _MTAFB);
        if (baseUpgradeState_Float != null)
        {  ThisDescPanel.SetDesc<float>(baseUpgradeState_Float); }

        BUState<int> baseUpgradeState_Int = BUShopEachData<int>.GetThisData(AllUpgradeDataList_Int, _MTAFB);
        if (baseUpgradeState_Int != null)
        { ThisDescPanel.SetDesc<int>(baseUpgradeState_Int); }
    }

    public void SetDescOff()
    {
        ThisDescPanel.SetDescOff();
    }

    #endregion

    #region Dur

    public void SetDur(int _DurState)
    {
        base.SetDur(_DurState, FillImgList, DurablityStateTxt);
    }

    #endregion
}

[System.Serializable]
public class BUShopEachData<T>
{
    [SerializeField] public ModifyTextAmountForBuy Upgrade_MTAFB;
    [SerializeField] public ModifyOwnEachBtn Upgrade_BuyBtn;

    [HideInInspector] public BUState<T> Upgrade_BUS;
    [HideInInspector] private BULevelData<T> Upgrade_BUOTD;

    public void Offset(BUState<T> _Upgrade_BUS, BULevelData<T> _Upgrade_BUOTD, BaseUpgradeUIController _Owner)
    {
        Upgrade_MTAFB.Offset();

        Upgrade_BUS = _Upgrade_BUS;
        Upgrade_BUOTD = _Upgrade_BUOTD;

        Upgrade_BUS.BuffedState = Upgrade_BUS.ActualState.Value;

        Upgrade_MTAFB.SkillNameTxt.text = Upgrade_BUS.Name;
        Upgrade_MTAFB.SkillOpenSimpleTxt.text = Upgrade_BUS.Desc;

        if (Upgrade_BuyBtn != null)
        {
            Upgrade_BuyBtn.Offset();
            Upgrade_BuyBtn.OwnerUIController = _Owner;
        }


        Upgrade_BUS.CurrentLevel
           .Subscribe(_CurrentLevel =>
           {
               int currentLv = _CurrentLevel;
               if(currentLv < Upgrade_BUOTD.BU_EachLevelDataList.Count)
               {
                   Upgrade_MTAFB.Set(currentLv, Upgrade_BUOTD.BU_EachLevelDataList[currentLv].NeedEC_ForUpgrade);
               }
               else if (currentLv == Upgrade_BUOTD.BU_EachLevelDataList.Count)
               {
                   Upgrade_MTAFB.Set(currentLv, 0);
               }
               Upgrade_MTAFB.SetInnerAlpha((float)currentLv/(float)Upgrade_BUOTD.BU_EachLevelDataList.Count);
           });

        _Owner.MainColorCompList.Add(Upgrade_MTAFB.SkillNameTxt);
        _Owner.SubColorCompList.Add(Upgrade_MTAFB.SkillLvTxt);
        _Owner.SubColorCompList.AddRange(Upgrade_MTAFB.ThisMIAAT.Img_List);
        _Owner.SubColorCompList.AddRange(Upgrade_MTAFB.InnerImgList);
        _Owner.MainColorCompList.Add(Upgrade_MTAFB.CostImg.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
        _Owner.MainColorCompList.Add(Upgrade_MTAFB.SimpleDescTxt);
        _Owner.MainColorCompList.Add(Upgrade_BuyBtn.ThisBtn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
    }

    public void TryBuy()
    {
        int index = Upgrade_BUS.CurrentLevel.Value;
        int needEC = Upgrade_BUOTD.BU_EachLevelDataList[index].NeedEC_ForUpgrade;
        int hadEC = PlayerManager.Instance.PlayerController.CurrentEC.Value;
        if (needEC <= hadEC)
        {
            Buy(needEC, Upgrade_BUOTD.BU_EachLevelDataList.Count, Upgrade_BUOTD.BU_EachLevelDataList[index].UpgradeValue);
        }
    }

    private void Buy(int _UseEC, int _MaxUpgradeLevel, T _SetValue)
    {
        BaseUpgradeController.UsingShop.TakeDamage(false);

        Upgrade_BUS.CurrentLevel.Value++;
        Upgrade_BUS.ActualState.Value = _SetValue;
        PlayerManager.Instance.PlayerController.CurrentEC.Value -= _UseEC;
        if (_MaxUpgradeLevel <= Upgrade_BUS.CurrentLevel.Value)
        {
            Upgrade_BuyBtn.ThisBtn.interactable = false;
        }

        MainGameUIManager.Instance.BaseUpgrade_UIController.SetDesc(Upgrade_MTAFB);
    }


    public static BUState<T> GetThisData(List<BUShopEachData<T>> _ShopDataList, ModifyTextAmountForBuy _InMTAFB)
    {
        foreach (BUShopEachData<T> Data in _ShopDataList)
        {
            if (Data.Upgrade_MTAFB == _InMTAFB)
            {
                return Data.Upgrade_BUS;
            }
        }
        return null;
    }
}
