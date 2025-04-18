using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    #region Value

    #region - Inspector

    [Header("=== Mouse")]
    [SerializeField] private RectTransform MousePointerRT;

    [Header("=== Buffered")]
    [Tooltip("선입력 가능 시간의 적용(최대) 시간")]
    [SerializeField] private float EndBufferedInputTime = 0.5f;

    #endregion

    #region - Hide

    // Buffered
    [HideInInspector] public bool IsPlayingBuffered = false;
    [HideInInspector] private float CurrentBufferedInputTime = 0f;

    // Mouse Vec
    [HideInInspector] public bool CanMouseInput = false;
    [HideInInspector] public Vector2 MousePos; // 현재 마우스 위치
    [HideInInspector] public Vector2 MousePosByWorld; // 세상 기준 마우스 위치
    [HideInInspector] public Vector2 DirFromPlayerPos; // 플레이어부터 마우스까지의 Vec

    // Movement Vec
    [HideInInspector] public Vector2 InputMoveDir;
    [HideInInspector] public Vector2Int InputArrowDir;

    // Input
    [HideInInspector] public PlayerInput PlayerInput;

    // Aim
    [HideInInspector] public AimController AimController;
    [HideInInspector] public AimRoundController AimRoundController;

    // Dele
    [HideInInspector] private Dele CurrentBufferedDele = null;

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        Set_AllPointer(false);
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
    }

    private void Start()
    {
        Offset();
    }

    private void FixedUpdate()
    {
        Set_MousePos();
    }

    private void LateUpdate()
    {
        Caculate_BufferedInput(Time.deltaTime);
    }

    #endregion

    #region Buffered Input

    private void Caculate_BufferedInput(float _DeltaTime)
    {
        if (CurrentBufferedDele != null)
        {
            CurrentBufferedInputTime += Time.deltaTime;

            if (CurrentBufferedInputTime >= EndBufferedInputTime)
            {
                SetOff_BufferedInput();
            }

            if (!IsPlayingBuffered && CurrentBufferedDele != null)
            {
                CurrentBufferedDele();
                SetOff_BufferedInput();
            }
        }
    }

    private void SetOn_BufferedInput(Dele _Buffered)
    {
        CurrentBufferedDele = _Buffered;
        CurrentBufferedInputTime = 0f;
    }

    private void SetOff_BufferedInput()
    {
        CurrentBufferedDele = null;
        CurrentBufferedInputTime = 0f;
    }

    private void Play_BuffedApplyInput(Dele _Func)
    {
        if (IsPlayingBuffered) SetOn_BufferedInput(_Func);
        else _Func();
    }

    #endregion

    #region Aim & Mouse

    public void Set_AllPointer(bool _OnOff)
    {
        Set_AllPointer(_OnOff, _OnOff);
    }

    public void Set_AllPointer(bool _Aim, bool _Mouse)
    {
        Set_AimPointer(_Aim);
        Set_MousePointer(_Mouse);
    }

    private void Set_AimPointer(bool _IsOn)
    {
        AimController.gameObject.SetActive(_IsOn);
        AimRoundController.gameObject.SetActive(_IsOn);
    }

    private void Set_MousePointer(bool _IsOn)
    {
        MousePointerRT.gameObject.SetActive(_IsOn);
    }


    #endregion

    #region Mouse

    private void Set_MousePos()
    {
        if (!CanMouseInput) return; 

        MousePos = Input.mousePosition;
        MousePosByWorld = Camera.main.ScreenToWorldPoint(MousePos);
        DirFromPlayerPos = MousePosByWorld - (Vector2)PlayerManager.Instance.PlayerController.gameObject.transform.position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            DevTool.Get_ComponentTType<RectTransform>(MousePointerRT.transform.parent.gameObject), // 변환할 UI(RectTransform)
            MousePos, // 현재 마우스 좌표 (Screen Space)
            MainGameUIManager.Instance.UICamera, // Canvas의 카메라 (Render Mode 따라 null 가능)
            out Vector2 localPoint); // 변환된 Local 좌표

        MousePointerRT.anchoredPosition = localPoint;
    }

    public void Play_MousePointerClick()
    {
        DevTool.Set_KillTween(MousePointerRT);

        Sequence seq = DOTween.Sequence();
        seq.Append(MousePointerRT.DOScale(1.4f, 0.05f));
        seq.Append(MousePointerRT.DOScale(1f, 0.05f));
    }

    #endregion

    #region Input Set

    public void SetOn_InputAction()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.Instance.PlayerController.gameObject, out PlayerInput input))
            PlayerInput = input; 

        PlayerInput.actions["Walk"].performed += Input_Walk;
        PlayerInput.actions["Arrow"].performed += Input_Arrow;
        PlayerInput.actions["Fire"].performed += Input_Fire;
        PlayerInput.actions["Dash"].performed += Input_Dash;

        PlayerInput.actions["CombatMode"].performed += Input_CombatMode;
        PlayerInput.actions["ChargeBettery"].performed += Input_ChargeBettery;

        PlayerInput.actions["Skill_0"].performed += Input_Skill_0;
        PlayerInput.actions["Skill_1"].performed += Input_Skill_1;

        PlayerInput.actions["STAlly"].performed += Input_STAlly;
        PlayerInput.actions["UTAlly"].performed += Input_UTAlly;
        PlayerInput.actions["NTAlly"].performed += Input_NTAlly;

        PlayerInput.actions["Interact"].performed += Input_Interact;
        PlayerInput.actions["TabInteract"].performed += Input_Tab;
        PlayerInput.actions["OutMainGame"].performed += Input_OMGUI;

        PlayerInput.actions["BUUI_Select"].performed += Input_BUUIClick;
        PlayerInput.actions["BUUI_OutPanel"].performed += Input_BUUIOutPanel;

        PlayerInput.actions["MUUI_Select"].performed += Input_MUUIClick;
        PlayerInput.actions["MUUI_SelectSub"].performed += Input_MUUIClickSub;
        PlayerInput.actions["MUUI_OutPanel"].performed += Input_MUUIOutPanel;
        PlayerInput.actions["MUUI_Drag"].performed += Input_MUUIDrag;

        PlayerInput.actions["AllyCard_Select"].performed += Input_AllyCardClick;

        PlayerInput.actions["OMGUI_Select"].performed += Input_OMGUIClick;
        PlayerInput.actions["OMGUI_OutPanel"].performed += Input_OMGUIOutPanel;
    }

    public void SetOff_InputAction()
    {
        if (DevTool.Get_ComponentTType(
            PlayerManager.Instance.PlayerController.gameObject, out PlayerInput input))
            PlayerInput = input;

        PlayerInput.actions["Walk"].performed -= Input_Walk;
        PlayerInput.actions["Arrow"].performed -= Input_Arrow;
        PlayerInput.actions["Fire"].performed -= Input_Fire;
        PlayerInput.actions["Dash"].performed -= Input_Dash;

        PlayerInput.actions["CombatMode"].performed -= Input_CombatMode;
        PlayerInput.actions["ChargeBettery"].performed -= Input_ChargeBettery;

        PlayerInput.actions["Skill_0"].performed -= Input_Skill_0;
        PlayerInput.actions["Skill_1"].performed -= Input_Skill_1;

        PlayerInput.actions["STAlly"].performed -= Input_STAlly;
        PlayerInput.actions["UTAlly"].performed -= Input_UTAlly;
        PlayerInput.actions["NTAlly"].performed -= Input_NTAlly;

        PlayerInput.actions["Interact"].performed -= Input_Interact;
        PlayerInput.actions["TabInteract"].performed -= Input_Tab;
        PlayerInput.actions["OutMainGame"].performed -= Input_OMGUI;

        PlayerInput.actions["BUUI_Select"].performed -= Input_BUUIClick;
        PlayerInput.actions["BUUI_OutPanel"].performed -= Input_BUUIOutPanel;

        PlayerInput.actions["MUUI_Select"].performed -= Input_MUUIClick;
        PlayerInput.actions["MUUI_SelectSub"].performed -= Input_MUUIClickSub;
        PlayerInput.actions["MUUI_OutPanel"].performed -= Input_MUUIOutPanel;
        PlayerInput.actions["MUUI_Drag"].performed -= Input_MUUIDrag;

        PlayerInput.actions["AllyCard_Select"].performed -= Input_AllyCardClick;

        PlayerInput.actions["OMGUI_Select"].performed -= Input_OMGUIClick;
        PlayerInput.actions["OMGUI_OutPanel"].performed -= Input_OMGUIOutPanel;
    }

    #endregion

    #region Player

    #region Movement

    public void Input_Walk(InputAction.CallbackContext _InputValue)
    {
        InputMoveDir = _InputValue.ReadValue<Vector2>().normalized;
    }

    public void Input_Dash(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
        {
            if (IsPlayingBuffered)
            {
                SetOn_BufferedInput(PlayerManager.Instance.PlayerController.Try_Dash);
                return;
            }

            PlayerManager.Instance.PlayerController.Try_Dash();
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
            Play_BuffedApplyInput(
                PlayerManager.Instance.PlayerController.Try_CombatModeCheck);
    }

    #endregion

    #region Skill

    private void Input_Skill_0(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.Instance.PlayerController.Try_Skill0);
    }

    private void Input_Skill_1(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.Instance.PlayerController.Try_Skill1);
    }

    #endregion

    #region Ally

    private void Input_STAlly(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            PlayerManager.Instance.PlayerController.Try_STAllyLvUp();
    }

    private void Input_UTAlly(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            PlayerManager.Instance.PlayerController.Try_UTAllyLvUp();
    }

    private void Input_NTAlly(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            PlayerManager.Instance.PlayerController.Try_NTAllyLvUp();
    }

    #endregion

    #region Fire

    private void Input_Fire(InputAction.CallbackContext _InputValue)
    {
        PlayerManager.Instance.PlayerController.BaseWeapon.IsInputed = _InputValue.ReadValueAsButton();
    }

    #endregion

    #region Charge Bettery

    private void Input_ChargeBettery(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            Play_BuffedApplyInput(
                PlayerManager.Instance.PlayerController.Try_ChargeBettery);
    }

    #endregion

    #region Interact

    private void Input_Interact(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            PlayerManager.Instance.PlayerController.Try_Interact();
    }


    #endregion

    #region Tab

    private void Input_Tab(InputAction.CallbackContext _InputValue)
    {
        MainGameUIManager.Instance.PlayerHUD_UIController.IsTabInputed = 
            _InputValue.ReadValueAsButton();
    }

    #endregion

    #endregion

    #region BaseUpgrade UI

    private void Input_BUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.BaseUpgrade_UIController.Try_Interact();
    }

    private void Input_BUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.BaseUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion
    
    #region ModuleUpgrade UI

    private void Input_MUUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Try_Interact();
    }
    private void Input_MUUIClickSub(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Try_InteractSub();
    }
    private void Input_MUUIDrag(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Try_InteractDragOn();
        else
            MainGameUIManager.Instance.ModuleUpgrade_UIController.Try_InteractDragOff();
    }

    private void Input_MUUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.ModuleUpgrade_UIController.SetOff_ThisPanel();
    }

    #endregion

    #region AllyCard UI

    private void Input_AllyCardClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.AllyCard_UIController.Try_Interact();
    }

    #endregion

    #region OutMainGame UI

    private void Input_OMGUI(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.OutMainGame_UIController.SetOn_ThisPanel();
    }

    private void Input_OMGUIClick(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.OutMainGame_UIController.Try_Interact();
    }

    private void Input_OMGUIOutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            MainGameUIManager.Instance.OutMainGame_UIController.SetOff_ThisPanel();
    }

    #endregion
}
