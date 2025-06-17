using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyShopUIController : ShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Shop")]

    [Space(10)]
    [Header("=== Profile List EUI")]
    [SerializeField] private Transform AllyProfileParentTF;
    [SerializeField] private RectTransform AllyProfileSelectSignRT;
    [SerializeField] private RectTransform AllyProfilePickedSignRT;

    [Space(10)]
    [Header("=== Profile Detail EUI")]

    [Space(5)]
    [Header("-- Detail")]
    [SerializeField] private AllyProfileDetailEUIController ProfileDetailEUI;
    [SerializeField] private TMP_Text ProfileDetailTxt;
    [SerializeField] private RectTransform ProfileDetailExtraRT;

    [Space(5)]
    [Header("-- Panel")]
    [SerializeField] private List<RectTransform> StatePanelRTList;
    [SerializeField] private CanvasGroup StatePanelCG;

    [Space(5)]
    [Header("-- Btn")]
    [SerializeField] private List<OwnCGBtnEUIController> StateBtnList;

    [Space(5)]
    [Header("-- In State")]
    [SerializeField] private List<TMP_Text> StateTitleTxtList;

    [Space(2)]
    [Header("* State")]
    [SerializeField] private ScrollPanelEUIController StateScrollPanel;
    [SerializeField] private TMP_Text StateLimitTxt;
    [SerializeField] private List<AllyProfileStateEUIController> StateEUIList;

    [Space(2)]
    [Header("* Tuner")]
    [SerializeField] private ScrollPanelEUIController TunerScrollPanel;
    [SerializeField] private RectTransform InStateTunerParentRT;
    [SerializeField] private GameObject InStateTunerPrefab;


    [Space(10)]
    [Header("=== Inner")]
    [SerializeField] private List<Image> ExtraMainColorImgList;
    [SerializeField] private List<TMP_Text> ExtraMainColorTxtList;

    [SerializeField] private List<Image> ExtraSubColorImgList;
    [SerializeField] private List<TMP_Text> ExtraSubColorTxtList;

    #endregion

    #region - Hide

    // Profile List
    [HideInInspector] private List<AllyProfileEUIController> AllAllyProfileEUIList;
    [HideInInspector] private List<AllyProfileEUIController> CurrentActiveProfileEUIList = new List<AllyProfileEUIController>();

    [HideInInspector] private AllyProfileEUIController CurrentSelectProfileEUI = null;
    [HideInInspector] protected AllyProfileEUIController CurrentPickedProfileEUI = null;

    [HideInInspector] private static readonly float AllyProfileIntervalY = 160;
    [HideInInspector] private static readonly float AllyProfileEachHeight = 140;
    [HideInInspector] private static readonly float AllyProfilePanelMinHeight = 800;

    // Picked
    [HideInInspector] protected AllyController CurrentPickedAlly = null;
    [HideInInspector] private CanvasGroup AllyProfilePickedSignCG;
    [HideInInspector] private int CurrentExtraPanelIndex = 0;

    // Profile Detail
    [HideInInspector] private bool ProfileDetailExtraIsOpen = false;
    [HideInInspector] protected bool IsTweening = false;
    [HideInInspector] private static readonly float ProfileDetailExtraRT_OpenHeight = 650f;
    [HideInInspector] private static readonly float ProfileDetailExtraRT_CloseHeight = 80f;
    [HideInInspector] private static readonly float ProfileDetailExtraRT_ScrollMin = 550f;

    // In State - Tuner
    [HideInInspector] private List<TunerEUIController> InStateTunerEUIList;
    [HideInInspector] private static readonly float InStateTunerEUI_BaseX = -12f;
    [HideInInspector] private static readonly float InStateTunerEUI_BaseY = 60f;
    [HideInInspector] private static readonly float InStateTunerEUI_Interval = 120f;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_Comp();
        Offset_ColorComp();
    }

    private void Offset_Comp()
    {
        // Profile List
        AllAllyProfileEUIList = new List<AllyProfileEUIController>();

        foreach (Transform TF in AllyProfileParentTF)
        {
            if (TF.TryGetComponent(out AllyProfileEUIController profile))
            {
                profile.OwnerUIController = this;
                profile.Offset();
                AllAllyProfileEUIList.Add(profile);
            }
        }

        AllyProfilePickedSignCG = DevTool.Get_ComponentTType(AllyProfilePickedSignRT.gameObject, out CanvasGroup cg) ? cg : null;


        // Profile Detail
        ProfileDetailEUI.Offset();
        
        for (int i = 0; i < StateBtnList.Count; i++)
        {
            StateBtnList[i].OwnerUIController = this;
            StateBtnList[i].Offset();
        }

        StateScrollPanel.Offset();
        TunerScrollPanel.Offset();

        // In State - Tuner
        InStateTunerEUIList = new List<TunerEUIController>();
    }

    private void Offset_ColorComp()
    {
        MainColorCompList = new List<Component>();
        SubColorCompList = new List<Component>();

        MainColorCompList.AddRange(ExtraMainColorImgList);
        MainColorCompList.AddRange(ExtraMainColorTxtList);
        SubColorCompList.AddRange(ExtraSubColorImgList);
        SubColorCompList.AddRange(ExtraSubColorTxtList);

        // Set Color
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

    #region Reset

    // 모든 데이터 초기화
    private void Reset_AllData()
    {
        CurrentActiveProfileEUIList.Clear();
        CurrentSelectProfileEUI = null;
        CurrentPickedProfileEUI = null;

        AllyProfilePickedSignRT.gameObject.SetActive(false);
        CurrentExtraPanelIndex = 0;

        ProfileDetailExtraIsOpen = false;
        IsTweening = false;
        StatePanelCG.alpha = 0; 
        
    }

    // 프로필 리스트 리셋
    private void Reset_AllyProfileListPanel()
    {
        List<AllyController> AllAlly = AllyManager.Instance.AllAllies;

        Set_AllAllyProfileListOff();
        Set_AllyProfileListOn(AllAlly);
        Set_AllyProfileListPanelY(AllAlly.Count);
        Select_DefaultAllyProfile();
    }
    
    // 프로필 디테일 리셋
    private void Reset_AllyProfileDetailPanel()
    {
        ProfileDetailEUI.SetOff_Panel();

    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        Reset_AllData();
        Reset_AllyProfileListPanel();
        Reset_AllyProfileDetailPanel();

        Play_ProfileExtraY(ProfileDetailExtraRT_CloseHeight, 0.3f);

    }

    #endregion

    #region Profile List

    // 모든 프로필 끄기
    private void Set_AllAllyProfileListOff()
    {
        for (int i = 0; i < AllAllyProfileEUIList.Count; i++)
        {
            AllAllyProfileEUIList[i].gameObject.SetActive(false);
            AllAllyProfileEUIList[i].Reset_Profile();
        }
    }

    // 프로필 키기 (현재 Ally)
    private void Set_AllyProfileListOn(List<AllyController> _AllAlly)
    {

        for (int i = 0; i < _AllAlly.Count; i++)
        {
            AllAllyProfileEUIList[i].gameObject.SetActive(true);
            AllAllyProfileEUIList[i].Set_Profile(_AllAlly[i], -(i * AllyProfileIntervalY));
            CurrentActiveProfileEUIList.Add(AllAllyProfileEUIList[i]);
        }
    }

    // 프로필 항목 패널의 사이즈 조절 (Scroll을 위함)
    private void Set_AllyProfileListPanelY(int _Amount)
    {
        float y = Mathf.Max(((_Amount - 1) * AllyProfileIntervalY) + AllyProfileEachHeight, AllyProfilePanelMinHeight);
        ThisPanelTabList[0].Set_ScrollPanel(y);
    }

    #endregion

    #region Profile Select

    // 처음 시작 시, 첫번째 Ally 선택 (없다면 NULL)
    private void Select_DefaultAllyProfile()
    {
        if (CurrentActiveProfileEUIList.Count <= 0) // 없다면
        {
            Select_AllyProfile(null);
            AllyProfileSelectSignRT.gameObject.SetActive(false);
        }
        else
        {
            Select_AllyProfile(CurrentActiveProfileEUIList[0]);
            AllyProfileSelectSignRT.gameObject.SetActive(true);
        }
    }

    public void Select_AllyProfile(AllyProfileEUIController _EUI)
    {
        if (CurrentSelectProfileEUI == _EUI) return;

        CurrentSelectProfileEUI = _EUI;

        AllyProfileSelectSignRT.SetParent(CurrentSelectProfileEUI.transform);
        AllyProfileSelectSignRT.SetAsLastSibling();

        DevTool.Set_KillTween(AllyProfileSelectSignRT);
        AllyProfileSelectSignRT.DOAnchorPos(Vector2.zero, 0.05f);
    }

    #endregion

    #region Profile Pick

    // 실제로 인풋으로 클릭이나 지정하는 것
    // 인풋을 통해, Ally를 픽 (UI 변경 필요)
    protected virtual void Pick_AllyProfile(AllyProfileEUIController _EUI)
    {
        if (IsTweening) return;

        CurrentPickedProfileEUI = _EUI;
        CurrentPickedAlly = CurrentPickedProfileEUI.Get_ThisAlly();

        AllyProfilePickedSignRT.gameObject.SetActive(true);
        AllyProfilePickedSignRT.SetParent(CurrentSelectProfileEUI.transform);
        AllyProfilePickedSignRT.SetAsLastSibling();
        AllyProfilePickedSignRT.anchoredPosition = Vector2.zero;

        DevTool.Set_KillTween(AllyProfilePickedSignRT);
        AllyProfilePickedSignRT.transform.localScale = Vector2.one * 1.2f;
        AllyProfilePickedSignRT.transform.DOScale(Vector2.one, 0.15f);

        DevTool.Set_KillTween(AllyProfilePickedSignCG);
        AllyProfilePickedSignCG.alpha = 0f;
        AllyProfilePickedSignCG.DOFade(1f, 0.2f);

        // Data UI Set
        Set_AllyState(CurrentPickedAlly);
        Set_AllyTuner(CurrentPickedAlly);

        // Detail Panel
        ProfileDetailEUI.SetOn_Panel(CurrentPickedAlly);

        // Extra Panel
        Play_ProfileExtraY_CloseAndOpen(CurrentExtraPanelIndex);
    }

    #endregion

    #region Profile Detail (Panel)

    // Extra, Y 패널
    private Sequence Play_ProfileDetailExtraCG(int _ExtraIndex, float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(StatePanelCG.DOFade(0f, _DurTime * 0.5f).OnComplete(() =>
        {
            for (int i = 0; i < StateBtnList.Count; i++)
            {
                int index = i;

                if (index == _ExtraIndex)
                    StatePanelRTList[index].gameObject.SetActive(true);
                else
                    StatePanelRTList[index].gameObject.SetActive(false);
            }
        }));
        seq.Append(StatePanelCG.DOFade(1f, _DurTime * 0.5f));

        return seq;
    }

    private Sequence Play_ProfileDetailExtraBtn(int _ExtraIndex, float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < StateBtnList.Count; i++)
        {
            int index = i;

            if (index == _ExtraIndex)
                seq.Join(StateBtnList[index].ThisCG.DOFade(1f, _DurTime));
            else
                seq.Join(StateBtnList[index].ThisCG.DOFade(0.4f, _DurTime));
        }

        return seq;
    }

    private Sequence Play_ProfileExtraY_CloseAndOpen(int _ExtraIndex)
    {
        if (IsTweening || CurrentPickedProfileEUI == null) return null;
        IsTweening = true;

        Sequence seq = DOTween.Sequence();

        CurrentExtraPanelIndex = _ExtraIndex;
        float durTime = ProfileDetailExtraIsOpen ? 0.24f : 0.12f;

        if (ProfileDetailExtraIsOpen)
        {
            seq.Append(Play_ProfileExtraY(ProfileDetailExtraRT_CloseHeight, 0.12f));
            seq.Append(Play_ProfileExtraY(ProfileDetailExtraRT_OpenHeight, 0.12f));
        }
        else
        {
            seq.Append(Play_ProfileExtraY(ProfileDetailExtraRT_OpenHeight, 0.12f));
        }

        Play_ProfileDetailExtraBtn(_ExtraIndex, durTime);
        Play_ProfileDetailExtraCG(_ExtraIndex, durTime);

        ProfileDetailExtraIsOpen = true;

        seq.OnComplete(() => { IsTweening = false; });

        return seq;
    }


    private Sequence Play_ProfileExtraY(float _Y, float _DurTime = 0f)
    {
        DevTool.Set_KillTween(ProfileDetailExtraRT);

        Sequence seq = DOTween.Sequence();

        seq.Append(ProfileDetailExtraRT.DOSizeDelta(new Vector2(ProfileDetailExtraRT.rect.width, _Y), _DurTime));

        Play_ProfileDetailExtraBtn(-1, 0.12f);

        return seq;
    }

    #endregion

    #region Profile Detail (State)

    // 실제 데이터값
    protected void Set_AllyState(AllyController _Ally)
    {
        AllyState cardBaseState = _Ally.Get_CardState();
        StateEUIList[0].ValueTxt.text = cardBaseState.Dmg.Value.ToString();
        StateEUIList[1].ValueTxt.text = cardBaseState.Rof.Value.ToString();
        StateEUIList[2].ValueTxt.text = cardBaseState.MovementSpeed.Value.ToString();

        AllyState upgradeState = _Ally.Get_UpgradeAllState();
        StateEUIList[0].ExtraValueTxt.text = upgradeState.Dmg.Value >= 0 ? 
            $"+{upgradeState.Dmg.Value.ToString()}" : upgradeState.Dmg.Value.ToString();
        StateEUIList[1].ExtraValueTxt.text = upgradeState.Rof.Value >= 0 ? 
            $"+{upgradeState.Rof.Value.ToString()}" : upgradeState.Rof.Value.ToString();
        StateEUIList[2].ExtraValueTxt.text = upgradeState.MovementSpeed.Value >= 0 ? 
            $"+{upgradeState.MovementSpeed.Value.ToString()}" : upgradeState.MovementSpeed.Value.ToString();
    }

    #endregion

    #region Profile Detail (Tuner)

    protected void Set_AllyTuner(AllyController _Ally)
    {
        List<AllyBaseTunerData> tunerData = _Ally.Get_ThisTunerData();

        // 만약 UI EUI가 부족하다면 생성
        if (InStateTunerEUIList.Count < tunerData.Count)
        {
            int needEUIAmount = tunerData.Count - InStateTunerEUIList.Count;
            for (int i = 0; i < needEUIAmount; i++)
                InStateTunerEUIList.Add(Gen_TunerEUI());
        }

        // 모든 Tuner EUI 끄기
        SetOff_AllTunerEUI();

        // 튜너에 맞추어 키기
        SetOn_TunerEUI(tunerData);

        float scrollY = Mathf.Max(
            ProfileDetailExtraRT_ScrollMin,
            InStateTunerEUI_BaseY + (tunerData.Count * InStateTunerEUI_Interval));

        TunerScrollPanel.Set_ScrollHeight(scrollY);
    }

    private TunerEUIController Gen_TunerEUI()
    {
        TunerEUIController result = DevTool.Get_ComponentTType<TunerEUIController>(Instantiate(InStateTunerPrefab, InStateTunerParentRT));
        result.Offset();

        return result;
    }

    private void SetOff_AllTunerEUI()
    {
        if (InStateTunerEUIList.Count <= 0) return;

        for (int i = 0; i < InStateTunerEUIList.Count; i++)
            InStateTunerEUIList[i].gameObject.SetActive(false);
    }

    private void SetOn_TunerEUI(List<AllyBaseTunerData> _Data)
    {
        for (int i = 0; i < _Data.Count; i++)
        {
            InStateTunerEUIList[i].gameObject.SetActive(true);
            InStateTunerEUIList[i].ThisRT.anchoredPosition = new Vector2(InStateTunerEUI_BaseX, -(InStateTunerEUI_BaseY + (InStateTunerEUI_Interval * i)));
            InStateTunerEUIList[i].Set_UI(_Data[i]);
        }

    }

    #endregion

    #region Interact

    public virtual bool Try_Interact()
    {
        if (Interact_ProfileList()) return true;
        if (Interact_ProfileExtraBtn()) return true;

        return false;
    }


    private bool Interact_ProfileList()
    {
        if ((CurrentBtn == CurrentSelectProfileEUI) &&
            (CurrentSelectProfileEUI != CurrentPickedProfileEUI))
        {
            Pick_AllyProfile(CurrentSelectProfileEUI);

            return true;
        }

        return false;
    }

    private bool Interact_ProfileExtraBtn()
    {
        if (CurrentBtn is OwnCGBtnEUIController btn &&
            StateBtnList.Contains(btn))
        {

            Play_ProfileExtraY_CloseAndOpen(StateBtnList.IndexOf(btn));

            return true;
        }

        return false;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Profile List (Tab)
        TabBtnTxtList = new List<string>
        {
            ResourceManager.Instance.Get_StaticWord(96)
        };

        // Profile Detail
        ProfileDetailTxt.text = ResourceManager.Instance.Get_StaticWord(101);

        string state = ResourceManager.Instance.Get_StaticWord(102);
        string bu = ResourceManager.Instance.Get_StaticWord(106);
        string mu = ResourceManager.Instance.Get_StaticWord(27);


        StateBtnList[0].ThisTxt.text = state;
        StateBtnList[1].ThisTxt.text = bu;
        StateBtnList[2].ThisTxt.text = mu;

        StateTitleTxtList[0].text = state;
        StateTitleTxtList[1].text = bu;
        StateTitleTxtList[2].text = mu;

        StateEUIList[0].NameTxt.text = $"< {ResourceManager.Instance.Get_StaticWord(12)} >";    // 공격력
        StateEUIList[1].NameTxt.text = $"< {ResourceManager.Instance.Get_StaticWord(13)} >";    // 연사력
        StateEUIList[2].NameTxt.text = $"< {ResourceManager.Instance.Get_StaticWord(9)} >";     // 이동속도

        // Limit
        StateLimitTxt.text = $"( {ResourceManager.Instance.Get_StaticWord(107)}: {AllyController.MinLimitUpgradeValue} )";

        base.Set_LanguageTxt();
    }

    #endregion

}
