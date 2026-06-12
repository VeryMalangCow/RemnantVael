using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Transform allyProfileParentTf;
    [SerializeField] private RectTransform allyProfileSelectSignRt;
    [SerializeField] private RectTransform allyProfilePickedSignRt;

    [Space(10)]
    [Header("=== Profile Detail EUI")]

    [Space(5)]
    [Header("-- Detail")]
    [SerializeField] private AllyProfileDetailEUIController profileDetailEui;
    [SerializeField] private TMP_Text profileDetailTxt;
    [SerializeField] private RectTransform profileDetailExtraRt;

    [Space(5)]
    [Header("-- Panel")]
    [SerializeField] private List<RectTransform> statePanelRtList;
    [SerializeField] private CanvasGroup statePanelCg;

    [Space(5)]
    [Header("-- Btn")]
    [SerializeField] private List<OwnCGBtnEUIController> stateBtnList;

    [Space(5)]
    [Header("-- In State")]
    [SerializeField] private List<TMP_Text> stateTitleTxtList;

    [Space(2)]
    [Header("* State")]
    [SerializeField] private ScrollPanelEUIController stateScrollPanel;
    [SerializeField] private TMP_Text stateLimitTxt;
    [SerializeField] private List<AllyProfileStateEUIController> stateEuiList;

    [Space(2)]
    [Header("* Tuner")]
    [SerializeField] private ScrollPanelEUIController tunerScrollPanel;
    [SerializeField] private RectTransform inStateTunerParentRt;
    [SerializeField] private GameObject inStateTunerPrefab;

    [Space(2)]
    [Header("* Sync")]
    [SerializeField] private ScrollPanelEUIController syncScrollPanel;
    [SerializeField] private RectTransform inStateSyncParentRt;
    [SerializeField] private GameObject inStateSyncPrefab;
    [SerializeField] private List<Sprite> pickedPanelSyncProgressSpriteList;


    [Space(10)]
    [Header("=== Inner")]
    [SerializeField] private List<Image> extraMainClrImgList;
    [SerializeField] private List<TMP_Text> extraMainClrTxtList;

    [SerializeField] private List<Image> extraSubClrImgList;
    [SerializeField] private List<TMP_Text> extraSubClrTxtList;

    #endregion

    #region - Hide

    // Profile List
    [HideInInspector] private List<AllyProfileEUIController> allAllyProfileEuiList;
    [HideInInspector] private List<AllyProfileEUIController> currentActiveProfileEuiList = new List<AllyProfileEUIController>();

    [HideInInspector] private AllyProfileEUIController currentSelectProfileEui = null;
    [HideInInspector] protected AllyProfileEUIController currentPickedProfileEui = null;

    [HideInInspector] private static readonly float allyProfileIntervalY = 160;
    [HideInInspector] private static readonly float allyProfilePanelMinHeight = 800;

    // Picked
    [HideInInspector] protected AllyController currentPickedAlly = null;
    [HideInInspector] private CanvasGroup allyProfilePickedSignCg;
    [HideInInspector] private int currentExtraPanelIndex = 0;

    // Profile Detail
    [HideInInspector] private bool profileDetailExtraIsOpen = false;
    [HideInInspector] protected bool isTweening = false;
    [HideInInspector] private static readonly float profileDetailExtraRt_OpenHeight = 650f;
    [HideInInspector] private static readonly float profileDetailExtraRt_CloseHeight = 80f;
    [HideInInspector] private static readonly float profileDetailExtraRt_ScrollMin = 550f;

    // In State - Tuner
    [HideInInspector] private List<TunerEUIController> inStateTunerEuiList;
    [HideInInspector] private static readonly float inStateTunerEui_BaseX = -12f;
    [HideInInspector] private static readonly float inStateTunerEui_BaseY = 60f;
    [HideInInspector] private static readonly float inStateTunerEui_Interval = 120f;

    // In State - Sync
    [HideInInspector] private List<AllySyncIconEUIController> inStateSyncEuiList;
    [HideInInspector] private List<AllySyncIconEUIController> inStateSyncActingEuiList;
    [HideInInspector] private static readonly float inStateSyncEui_BaseX = 74f;
    [HideInInspector] private static readonly float inStateSyncEui_BaseY = -116;
    [HideInInspector] private static readonly float inStateSyncEui_IntervalX = 108;
    [HideInInspector] private static readonly float inStateSyncEui_IntervalY = -140;
    [HideInInspector] private static readonly int inStateSyncEui_WidthAmount = 4;

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
        allAllyProfileEuiList = new List<AllyProfileEUIController>();

        foreach (Transform TF in allyProfileParentTf)
        {
            if (TF.TryGetComponent(out AllyProfileEUIController profile))
            {
                profile.ownerUIController = this;
                profile.Offset();
                allAllyProfileEuiList.Add(profile);
            }
        }

        allyProfilePickedSignCg = DevTool.Get_ComponentTType(allyProfilePickedSignRt.gameObject, out CanvasGroup cg) ? cg : null;


        // Profile Detail
        profileDetailEui.Offset();
        
        for (int i = 0; i < stateBtnList.Count; i++)
        {
            stateBtnList[i].ownerUIController = this;
            stateBtnList[i].Offset();
        }

        stateScrollPanel.Offset();
        tunerScrollPanel.Offset();
        syncScrollPanel.Offset();

        // In State - Tuner
        inStateTunerEuiList = new List<TunerEUIController>();

        // In State - Sync
        inStateSyncEuiList = new List<AllySyncIconEUIController>();
        inStateSyncActingEuiList = new List<AllySyncIconEUIController>();
    }

    public void Offset_ColorComp()
    {
        mainColorCompList = new List<Component>();
        subColorCompList = new List<Component>();

        mainColorCompList.AddRange(extraMainClrImgList);
        mainColorCompList.AddRange(extraMainClrTxtList);
        subColorCompList.AddRange(extraSubClrImgList);
        subColorCompList.AddRange(extraSubClrTxtList);

        // Set Color
        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;

        for (int i = 0; i < inStateSyncEuiList.Count; i++)
            inStateSyncEuiList[i].Set_Color();
    }

    #endregion

    #region Reset

    // 모든 데이터 초기화
    private void Reset_AllData()
    {
        currentActiveProfileEuiList.Clear();
        currentSelectProfileEui = null;
        currentPickedProfileEui = null;

        allyProfilePickedSignRt.gameObject.SetActive(false);
        currentExtraPanelIndex = 0;

        profileDetailExtraIsOpen = false;
        isTweening = false;
        statePanelCg.alpha = 0; 
        
    }

    // 프로필 리스트 리셋
    private void Reset_AllyProfileListPanel()
    {
        List<AllyController> allAlly = AllyManager.instance.allAlly;

        Set_AllAllyProfileListOff();
        Set_AllyProfileListOn(allAlly);
        Set_AllyProfileListPanelY(allAlly.Count);
        Select_DefaultAllyProfile();
    }
    
    // 프로필 디테일 리셋
    private void Reset_AllyProfileDetailPanel()
    {
        profileDetailEui.SetOff_Panel();

    }

    #endregion

    #region Set (Panel)

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        Reset_AllData();
        Reset_AllyProfileListPanel();
        Reset_AllyProfileDetailPanel();

        Play_ProfileExtraY(profileDetailExtraRt_CloseHeight, 0.3f);
    }

    #endregion

    #region Profile List

    // 모든 프로필 끄기
    private void Set_AllAllyProfileListOff()
    {
        for (int i = 0; i < allAllyProfileEuiList.Count; i++)
        {
            allAllyProfileEuiList[i].gameObject.SetActive(false);
            allAllyProfileEuiList[i].Reset_Profile();
        }
    }

    // 프로필 키기 (현재 Ally)
    private void Set_AllyProfileListOn(List<AllyController> allAlly)
    {

        for (int i = 0; i < allAlly.Count; i++)
        {
            allAllyProfileEuiList[i].gameObject.SetActive(true);
            allAllyProfileEuiList[i].Set_Profile(allAlly[i], -(i * allyProfileIntervalY));
            currentActiveProfileEuiList.Add(allAllyProfileEuiList[i]);
        }
    }

    // 프로필 항목 패널의 사이즈 조절 (Scroll을 위함)
    private void Set_AllyProfileListPanelY(int amount)
    {
        float y = Mathf.Max(amount * allyProfileIntervalY, allyProfilePanelMinHeight);
        panelTabList[0].Set_ScrollHeight(y);
    }

    #endregion

    #region Profile Select

    // 처음 시작 시, 첫번째 Ally 선택 (없다면 NULL)
    private void Select_DefaultAllyProfile()
    {
        if (currentActiveProfileEuiList.Count <= 0) // 없다면
        {
            Select_AllyProfile(null);
            allyProfileSelectSignRt.gameObject.SetActive(false);
        }
        else
        {
            Select_AllyProfile(currentActiveProfileEuiList[0]);
            allyProfileSelectSignRt.gameObject.SetActive(true);
        }
    }

    public void Select_AllyProfile(AllyProfileEUIController eui)
    {
        if (currentSelectProfileEui == eui) return;

        currentSelectProfileEui = eui;

        allyProfileSelectSignRt.SetParent(currentSelectProfileEui.transform);
        allyProfileSelectSignRt.SetAsLastSibling();

        DevTool.SetKillTween(allyProfileSelectSignRt);
        allyProfileSelectSignRt.DOAnchorPos(Vector2.zero, 0.05f);
    }

    #endregion

    #region Profile Pick

    // 실제로 인풋으로 클릭이나 지정하는 것
    // 인풋을 통해, Ally를 픽 (UI 변경 필요)
    protected virtual void Pick_AllyProfile(AllyProfileEUIController eui)
    {
        if (isTweening) return;

        currentPickedProfileEui = eui;
        currentPickedAlly = currentPickedProfileEui.Get_ThisAlly();

        allyProfilePickedSignRt.gameObject.SetActive(true);
        allyProfilePickedSignRt.SetParent(currentSelectProfileEui.transform);
        allyProfilePickedSignRt.SetAsLastSibling();
        allyProfilePickedSignRt.anchoredPosition = Vector2.zero;

        DevTool.SetKillTween(allyProfilePickedSignRt);
        allyProfilePickedSignRt.transform.localScale = Vector2.one * 1.2f;
        allyProfilePickedSignRt.transform.DOScale(Vector2.one, 0.15f);

        DevTool.SetKillTween(allyProfilePickedSignCg);
        allyProfilePickedSignCg.alpha = 0f;
        allyProfilePickedSignCg.DOFade(1f, 0.2f);

        // Data UI Set
        Set_AllyState(currentPickedAlly);
        Set_AllyTuner(currentPickedAlly);
        Set_AllySync(currentPickedAlly);

        // Detail Panel
        profileDetailEui.SetOn_Panel(currentPickedAlly);

        // Extra Panel
        Play_ProfileExtraY_CloseAndOpen(currentExtraPanelIndex);

        // Sound
        SoundManager.instance.Play_2D_SFX_UI("Click_01");
    }

    #endregion

    #region Profile Detail (Panel)

    // Extra, Y 패널
    private Sequence Play_ProfileDetailExtraCG(int extraIndex, float durTime)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(statePanelCg.DOFade(0f, durTime * 0.5f).OnComplete(() =>
        {
            for (int i = 0; i < stateBtnList.Count; i++)
            {
                int index = i;

                if (index == extraIndex)
                    statePanelRtList[index].gameObject.SetActive(true);
                else
                    statePanelRtList[index].gameObject.SetActive(false);
            }
        }));
        seq.Append(statePanelCg.DOFade(1f, durTime * 0.5f));

        return seq;
    }

    private Sequence Play_ProfileDetailExtraBtn(int extraIndex, float durTime)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < stateBtnList.Count; i++)
        {
            int index = i;

            if (index == extraIndex)
                seq.Join(stateBtnList[index].cg.DOFade(1f, durTime));
            else
                seq.Join(stateBtnList[index].cg.DOFade(0.4f, durTime));
        }

        return seq;
    }

    private Sequence Play_ProfileExtraY_CloseAndOpen(int extraIndex)
    {
        if (isTweening || currentPickedProfileEui == null) return null;
        isTweening = true;

        Sequence seq = DOTween.Sequence();

        currentExtraPanelIndex = extraIndex;
        float durTime = profileDetailExtraIsOpen ? 0.24f : 0.12f;

        if (profileDetailExtraIsOpen)
        {
            seq.Append(Play_ProfileExtraY(profileDetailExtraRt_CloseHeight, 0.12f));
            seq.Append(Play_ProfileExtraY(profileDetailExtraRt_OpenHeight, 0.12f));
        }
        else
        {
            seq.Append(Play_ProfileExtraY(profileDetailExtraRt_OpenHeight, 0.12f));
        }

        Play_ProfileDetailExtraBtn(extraIndex, durTime);
        Play_ProfileDetailExtraCG(extraIndex, durTime);

        profileDetailExtraIsOpen = true;

        seq.OnComplete(() => { isTweening = false; });

        // Sound
        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        return seq;
    }


    private Sequence Play_ProfileExtraY(float y, float durTime = 0f)
    {
        DevTool.SetKillTween(profileDetailExtraRt);

        Sequence seq = DOTween.Sequence();

        seq.Append(profileDetailExtraRt.DOSizeDelta(new Vector2(profileDetailExtraRt.rect.width, y), durTime));

        Play_ProfileDetailExtraBtn(-1, 0.12f);

        return seq;
    }

    #endregion

    #region Profile Detail (State)

    // 실제 데이터값
    protected void Set_AllyState(AllyController ally)
    {
        AllyState cardBaseState = ally.Get_CardState();
        stateEuiList[0].valueTxt.text = $"{DevTool.Get_RoundFloatString(cardBaseState.dmg.value).Replace("+", "")}";
        stateEuiList[1].valueTxt.text = $"{DevTool.Get_RoundFloatString(cardBaseState.rof.value).Replace("+", "")}<size=65%>/s</size>";
        stateEuiList[2].valueTxt.text = $"{DevTool.Get_RoundFloatString(cardBaseState.movementSpeed.value).Replace("+", "")}";
        stateEuiList[3].valueTxt.text = $"{DevTool.Get_RoundFloatString(cardBaseState.attackSize.value).Replace("+", "")}";
        stateEuiList[4].valueTxt.text = $"{DevTool.Get_RoundFloatString(cardBaseState.criticalChacne.value * 100).Replace("+", "")}<size=65%>%</size>";
        stateEuiList[5].valueTxt.text = $"{DevTool.Get_RoundFloatString(cardBaseState.criticalDmg.value + 1).Replace("+", "")}<size=65%>x</size>";
        stateEuiList[6].valueTxt.text = $"{DevTool.Get_RoundFloatString(cardBaseState.muzzleSpeed.value + 1).Replace("+", "")}";
        stateEuiList[7].valueTxt.text = $"{DevTool.Get_RoundFloatString(cardBaseState.kbPower.value).Replace("+", "")}";
        stateEuiList[8].valueTxt.text = $"{DevTool.Get_RoundFloatString(cardBaseState.dur.value).Replace("+", "")}<size=65%>s</size>";

        AllyState upgradeState = ally.Get_UpgradeAllState();
        stateEuiList[0].extraValueTxt.text = $"{DevTool.Get_RoundFloatString(upgradeState.dmg.value)}";
        stateEuiList[1].extraValueTxt.text = $"{DevTool.Get_RoundFloatString(upgradeState.rof.value)}<size=65%>/s</size>";
        stateEuiList[2].extraValueTxt.text = $"{DevTool.Get_RoundFloatString(upgradeState.movementSpeed.value)}";
        stateEuiList[3].extraValueTxt.text = $"{DevTool.Get_RoundFloatString(upgradeState.attackSize.value)}";
        stateEuiList[4].extraValueTxt.text = $"{DevTool.Get_RoundFloatString(upgradeState.criticalChacne.value * 100)}<size=65%>%</size>";
        stateEuiList[5].extraValueTxt.text = $"{DevTool.Get_RoundFloatString(upgradeState.criticalDmg.value)}<size=65%>x</size>";
        stateEuiList[6].extraValueTxt.text = $"{DevTool.Get_RoundFloatString(upgradeState.muzzleSpeed.value)}";
        stateEuiList[7].extraValueTxt.text = $"{DevTool.Get_RoundFloatString(upgradeState.kbPower.value)}";
        stateEuiList[8].extraValueTxt.text = $"{DevTool.Get_RoundFloatString(upgradeState.dur.value)}<size=65%>s</size>";

        stateScrollPanel.Set_ScrollPanel();
    }

    
    #endregion

    #region Profile Detail (Tuner)

    protected void Set_AllyTuner(AllyController ally)
    {
        List<AllyBaseTunerData> tunerData = ally.Get_ThisTunerData();

        // 만약 UI EUI가 부족하다면 생성
        if (inStateTunerEuiList.Count < tunerData.Count)
        {
            int needEUIAmount = tunerData.Count - inStateTunerEuiList.Count;
            for (int i = 0; i < needEUIAmount; i++)
                inStateTunerEuiList.Add(Gen_TunerEUI());
        }

        // 모든 Tuner EUI 끄기
        SetOff_AllTunerEUI();

        // Tuner에 맞추어 키기
        SetOn_TunerEUI(tunerData);

        float scrollY = Mathf.Max(
            profileDetailExtraRt_ScrollMin,
            inStateTunerEui_BaseY + (tunerData.Count * inStateTunerEui_Interval));

        tunerScrollPanel.Set_ScrollHeight(scrollY);
    }

    private TunerEUIController Gen_TunerEUI()
    {
        TunerEUIController result = DevTool.Get_ComponentTType<TunerEUIController>(Instantiate(inStateTunerPrefab, inStateTunerParentRt));
        result.Offset();

        return result;
    }

    private void SetOff_AllTunerEUI()
    {
        if (inStateTunerEuiList.Count <= 0) return;

        for (int i = 0; i < inStateTunerEuiList.Count; i++)
            inStateTunerEuiList[i].gameObject.SetActive(false);
    }

    private void SetOn_TunerEUI(List<AllyBaseTunerData> data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            inStateTunerEuiList[i].gameObject.SetActive(true);
            inStateTunerEuiList[i].rt.anchoredPosition = new Vector2(inStateTunerEui_BaseX, -(inStateTunerEui_BaseY + (inStateTunerEui_Interval * i)));
            inStateTunerEuiList[i].Set_UI(data[i]);
        }

    }

    #endregion

    #region Profile Detail (Sync)

    protected void Set_AllySync(AllyController ally)
    {
        Dictionary<int, int> syncData = ally.Get_ThisSyncData();

        // 만약 UI EUI가 부족하다면 생성
        if (inStateSyncEuiList.Count < syncData.Count)
        {
            int needEUIAmount = syncData.Count - inStateSyncEuiList.Count;
            for (int i = 0; i < needEUIAmount; i++)
                inStateSyncEuiList.Add(Gen_SyncEUI());
        }

        // 모든 Sync EUI 끄기
        SetOff_AllSyncEUI();

        // Sync에 맞추어 키기
        SetOn_SyncEUI(syncData, out float lastY);

        float scrollY = Mathf.Max(
            profileDetailExtraRt_ScrollMin,
            lastY + 100f);
        
        syncScrollPanel.Set_ScrollHeight(scrollY);

        // 모든 Sync UI의 연결로서 활성화되었는지
        Set_AllySync_ApplyState();
    }

    private void Set_AllySync_ApplyState()
    {
        if (currentPickedAlly == null) return;

        List<int> connectIdList = currentPickedAlly.Get_ConnectingSyncToKeyList();
        for (int i = 0; i < inStateSyncActingEuiList.Count; i++)
        {
            inStateSyncActingEuiList[i].Set_ConnectUI(
                connectIdList.Contains(inStateSyncActingEuiList[i].id));
        }

        List<int> completelyIdList = currentPickedAlly.Get_CompletelySyncToKeyList();
        for (int i = 0; i < inStateSyncActingEuiList.Count; i++)
        {
            inStateSyncActingEuiList[i].Set_Completely(
                completelyIdList.Contains(inStateSyncActingEuiList[i].id));
        }
    }

    private AllySyncIconEUIController Gen_SyncEUI()
    {
        AllySyncIconEUIController result = DevTool.Get_ComponentTType<AllySyncIconEUIController>(Instantiate(inStateSyncPrefab, inStateSyncParentRt));
        result.Offset();

        return result;
    }

    private void SetOff_AllSyncEUI()
    {
        if (inStateSyncEuiList.Count <= 0) return;

        for (int i = 0; i < inStateSyncEuiList.Count; i++)
            inStateSyncEuiList[i].gameObject.SetActive(false);
    }

    private void SetOn_SyncEUI(Dictionary<int, int> data, out float lastY)
    {
        inStateSyncActingEuiList.Clear();

        int currentOrder = 0;
        lastY = 0;
        foreach (KeyValuePair<int, int> value in data)
        {
            int x = currentOrder % inStateSyncEui_WidthAmount;
            int y = currentOrder / inStateSyncEui_WidthAmount;

            inStateSyncEuiList[currentOrder].gameObject.SetActive(true);
            inStateSyncEuiList[currentOrder].rt.anchoredPosition = new Vector2(
                inStateSyncEui_BaseX + (x * inStateSyncEui_IntervalX),
                inStateSyncEui_BaseY + (y * inStateSyncEui_IntervalY));
            lastY = inStateSyncEuiList[currentOrder].rt.anchoredPosition.y;

            inStateSyncEuiList[currentOrder].Set_UI(value.Key, value.Value);
            inStateSyncActingEuiList.Add(inStateSyncEuiList[currentOrder]);
            currentOrder++;
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
        if ((currentBtn == currentSelectProfileEui) &&
            (currentSelectProfileEui != currentPickedProfileEui))
        {
            Pick_AllyProfile(currentSelectProfileEui);

            return true;
        }

        return false;
    }

    private bool Interact_ProfileExtraBtn()
    {
        if (currentBtn is OwnCGBtnEUIController btn &&
            stateBtnList.Contains(btn))
        {
            Play_ProfileExtraY_CloseAndOpen(stateBtnList.IndexOf(btn));

            return true;
        }

        return false;
    }

    #endregion

    #region Play

    protected void Play_UseTxt(TMP_Text txt, int pay, float upY, float durTime = 0.5f)
    {
        RectTransform rt = DevTool.Get_ComponentTType<RectTransform>(txt.gameObject);

        DevTool.SetKillTween(txt);
        DevTool.SetKillTween(rt);

        txt.text = $"-{pay}";
        DevTool.Set_AlphaColor(txt, 1f);
        rt.anchoredPosition = Vector2.zero;

        txt.DOFade(0f, durTime);
        rt.DOAnchorPosY(upY, durTime);
    }

    #endregion

    #region Set (Language)

    public override void SetLanguageTxt()
    {
        // Profile List (Tab)
        tabBtnTxtList = new List<string>
        {
            ResourceManager.instance.Get_StaticWord(96)
        };

        // Profile Detail
        profileDetailTxt.text = ResourceManager.instance.Get_StaticWord(101);

        string state = ResourceManager.instance.Get_StaticWord(102);
        string bu = ResourceManager.instance.Get_StaticWord(106);
        string mu = ResourceManager.instance.Get_StaticWord(27);


        stateBtnList[0].txt.text = state;
        stateBtnList[1].txt.text = bu;
        stateBtnList[2].txt.text = mu;

        stateTitleTxtList[0].text = state;
        stateTitleTxtList[1].text = bu;
        stateTitleTxtList[2].text = mu;

        stateEuiList[0].nameTxt.text = $"< {ResourceManager.instance.Get_StaticWord(12)} >";    // 공격력
        stateEuiList[1].nameTxt.text = $"< {ResourceManager.instance.Get_StaticWord(13)} >";    // 연사력
        stateEuiList[2].nameTxt.text = $"< {ResourceManager.instance.Get_StaticWord(9)} >";     // 이동속도
        stateEuiList[3].nameTxt.text = $"< {ResourceManager.instance.Get_StaticWord(114)} >";    // 크기
        stateEuiList[4].nameTxt.text = $"< {ResourceManager.instance.Get_StaticWord(15)} >";    // 치확
        stateEuiList[5].nameTxt.text = $"< {ResourceManager.instance.Get_StaticWord(16)} >";     // 치뎀
        stateEuiList[6].nameTxt.text = $"< {ResourceManager.instance.Get_StaticWord(43)} >";     // 탄속
        stateEuiList[7].nameTxt.text = $"< {ResourceManager.instance.Get_StaticWord(44)} >";     // 넉백 (파워)
        stateEuiList[8].nameTxt.text = $"< {ResourceManager.instance.Get_StaticWord(116)} >";     // 지속시간

        // Limit
        stateLimitTxt.text = $"( {ResourceManager.instance.Get_StaticWord(107)}: {AllyController.minLimitUpgradeValue} )";

        base.SetLanguageTxt();
    }

    #endregion

    #region Get

    public Sprite Get_SyncProgressSprite(int progress)
    {
        return pickedPanelSyncProgressSpriteList[Mathf.Clamp(progress - 1, 0, AllyController.syncMax - 1)];
    }

    #endregion
}
