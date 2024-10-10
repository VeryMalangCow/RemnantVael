using UnityEngine;
using UnityEngine.InputSystem;

public class InputTitleManager : Singleton<InputTitleManager>
{
    #region Value

    [Header("=== Component")]
    [SerializeField] public PlayerInput PlayerInput;

    [Header("=== Movement")]
    [SerializeField] public Vector2 InputMoveDir;

    [Header("=== First Input")]
    [SerializeField] public bool IsPlayingSkill = false;
    [SerializeField] private float EndFirstInputTime = 0.5f;
    [SerializeField] private float CurrentFirstInputTime = 0f;

    private delegate void FirstInputDele();
    private FirstInputDele CurrentFirstInputDele = null;

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
    private void Update()
    {
        if (CurrentFirstInputDele != null)
        {
            CurrentFirstInputTime += Time.deltaTime;

            if (CurrentFirstInputTime >= EndFirstInputTime)
            {
                UnsetFirstInput();
            }

            if (!IsPlayingSkill)
            {
                Debug.Log("선입력 실행");
                CurrentFirstInputDele();
                UnsetFirstInput();
            }
        }
    }
    #endregion

    #region FirstInput
    private void SetFirstInput(FirstInputDele _Skill)
    {
        CurrentFirstInputDele = _Skill;
        CurrentFirstInputTime = 0f;
    }

    private void UnsetFirstInput()
    {
        CurrentFirstInputDele = null;
        CurrentFirstInputTime = 0f;
    }

    #endregion

    #region On Enable / Disable

    private void OnEnableInput()
    {
        PlayerInput.actions["Walk"].performed += Input_InTitleUI;
        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Walk"].performed += Input_Interact;

        PlayerInput.actions["Walk"].performed += Input_OutTitleUI;
        PlayerInput.actions["Walk"].performed += Input_TU_Select;
    }

    private void OnDisableInput()
    {
        PlayerInput.actions["Walk"].performed -= Input_InTitleUI;
        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Walk"].performed -= Input_Interact;

        PlayerInput.actions["Walk"].performed -= Input_OutTitleUI;
        PlayerInput.actions["Walk"].performed -= Input_TU_Select;
    }

    #endregion

    #region Input


    private void Input_InTitleUI(InputAction.CallbackContext _InputValue)
    {

    }

    private void Input_OutTitleUI(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                //SetFirstInput(PlayerManager.Instance.PlayerController.CanChange_ChargeBettery);

                return;
            }

            //PlayerManager.Instance.PlayerController.CanChange_ChargeBettery();
        }

    }
    private void Input_Walk(InputAction.CallbackContext _InputValue)
    {

    }

    private void Input_Interact(InputAction.CallbackContext _InputValue)
    {

    }

    private void Input_TU_Select(InputAction.CallbackContext _InputValue)
    {

    }


    #endregion
}
