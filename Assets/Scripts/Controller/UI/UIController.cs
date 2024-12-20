using System.Collections.Generic;
using UnityEngine;

public abstract class UIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Populer")]
    [SerializeField] protected List<ModifyEachTab> ThisPanelTabList;
    [SerializeField] protected ModifyEachTab CurrentThisPanelTab;
    [SerializeField] protected string ThisPanelInputMapName;

    [SerializeField] public ModifyOwnEachBtn CurrentBtn = null;

    #endregion

    #region Abstract Function

    protected abstract void Offset_Module();
    protected abstract void Offset_UI();

    #endregion

    #region Framework

    public virtual void Offset_Main()
    {
        Offset_Module();
        Offset_UI();
    }

    #endregion

    #region Set Panel

    public virtual void OpenThisPanel()
    {
        // Other
        MainGameUIManager.Instance.CurrentOpening_UIController = this;
        InputManager.Instance.InputMoveDir = Vector2.zero;
        InputManager.Instance.PlayerInput.SwitchCurrentActionMap(ThisPanelInputMapName);
        InputManager.Instance.SetAim(false);

        this.gameObject.SetActive(true);

        if (ThisPanelTabList != null && ThisPanelTabList.Count > 0)
        {
            CurrentThisPanelTab = ThisPanelTabList[0];
            OpenWindow(ThisPanelTabList[0]);
        }
    }

    public virtual void ChangeThisPanel(int _indexWindow)
    {
        //Other
        if (CurrentThisPanelTab == ThisPanelTabList[_indexWindow])
        { return; }


        if (ThisPanelTabList != null && ThisPanelTabList.Count > 0)
        {
            CurrentThisPanelTab = ThisPanelTabList[_indexWindow];
            OpenWindow(ThisPanelTabList[_indexWindow]);
        }
    }

    public virtual void CloseThisPanel()
    {
        // Seq
        this.gameObject.SetActive(false);
        MainGameUIManager.Instance.CurrentOpening_UIController = null;
        InputManager.Instance.PlayerInput.SwitchCurrentActionMap("Player");
        InputManager.Instance.SetAim(true);
    }

    #endregion

    #region Set Window


    private void OpenWindow(ModifyEachTab _TargetTab)
    {
        CloseWindowAll(ThisPanelTabList);
        _TargetTab.ThisPanelRT.gameObject.SetActive(true);
    }

    private void CloseWindowAll(List<ModifyEachTab> _AllWindow)
    {
        for (int i = 0; i < _AllWindow.Count; i++)
        {
            _AllWindow[i].ThisPanelRT.gameObject.SetActive(false);
        }
    }

    #endregion

    #region For UI Case

    protected string GetKindOfCaseString(IInteract _II)
    {
        if (_II == null)
        { return ""; }
        if (_II is EnemyController EC && EC.IsLethargy)
        { return "KILL"; }
        else if (_II is DestructibleBuildingController DBC && !DBC.IsBroken && (_II is BaseUpgradeController || _II is ModuleUpgradeController))
        { return "SHOP"; }
        else if (_II is GateController GC && GC.IsOpen)
        { return "GATE"; }
        else if (_II is InteractItemController)
        { return "MODULE"; }
        else if (_II is DownstartElevatorController DEC && DEC.IsOn)
        { return "NEXT STAGE"; }

        return "";
    }

    #endregion
}