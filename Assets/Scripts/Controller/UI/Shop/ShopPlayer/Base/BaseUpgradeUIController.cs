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
    [SerializeField] public TMP_Text bcTxt;
    [SerializeField] public TMP_Text ecTxt;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] protected DescBUEUIController descPanel;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] public Image frameInnerImg;

    #region - BU State

    [Space(10)]
    [Header("=== BU Data")]

    [Space(5)]
    [Header("-- Attack")]
    [SerializeField] private BUShopData<float> dmgShop;
    [SerializeField] private BUShopData<float> rofShop;
    [SerializeField] private BUShopData<float> ccShop;
    [SerializeField] private BUShopData<float> cdShop;
    [SerializeField] private BUShopData<float> muzzleShop;
    [SerializeField] private BUShopData<float> accuracyRateShop;
    [SerializeField] private BUShopData<float> knockbackShop;

    [Space(5)]
    [Header("-- EP")]
    [SerializeField] private BUShopData<float> maxEpShop;
    [SerializeField] private BUShopData<float> spawnEsMultipleShop;
    [SerializeField] private BUShopData<float> needEp_ForSkillMultipleShop;
    //[SerializeField] private BUShopData<float> DecEnergyPointMultipleShop;
    [SerializeField] private BUShopData<float> resistShop;

    [Space(5)]
    [Header("-- Movement")]
    [SerializeField] private BUShopData<float> walkSpeedShop;
    [SerializeField] private BUShopData<float> walkSpeedWhenShotMultipleShop;
    [SerializeField] private BUShopData<float> dashSpeedShop;
    [SerializeField] private BUShopData<float> walkAvoidChance;

    [Space(5)]
    [Header("-- Skill")]
    [SerializeField] private List<BUShopSkillData<float, int>> skillShopList;

    #endregion

    #endregion

    #region - Hide

    // BU Stata Data -> List
    [HideInInspector] public List<BUShopData<float>> allBuData_Float = new List<BUShopData<float>>();
    [HideInInspector] public List<BUShopData<int>> allBuData_Int = new List<BUShopData<int>>();

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

        SetLanguageTxt();
    }

    private void Offset_Basic()
    {
        // Desc
        descPanel.Offset();

    }

    private void Offset_BUShop()
    {
        #region Each Offset

        PlayerController pc = PlayerManager.instance.playerController;
        PlayerWeaponController pwc = pc.baseWeapon;
        SkillWeaponController pswc = pc.skillWeapon;
        BaseUpgradeManager bm = BaseUpgradeManager.instance;

        Debug.Log("액션 추가?");
        //PlayerManager.instance.playerController.OnMaxEpChanged
        maxEpShop.Offset(pc.maxEP, bm.baseMaxEP_BUData, allBuData_Float, this, null);
        spawnEsMultipleShop.Offset(pc.spawnESMultiple, bm.baseSpawnESMultiple_BUData, allBuData_Float, this);
        needEp_ForSkillMultipleShop.Offset(pc.needEP_ForSkillMultiple, bm.baseNeedEP_ForSkillMultiple_BUData, allBuData_Float, this);
        resistShop.Offset(pc.takingDmgMultiple, bm.baseResist_BUData, allBuData_Float, this);

        walkSpeedShop.Offset(pc.walkSpeed, bm.baseWalkSpeed_BUData, allBuData_Float, this);
        walkSpeedWhenShotMultipleShop.Offset(pc.walkSpeedWhenShotMultiple, bm.baseWalkSpeedWhenShotMultiple_BUData, allBuData_Float, this);
        walkAvoidChance.Offset(pc.avoidChance, bm.baseAvoidChance_BUData, allBuData_Float, this);
        dashSpeedShop.Offset(pc.dash.dashSpeed, bm.baseDashSpeed_BUData, allBuData_Float, this);

        dmgShop.Offset(pwc.baseDamage, bm.baseDamage_BUData, allBuData_Float, this);
        rofShop.Offset(pwc.rof, bm.baseROF_BUData, allBuData_Float, this);
        ccShop.Offset(pwc.cc, bm.baseCC_BUData, allBuData_Float, this);
        cdShop.Offset(pwc.cd, bm.baseCD_BUData, allBuData_Float, this);
        muzzleShop.Offset(pwc.muzzleSpeed, bm.baseMuzzleSpeed_BUData, allBuData_Float, this);
        accuracyRateShop.Offset(pwc.accRate, bm.baseAccuracyRate_BUData, allBuData_Float, this);
        knockbackShop.Offset(pwc.kbPower, bm.knockback_BUData, allBuData_Float, this);

        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            skillShopList[i].skill_CooltimeShop.Offset(pswc.skillList[i].maxCooltime, bm.skill_BUDataList[i].skill_Cooltime_BUData, allBuData_Float, this);
            skillShopList[i].skill_PowerShop.Offset(pswc.skillList[i].power, bm.skill_BUDataList[i].skill_Power_BUData, allBuData_Float, this);
            skillShopList[i].skill_TierShop.Offset(pswc.skillList[i].tier, bm.skill_BUDataList[i].skill_Tier_BUData, allBuData_Int, this);
        }

        #endregion
    }

    private void Offset_Subscribe()
    {
        /*
        PlayerManager.instance.playerController.needEP_ForSkillMultiple.actualState
            .Subscribe(_Value =>
            {
                for (int i = 0; i < DevTool.skillAmount; i++)
                {
                    MainGameUIManager.instance.playerHud.skillList[i].Set_CostText(
                        _Value * PlayerManager.instance.playerController.skillWeapon.skillList[i].needEP.Value);
                }
            });
        */
    }

    public void SetEmptyBetteryUI(int value)
    {
        bcTxt.text = value.ToString();
    }

    public void SetChargedBetteryUI(int value)
    {
        ecTxt.text = value.ToString();
    }

    public void Offset_ColorComp()
    {
        mainColorCompList = new List<Component>();
        subColorCompList = new List<Component>();

        // BUShop
        for (int i = 0; i < allBuData_Float.Count; i++)
        {
            Offset_ColorComp(allBuData_Float[i]);
        }
        for (int i = 0; i < allBuData_Int.Count; i++)
        {
            Offset_ColorComp(allBuData_Int[i]);
        }

        void Offset_ColorComp<T>(BUShopData<T> _BUShop)
        {
            mainColorCompList.Add(_BUShop.upgradeEUI.skillNameTxt);
            mainColorCompList.Add(_BUShop.upgradeEUI.costImg.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());
            mainColorCompList.Add(_BUShop.upgradeEUI.descTxt);
            mainColorCompList.Add(_BUShop.upgradeEUI.buyBtn.btn.gameObject.transform.GetChild(0).GetComponent<TMP_Text>());

            subColorCompList.Add(_BUShop.upgradeEUI.skillLvTxt);
            subColorCompList.AddRange(_BUShop.upgradeEUI.imgTxtAmountEui.amountImgs);
            subColorCompList.AddRange(_BUShop.upgradeEUI.innerImgList);
        }

        // Desc
        mainColorCompList.AddRange(descPanel.Get_MainColorList());
        subColorCompList.AddRange(descPanel.Get_SubColorList());

        subColorCompList.Add(frameInnerImg);

        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;
    }


    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        msgEui.Reset_Data();
    }

    #endregion

    #region Set (Panel)

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        Play_OnTween();

        // Dur
        durEui.Set_Dur(BaseUpgradeController.usingShop.currentDur);
    }

    public override void SetOff_ThisPanel()
    {
        if (Is_Interact_Msg()) return;

        base.SetOff_ThisPanel();

        BaseUpgradeController.usingShop = null;
    }

    #endregion

    #region Input

    public void Try_Interact()
    {
        InputManager.instance.Play_MousePointerClick();

        if (Is_Interact_Msg()) return;

        if (currentBtn == null || BaseUpgradeController.usingShop == null) return;

        if (Is_Interact_Buy_Float()) return;
        if (Is_Interact_Buy_Int()) return;
        if (Is_Interact_CloseBtn()) return;
        if (Is_Interact_TabPanel()) return;
    }

    #region Buy

    private bool Is_Interact_Buy_Float()
    {
        // 구매 코드 (float)
        for (int i = 0; i < allBuData_Float.Count; i++)
        {
            if (allBuData_Float[i].upgradeEUI.buyBtn == currentBtn &&
                currentBtn.btn.interactable)
            {
                if (allBuData_Float[i].Try_Buy())
                {
                    PlayerController player = PlayerManager.instance.playerController;
                    if (allBuData_Float[i] == maxEpShop)
                    {
                        player.SetMaxEp();
                    }
                    else if (allBuData_Float[i] == accuracyRateShop)
                    {
                        player.baseWeapon.SetAccAimRound();
                    }
                }
                return true;
            }
        }
        return false;
    }

    private bool Is_Interact_Buy_Int()
    {
        // 구매 코드 (int)
        for (int i = 0; i < allBuData_Int.Count; i++)
        {
            if (allBuData_Int[i].upgradeEUI.buyBtn == currentBtn &&
                currentBtn.btn.interactable)
            {
                if (allBuData_Int[i].Try_Buy())
                {

                }
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Interact

    private bool Is_Interact_TabPanel()
    {
        for (int i = 0; i < panelTabList.Count; i++)
        {
            if (panelTabList[i].tabBtn == currentBtn)
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

    public void SetOn_Desc(TxtAmountForBuyEUIController mtafb, string name)
    {
        BUState<float> baseUpgradeState_Float = DevTool.Get_ThisData(allBuData_Float, mtafb);
        if (baseUpgradeState_Float != null)
        { descPanel.SetOn_Desc<float>(baseUpgradeState_Float, name); }

        BUState<int> baseUpgradeState_Int = DevTool.Get_ThisData(allBuData_Int, mtafb);
        if (baseUpgradeState_Int != null)
        { descPanel.SetOn_Desc<int>(baseUpgradeState_Int, name); }
    }

    public void SetOff_Desc()
    {
        descPanel.SetOff_Desc();
    }

    #endregion

    #region Tween

    private void Play_OnTween()
    {
        DevTool.Set_CompleteTween(frameInnerImg);

        Sequence seq = DOTween.Sequence();
        seq.Append(frameInnerImg.DOFade(1, 0.5f));
        seq.Append(frameInnerImg.DOFade(0.5f, 0.5f));
    }

    #endregion

    #region Set (Language)

    public override void SetLanguageTxt()
    {
        // Label
        labelName = ResourceManager.instance.Get_StaticWord(26) + " " + ResourceManager.instance.Get_StaticWord(2);
        labelTxt.text = labelName;

        // Tab
        tabBtnTxtList = new List<string>
        {
            ResourceManager.instance.Get_StaticWord(29),
            ResourceManager.instance.Get_StaticWord(30),
            ResourceManager.instance.Get_StaticWord(31),
            ResourceManager.instance.Get_SkillName(PlayerManager.instance.playerController.Get_ID(), 0),
            ResourceManager.instance.Get_SkillName(PlayerManager.instance.playerController.Get_ID(), 1)
        };

        // Shop
        maxEpShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(8), ResourceManager.instance.Get_StaticDesc(0));
        spawnEsMultipleShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(38), ResourceManager.instance.Get_StaticDesc(1));
        needEp_ForSkillMultipleShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(39), ResourceManager.instance.Get_StaticDesc(2));
        resistShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(37), ResourceManager.instance.Get_StaticDesc(4));

        walkSpeedShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(40), ResourceManager.instance.Get_StaticDesc(5));
        walkSpeedWhenShotMultipleShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(41), ResourceManager.instance.Get_StaticDesc(6));
        walkAvoidChance.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(36), ResourceManager.instance.Get_StaticDesc(7));
        dashSpeedShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(10), ResourceManager.instance.Get_StaticDesc(8));

        dmgShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(12), ResourceManager.instance.Get_StaticDesc(9));
        rofShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(13), ResourceManager.instance.Get_StaticDesc(10));
        ccShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(15), ResourceManager.instance.Get_StaticDesc(11));
        cdShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(16), ResourceManager.instance.Get_StaticDesc(12));
        muzzleShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(43), ResourceManager.instance.Get_StaticDesc(13));
        accuracyRateShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(14), ResourceManager.instance.Get_StaticDesc(14));
        knockbackShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(44), ResourceManager.instance.Get_StaticDesc(15));

        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            skillShopList[i].skill_CooltimeShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(45), ResourceManager.instance.Get_SkillDesc(PlayerManager.instance.playerController.Get_ID(), (i * (DevTool.skillAmount + 1)) + 0));
            skillShopList[i].skill_PowerShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(18), ResourceManager.instance.Get_SkillDesc(PlayerManager.instance.playerController.Get_ID(), (i * (DevTool.skillAmount + 1)) + 1));
            skillShopList[i].skill_TierShop.Set_LanguageTxt(ResourceManager.instance.Get_StaticWord(17), ResourceManager.instance.Get_SkillDesc(PlayerManager.instance.playerController.Get_ID(), (i * (DevTool.skillAmount + 1)) + 2));
        }

        // Desc
        descPanel.Set_LanguageTxt();

        base.SetLanguageTxt();
    }

    #endregion
}