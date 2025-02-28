using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Populer")]
    [SerializeField] protected string ThisPanelInputMapName;

    [SerializeField] protected List<ModifyEachTab> ThisPanelTabList;
    [SerializeField] protected ModifyEachTab CurrentThisPanelTab;

    [SerializeField] public ModifyOwnEachBtn CurrentBtn = null;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {

    }

    protected override void Offset_UI()
    {

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
            if (this is BaseUpgradeUIController)
            { _TxtList[i].text = PlayerManager.Instance.PlayerController.BUUITabStringList[i]; }
            else if (this is ModuleUpgradeUIController)
            { _TxtList[i].text = PlayerManager.Instance.PlayerController.MUUITabStringList[i]; }

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

}
