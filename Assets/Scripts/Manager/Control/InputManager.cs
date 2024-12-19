using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    #region Value

    [Header("=== State")]
    [SerializeField] private bool OnAim = true;
    [SerializeField] private bool OnMouse = false;

    [Header("=== Mouse")]
    [SerializeField] private RectTransform MousePointerRT;
    [HideInInspector] public bool CanMouseInput = false;
    [SerializeField] public Vector2 MousePos;
    [SerializeField] public Vector2 MousePosByWorld;
    [SerializeField] public Vector2 DirFromPlayerPos;

    [Header("=== Aim")]
    [SerializeField] public AimController AimController;

    [Header("=== Component")]
    [SerializeField] public PlayerInput PlayerInput;

    [Header("=== Movement")]
    [SerializeField] public Vector2 InputMoveDir;
    [SerializeField] public Vector2Int InputArrowDir;

    [Header("=== First Input")] 
    [SerializeField] public bool IsPlayingSkill = false;
    [SerializeField] private float EndFirstInputTime = 0.5f;
    [SerializeField] private float CurrentFirstInputTime = 0f;


    private delegate void FirstInputDele();
    private FirstInputDele CurrentFirstInputDele = null;

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
    }

    private void Start()
    {
        SetAimAllOff();
    }
    private void FixedUpdate()
    {
        SetMousePos();
    }

    private void Update()
    {
        if (CurrentFirstInputDele != null)
        {
            CurrentFirstInputTime += Time.deltaTime;

            if(CurrentFirstInputTime >= EndFirstInputTime)
            {
                UnsetFirstInput();
            }

            if(!IsPlayingSkill)
            {
                Debug.Log("선입력 실행");
                CurrentFirstInputDele();
                UnsetFirstInput();
            }
        }
    }


    #endregion

    #region State

    public void SetAimAllOff()
    {
        OnAim = false;
        OnMouse = false;

        AimController.gameObject.SetActive(false);
        PlayerManager.Instance.PlayerController.AimRoundController.gameObject.SetActive(false);
        MousePointerRT.gameObject.SetActive(false);
    }

    public void SetAim(bool _IsOn)
    {
        OnAim = _IsOn;
        OnMouse = !_IsOn;

        AimController.gameObject.SetActive(_IsOn);
        PlayerManager.Instance.PlayerController.AimRoundController.gameObject.SetActive(_IsOn);
        MousePointerRT.gameObject.SetActive(!_IsOn);
    }

    #endregion

    #region Mouse

    private void SetMousePos()
    {
        if (!CanMouseInput)
        { return; }

        MousePos = Input.mousePosition;
        MousePosByWorld = Camera.main.ScreenToWorldPoint(MousePos);
        DirFromPlayerPos = MousePosByWorld - (Vector2)PlayerManager.Instance.PlayerController.gameObject.transform.position;

        MousePointerRT.anchoredPosition = MousePos;
    }

    #endregion

    #region Input Set

    public void OnEnableInput()
    {
        if (PlayerManager.Instance.PlayerController.gameObject.TryGetComponent(out PlayerInput PI))
        { PlayerInput = PI; }

        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Arrow"].performed += Input_Arrow;
        PlayerInput.actions["Fire_0"].performed += Input_Fire_0;
        PlayerInput.actions["Dash"].performed += Input_Dash;

        PlayerInput.actions["CombatMode"].performed += Input_CombatMode;
        PlayerInput.actions["BoostMode"].performed += Input_BoostMode;
        PlayerInput.actions["UnBoostMode"].performed += Input_UnBoostMode;
        PlayerInput.actions["ChargeBettery"].performed += Input_ChargeBettery;

        PlayerInput.actions["Skill_0"].performed += Input_Skill_0;
        PlayerInput.actions["Skill_1"].performed += Input_Skill_1;

        PlayerInput.actions["Interact"].performed += Input_Interact;
        PlayerInput.actions["TabInteract"].performed += Input_Tab;
        PlayerInput.actions["OutMainGame"].performed += Input_OMGUI;

        PlayerInput.actions["BUUI_Select"].performed += Input_BUUIClick;
        PlayerInput.actions["BUUI_OutPanel"].performed += Input_BUUIOutPanel;

        PlayerInput.actions["MUUI_Select"].performed += Input_MUUIClick;
        PlayerInput.actions["MUUI_OutPanel"].performed += Input_MUUIOutPanel;

        PlayerInput.actions["OMGUI_Select"].performed += Input_OMGUIClick;
        PlayerInput.actions["OMGUI_OutPanel"].performed += Input_OMGUIOutPanel;
    }

    public void OnDisableInput()
    {
        if (PlayerManager.Instance.PlayerController.gameObject.TryGetComponent(out PlayerInput PI))
        { PlayerInput = PI; }

        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Arrow"].performed -= Input_Arrow;
        PlayerInput.actions["Fire_0"].performed -= Input_Fire_0;
        PlayerInput.actions["Dash"].performed -= Input_Dash;

        PlayerInput.actions["CombatMode"].performed -= Input_CombatMode;
        PlayerInput.actions["BoostMode"].performed -= Input_BoostMode;
        PlayerInput.actions["UnBoostMode"].performed -= Input_UnBoostMode;
        PlayerInput.actions["ChargeBettery"].performed -= Input_ChargeBettery;

        PlayerInput.actions["Skill_0"].performed -= Input_Skill_0;
        PlayerInput.actions["Skill_1"].performed -= Input_Skill_1;

        PlayerInput.actions["Interact"].performed -= Input_Interact;
        PlayerInput.actions["TabInteract"].performed -= Input_Tab;
        PlayerInput.actions["OutMainGame"].performed -= Input_OMGUI;

        PlayerInput.actions["BUUI_Select"].performed -= Input_BUUIClick;
        PlayerInput.actions["BUUI_OutPanel"].performed -= Input_BUUIOutPanel;

        PlayerInput.actions["MUUI_Select"].performed -= Input_MUUIClick;
        PlayerInput.actions["MUUI_OutPanel"].performed -= Input_MUUIOutPanel;

        PlayerInput.actions["OMGUI_Select"].performed -= Input_OMGUIClick;
        PlayerInput.actions["OMGUI_OutPanel"].performed -= Input_OMGUIOutPanel;
    }


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

    #region Movement

    public void Input_Walk(InputAction.CallbackContext _InputValue)
    {
        InputMoveDir = _InputValue.ReadValue<Vector2>().normalized;
    }

    public void Input_Dash(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetFirstInput(PlayerManager.Instance.PlayerController.CanDashCheck);
                return;
            }

            PlayerManager.Instance.PlayerController.CanDashCheck();
        }
    }

    public void Input_Arrow(InputAction.CallbackContext _InputValue)
    {
        Vector2 v2 = _InputValue.ReadValue<Vector2>();
        if (v2.x != 0)
        {
            InputArrowDir = v2.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else if (v2.y != 0)
        {
            InputArrowDir = v2.y > 0 ? Vector2Int.up : Vector2Int.down;
        }
        else
        {
            InputArrowDir = Vector2Int.zero;
        }
    }

    #endregion

    #region Combat

    private void Input_CombatMode(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetFirstInput(PlayerManager.Instance.PlayerController.CanChange_CombatModeCheck);
                return;
            }

            PlayerManager.Instance.PlayerController.CanChange_CombatModeCheck();
        }
    }

    private void Input_BoostMode(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetFirstInput(PlayerManager.Instance.PlayerController.CanChange_BoostModeCheck);
                return;
            }

            PlayerManager.Instance.PlayerController.CanChange_BoostModeCheck(); 
        }
    }

    private void Input_UnBoostMode(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetFirstInput(PlayerManager.Instance.PlayerController.CanChange_UnBoostModeCheck);
                return;
            }

            PlayerManager.Instance.PlayerController.CanChange_UnBoostModeCheck();
        }
    }

    #endregion

    #region Skill

    private void Input_Skill_0(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetFirstInput(PlayerManager.Instance.PlayerController.CanChange_Skill0);
                return;
            }

            PlayerManager.Instance.PlayerController.CanChange_Skill0();
        }
    }

    private void Input_Skill_1(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetFirstInput(PlayerManager.Instance.PlayerController.CanChange_Skill1);
                return;
            }

            PlayerManager.Instance.PlayerController.CanChange_Skill1();
        }
    }

    #endregion

    #region Fire

    private void Input_Fire_0(InputAction.CallbackContext _InputValue)
    {
        PlayerManager.Instance.PlayerController.BaseWeapon.IsInputed = _InputValue.ReadValueAsButton();
    }

    #endregion

    #region Charge Bettery

    // Charge Bettery
    private void Input_ChargeBettery(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingSkill)
            {
                SetFirstInput(PlayerManager.Instance.PlayerController.CanChange_ChargeBettery);
                return;
            }

            PlayerManager.Instance.PlayerController.CanChange_ChargeBettery();
        }
    }

    #endregion

    #region Interact

    private void Input_Interact(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            PlayerManager.Instance.PlayerController.TryInteract();
        }
    }

    #endregion

    #region Tab

    private void Input_Tab(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.IsTabInputed = true;
        }
        else
        {
            MainGameUIManager.Instance.PlayerHUD_UIController.IsTabInputed = false;
        }
    }

    #endregion

    #region BUUI


    private void Input_BUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.TryInteractClick();
        }
    }

    private void Input_BUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.BaseUpgrade_UIController.CloseThisPanel(
                MainGameUIManager.Instance.BaseUpgrade_UIController.TabDurTime);
        }
    }

    #endregion
    
    #region MUUI

    private void Input_MUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.TryInteractClick();
        }
    }


    private void Input_MUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.ModuleUpgrade_UIController.CloseThisPanel(
                MainGameUIManager.Instance.ModuleUpgrade_UIController.TabDurTime);
        }
    }

    #endregion

    #region OMGUI

    private void Input_OMGUI(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.OutMainGame_UIController.OpenThisPanel();
        }
    }


    private void Input_OMGUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.OutMainGame_UIController.TryInteractClick();
        }
    }


    private void Input_OMGUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            MainGameUIManager.Instance.OutMainGame_UIController.CloseThisPanel();
        }
    }

    #endregion
}
