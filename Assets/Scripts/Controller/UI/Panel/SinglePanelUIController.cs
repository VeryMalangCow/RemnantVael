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

    // Inven
    [SerializeField] public InventorySlotEUIController CurrentSlotBtn = null;
    [SerializeField] public InventoryItemEUIController CurrentItemBtn = null;

    #endregion

    #endregion

    #region Set Panel

    public virtual void SetOn_ThisPanel()
    {
        // Basic
        MainGameUIManager.currentOpening_UIController = this;
        this.gameObject.SetActive(true);

        // Aim & Mouse
        InputManager.instance.Set_AllPointer(_Aim: false, _Mouse: true);

        // Input
        InputManager.instance.playerInput.SwitchCurrentActionMap(ThisPanelInputMapName);
        InputManager.instance.inputMoveDir = Vector2.zero;

        // Tab Input
        MainGameUIManager.instance.playerHUD_UIController.IsTabInputed = false;
        MainGameUIManager.instance.playerHUD_UIController.SetOff_TabInteract();

    }

    public virtual void SetOff_ThisPanel()
    {
        // Basic
        MainGameUIManager.currentOpening_UIController = null;
        this.gameObject.SetActive(false);

        // Aim & Mouse
        InputManager.instance.Set_AllPointer(_Aim: true, _Mouse: false);

        // Input
        InputManager.instance.playerInput.SwitchCurrentActionMap("Player");

        // Inven
        if (CurrentSlotBtn != null)
            CurrentSlotBtn.Set_SelectedOff();
        
        CurrentSlotBtn = null;
        CurrentItemBtn = null;
    }

    #endregion
}
