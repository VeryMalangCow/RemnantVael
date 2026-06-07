using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudHittedView : MonoBehaviour
{
    [Header("=== Hitted")]
    [SerializeField] private Image hittedScreen;
    [SerializeField] private Transform hittedInfoPivotTf;
    [SerializeField] private RectTransform hittedInfoRt;
    [SerializeField] private TMP_Text hittedDmgTxt;
    [SerializeField] private CanvasGroup paneltyAnnoCg;
    [SerializeField] private TMP_Text paneltyAnnoNameTxt;
    [SerializeField] private TMP_Text paneltyAnnoDescTxt;

    [SerializeField] private Color uninteractableClr;
    [SerializeField] private Color interactableClr;

    private float offsetXPos;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private Image[] mainClrImgs;
    [SerializeField] private TMP_Text[] mainClrTmps;
    [SerializeField] private Image[] subClrImgs;

    public void Init(Color mainClr, Color subClr)
    {
        offsetXPos = hittedInfoRt.anchoredPosition.x;

        paneltyAnnoCg.alpha = 0f;
        DevTool.SetColor(uninteractableClr, paneltyAnnoNameTxt);
        paneltyAnnoNameTxt.text = "";
        DevTool.SetColor(uninteractableClr, paneltyAnnoDescTxt);
        paneltyAnnoDescTxt.text = "";
        paneltyAnnoCg.gameObject.SetActive(false);

        ColorInit(mainClr, subClr);

        gameObject.SetActive(true);
    }

    private void ColorInit(Color mainClr, Color subClr)
    {
        DevTool.SetColorImgs(mainClr, mainClrImgs);
        mainClrImgs = null;
        DevTool.SetColorTmps(mainClr, mainClrTmps);
        mainClrTmps = null;

        DevTool.SetColorImgs(subClr, subClrImgs);
        subClrImgs = null;
    }

    // 피격 시 효과
    public void PlayHittedPlayScreen(float dmg, float durTime)
    {
        DevTool.SetKillTween(hittedScreen);

        Sequence seq = DOTween.Sequence();
        seq.Append(hittedScreen.DOFade((Mathf.Min(100, dmg) * 0.01f), durTime));
        seq.Append(hittedScreen.DOFade(0, durTime));
    }


    // 피격 정보
    public void PlayHittedPlayInfo(float dmg, float durTime)
    {
        hittedDmgTxt.text = $"<size=75%>{ResourceManager.instance.Get_StaticWord(74)}:</size> {dmg.ToString("0.0")}";
        hittedDmgTxt.color = uninteractableClr;

        PlayInfo(durTime);
    }

    // 회피 정보
    public void PlayAvoidPlayInfo(float durTime)
    {
        hittedDmgTxt.text = $"{ResourceManager.instance.Get_StaticWord(75)}";
        hittedDmgTxt.color = Color.white;

        PlayInfo(durTime);
    }

    // 정보
    private void PlayInfo(float durTime)
    {
        hittedInfoPivotTf.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-5f, 5f));

        DevTool.SetKillTween(hittedInfoRt);

        Sequence seq = DOTween.Sequence();

        hittedInfoRt.anchoredPosition = new Vector2(offsetXPos, 0);
        seq.Append(hittedInfoRt.DOAnchorPosX(-50f, durTime * 0.2f).SetEase(Ease.Linear));
        seq.Append(hittedInfoRt.DOAnchorPosX(50f, durTime * 0.6f).SetEase(Ease.Linear));
        seq.Append(hittedInfoRt.DOAnchorPosX(-offsetXPos, durTime * 0.2f).SetEase(Ease.Linear));
    }


    public void PlayPrisonPanelty()
    {
        PlayHittedPlayScreen(20, 1f);

        string title = $"< {ResourceManager.instance.Get_StaticDesc(36).Replace("\\n", "\n")} >";
        string desc = ResourceManager.instance.Get_StaticDesc(37).Replace("\\n", "\n");
        paneltyAnnoNameTxt.text = "";
        paneltyAnnoDescTxt.text = "";

        Sequence seq = DOTween.Sequence();
        paneltyAnnoCg.gameObject.SetActive(true);

        seq.Append(paneltyAnnoCg.DOFade(1f, 0.5f));
        seq.Join(paneltyAnnoNameTxt.DOText(title, 0.5f));
        seq.Join(paneltyAnnoDescTxt.DOText(desc, 0.5f));
        seq.AppendInterval(1f);
        seq.Append(paneltyAnnoCg.DOFade(0f, 2f));

        seq.OnComplete(() => { paneltyAnnoCg.gameObject.SetActive(false); });
    }
}
