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
    [SerializeField] protected string panelInputMapName;

    #endregion

    #region - Hide

    // Btn
    [HideInInspector] public OwnBtnEUIController currentBtn = null;

    // Visual
    [HideInInspector] public List<Component> mainColorCompList = new List<Component>();
    [HideInInspector] public List<Component> subColorCompList = new List<Component>();

    // Inven
    [SerializeField] public InventorySlotEUIController currentSlotBtn = null;
    [SerializeField] public InventoryItemEUIController currentItemBtn = null;

    #endregion

    #endregion

    #region Set Panel

    public virtual void SetOn_ThisPanel()
    {
        // Basic
        MainGameUIManager.currentOpening_UIController = this;
        this.gameObject.SetActive(true);

        // Aim & Mouse
        InputManager.instance.Set_AllPointer(aim: false, mouse: true);

        // Input
        InputManager.instance.playerInput.SwitchCurrentActionMap(panelInputMapName);
        InputManager.instance.inputMoveDir = Vector2.zero;

        // Tab Input
        MainGameUIManager.instance.playerHUD_UIController.isTabInputed = false;
        MainGameUIManager.instance.playerHUD_UIController.SetOff_TabInteract();

    }

    public virtual void SetOff_ThisPanel()
    {
        // Basic
        MainGameUIManager.currentOpening_UIController = null;
        this.gameObject.SetActive(false);

        // Aim & Mouse
        InputManager.instance.Set_AllPointer(aim: true, mouse: false);

        // Input
        InputManager.instance.playerInput.SwitchCurrentActionMap("Player");

        // Inven
        if (currentSlotBtn != null)
            currentSlotBtn.Set_SelectedOff();
        
        currentSlotBtn = null;
        currentItemBtn = null;
    }

    #endregion
}
