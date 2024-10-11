using UnityEngine;
using UnityEngine.InputSystem;

public class InputTitleManager : Singleton<InputTitleManager>
{
    #region Value

    [Header("=== Component")]
    [SerializeField] public PlayerInput PlayerInput;

    [Header("=== Movement")]
    [SerializeField] public Vector2 InputMoveDir;

    #endregion

    #region Framework

    private void OnEnable()
    {
        if (TitlePlayerManager.Instance.PlayerController.gameObject.TryGetComponent(out PlayerInput PI))
        { PlayerInput = PI; }
        OnEnableInput();
    }
    private void OnDisable()
    {
        OnDisableInput();
    }

    #endregion

    #region On Enable / Disable

    private void OnEnableInput()
    {
        PlayerInput.actions["InTitleUI"].performed += Input_InTitleUI;
        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Interact"].performed += Input_Interact;

        PlayerInput.actions["OutTitleUI"].performed += Input_OutTitleUI;
        PlayerInput.actions["TU_Select"].performed += Input_TU_Select;
    }

    private void OnDisableInput()
    {
        PlayerInput.actions["InTitleUI"].performed -= Input_InTitleUI;
        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Interact"].performed -= Input_Interact;

        PlayerInput.actions["OutTitleUI"].performed -= Input_OutTitleUI;
        PlayerInput.actions["TU_Select"].performed -= Input_TU_Select;
    }

    #endregion

    #region Input


    private void Input_InTitleUI(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.TitleLobby_UIController.OpenThisPanel();
        }
    }

    private void Input_OutTitleUI(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.TitleLobby_UIController.CloseThisPanel();
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
            TitlePlayerManager.Instance.PlayerController.TryInteract();
        }
    }

    private void Input_TU_Select(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            TitleLobbyUIManager.Instance.TitleLobby_UIController.TitleInput();
        }
    }


    #endregion
}
