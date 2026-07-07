using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunerDetailEUIController : TunerEUIController
{
    [Space(10)]
    [Header("-- Tuner Element Desc")]
    [SerializeField] private TunerDescEUIController positive0_ElementDescEui;
    [SerializeField] private TunerDescEUIController positive1_ElementDescEui;
    [SerializeField] private TunerDescEUIController negative_ElementDescEui;

    public void Set_TunerDetailUI(AllyTunerData data)
    {
        Set_UI(data);

        positive0_ElementDescEui.Set_UI(data.positive0, true);
        positive1_ElementDescEui.Set_UI(data.positive1, true);
        negative_ElementDescEui.Set_UI(data.negative, false);
    }

    public void SetLanguageTxt()
    {
        var words = StaticResourceManager.instance.staticWords;
        positive0_ElementDescEui.increaseTxt.text = words.GetLanguage(108);
        positive1_ElementDescEui.increaseTxt.text = words.GetLanguage(108);
        negative_ElementDescEui.increaseTxt.text = words.GetLanguage(109);
    }
}
