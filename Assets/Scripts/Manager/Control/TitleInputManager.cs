using UnityEngine;
using UnityEngine.InputSystem;

public class TitleInputManager : Singleton<TitleInputManager>
{
    #region Value

    [Header("=== Component")]
    [SerializeField] public PlayerInput PlayerInput;

    [Header("=== Movement")]
    [SerializeField] public Vector2 InputMoveDir;

    #endregion

    #region Input Set

    public void SetOn_InputActive()
    {
        if (TitlePlayerManager.Instance.PlayerController.gameObject.TryGetComponent(out PlayerInput PI))
        { PlayerInput = PI; }

        PlayerInput.actions["InTitleUI"].performed += Input_InTitleUI;
        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Interact"].performed += Input_Interact;

        PlayerInput.actions["OutTitleUI"].performed += Input_OutTitleUI;
        PlayerInput.actions["TU_Select"].performed += Input_TU_Select;

        PlayerInput.actions["CC_Select"].performed += Input_CC_Select;
        PlayerInput.actions["CC_OutPanel"].performed += Input_CC_OutPanel;

        PlayerInput.actions["Et_Select"].performed += Input_Et_Select;
        PlayerInput.actions["Et_OutPanel"].performed += Input_Et_OutPanel;
    }

    public void SetOff_InputActive()
    {
        if (TitlePlayerManager.Instance.PlayerController.gameObject.TryGetComponent(out PlayerInput PI))
        { PlayerInput = PI; }

        PlayerInput.actions["InTitleUI"].performed -= Input_InTitleUI;
        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Interact"].performed -= Input_Interact;

        PlayerInput.actions["OutTitleUI"].performed -= Input_OutTitleUI;
        PlayerInput.actions["TU_Select"].performed -= Input_TU_Select;

        PlayerInput.actions["CC_Select"].performed -= Input_CC_Select;
        PlayerInput.actions["CC_OutPanel"].performed -= Input_CC_OutPanel;

        PlayerInput.actions["Et_Select"].performed -= Input_Et_Select;
        PlayerInput.actions["Et_OutPanel"].performed -= Input_Et_OutPanel;
    }

    #endregion

    #region Player

    private void Input_InTitleUI(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.TitleLobby_UIController.SetOn_ThisPanel();
        }
    }

    private void Input_Walk(InputAction.CallbackContext _InputValue)
    {
        InputMoveDir = _InputValue.ReadValue<Vector2>().normalized;
    }

    private void Input_Interact(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitlePlayerManager.Instance.PlayerController.Try_Interact();
        }
    }

    #endregion

    #region Title UI

    private void Input_OutTitleUI(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.TitleLobby_UIController.SetOff_ThisPanel();
        }
    }

    private void Input_TU_Select(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.TitleLobby_UIController.Try_Interact();
        }
    }

    #endregion

    #region ChoiceChar UI

    private void Input_CC_Select(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.ChoiceCharacter_UIController.Try_Interact();
        }
    }

    private void Input_CC_OutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.ChoiceCharacter_UIController.SetOff_ThisPanel();
        }
    }

    #endregion

    #region Entrance UI

    private void Input_Et_Select(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.EntranceSpace_UIController.Try_Interact();
        }
    }
    private void Input_Et_OutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.EntranceSpace_UIController.SetOff_ThisPanel();
        }
    }

    #endregion
}
