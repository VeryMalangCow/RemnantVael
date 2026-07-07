using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseOptionView : MonoBehaviour
{
    [SerializeField] public RectTransform panelRt;
    [SerializeField] public OwnBtnEUIController backBtn;
    [SerializeField] public OwnBtnEUIController applyBtn;
    [SerializeField] public TMP_Text warningTxt;

    [Space(10)]
    [SerializeField] public LRSlidingItemEUIController languagePanelEui;
    [SerializeField] public LRSlidingItemEUIController screenModePanelEui;
    [SerializeField] public LRSlidingItemEUIController resolutionPanelEui;
    [SerializeField] public LRSlidingItemEUIController fpsPanelEui;
    [SerializeField] public FillScrollbarEUIController bgmVolumePanelEui;
    [SerializeField] public FillScrollbarEUIController sfxVolumePanelEui;

    public void Init(PauseUIController uiController)
    {
        backBtn.ownerUIController = uiController;
        backBtn.Offset();

        applyBtn.ownerUIController = uiController;
        applyBtn.Offset();

        languagePanelEui.Set_OwnerUIController(uiController);
        languagePanelEui.Offset();

        screenModePanelEui.Set_OwnerUIController(uiController);
        screenModePanelEui.Offset();

        resolutionPanelEui.Set_OwnerUIController(uiController);
        resolutionPanelEui.Offset();

        fpsPanelEui.Set_OwnerUIController(uiController);
        fpsPanelEui.Offset();

        bgmVolumePanelEui.Set_OwnerUIController(uiController);
        bgmVolumePanelEui.Offset();

        sfxVolumePanelEui.Set_OwnerUIController(uiController);
        sfxVolumePanelEui.Offset();


    }

    public void SetColor(Color mainClr)
    {
        List<TMP_Text> mainClrTmps = new List<TMP_Text>();

        mainClrTmps.AddRange(languagePanelEui.MainClrTmps);
        mainClrTmps.AddRange(screenModePanelEui.MainClrTmps);
        mainClrTmps.AddRange(resolutionPanelEui.MainClrTmps);
        mainClrTmps.AddRange(fpsPanelEui.MainClrTmps);
        mainClrTmps.AddRange(bgmVolumePanelEui.MainClrTmps);
        mainClrTmps.AddRange(sfxVolumePanelEui.MainClrTmps);

        DevTool.SetColorTmps(mainClr, mainClrTmps);
        mainClrTmps = null;
    }

    public void SetPanel()
    {
        panelRt.gameObject.SetActive(true);
        warningTxt.gameObject.SetActive(false);
        languagePanelEui.Set_Item(GameManager.languageID);
        screenModePanelEui.Set_Item((int)GameManager.screenMode);
        resolutionPanelEui.Set_Item((int)GameManager.resolutionMode);
        fpsPanelEui.Set_Item((int)GameManager.fps);
        bgmVolumePanelEui.Set_Value(SoundManager.instance.bgmVolume);
        sfxVolumePanelEui.Set_Value(SoundManager.instance.sfxVolume);
    }

    public void SetLanguageTxt()
    {
        var words = StaticResourceManager.instance.staticWords;

        warningTxt.text = StaticResourceManager.instance.staticDescs.GetLanguage(31);

        DevTool.Get_ComponentTType<TMP_Text>(applyBtn.gameObject.transform.GetChild(0).gameObject).text = words.GetLanguage(91);
        languagePanelEui.headerTxt.text = words.GetLanguage(92);
        screenModePanelEui.headerTxt.text = words.GetLanguage(140);
        resolutionPanelEui.headerTxt.text = words.GetLanguage(137);
        fpsPanelEui.headerTxt.text = words.GetLanguage(141);
        bgmVolumePanelEui.headerTxt.text = words.GetLanguage(138);
        sfxVolumePanelEui.headerTxt.text = words.GetLanguage(139);
    }
}
