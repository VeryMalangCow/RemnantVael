using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePanelUIController : UIController
{

    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Single")]

    [Space(10)]
    [Header("=== Input Map")]
    [SerializeField] protected string ThisPanelInputMapName;

    #endregion

    #region - Hide

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
}
