using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Diagnostics;

public class BaseUpgradeUIController : PlayerShopUIController
{

    [Space(20)]
    [Header("<><><><><> Base Upgrade Shop")]

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public TMP_Text bcTxt;
    [SerializeField] public TMP_Text ecTxt;

    [Space(10)]
    [Header("=== Element")]
    [SerializeField] private TxtAmountForBuyEUIController buyEuiPrefab;

    [Space(10)]
    [Header("=== Desc")]
    [SerializeField] private DescBUEUIController descPanelPrefab;
    protected DescBUEUIController descPanel;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] public Image frameInnerImg;

    private BUShopData<float> dmgShop = new BUShopData<float>();
    private BUShopData<float> rofShop = new BUShopData<float>();
    private BUShopData<float> ccShop = new BUShopData<float>();
    private BUShopData<float> cdShop = new BUShopData<float>();
    private BUShopData<float> muzzleShop = new BUShopData<float>();
    private BUShopData<float> accuracyRateShop = new BUShopData<float>();
    private BUShopData<float> knockbackShop = new BUShopData<float>();
    [SerializeField] private Sprite[] attackSprites;


    private BUShopData<float> maxEpShop = new BUShopData<float>();
    private BUShopData<float> spawnEsMultipleShop = new BUShopData<float>();
    private BUShopData<float> needEp_ForSkillMultipleShop = new BUShopData<float>();
    private BUShopData<float> resistShop = new BUShopData<float>();
    [SerializeField] private Sprite[] epSprites;


    private BUShopData<float> walkSpeedShop = new BUShopData<float>();
    private BUShopData<float> walkSpeedWhenShotMultipleShop = new BUShopData<float>();
    private BUShopData<float> dashSpeedShop = new BUShopData<float>();
    private BUShopData<float> walkAvoidChance = new BUShopData<float>();
    [SerializeField] private Sprite[] movementSprites;


    private List<BUShopSkillData<float, int>> skillShopList = new List<BUShopSkillData<float, int>>();
    [SerializeField] private Sprite[] skillSprites;

    // BU Stata Data -> List
    [HideInInspector] public List<BUShopData<float>> allBuData_Float = new List<BUShopData<float>>();
    [HideInInspector] public List<BUShopData<int>> allBuData_Int = new List<BUShopData<int>>();

    // Init
    public override IEnumerator InitAsync(Color mainClr, Color subClr)
    {
        yield return base.InitAsync(mainClr, subClr);

#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        descPanel = Instantiate(descPanelPrefab, transform);
        descPanel.Offset();
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Desc</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

        // BU
        PlayerController pc = PlayerManager.instance.playerController;
        PlayerWeaponController pwc = pc.baseWeapon;
        SkillWeaponController pswc = pc.skillWeapon;
        BaseUpgradeManager bm = BaseUpgradeManager.instance;

        #region Attack

#if UNITY_EDITOR
        sw.Restart();
        string s = "";
#endif
        dmgShop.Offset(pwc.baseDamage, bm.baseDamage_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[0].actualMovableRt), 0, attackSprites[0]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif  
        yield return null;
#if UNITY_EDITOR
        sw.Restart();
#endif
        rofShop.Offset(pwc.rof, bm.baseROF_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[0].actualMovableRt), 1, attackSprites[1]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif  
        yield return null;
#if UNITY_EDITOR
        sw.Restart();
#endif
        ccShop.Offset(pwc.cc, bm.baseCC_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[0].actualMovableRt), 2, attackSprites[2]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif  
        yield return null;
#if UNITY_EDITOR
        sw.Restart();
#endif
        cdShop.Offset(pwc.cd, bm.baseCD_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[0].actualMovableRt), 3, attackSprites[3]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif  
        yield return null;
#if UNITY_EDITOR
        sw.Restart();
#endif
        muzzleShop.Offset(pwc.muzzleSpeed, bm.baseMuzzleSpeed_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[0].actualMovableRt), 4, attackSprites[4]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif  
        yield return null;
#if UNITY_EDITOR
        sw.Restart();
#endif
        accuracyRateShop.Offset(pwc.accRate, bm.baseAccuracyRate_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[0].actualMovableRt), 5, attackSprites[5]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif  
        yield return null;
#if UNITY_EDITOR
        sw.Restart();
#endif
        knockbackShop.Offset(pwc.kbPower, bm.knockback_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[0].actualMovableRt), 6, attackSprites[6]);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>BU Attack</color> : <color=red>{s} {sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif        
        yield return null;

        #endregion

        #region Ep

#if UNITY_EDITOR
        sw.Restart();
        s = "";
#endif
        maxEpShop.Offset(pc.maxEP, bm.baseMaxEP_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[1].actualMovableRt), 0, epSprites[0]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif        
        yield return null;

#if UNITY_EDITOR
        sw.Restart();
#endif
        spawnEsMultipleShop.Offset(pc.spawnESMultiple, bm.baseSpawnESMultiple_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[1].actualMovableRt), 1, epSprites[1]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif        
        yield return null;

#if UNITY_EDITOR
        sw.Restart();
#endif
        needEp_ForSkillMultipleShop.Offset(pc.needEP_ForSkillMultiple, bm.baseNeedEP_ForSkillMultiple_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[1].actualMovableRt), 2, epSprites[2]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif        
        yield return null;

#if UNITY_EDITOR
        sw.Restart();
#endif
        resistShop.Offset(pc.takingDmgMultiple, bm.baseResist_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[1].actualMovableRt), 3, epSprites[3]);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>BU Ep</color> : <color=red>{s} {sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif        
        yield return null;

        #endregion

        #region Movement

#if UNITY_EDITOR
        sw.Restart();
        s = "";
#endif
        walkSpeedShop.Offset(pc.walkSpeed, bm.baseWalkSpeed_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[2].actualMovableRt), 0, movementSprites[0]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif        
        yield return null;

#if UNITY_EDITOR
        sw.Restart();
#endif
        walkSpeedWhenShotMultipleShop.Offset(pc.walkSpeedWhenShotMultiple, bm.baseWalkSpeedWhenShotMultiple_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[2].actualMovableRt), 1, movementSprites[1]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif        
        yield return null;

#if UNITY_EDITOR
        sw.Restart();
#endif
        walkAvoidChance.Offset(pc.avoidChance, bm.baseAvoidChance_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[2].actualMovableRt), 2, movementSprites[2]);
#if UNITY_EDITOR
        sw.Stop();
        s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif        
        yield return null;

#if UNITY_EDITOR
        sw.Restart();
#endif
        dashSpeedShop.Offset(pc.dash.dashSpeed, bm.baseDashSpeed_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[2].actualMovableRt), 3, movementSprites[3]);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>BU Movement</color> : <color=red>{s} {sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        #endregion

        #region Skill

        for (int i = 0; i < DevTool.skillAmount; i++)
        {
#if UNITY_EDITOR
            sw.Restart();
            s = "";
#endif
            skillShopList.Add(new BUShopSkillData<float, int>());
            skillShopList[i].skill_CooltimeShop = new BUShopData<float>();
            skillShopList[i].skill_PowerShop = new BUShopData<float>();
            skillShopList[i].skill_TierShop = new BUShopData<int>();
            skillShopList[i].skill_CooltimeShop.Offset(pswc.skillList[i].maxCooltime, bm.skill_BUDataList[i].skill_Cooltime_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[i + 3].actualMovableRt), 0, skillSprites[0]);
#if UNITY_EDITOR
            sw.Stop();
            s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif
            yield return null;

#if UNITY_EDITOR
            sw.Restart();
#endif
            skillShopList[i].skill_PowerShop.Offset(pswc.skillList[i].power, bm.skill_BUDataList[i].skill_Power_BUData, allBuData_Float, this, Instantiate(buyEuiPrefab, panelTabList[i + 3].actualMovableRt), 1, skillSprites[1]);
#if UNITY_EDITOR
            sw.Stop();
            s += $"{sw.Elapsed.TotalMilliseconds:F2} / ";
#endif
            yield return null;

#if UNITY_EDITOR
            sw.Restart();
#endif
            skillShopList[i].skill_TierShop.Offset(pswc.skillList[i].tier, bm.skill_BUDataList[i].skill_Tier_BUData, allBuData_Int, this, Instantiate(buyEuiPrefab, panelTabList[i + 3].actualMovableRt), 2, skillSprites[2]);
#if UNITY_EDITOR
            sw.Stop();
            UnityEngine.Debug.Log($"<color=yellow>BU Skill {i}</color> : <color=red>{s} {sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif        
            yield return null;
        }

        #endregion

#if UNITY_EDITOR
        sw.Restart();
#endif
        SetColor(mainClr, subClr);
        SetLanguageTxt();
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Visual</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }


    // Betteries
    public void SetEmptyBetteryUI(int value)
    {
        bcTxt.text = value.ToString();
    }

    public void SetChargedBetteryUI(int value)
    {
        ecTxt.text = value.ToString();
    }


    // Color
    public void SetColor(Color mainClr, Color subClr)
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

        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;
    }


    // Panel
    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        Play_OnTween();

        durEui.Set_Dur(BaseUpgradeController.usingShop.currentDur);
    }

    public override void SetOff_ThisPanel()
    {
        if (Is_Interact_Msg()) return;

        base.SetOff_ThisPanel();

        BaseUpgradeController.usingShop = null;
    }


    // Interact
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

    private bool Is_Interact_Buy_Float()
    {
        // 备概 内靛 (float)
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
        // 备概 内靛 (int)
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


    // Desc
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


    // Tween
    private void Play_OnTween()
    {
        DevTool.Set_CompleteTween(frameInnerImg);

        Sequence seq = DOTween.Sequence();
        seq.Append(frameInnerImg.DOFade(1, 0.5f));
        seq.Append(frameInnerImg.DOFade(0.5f, 0.5f));
    }

    // Lang
    public override void SetLanguageTxt()
    {
        var words = StaticResourceManager.instance.staticWords;
        var descs = StaticResourceManager.instance.staticDescs;

        // Label
        labelName = $"{words.GetLanguage(26)} {words.GetLanguage(2)}";
        labelTxt.text = labelName;

        // Tab
        tabBtnTxtList = new List<string>
        {
            words.GetLanguage(29),
            words.GetLanguage(30),
            words.GetLanguage(31),
            ResourceManager.instance.Get_SkillName(PlayerManager.instance.playerController.Get_ID(), 0),
            ResourceManager.instance.Get_SkillName(PlayerManager.instance.playerController.Get_ID(), 1)
        };

        // Shop
        maxEpShop.Set_LanguageTxt(words.GetLanguage(8), descs.GetLanguage(0));
        spawnEsMultipleShop.Set_LanguageTxt(words.GetLanguage(38), descs.GetLanguage(1));
        needEp_ForSkillMultipleShop.Set_LanguageTxt(words.GetLanguage(39), descs.GetLanguage(2));
        resistShop.Set_LanguageTxt(words.GetLanguage(37), descs.GetLanguage(4));

        walkSpeedShop.Set_LanguageTxt(words.GetLanguage(40), descs.GetLanguage(5));
        walkSpeedWhenShotMultipleShop.Set_LanguageTxt(words.GetLanguage(41), descs.GetLanguage(6));
        walkAvoidChance.Set_LanguageTxt(words.GetLanguage(36), descs.GetLanguage(7));
        dashSpeedShop.Set_LanguageTxt(words.GetLanguage(10), descs.GetLanguage(8));

        dmgShop.Set_LanguageTxt(words.GetLanguage(12), descs.GetLanguage(9));
        rofShop.Set_LanguageTxt(words.GetLanguage(13), descs.GetLanguage(10));
        ccShop.Set_LanguageTxt(words.GetLanguage(15), descs.GetLanguage(11));
        cdShop.Set_LanguageTxt(words.GetLanguage(16), descs.GetLanguage(12));
        muzzleShop.Set_LanguageTxt(words.GetLanguage(43), descs.GetLanguage(13));
        accuracyRateShop.Set_LanguageTxt(words.GetLanguage(14), descs.GetLanguage(14));
        knockbackShop.Set_LanguageTxt(words.GetLanguage(44), descs.GetLanguage(15));

        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            skillShopList[i].skill_CooltimeShop.Set_LanguageTxt(words.GetLanguage(45), ResourceManager.instance.Get_SkillDesc(PlayerManager.instance.playerController.Get_ID(), (i * (DevTool.skillAmount + 1)) + 0));
            skillShopList[i].skill_PowerShop.Set_LanguageTxt(words.GetLanguage(18), ResourceManager.instance.Get_SkillDesc(PlayerManager.instance.playerController.Get_ID(), (i * (DevTool.skillAmount + 1)) + 1));
            skillShopList[i].skill_TierShop.Set_LanguageTxt(words.GetLanguage(17), ResourceManager.instance.Get_SkillDesc(PlayerManager.instance.playerController.Get_ID(), (i * (DevTool.skillAmount + 1)) + 2));
        }

        // Desc
        descPanel.Set_LanguageTxt();

        base.SetLanguageTxt();
    }
}