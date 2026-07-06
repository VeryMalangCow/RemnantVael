using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyProfileDetailView : MonoBehaviour
{
    [Space(20)]
    [Header("<><><><><> Profile Detail")]

    [Space(10)]
    [Header("=== On Panel")]
    [SerializeField] private GameObject onPanelGo;
    [SerializeField] private Image faceImg;
    [SerializeField] private TMP_Text nameTxt;

    [Space(10)]
    [Header("=== Off Panel")]
    [SerializeField] private GameObject offPanelGo;

    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainTmps;
    [SerializeField] private Image[] subImgs;

    [Space(10)]
    [Header("=== Extra")]
    [SerializeField] private TMP_Text profileDetailTxt;
    [SerializeField] private RectTransform profileDetailExtraRt;
    [SerializeField] private CanvasGroup statePanelCg;
    [SerializeField] private List<RectTransform> statePanelRtList;
    [SerializeField] public List<OwnCGBtnEUIController> stateBtnList;

    [Space(5)]
    [Header("-- In State")]
    [SerializeField] private List<TMP_Text> stateTitleTxtList;

    private bool profileDetailExtraIsOpen = false;
    private static readonly float profileDetailExtraRt_OpenHeight = 650f;
    private static readonly float profileDetailExtraRt_CloseHeight = 80f;
    private int currentExtraPanelIndex = 0;

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

    private List<TunerEUIController> inStateTunerEuiList = new List<TunerEUIController>();

    private static readonly float inStateTunerEui_BaseX = -12f;
    private static readonly float inStateTunerEui_BaseY = 60f;
    private static readonly float inStateTunerEui_Interval = 120f;
    private static readonly float profileDetailExtraRt_ScrollMin = 550f;

    [Space(2)]
    [Header("* Sync")]
    [SerializeField] private ScrollPanelEUIController syncScrollPanel;
    [SerializeField] private RectTransform inStateSyncParentRt;
    [SerializeField] private GameObject inStateSyncPrefab;
    [SerializeField] private List<Sprite> pickedPanelSyncProgressSpriteList;

    private List<AllySyncIconEUIController> inStateSyncEuiList = new List<AllySyncIconEUIController>();
    private List<AllySyncIconEUIController> inStateSyncActingEuiList = new List<AllySyncIconEUIController>();

    private static readonly float inStateSyncEui_BaseX = 74f;
    private static readonly float inStateSyncEui_BaseY = -116;
    private static readonly float inStateSyncEui_IntervalX = 108;
    private static readonly float inStateSyncEui_IntervalY = -140;
    private static readonly int inStateSyncEui_WidthAmount = 4;

    public void Init(AllyShopUIController ui)
    {
        for (int i = 0; i < stateBtnList.Count; i++)
        {
            stateBtnList[i].ownerUIController = ui;
            stateBtnList[i].Offset();
        }

        stateScrollPanel.Offset();
        tunerScrollPanel.Offset();
        syncScrollPanel.Offset();

        SetOff_Panel();
    }

    public void ResetData()
    {
        currentExtraPanelIndex = 0;

        profileDetailExtraIsOpen = false;
        statePanelCg.alpha = 0;
    }

    // Color
    public void SetColor(Color mainClr, Color subClr)
    {
        DevTool.SetColorTmps(mainClr, mainTmps);
        DevTool.SetColorImgs(subClr, subImgs);

        for (int i = 0; i < inStateSyncEuiList.Count; i++)
            inStateSyncEuiList[i].Set_Color();
    }

    #region State

    public void Set_AllyState(AllyController ally)
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

    #region Tuner

    public void Set_AllyTuner(AllyController ally)
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

    #region Sync

    public void Set_AllySync(AllyController ally)
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
        Set_AllySync_ApplyState(ally);
    }

    public void Set_AllySync_ApplyState(AllyController currentPickedAlly)
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

    public Sprite Get_SyncProgressSprite(int progress)
        => pickedPanelSyncProgressSpriteList[Mathf.Clamp(progress - 1, 0, AllyController.syncMax - 1)];
    
    #endregion

    // Panel
    public void SetOn_Panel(AllyController ally)
    {
        Set_Panel(true);
        nameTxt.text = $"-[ {ally.Get_Name()} ]-";
        faceImg.sprite = ally.Get_FrontFaceImg();
    }

    public void SetOff_Panel()
    {
        Set_Panel(false);
        nameTxt.text = "";

    }

    private void Set_Panel(bool onOff)
    {
        onPanelGo.SetActive(onOff);
        offPanelGo.SetActive(!onOff);
    }

    // Panel Tween
    public Sequence Play_ProfileExtraY_CloseAndOpen(int extraIndex, AllyProfileEUIController currentPickedProfileEui)
    {
        if (currentPickedProfileEui == null) return null;

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

        // Sound
        SoundManager.instance.Play_2D_SFX_UI("Click_01");

        return seq;
    }

    public Sequence Play_ProfileExtraY(float durTime)
    {
        return Play_ProfileExtraY(profileDetailExtraRt_CloseHeight, durTime);
    }

    private Sequence Play_ProfileExtraY(float y, float durTime)
    {
        DevTool.SetKillTween(profileDetailExtraRt);

        Sequence seq = DOTween.Sequence();

        seq.Append(profileDetailExtraRt.DOSizeDelta(new Vector2(profileDetailExtraRt.rect.width, y), durTime));

        Play_ProfileDetailExtraBtn(-1, 0.12f);

        return seq;
    }

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


    // Language
    public void SetLanguageTxt()
    {
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
    }
}
