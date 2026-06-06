using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudEpView : MonoBehaviour
{
    // Value
    [Space(10)]
    [Header("=== Energy Gauge")]
    [SerializeField] private ProgressFrameBarEUIController epEui;

    [Space(10)]
    [Header("=== Shield Gauge")]
    [SerializeField] private RectTransform shieldRt;
    [SerializeField] private TMP_Text shieldTxt;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private Image[] mainClrImgs;
    [SerializeField] private Image[] subClrImgs;


    // Init
    public void Init(Color mainClr, Color subClr)
    {
        epEui.Offset();
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

    
    public void SetEpUI(float currentEp, float maxEp)
    {
        epEui.SetFillImg(currentEp, maxEp);
    }

    public void SetMaxEpUI(float currentEp, float maxEp)
    {
        epEui.SetMaxUI(maxEp);
        epEui.SetFillImg(currentEp, maxEp);
    }

    public void SetShieldGauge(float totalShield)
    {
        shieldRt.sizeDelta = new Vector2(Get_ShieldGageX(totalShield), shieldRt.sizeDelta.y);
        shieldTxt.text = "<size=75%>( </size>" + Mathf.Round(totalShield).ToString() + "<size=75%> )</size>";
    }

    private float Get_ShieldGageX(float shieldValue)
        => 8 + (shieldValue * 3);
    
}
