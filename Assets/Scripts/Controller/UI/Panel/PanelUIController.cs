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
    [SerializeField] protected List<TabEUIController> ThisPanelTabList;

    #endregion

    #region - Hide

    // Tab
    [HideInInspector] protected TabEUIController CurrentThisPanelTab;

    #endregion

    #endregion

    #region Set Panel

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        // Actual Tab
        SetOn_Window(ThisPanelTabList[0]);
    }

    public virtual void Change_ThisPanel(int _indexWindow)
    {
        //Other
        if (CurrentThisPanelTab == ThisPanelTabList[_indexWindow]) return; 

        SetOn_Window(ThisPanelTabList[_indexWindow]);
    }

    #endregion

    #region On/Off Tab Window

    private void SetOn_Window(TabEUIController _TargetTab)
    {
        SetOff_WindowAll(ThisPanelTabList);

        CurrentThisPanelTab = _TargetTab;
        _TargetTab.ThisPanelRT.gameObject.SetActive(true);
        _TargetTab.ThisTabBtn.ToggleOn_ThisBtn();
    }

    private void SetOff_WindowAll(List<TabEUIController> _AllWindow)
    {
        for (int i = 0; i < _AllWindow.Count; i++)
        {
            SetOff_Window(_AllWindow[i]);
        }
    }

    private void SetOff_Window(TabEUIController _TargetTab)
    {
        _TargetTab.ThisPanelRT.gameObject.SetActive(false);
        _TargetTab.ThisTabBtn.ToggleOff_ThisBtn();
    }

    #endregion

    #region Get

    protected List<TMP_Text> Get_AllTabBtn_Txt()
    {
        List<TMP_Text> result = new List<TMP_Text>();
        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            result.Add(ThisPanelTabList[i].ThisTabBtn.ThisTxt);
        }
        return result;
    }

    protected List<Image> Get_AllTabBtn_Img()
    {
        List<Image> result = new List<Image>();
        for (int i = 0; i < ThisPanelTabList.Count; i++)
        {
            result.Add(ThisPanelTabList[i].ThisTabBtn.ThisImg);
        }
        return result;
    }

    protected Vector2 Get_PanelLocalPoint()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            DevTool.Get_ComponentTType<RectTransform>(gameObject), // 변환할 UI(RectTransform)
            InputManager.Instance.MousePos, // 현재 마우스 좌표 (Screen Space)
            MainGameUIManager.Instance.UICamera, // Canvas의 카메라 (Render Mode 따라 null 가능)
            out Vector2 localPoint); // 변환된 Local 좌표

        return localPoint;
    }

    #endregion
}