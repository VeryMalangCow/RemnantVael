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

    public void Set_Panel()
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

    public void Set_LanguageTxt()
    {
        warningTxt.text = ResourceManager.instance.Get_StaticDesc(31);

        DevTool.Get_ComponentTType<TMP_Text>(applyBtn.gameObject.transform.GetChild(DevTool.Get_TSChildIndex(applyBtn, 0)).gameObject).text = ResourceManager.instance.Get_StaticWord(91);
        languagePanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(92);
        screenModePanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(140);
        resolutionPanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(137);
        fpsPanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(141);
        bgmVolumePanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(138);
        sfxVolumePanelEui.headerTxt.text = ResourceManager.instance.Get_StaticWord(139);
    }

    public List<Component> Get_MainColorComps()
    {
        List<Component> result = new List<Component>();

        result.AddRange(languagePanelEui.Get_InnerMainColorList());
        result.AddRange(screenModePanelEui.Get_InnerMainColorList());
        result.AddRange(resolutionPanelEui.Get_InnerMainColorList());
        result.AddRange(fpsPanelEui.Get_InnerMainColorList());
        result.AddRange(bgmVolumePanelEui.Get_InnerMainColorList());
        result.AddRange(sfxVolumePanelEui.Get_InnerMainColorList());

        return result;
    }
}
