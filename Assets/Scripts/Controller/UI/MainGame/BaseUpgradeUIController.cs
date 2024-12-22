using UnityEngine;
using UniRx;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class BaseUpgradeUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Base Upgrade Shop")]

    [Space(10)]
    [Header("=== Label")]
    [SerializeField] private TMP_Text LabelTxt;
    [SerializeField] private string LabelName;

    [Space(10)]
    [Header("=== Skill")]

    [Space(10)]
    [Header("-- Attack")]
    [SerializeField] private OneOffShopEachData<float> DamageShop;
    [SerializeField] private OneOffShopEachData<float> ROFShop;
    [SerializeField] private OneOffShopEachData<float> CCShop;
    [SerializeField] private OneOffShopEachData<float> CDShop;
    [SerializeField] private OneOffShopEachData<float> MuzzleShop;
    [SerializeField] private OneOffShopEachData<float> AccuracyRateShop;
    [SerializeField] private OneOffShopEachData<float> KnockbackShop;
    
    [Space(10)]
    [Header("-- EP")]
    [SerializeField] private OneOffShopEachData<float> MaxEPShop;
    [SerializeField] private OneOffShopEachData<float> SpawnESMultipleShop;
    [SerializeField] private OneOffShopEachData<float> NeedEP_ForSkillMultipleShop;
    [SerializeField] private OneOffShopEachData<float> DecEnergyPointMultipleShop;
    
    [Space(10)]
    [Header("-- Movement")]
    [SerializeField] private OneOffShopEachData<float> WalkSpeedShop;
    [SerializeField] private OneOffShopEachData<float> WalkSpeedWhenShotMultipleShop;
    [SerializeField] private OneOffShopEachData<float> DashSpeedShop;
    [SerializeField] private OneOffShopEachData<float> WalkAvoidChance;

    [Space(10)]
    [Header("-- Skill 0")]
    [SerializeField] private OneOffShopEachData<float> Skill0_CooltimeShop;
    [SerializeField] private OneOffShopEachData<float> Skill0_PowerShop;
    [SerializeField] private OneOffShopEachData<int> Skill0_TierShop;

    [Space(10)]
    [Header("-- Skill 1")]
    [SerializeField] private OneOffShopEachData<float> Skill1_CooltimeShop;
    [SerializeField] private OneOffShopEachData<float> Skill1_PowerShop;
    [SerializeField] private OneOffShopEachData<int> Skill1_TierShop;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] private ModifyDescPanel_ForBaseUpgrade ThisDescPanel;

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

    [HideInInspector] public List<OneOffShopEachData<float>> AllUpgradeDataList_Float;
    [HideInInspector] public List<OneOffShopEachData<int>> AllUpgradeDataList_Int;

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


        AllUpgradeDataList_Float = new List<OneOffShopEachData<float>>()
        {
            DamageShop, ROFShop, CCShop, CDShop, MuzzleShop, AccuracyRateShop, KnockbackShop,
            MaxEPShop,SpawnESMultipleShop, NeedEP_ForSkillMultipleShop, DecEnergyPointMultipleShop,
            WalkSpeedShop, WalkSpeedWhenShotMultipleShop, WalkAvoidChance, DashSpeedShop,
            Skill0_CooltimeShop, Skill0_PowerShop,
            Skill1_CooltimeShop, Skill1_PowerShop
        };
        AllUpgradeDataList_Int = new List<OneOffShopEachData<int>>()
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

        SetTabTxt(TabTxtList);
        TabTxtList.Clear(); TabTxtList = null;

        SetTabLightAlpha(0.1f, LightTabCGList);
        LightTabCGList.Clear(); LightTabCGList = null;

        Color mainClr = PlayerManager.Instance.PlayerController.GetCorrectHitted_C(eDamageType.Energy, false);
        SetColor(mainClr, MainColorCompList);

        Color subClr = PlayerManager.Instance.PlayerController.GetCorrectHitted_C(eDamageType.Energy, true);
        SetColor(subClr, SubColorCompList);

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

    private void SetTabTxt(List<TMP_Text> _TxtList)
    {
        for (int i = 0; i < _TxtList.Count; i++)
        {
            _TxtList[i].text = PlayerManager.Instance.PlayerController.TabStringList[i];
            MainColorCompList.Add(_TxtList[i]);
        }
    }

    private void SetTabLightAlpha(float _A, List<CanvasGroup> _CG)
    {
        for (int i = 0; i < _CG.Count; i++)
        {
            _CG[i].alpha = _A;
            if (_CG[i].gameObject.TryGetComponent(out Image img))
            { SubColorCompList.Add(img); }
        }

    }

    private void SetColor(Color _Clr, List<Component> _ApplyCompList)
    {
        
        for (int i = 0; i < _ApplyCompList.Count; i++)
        {
            Color clr = _Clr;
            if (_ApplyCompList[i].TryGetComponent(out TMP_Text tmp))
            {
                clr.a = tmp.color.a;
                tmp.color = clr;
            }
            else if (_ApplyCompList[i].TryGetComponent(out Image img))
            {
                clr.a = img.color.a;
                img.color = clr;
            }
        }
    }

    public override void OpenThisPanel()
    {
        base.OpenThisPanel();

        if (DOTween.IsTweening(FrameInnerImg))
        { DOTween.Complete(FrameInnerImg); }

        Sequence seq = DOTween.Sequence();
        seq.Append(FrameInnerImg.DOFade(1, 0.5f));
        seq.Append(FrameInnerImg.DOFade(0.5f, 0.5f));
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
        BaseUpgradeState<float> baseUpgradeState_Float = OneOffShopEachData<float>.GetThisData(AllUpgradeDataList_Float, _MTAFB);
        if (baseUpgradeState_Float != null)
        {  ThisDescPanel.SetDesc<float>(baseUpgradeState_Float); }

        BaseUpgradeState<int> baseUpgradeState_Int = OneOffShopEachData<int>.GetThisData(AllUpgradeDataList_Int, _MTAFB);
        if (baseUpgradeState_Int != null)
        { ThisDescPanel.SetDesc<int>(baseUpgradeState_Int); }
    }

    #endregion
}

[System.Serializable]
public class OneOffShopEachData<T>
{
    [SerializeField] public ModifyTextAmountForBuy Upgrade_MTAFB;
    [SerializeField] public ModifyOwnEachBtn Upgrade_BuyBtn;

    [HideInInspector] public BaseUpgradeState<T> Upgrade_BUS;
    [HideInInspector] private BU_OneTypeData<T> Upgrade_BUOTD;

    public void Offset(BaseUpgradeState<T> _Upgrade_BUS, BU_OneTypeData<T> _Upgrade_BUOTD, BaseUpgradeUIController _Owner)
    {
        Upgrade_MTAFB.Offset();

        Upgrade_BUS = _Upgrade_BUS;
        Upgrade_BUOTD = _Upgrade_BUOTD;

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
        Debug.Log(needEC + " / " + hadEC);
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


    public static BaseUpgradeState<T> GetThisData(List<OneOffShopEachData<T>> _ShopDataList, ModifyTextAmountForBuy _InMTAFB)
    {
        foreach (OneOffShopEachData<T> Data in _ShopDataList)
        {
            if (Data.Upgrade_MTAFB == _InMTAFB)
            {
                return Data.Upgrade_BUS;
            }
        }
        return null;
    }
}
