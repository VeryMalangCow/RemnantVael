using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StandbyAllyBUEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Comp")]
    [FormerlySerializedAs("FaceImg")][SerializeField] private Image faceImg;
    [FormerlySerializedAs("NameTxt")][SerializeField] private TMP_Text nameTxt;

    [Space(10)]
    [Header("=== BU")]
    [FormerlySerializedAs("BUPanelGO")][SerializeField] private GameObject buPanelGo;
    // Dmg, Rof, CC, CD, Size, MSpd, KB, Dur, Spd
    [FormerlySerializedAs("StateValueArr")][SerializeField] private TMP_Text[] stateValueArr;

    [Space(10)]
    [Header("=== MU")]
    [FormerlySerializedAs("MUPanelGO")][SerializeField] private GameObject muPanelGo;
    [FormerlySerializedAs("AllySyncIconEUIArr")][SerializeField] private AllySyncIconEUIController[] allySyncIconEuiArr;

    [FormerlySerializedAs("ExtraTxt")][SerializeField] private TMP_Text extraTxt;
    [FormerlySerializedAs("ApplyStateImg")][SerializeField] private Image applyStateImg;
    [FormerlySerializedAs("ApplyStateAmountTxt")][SerializeField] private TMP_Text applyStateAmountTxt;
    [FormerlySerializedAs("ConnectStateImg")][SerializeField] private Image connectStateImg;
    [FormerlySerializedAs("ConnectStateAmountTxt")][SerializeField] private TMP_Text connectStateAmountTxt;

    #endregion

    #region - Hide

    [HideInInspector] private RectTransform rt;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        rt = TryGetComponent(out RectTransform _rt) ? _rt : null;
        Set_Panel(true);
        Set_Color();
    }

    #endregion

    #region Set

    public void Set_Pos(Vector2 pos)
    {
        rt.anchoredPosition = pos;
    }

    public void Set_Data(AllyController ally)
    {
        faceImg.sprite = ally.Get_FrontFaceImg();
        nameTxt.text = ally.Get_Name();

        // BU
        AllyState state = ally.Get_ActaulAllyState();
        stateValueArr[0].text = $"{DevTool.Get_RoundFloatString(state.dmg.value).Replace("+", "")}";
        stateValueArr[1].text = $"{DevTool.Get_RoundFloatString(state.rof.value).Replace("+", "")}<size=65%>/s</size>";
        stateValueArr[2].text = $"{DevTool.Get_RoundFloatString(state.criticalChacne.value * 100).Replace("+", "")}<size=65%>%</size>";
        stateValueArr[3].text = $"{DevTool.Get_RoundFloatString(state.criticalDmg.value + 1).Replace("+", "")}<size=65%>x</size>";
        stateValueArr[4].text = $"{DevTool.Get_RoundFloatString(state.attackSize.value).Replace("+", "")}";
        stateValueArr[5].text = $"{DevTool.Get_RoundFloatString(state.muzzleSpeed.value + 1).Replace("+", "")}";
        stateValueArr[6].text = $"{DevTool.Get_RoundFloatString(state.kbPower.value).Replace("+", "")}";
        stateValueArr[7].text = $"{DevTool.Get_RoundFloatString(state.dur.value).Replace("+", "")}<size=65%>s</size>";
        stateValueArr[8].text = $"{DevTool.Get_RoundFloatString(state.movementSpeed.value).Replace("+", "")}";
        
        
        // MU
        Dictionary<int, int> syncData = ally.Get_ThisSyncData();

        // 모든 Sync EUI 끄기
        for (int i = 0; i < allySyncIconEuiArr.Length; i++)
            allySyncIconEuiArr[i].gameObject.SetActive(false);

        // List Data
        List<int> connectIdList = ally.Get_ConnectingSyncToKeyList();
        List<int> completelyIdList = ally.Get_CompletelySyncToKeyList();

        // Sync에 맞추어 키기
        int currentIndex = 0;
        int extraApplyAmount = 0;
        int extraConnectAmount = 0;
        foreach (KeyValuePair<int, int> value in syncData)
        {
            if (allySyncIconEuiArr.Length > currentIndex)
            {
                allySyncIconEuiArr[currentIndex].gameObject.SetActive(true);
                allySyncIconEuiArr[currentIndex].Set_UI(value.Key, value.Value);
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
        applyStateAmountTxt.text = $"+{extraApplyAmount}";
        connectStateAmountTxt.text = $"+{extraConnectAmount}";


        // 모든 Sync UI의 연결로서 활성화되었는지

        for (int i = 0; i < allySyncIconEuiArr.Length; i++)
        {
            allySyncIconEuiArr[i].Set_ConnectUI(
                connectIdList.Contains(allySyncIconEuiArr[i].id));
        }

        for (int i = 0; i < allySyncIconEuiArr.Length; i++)
        {
            allySyncIconEuiArr[i].Set_Completely(
                completelyIdList.Contains(allySyncIconEuiArr[i].id));
        }

        extraTxt.text = ResourceManager.instance.Get_StaticWord(142);
    }

    public void Set_Panel(bool isBUPanelOn)
    {
        buPanelGo.SetActive(isBUPanelOn);
        muPanelGo.SetActive(!isBUPanelOn);
    }

    public void Set_Color()
    {
        for (int i = 0; i < allySyncIconEuiArr.Length; i++)
        {
            allySyncIconEuiArr[i].Set_Color();
        }

        connectStateImg.color = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        connectStateAmountTxt.color = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
    }

    #endregion
}
