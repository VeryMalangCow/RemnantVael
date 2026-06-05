using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class HudTabStateView : MonoBehaviour
{
    [Space(10)]
    [Header("=== Tab States")]
    [SerializeField] private RectTransform playerStatesCostParentRt;
    [SerializeField] private TMP_Text playerStatesTxt;

    private float defaultPlayerStatesRectX;
    private List<string> playerStatesStringList = new List<string>();

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainClrTmps;

    private Tween tabTween;

    // Init
    public IEnumerator Init(Color mainClr)
    {
        Stopwatch sw = Stopwatch.StartNew();

        // 플레이어 스탯
        defaultPlayerStatesRectX = DevTool.Get_ComponentTType(
            playerStatesCostParentRt.gameObject, out RectTransform state_Rt) ?
                state_Rt.anchoredPosition.x : 0f;

        ColorInit(mainClr);

        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=orange>Tab State View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
        yield return null;
    }

    private void ColorInit(Color mainClr)
    {
        DevTool.SetColorTmps(mainClr, mainClrTmps);
        mainClrTmps = null;
    }

    public void TabOn(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        tabTween = playerStatesCostParentRt.DOAnchorPosX(0, durTime);
    }

    public void TabOff(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        tabTween = playerStatesCostParentRt.DOAnchorPosX(defaultPlayerStatesRectX, durTime);
    }

    public void ResetTab(PlayerController player)
    {
        playerStatesTxt.text = GetPlayerStateTxt(player);
    }

    // Tab 플레이어 스탯 Txt 
    private string GetPlayerStateTxt(PlayerController player)
    {
        string result = "";
        List<string> strings = GetPlayerStateStrings(player);

        for (int i = 0; i < playerStatesStringList.Count; i++)
        {
            result += "<size=70%>" + playerStatesStringList[i] + ": </size>";
            result += "<b>" + strings[i] + "</b>\n";
        }
        return result;
    }    
    
    // Tab 플레이어 스탯의 엘레먼트
    private List<string> GetPlayerStateStrings(PlayerController player)
    {
        PlayerWeaponController weapon = player.baseWeapon;
        return new List<string>()
        {
            player.maxEP.actualState.ToString(),
            player.walkSpeed.actualState.ToString(),
            player.dash.dashSpeed.actualState.ToString(),
            player.dash.Get_ActualNeedEP().ToString(),
            weapon.baseDamage.actualState.ToString(),
            weapon.rof.actualState.ToString(),
            weapon.accRate.actualState.ToString(),
            weapon.cc.actualState.ToString(),
            weapon.cd.actualState.ToString()
        };
    }

    public void SetLanguage()
    {
        playerStatesStringList.Clear();
        for (int i = 8; i <= 16; i++)
            playerStatesStringList.Add(ResourceManager.instance.Get_StaticWord(i));
    }
}
