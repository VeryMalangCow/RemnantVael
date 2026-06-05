using DG.Tweening;
using System.Collections;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HudBetteryView : MonoBehaviour
{
    [Space(10)]
    [Header("=== Image")]
    [SerializeField] private ImgTxtAmountEUIController emptyBetteryEui;
    [SerializeField] private ImgTxtAmountEUIController chargedBetteryEui;
    [SerializeField] private Image ecCostArrowImg;
    [SerializeField] private TMP_Text ecCostTxt;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainClrTmps;
    [SerializeField] private Image[] subClrImgs;


    // Init
    public IEnumerator Init(PlayerController player, Color mainClr, Color subClr)
    {
        Stopwatch sw = Stopwatch.StartNew();

        emptyBetteryEui.Offset();
        chargedBetteryEui.Offset();

        // EP Txt
        ecCostTxt.text = player.needEP_ForMakeEC.ToString();

        ColorInit(mainClr, subClr);

        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=orange>Betteries View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
        yield return null;
    }

    private void ColorInit(Color mainClr, Color subClr)
    {
        DevTool.SetColorTmps(mainClr, mainClrTmps);
        mainClrTmps = null;

        DevTool.SetColorImgs(subClr, subClrImgs);
        subClrImgs = null;
    }
    public void SetEmptyBettery(int amount)
    {
        emptyBetteryEui.Set_Amount(amount, 0.5f);
    }

    public void SetChargedBettery(int amount)
    {
        chargedBetteryEui.Set_Amount(amount, 0.5f);

        DOTween.Kill(ecCostArrowImg);
        ecCostArrowImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                ecCostArrowImg.DOFade(0.25f, 0.2f);
            });
    }


}
