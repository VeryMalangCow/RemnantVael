using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AllyBaseUpgradeUIController : AllyShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Base Upgrade Shop")]

    [Space(10)]
    [Header("=== Tuner Detail")]
    [SerializeField] private GameObject TunerDetailOffGO;
    [SerializeField] private GameObject TunerDetailOnGO;
    [SerializeField] private TunerEUIController DetailTunerEUI;
    [SerializeField] private RectTransform TunerDetailExtraRT;

    [Space(10)]
    [Header("-- Tuner Element Desc")]
    [SerializeField] private TunerDescEUIController Positive0_ElementDescEUI;
    [SerializeField] private TunerDescEUIController Positive1_ElementDescEUI;
    [SerializeField] private TunerDescEUIController Negative_ElementDescEUI;

    [Space(5)]
    [Header("-- Buy")]
    [SerializeField] private OwnCGBtnEUIController BuyBtnEUI;
    [SerializeField] private GameObject CanBuyArrowGO;

    [Space(5)]
    [Header("-- Goods")]
    [SerializeField] private TMP_Text ChargeBetteryTxt;
    [SerializeField] private TMP_Text ChargeBetteryUseTxt;
    [SerializeField] private TMP_Text OverriderTxt;
    [SerializeField] private TMP_Text OverriderUseTxt;

    [Space(10)]
    [Header("=== Tuner List")]
    [SerializeField] private List<TunerForBuyEUIController> AllTunerEUI;
    [SerializeField] private RectTransform PickTunerListSignRT;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text TunerDetailTxt;
    [SerializeField] private TMP_Text TunerListTxt;


    #endregion

    #region - Hide

    // Tuner
    [SerializeField] private AllyBaseUpradeTunerSet AllyTunerSet;
    [SerializeField] private AllyTunerData PickedTunerData;
    [HideInInspector] private static readonly int TunerAmount = 5;
    [HideInInspector] private static int NeedOverrider = 1;
    [HideInInspector] private int NeedChargedBettery = 0;

    // Tuner Detail
    [HideInInspector] private Vector2 TunerDetailExtraRTOpen;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_TunerSet();
        Offset_Subscribe();
        Set_LanguageTxt();
    }

    private void Offset_TunerSet()
    {
        // Data Set (Must set First)
        AllyTunerSet = new AllyBaseUpradeTunerSet();
        AllyTunerSet.Offset(TunerAmount, AllyManager.TunerTypeList, AllyManager.TunerTypePercent);

        // UI Set
        for (int i = 0; i < AllTunerEUI.Count; i++)
        {
            AllTunerEUI[i].OwnerUIController = this;
            AllTunerEUI[i].RerollBtnEUI.OwnerUIController = this;
            AllTunerEUI[i].Offset();
            Set_TunerUI(i);
        }

        // Tuner Detail
        TunerDetailExtraRTOpen = TunerDetailExtraRT.sizeDelta;
        DetailTunerEUI.Offset();

        // Buy
        BuyBtnEUI.OwnerUIController = this;
        BuyBtnEUI.Offset();
    }

    private void Offset_Subscribe()
    {
        PlayerManager.Instance.PlayerController.CurrentChargedBettery
            .Subscribe(_Value =>
            {
                Set_ChargedBetteryUI(_Value, NeedChargedBettery);
            });

        PlayerManager.Instance.PlayerController.CurrentOverrider
            .Subscribe(_Value =>
            {
                Set_OverriderUI(_Value);
            });
    }

    #endregion

    #region Set (Profile)

    protected override void Pick_AllyProfile(AllyProfileEUIController _EUI)
    {
        if (IsTweening) return;

        base.Pick_AllyProfile(_EUI);

        BuyBtnEUI.ThisCG.alpha = Can_Buy() ? 1f : 0.5f;
        CanBuyArrowGO.gameObject.SetActive(Can_Buy());
    }

    #endregion

    #region Set (Tuner Detail)

    private void Set_PickedTuner(AllyTunerData _TargetData)
    {
        PickedTunerData = _TargetData;

        bool onOff = PickedTunerData != null;

        TunerDetailOffGO.SetActive(!onOff);
        TunerDetailOnGO.SetActive(onOff);
        CanBuyArrowGO.gameObject.SetActive(false);

        PickTunerListSignRT.gameObject.SetActive(onOff);
        NeedChargedBettery = 0;

        if (onOff)
        {
            Set_TunerDetailUI(DetailTunerEUI, PickedTunerData);
            NeedChargedBettery = PickedTunerData.NeedPay;

            PickTunerListSignRT.SetParent(AllTunerEUI[AllyTunerSet.AllyTunerDataList.IndexOf(PickedTunerData)].transform);
            PickTunerListSignRT.anchoredPosition = Vector2.zero;
            PickTunerListSignRT.localScale = Vector2.one;
            PickTunerListSignRT.SetAsLastSibling();

            BuyBtnEUI.ThisCG.alpha = Can_Buy() ? 1f : 0.5f;
            CanBuyArrowGO.gameObject.SetActive(Can_Buy());
        }
        Set_ChargedBetteryUI(PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value, NeedChargedBettery);

        DevTool.Set_KillTween(TunerDetailExtraRT);
        TunerDetailExtraRT.DOSizeDelta(onOff ? TunerDetailExtraRTOpen : new Vector2(TunerDetailExtraRTOpen.x, 0), 0.2f);
    }

    #endregion

    #region Set (Tuner List)

    private void Set_TunerData(int _Index)
    {
        AllyTunerSet.AllyTunerDataList[_Index].Set_Data(AllyManager.TunerTypeList, AllyManager.TunerTypePercent);
    }

    private void Set_TunerUI(int _Index)
    {
        AllTunerEUI[_Index].Set_UI(AllyTunerSet.AllyTunerDataList[_Index], NeedOverrider);
    }

    private void Set_TunerDetailUI(TunerEUIController _TargetEUI, AllyTunerData _Data)
    {
        _TargetEUI.Set_UI(_Data);

        Positive0_ElementDescEUI.Set_UI(_Data.Positive0, true);
        Positive1_ElementDescEUI.Set_UI(_Data.Positive1, true);
        Negative_ElementDescEUI.Set_UI(_Data.Negative, false);
    }

    #endregion

    #region Interact

    public override bool Try_Interact()
    {
        if (base.Try_Interact()) return true;
        if (Is_Interact_CloseBtn()) return true;

        if (Try_Interact_TunerBuy()) return true;
        if (Try_Interact_TunerList()) return true;
        if (Try_Interact_Reroll()) return true;

        return false;
    }

    private bool Is_Interact_CloseBtn()
    {
        if (CurrentBtn == CloseBtn)
        {
            MainGameUIManager.Instance.AllyBaseUpgrade_UIController.SetOff_ThisPanel();
            return true;
        }
        return false;
    }
    #endregion

    #region Interact (Tuner List)

    private bool Try_Interact_TunerList()
    {
        if (CurrentBtn == null ||
            CurrentBtn is not TunerForBuyEUIController) return false;

        for (int i = 0; i < AllTunerEUI.Count; i++)
        {
            AllyTunerData data = AllyTunerSet.AllyTunerDataList[i];
            if (AllTunerEUI[i] == CurrentBtn &&
                PickedTunerData != data)
            {
                Set_PickedTuner(data);
                return true;
            }
        }

        return false;
    }

    private bool Try_Interact_Reroll()
    {
        if (CurrentBtn == null ||
            CurrentBtn is not TunerRerollBtnEUIController) return false;

        for (int i = 0; i < AllTunerEUI.Count; i++)
        {
            if (AllTunerEUI[i].RerollBtnEUI == CurrentBtn &&
                PlayerManager.Instance.PlayerController.CurrentOverrider.Value >= NeedOverrider)
            {
                Set_TunerData(i);
                Set_TunerUI(i);
                PlayerManager.Instance.PlayerController.Add_CurrentOverrider(-NeedOverrider);

                Set_PickedTuner(PickedTunerData);

                Play_RerollTuner(i);
                Play_UseTxt(OverriderUseTxt, NeedOverrider, 30f);

                return true;
            }
        }

        return false;
    }

    private void Play_RerollTuner(int _Index)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(AllTunerEUI[_Index].Play_Scale(0.95f));
        seq.Append(AllTunerEUI[_Index].Play_Scale(1f));

        Sequence seq2 = DOTween.Sequence();
        seq2.Append(AllTunerEUI[_Index].Play_ScaleElements(0.5f));
        seq2.Append(AllTunerEUI[_Index].Play_ScaleElements(1f));
    }

    #endregion

    #region Interact (Tuner Detail)

    private bool Try_Interact_TunerBuy()
    {
        if (CurrentBtn == BuyBtnEUI)
        {
            if (Can_Buy())
            {
                Buy();
                Set_PickedTuner(null);
            }

            return true;
        }

        return false;
    }

    #endregion

    #region Buy

    private bool Can_Buy()
    {
        return (PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value >= NeedChargedBettery) &&
            CurrentPickedProfileEUI != null;
    }
    
    private void Buy()
    {
        // 데이터
        PlayerManager.Instance.PlayerController.CurrentChargedBettery.Value -= NeedChargedBettery;
        CurrentPickedAlly.Add_Tuner(PickedTunerData);

        // 소비 효과
        Play_UseTxt(ChargeBetteryUseTxt, NeedChargedBettery, 30f);

        // Extra 창에 State UI
        Set_AllyState(CurrentPickedAlly);

        // Extra 창에 Tuner UI
        Set_AllyTuner(CurrentPickedAlly);

        // 구매한 튜너를 바꿈
        int index = AllyTunerSet.AllyTunerDataList.IndexOf(PickedTunerData);
        Set_TunerData(index);
        Set_TunerUI(index);

    }

    #endregion

    #region Goods

    private void Set_ChargedBetteryUI(int _Amount, int _NeedAmount = 0)
    {
        if (_NeedAmount == 0)
        {
            ChargeBetteryTxt.text = _Amount.ToString();
        }
        else
        {
            ChargeBetteryTxt.text = $"{_Amount} <color=#933C8E>- {_NeedAmount}</color>";
        }
    }

    private void Set_OverriderUI(int _Amount)
    {
        OverriderTxt.text = _Amount.ToString();
    }


    private void Play_UseTxt(TMP_Text _Txt, int _Pay, float _UpY, float _DurTime = 0.5f)
    {
        RectTransform rt = DevTool.Get_ComponentTType<RectTransform>(_Txt.gameObject);

        DevTool.Set_KillTween(_Txt);
        DevTool.Set_KillTween(rt);

        _Txt.text = $"-{_Pay}";
        DevTool.Set_AlphaColor(_Txt, 1f);
        rt.anchoredPosition = Vector2.zero;

        _Txt.DOFade(0f, _DurTime);
        rt.DOAnchorPosY(_UpY, _DurTime);
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Label
        LabelName = ResourceManager.Instance.Get_StaticWord(95) + " " + ResourceManager.Instance.Get_StaticWord(26) + " " + ResourceManager.Instance.Get_StaticWord(2);
        LabelTxt.text = LabelName;

        // Tuner
        TunerDetailTxt.text = ResourceManager.Instance.Get_StaticWord(103);
        TunerListTxt.text = ResourceManager.Instance.Get_StaticWord(104);

        for (int i = 0; i < AllTunerEUI.Count; i++)
            AllTunerEUI[i].Set_Language();

        BuyBtnEUI.ThisTxt.text = ResourceManager.Instance.Get_StaticWord(47) + " & " + ResourceManager.Instance.Get_StaticWord(105);

        // Desc
        Positive0_ElementDescEUI.IncreaseTxt.text = ResourceManager.Instance.Get_StaticWord(108);
        Positive1_ElementDescEUI.IncreaseTxt.text = ResourceManager.Instance.Get_StaticWord(108);
        Negative_ElementDescEUI.IncreaseTxt.text = ResourceManager.Instance.Get_StaticWord(109);

        base.Set_LanguageTxt();
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        // Dur
        ThisDurEUI.Set_Dur(AllyBaseUpgradeController.UsingShop.CurrentDur);

        // Picked Tuner Detail
        Set_PickedTuner(null);
    }

    #endregion
}
