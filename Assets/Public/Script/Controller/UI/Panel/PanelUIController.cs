using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelUIController : SinglePanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Panel")]

    [Space(10)]
    [Header("=== Tab")]
    [SerializeField] protected List<TabEUIController> panelTabList;

    #endregion

    #region - Hide

    // Tab
    [HideInInspector] protected TabEUIController currentThisPanelTab;

    #endregion

    #endregion

    #region Set Panel

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        // Actual Tab
        SoundManager.instance.PlayUiSfx("Approve");
        SetOn_Window(panelTabList[0]);
    }

    public virtual void Change_ThisPanel(int indexWindow)
    {
        //Other
        if (currentThisPanelTab == panelTabList[indexWindow]) return;

        SoundManager.instance.PlayUiSfx("Click01");
        SetOn_Window(panelTabList[indexWindow]);
    }

    public override void SetOff_ThisPanel()
    {
        base.SetOff_ThisPanel();

        SoundManager.instance.PlayUiSfx("Reject");
    }

    #endregion

    #region On/Off Tab Window

    private void SetOn_Window(TabEUIController targetTab)
    {
        SetOff_WindowAll(panelTabList);

        currentThisPanelTab = targetTab;
        targetTab.panelRt.gameObject.SetActive(true);
        targetTab.tabBtn.ToggleOn_ThisBtn();
    }

    private void SetOff_WindowAll(List<TabEUIController> allWindow)
    {
        for (int i = 0; i < allWindow.Count; i++)
        {
            SetOff_Window(allWindow[i]);
        }
    }

    private void SetOff_Window(TabEUIController targetTab)
    {
        targetTab.panelRt.gameObject.SetActive(false);
        targetTab.tabBtn.ToggleOff_ThisBtn();
    }

    #endregion

    #region Get

    protected List<TMP_Text> Get_AllTabBtn_Txt()
    {
        List<TMP_Text> result = new List<TMP_Text>();
        for (int i = 0; i < panelTabList.Count; i++)
        {
            result.Add(panelTabList[i].tabBtn.txt);
        }
        return result;
    }

    protected List<Image> Get_AllTabBtn_Img()
    {
        List<Image> result = new List<Image>();
        for (int i = 0; i < panelTabList.Count; i++)
        {
            result.Add(panelTabList[i].tabBtn.img);
        }
        return result;
    }



    #endregion
}