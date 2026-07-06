using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class AllyBaseUpgradeUIController : AllyShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Base Upgrade Shop")]

    [Space(10)]
    [Header("=== Tuner Detail")]
    [SerializeField] private GameObject tunerDetailOffGo;
    [SerializeField] private GameObject tunerDetailOnGo;
    [SerializeField] private TunerDetailEUIController detailTunerEuiPrefab;
    [SerializeField] private Transform detailTunerEuiParentTf;
    private TunerDetailEUIController detailTunerEui;

    [Space(5)]
    [Header("-- Buy")]
    [SerializeField] private OwnCGBtnEUIController buyBtnEui;
    [SerializeField] private GameObject canBuyArrowGo;
    [SerializeField] private RectTransform tunerDetailExtraRt;

    [Space(5)]
    [Header("-- Goods")]
    [SerializeField] private TMP_Text chargeBetteryTxt;
    [SerializeField] private TMP_Text chargeBetteryUseTxt;
    [SerializeField] private TMP_Text overriderTxt;
    [SerializeField] private TMP_Text overriderUseTxt;

    [Space(10)]
    [Header("=== Tuner List")]
    [SerializeField] private Transform tunerBuyEuiParentTf;
    [Space(5)]
    [SerializeField] private TunerForBuyEUIController tunerBuyEuiPrefab;
    [SerializeField] private float tunerBuyIntervalY;
    private List<TunerForBuyEUIController> allTunerEui;
    [Space(5)]
    [SerializeField] private TunerRerollBtnEUIController tunerRerollBtnPrefab;
    [SerializeField] private float tunerRerollBtnIntervalX;
    [Space(5)]
    [SerializeField] private RectTransform pickTunerListSignRt;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text tunerDetailTxt;
    [SerializeField] private TMP_Text tunerListTxt;


    #endregion

    #region - Hide

    // Tuner
    [SerializeField] private AllyBaseUpradeTunerSet allyTunerSet;
    [SerializeField] private AllyTunerData pickedTunerData;
    [HideInInspector] private static readonly int tunerAmount = 5;
    [HideInInspector] private static int needOverrider = 1;
    [HideInInspector] private int needChargedBettery = 0;

    // Tuner Detail
    [HideInInspector] private Vector2 tunerDetailExtraRTOpen;

    #endregion

    #endregion

    #region Init

    public override IEnumerator InitAsync(Color mainClr, Color subClr)
    {
        yield return base.InitAsync(mainClr, subClr);

#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        detailTunerEui = Instantiate(detailTunerEuiPrefab, detailTunerEuiParentTf);
        detailTunerEuiPrefab = null; 
        tunerDetailExtraRTOpen = tunerDetailExtraRt.sizeDelta;
        detailTunerEui.Offset();
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>TunerPanel Gen</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        // Data Set (Must set First)
        allyTunerSet = new AllyBaseUpradeTunerSet();
        allyTunerSet.Offset(tunerAmount, AllyManager.stateTypeList, AllyManager.tunerTypePercent);

        // UI Set
        int tunerBuyEuiAmount = 5;
        allTunerEui = new List<TunerForBuyEUIController>(5);
        for (int i = 0; i < tunerBuyEuiAmount; i++)
        {
            TunerForBuyEUIController tunerBuyEui = Instantiate(tunerBuyEuiPrefab, tunerBuyEuiParentTf);
            TunerRerollBtnEUIController tunerRerollBtn = Instantiate(tunerRerollBtnPrefab, tunerBuyEuiParentTf);

            tunerBuyEui.Offset();
            tunerBuyEui.Init(this, tunerRerollBtn, new Vector2(0, tunerBuyIntervalY * (i - 2)), tunerRerollBtnIntervalX);

            allTunerEui.Add(tunerBuyEui);

            Set_TunerUI(i);
        }
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>TunerBuy Panel Gen</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;


#if UNITY_EDITOR
        sw.Restart();
#endif
        // Buy
        buyBtnEui.ownerUIController = this;
        buyBtnEui.Offset();

        SetLanguageTxt();

#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>DataSet</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
    }

    #endregion

    public void SetChargedBetteryUI(int value)
    {
        Set_ChargedBetteryUI(value, needChargedBettery);
    }

    public void SetOverriderUI(int value)
    {
        Set_OverriderUI(value);
    }

    #region Set (Profile)

    protected override void Pick_AllyProfile(AllyProfileEUIController eui)
    {
        if (isTweening) return;

        base.Pick_AllyProfile(eui);

        buyBtnEui.cg.alpha = Can_Buy() ? 1f : 0.5f;
        canBuyArrowGo.gameObject.SetActive(Can_Buy());
    }

    #endregion

    #region Set (Tuner Detail)

    private void Set_PickedTuner(AllyTunerData targetData)
    {
        pickedTunerData = targetData;

        bool onOff = pickedTunerData != null;

        tunerDetailOffGo.SetActive(!onOff);
        tunerDetailOnGo.SetActive(onOff);
        canBuyArrowGo.gameObject.SetActive(false);

        pickTunerListSignRt.gameObject.SetActive(onOff);
        needChargedBettery = 0;

        if (onOff)
        {
            detailTunerEui.Set_TunerDetailUI(pickedTunerData);
            needChargedBettery = pickedTunerData.needPay;

            pickTunerListSignRt.SetParent(allTunerEui[allyTunerSet.allyTunerDataList.IndexOf(pickedTunerData)].transform);
            pickTunerListSignRt.anchoredPosition = Vector2.zero;
            pickTunerListSignRt.localScale = Vector2.one;
            pickTunerListSignRt.SetAsLastSibling();

            buyBtnEui.cg.alpha = Can_Buy() ? 1f : 0.5f;
            canBuyArrowGo.gameObject.SetActive(Can_Buy());

            // 사운드
            SoundManager.instance.PlayUiSfx("Click01");
        }

        Set_ChargedBetteryUI(PlayerManager.instance.playerController.chargedBettery, needChargedBettery);

        if (!AllyBaseUpgradeController.usingShop.isBroken)
        {
            DevTool.SetKillTween(tunerDetailExtraRt);
            tunerDetailExtraRt.DOSizeDelta(onOff ? tunerDetailExtraRTOpen : new Vector2(tunerDetailExtraRTOpen.x, 0), 0.2f);
        }
    }

    #endregion

    #region Set (Tuner List)

    private void Set_TunerData(int index)
    {
        allyTunerSet.allyTunerDataList[index].Set_Data(AllyManager.stateTypeList, AllyManager.tunerTypePercent);
    }

    private void Set_TunerUI(int index)
    {
        allTunerEui[index].Set_UI(allyTunerSet.allyTunerDataList[index], needOverrider);
    }

    #endregion

    #region Interact

    public override bool Try_Interact()
    {
        if (Is_Interact_Msg()) return true;

        if (currentBtn == null || AllyBaseUpgradeController.usingShop == null) return true;

        if (base.Try_Interact()) return true;
        if (Is_Interact_CloseBtn()) return true;

        if (Try_Interact_TunerBuy()) return true;
        if (Try_Interact_TunerList()) return true;
        if (Try_Interact_Reroll()) return true;

        return false;
    }

    #endregion

    #region Interact (Tuner List)

    private bool Try_Interact_TunerList()
    {
        if (currentBtn == null ||
            currentBtn is not TunerForBuyEUIController) return false;

        for (int i = 0; i < allTunerEui.Count; i++)
        {
            AllyTunerData data = allyTunerSet.allyTunerDataList[i];
            if (allTunerEui[i] == currentBtn &&
                pickedTunerData != data)
            {
                Set_PickedTuner(data);
                return true;
            }
        }

        return false;
    }

    private bool Try_Interact_Reroll()
    {
        if (currentBtn == null ||
            currentBtn is not TunerRerollBtnEUIController) return false;

        for (int i = 0; i < allTunerEui.Count; i++)
        {
            if (allTunerEui[i].rerollBtnEUI == currentBtn &&
                PlayerManager.instance.playerController.overrider >= needOverrider)
            {
                Set_TunerData(i);
                Set_TunerUI(i);
                PlayerManager.instance.playerController.UseOverrider(needOverrider);

                Set_PickedTuner(pickedTunerData);

                Play_RerollTuner(i);
                Play_UseTxt(overriderUseTxt, needOverrider, 30f);

                // 사운드
                SoundManager.instance.PlayUiSfx("Reroll");

                return true;
            }
        }

        return false;
    }

    private void Play_RerollTuner(int index)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(allTunerEui[index].Play_Scale(0.95f));
        seq.Append(allTunerEui[index].Play_Scale(1f));

        Sequence seq2 = DOTween.Sequence();
        seq2.Append(allTunerEui[index].Play_ScaleElements(0.5f));
        seq2.Append(allTunerEui[index].Play_ScaleElements(1f));
    }

    #endregion

    #region Interact (Tuner Detail)

    private bool Try_Interact_TunerBuy()
    {
        if (currentBtn == buyBtnEui)
        {
            if (Can_Buy())
            {
                Buy();
                Set_PickedTuner(null);


                AllyBaseUpgradeController.usingShop.Take_Damage(spawnItem: false, soundOn: false);
            }

            return true;
        }

        return false;
    }

    #endregion

    #region Buy

    private bool Can_Buy()
        => !AllyBaseUpgradeController.usingShop.isBroken &&
            (PlayerManager.instance.playerController.chargedBettery >= needChargedBettery) &&
            profileListEui.currentPickedProfileEui != null;
    
    
    private void Buy()
    {
        // 데이터
        PlayerManager.instance.playerController.UseChargedBettery(needChargedBettery);
        AllyController ally = profileListEui.currentPickedAlly;

        ally.Add_Tuner(pickedTunerData);

        // 소비 효과
        Play_UseTxt(chargeBetteryUseTxt, needChargedBettery, 30f);

        // Extra 창에 State UI
        profileDetailEui.Set_AllyState(ally);
        profileDetailEui.Set_AllyTuner(ally);

        // 구매한 튜너를 바꿈
        int index = allyTunerSet.allyTunerDataList.IndexOf(pickedTunerData);
        Set_TunerData(index);
        Set_TunerUI(index);

        // 사운드
        SoundManager.instance.PlayUiSfx("Approve");
    }

    #endregion

    #region Goods

    private void Set_ChargedBetteryUI(int amount, int needAmount = 0)
    {
        if (needAmount == 0)
        {
            chargeBetteryTxt.text = amount.ToString();
        }
        else
        {
            chargeBetteryTxt.text = $"{amount} <color=#933C8E>- {needAmount}</color>";
        }
    }

    private void Set_OverriderUI(int amount)
    {
        overriderTxt.text = amount.ToString();
    }

    #endregion

    #region Set (Language)

    public override void SetLanguageTxt()
    {
        // Label
        labelName = ResourceManager.instance.Get_StaticWord(95) + " " + ResourceManager.instance.Get_StaticWord(26) + " " + ResourceManager.instance.Get_StaticWord(2);
        labelTxt.text = labelName;

        // Tuner
        tunerDetailTxt.text = ResourceManager.instance.Get_StaticWord(103);
        tunerListTxt.text = ResourceManager.instance.Get_StaticWord(104);

        for (int i = 0; i < allTunerEui.Count; i++)
            allTunerEui[i].Set_Language();

        // Buy Btn
        buyBtnEui.txt.text = ResourceManager.instance.Get_StaticWord(47) + " & " + ResourceManager.instance.Get_StaticWord(105);

        // Desc
        detailTunerEui.SetLanguageTxt();

        base.SetLanguageTxt();
    }

    #endregion

    #region Set (Panel)

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        // Dur
        durEui.Set_Dur(AllyBaseUpgradeController.usingShop.currentDur);

        // Picked Tuner Detail
        Set_PickedTuner(null);
    }

    public override void SetOff_ThisPanel()
    {
        if (Is_Interact_Msg()) return;

        base.SetOff_ThisPanel();

        AllyBaseUpgradeController.usingShop = null;
    }

    #endregion
}
