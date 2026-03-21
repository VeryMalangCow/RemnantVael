using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandbyAllyBUEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private Image FaceImg;
    [SerializeField] private TMP_Text NameTxt;

    [Space(10)]
    [Header("=== BU")]
    [SerializeField] private GameObject BUPanelGO;
    // Dmg, Rof, CC, CD, Size, MSpd, KB, Dur, Spd
    [SerializeField] private TMP_Text[] StateValueArr;

    [Space(10)]
    [Header("=== MU")]
    [SerializeField] private GameObject MUPanelGO;
    [SerializeField] private AllySyncIconEUIController[] AllySyncIconEUIArr;

    [SerializeField] private TMP_Text ExtraTxt;
    [SerializeField] private Image ApplyStateImg;
    [SerializeField] private TMP_Text ApplyStateAmountTxt;
    [SerializeField] private Image ConnectStateImg;
    [SerializeField] private TMP_Text ConnectStateAmountTxt;

    #endregion

    #region - Hide

    [HideInInspector] private RectTransform ThisRT;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = TryGetComponent(out RectTransform rt) ? rt : null;
        Set_Panel(true);
        Set_Color();
    }

    #endregion

    #region Set

    public void Set_Pos(Vector2 _Pos)
    {
        ThisRT.anchoredPosition = _Pos;
    }

    public void Set_Data(AllyController _Ally)
    {
        FaceImg.sprite = _Ally.Get_FrontFaceImg();
        NameTxt.text = _Ally.Get_Name();

        // BU
        AllyState state = _Ally.Get_ActaulAllyState();
        StateValueArr[0].text = $"{DevTool.Get_RoundFloatString(state.dmg.value).Replace("+", "")}";
        StateValueArr[1].text = $"{DevTool.Get_RoundFloatString(state.rof.value).Replace("+", "")}<size=65%>/s</size>";
        StateValueArr[2].text = $"{DevTool.Get_RoundFloatString(state.criticalChacne.value * 100).Replace("+", "")}<size=65%>%</size>";
        StateValueArr[3].text = $"{DevTool.Get_RoundFloatString(state.criticalDmg.value + 1).Replace("+", "")}<size=65%>x</size>";
        StateValueArr[4].text = $"{DevTool.Get_RoundFloatString(state.attackSize.value).Replace("+", "")}";
        StateValueArr[5].text = $"{DevTool.Get_RoundFloatString(state.muzzleSpeed.value + 1).Replace("+", "")}";
        StateValueArr[6].text = $"{DevTool.Get_RoundFloatString(state.kbPower.value).Replace("+", "")}";
        StateValueArr[7].text = $"{DevTool.Get_RoundFloatString(state.dur.value).Replace("+", "")}<size=65%>s</size>";
        StateValueArr[8].text = $"{DevTool.Get_RoundFloatString(state.movementSpeed.value).Replace("+", "")}";
        
        
        // MU
        Dictionary<int, int> syncData = _Ally.Get_ThisSyncData();

        // 모든 Sync EUI 끄기
        for (int i = 0; i < AllySyncIconEUIArr.Length; i++)
            AllySyncIconEUIArr[i].gameObject.SetActive(false);

        // List Data
        List<int> connectIdList = _Ally.Get_ConnectingSyncToKeyList();
        List<int> completelyIdList = _Ally.Get_CompletelySyncToKeyList();

        // Sync에 맞추어 키기
        int currentIndex = 0;
        int extraApplyAmount = 0;
        int extraConnectAmount = 0;
        foreach (KeyValuePair<int, int> value in syncData)
        {
            if (AllySyncIconEUIArr.Length > currentIndex)
            {
                AllySyncIconEUIArr[currentIndex].gameObject.SetActive(true);
                AllySyncIconEUIArr[currentIndex].Set_UI(value.Key, value.Value);
                currentIndex++;
            }
            else
            {
                if (value.Value >= AllyController.syncMax)
                    extraApplyAmount++;

                if (connectIdList.Contains(value.Key))
                    extraConnectAmount++;
            }
        }
        ApplyStateAmountTxt.text = $"+{extraApplyAmount}";
        ConnectStateAmountTxt.text = $"+{extraConnectAmount}";


        // 모든 Sync UI의 연결로서 활성화되었는지

        for (int i = 0; i < AllySyncIconEUIArr.Length; i++)
        {
            AllySyncIconEUIArr[i].Set_ConnectUI(
                connectIdList.Contains(AllySyncIconEUIArr[i].ID));
        }

        for (int i = 0; i < AllySyncIconEUIArr.Length; i++)
        {
            AllySyncIconEUIArr[i].Set_Completely(
                completelyIdList.Contains(AllySyncIconEUIArr[i].ID));
        }

        ExtraTxt.text = ResourceManager.instance.Get_StaticWord(142);
    }

    public void Set_Panel(bool _IsBUPanelOn)
    {
        BUPanelGO.SetActive(_IsBUPanelOn);
        MUPanelGO.SetActive(!_IsBUPanelOn);
    }

    public void Set_Color()
    {
        for (int i = 0; i < AllySyncIconEUIArr.Length; i++)
        {
            AllySyncIconEUIArr[i].Set_Color();
        }

        ConnectStateImg.color = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        ConnectStateAmountTxt.color = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
    }

    #endregion
}
