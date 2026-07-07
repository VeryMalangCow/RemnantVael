using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudAllyPresenceView : MonoBehaviour
{
    [Space(10)]
    [Header("=== Presence")]
    [SerializeField] private AllyPresenceEUIController stAllyPresence;
    [SerializeField] private AllyPresenceEUIController utAllyPresence;
    [SerializeField] private AllyPresenceEUIController ntAllyPresence; 
    private List<AllyPresenceEUIController> allAllyPresence;

    [Space(10)]
    [Header("=== Reputation")]
    [SerializeField] private Image allyReputationImg;
    [SerializeField] private TMP_Text allyReputationTxt;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private Image[] mainClrImgs;
    [SerializeField] private Image[] subClrImgs;

    public void Init(Color mainClr, Color subClr)
    {
        allAllyPresence = new List<AllyPresenceEUIController> { stAllyPresence, utAllyPresence, ntAllyPresence };

        for (int i = 0; i < allAllyPresence.Count; i++)
            allAllyPresence[i].Offset();

        ColorInit(mainClr, subClr);

        gameObject.SetActive(true);
    }

    private void ColorInit(Color mainClr, Color subClr)
    {
        DevTool.SetColorImgs(mainClr, mainClrImgs);
        mainClrImgs = null;

        DevTool.SetColorImgs(subClr, subClrImgs);
        subClrImgs = null;
    }

    public void StrikePresence(int current, int max) => stAllyPresence.Play_Amount(current, max);
    public void UplinkPresence(int current, int max) => utAllyPresence.Play_Amount(current, max);
    public void NeoPresence(int current, int max) => ntAllyPresence.Play_Amount(current, max);


    public void SetReputation(float value)
    {
        allyReputationTxt.text = $"{value} %";
        allyReputationImg.fillAmount = value * 0.01f;
        allyReputationImg.color = Color.Lerp(new Color(0.8f, 1f, 0.8f, 1f), new Color(0.35f, 1f, 0.35f, 1f), allyReputationImg.fillAmount);
    }

    public void SetLanguage()
    {
        var words = StaticResourceManager.instance.staticWords;
        for (int i = 0; i < allAllyPresence.Count; i++)
            allAllyPresence[i].presenceLangTxt.text = $"{words.GetLanguage(i + 61)}<size=85%> {words.GetLanguage(70)}</size>";
    }

}
