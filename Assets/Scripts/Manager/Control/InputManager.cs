using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    #region Value

    [Header("=== Mouse")]
    [SerializeField] public Vector2 MousePos;
    [SerializeField] public Vector2 MousePosByWorld;
    [SerializeField] public Vector2 DirFromPlayerPos;

    [Header("=== Component")]
    [SerializeField] public PlayerInput PlayerInput;

    [Header("=== Movement")]
    [SerializeField] public Vector2 InputMoveDir;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
        if (PlayerManager.Instance.PlayerController.gameObject.TryGetComponent(out PlayerInput PI))
        { PlayerInput = PI; }
    }

    private void OnEnable()
    {
        OnEnableInput();
    }

    private void OnDisable()
    {
        OnDisableInput();
    }

    private void FixedUpdate()
    {
        SetMousePos();
    }

    #endregion

    #region Mouse

    private void SetMousePos()
    {
        MousePos = Input.mousePosition;
        MousePosByWorld = Camera.main.ScreenToWorldPoint(MousePos);

        DirFromPlayerPos = MousePosByWorld - (Vector2)PlayerManager.Instance.PlayerController.gameObject.transform.position;
    }

    #endregion

    #region Input Set

    private void OnEnableInput()
    {
        PlayerInput.actions["Walk"].Enable();

        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Fire00"].performed += Input_Fire00;
        PlayerInput.actions["Dash"].performed += Input_Dash;
    }

    private void OnDisableInput()
    {
        PlayerInput.actions["Walk"].Disable();

        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Fire00"].performed -= Input_Fire00;
        PlayerInput.actions["Dash"].performed -= Input_Dash;
    }


    #endregion

    #region Movement

    public void Input_Walk(InputAction.CallbackContext _InputValue)
    {
        InputMoveDir = _InputValue.ReadValue<Vector2>().normalized;
    }

    public void Input_Dash(InputAction.CallbackContext _InputValue)
    {
        if(_InputValue.ReadValueAsButton())
        {
            PlayerManager.Instance.PlayerController.CanDashCheck();
        }
    }

    #endregion

    #region Fire

    private void Input_Fire00(InputAction.CallbackContext _InputValue)
    {
        PlayerManager.Instance.PlayerController.BaseWeapon.IsInputed = _InputValue.ReadValueAsButton();
    }

    #endregion
}
