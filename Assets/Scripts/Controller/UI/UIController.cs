using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    #region Set

    protected void SetColor(Color _Clr, List<Component> _ApplyCompList)
    {

        for (int i = 0; i < _ApplyCompList.Count; i++)
        {
            Color clr = _Clr;
            if (_ApplyCompList[i].TryGetComponent(out TMP_Text tmp))
            {
                clr.a = tmp.color.a;
                tmp.color = clr;
            }
            else if (_ApplyCompList[i].TryGetComponent(out Image img))
            {
                clr.a = img.color.a;
                img.color = clr;
            }
        }
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

        if (MainGameUIManager.Instance != null)
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.IsTabInputed = false;
            MainGameUIManager.Instance.PlayerHUD_UIController.OffTabInteract();
            
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

    #region Set Tab Btn

    protected void SetTabTxt(List<TMP_Text> _TxtList, List<Component> _ColorComp)
    {
        for (int i = 0; i < _TxtList.Count; i++)
        {
            _TxtList[i].text = PlayerManager.Instance.PlayerController.TabStringList[i];
            _ColorComp.Add(_TxtList[i]);
        }
    }

    protected void SetTabLightAlpha(float _A, List<CanvasGroup> _CG, List<Component> _ColorComp)
    {
        for (int i = 0; i < _CG.Count; i++)
        {
            _CG[i].alpha = _A;
            if (_CG[i].gameObject.TryGetComponent(out Image img))
            { _ColorComp.Add(img); }
        }
    }

    #endregion

    #region Set Window


    private void OpenWindow(ModifyEachTab _TargetTab)
    {
        CloseWindowAll(ThisPanelTabList);
        _TargetTab.ThisPanelRT.gameObject.SetActive(true); 
        if (_TargetTab.ThisTabBtn.transform.GetChild(0).TryGetComponent(out CanvasGroup cg))
        { cg.alpha = 0.5f; }
    }

    private void CloseWindowAll(List<ModifyEachTab> _AllWindow)
    {
        for (int i = 0; i < _AllWindow.Count; i++)
        {
            _AllWindow[i].ThisPanelRT.gameObject.SetActive(false);
            if (_AllWindow[i].ThisTabBtn.transform.GetChild(0).TryGetComponent(out CanvasGroup cg))
            { cg.alpha = 0.1f; }
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