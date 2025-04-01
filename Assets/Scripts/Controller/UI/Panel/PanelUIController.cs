using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelUIController : UIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Panel")]

    [Space(10)]
    [Header("=== Input Map")]
    [SerializeField] protected string ThisPanelInputMapName;

    [Space(10)]
    [Header("=== Tab")]
    [SerializeField] protected List<TabEUIController> ThisPanelTabList;

    #endregion
    
    #region - Hide

    // Tab
    [HideInInspector] protected TabEUIController CurrentThisPanelTab;

    // Btn
    [HideInInspector] public OwnBtnEUIController CurrentBtn = null;

    // Visual
    [HideInInspector] public List<Component> MainColorCompList = new List<Component>();
    [HideInInspector] public List<Component> SubColorCompList = new List<Component>();

    #endregion

    #endregion

    #region Set Panel

    public virtual void SetOn_ThisPanel()
    {
        // Basic
        MainGameUIManager.Instance.CurrentOpening_UIController = this;
        this.gameObject.SetActive(true);

        // Aim & Mouse
        InputManager.Instance.Set_AllPointer(_Aim: false, _Mouse: true);

        // Input
        InputManager.Instance.PlayerInput.SwitchCurrentActionMap(ThisPanelInputMapName);
        InputManager.Instance.InputMoveDir = Vector2.zero;

        // Tab Input
        MainGameUIManager.Instance.PlayerHUD_UIController.IsTabInputed = false;
        MainGameUIManager.Instance.PlayerHUD_UIController.SetOff_TabInteract();

        // Actual Tab
        SetOn_Window(ThisPanelTabList[0]);

    }

    public virtual void Change_ThisPanel(int _indexWindow)
    {
        //Other
        if (CurrentThisPanelTab == ThisPanelTabList[_indexWindow]) return; 

        SetOn_Window(ThisPanelTabList[_indexWindow]);
    }

    public virtual void SetOff_ThisPanel()
    {
        // Basic
        MainGameUIManager.Instance.CurrentOpening_UIController = null;
        this.gameObject.SetActive(false);

        // Aim & Mouse
        InputManager.Instance.Set_AllPointer(_Aim: true, _Mouse: false);

        // Input
        InputManager.Instance.PlayerInput.SwitchCurrentActionMap("Player");
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