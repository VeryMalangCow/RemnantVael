using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleInputManager : Singleton<TitleInputManager>
{
    #region Value

    #region - Inspector

    [Header("=== Mouse")]
    [SerializeField] private Canvas MousePointerCanvas;
    [SerializeField] private RectTransform MousePointerRT;

    #endregion

    #region - Hide

    // Mouse Vec
    [HideInInspector] public Vector2 MousePos; // 현재 마우스 위치

    // Input
    [HideInInspector] public PlayerInput PlayerInput;

    #endregion

    #endregion

    #region 

    #region Offset

    private void Offset()
    {
        if (DevTool.Get_ComponentTType(gameObject, out PlayerInput input))
            PlayerInput = input;
    }

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;

        Offset();
    }

    private void OnEnable()
    {
        SetOn_InputAction();
    }

    private void OnDisable()
    {
        SetOff_InputAction();
    }

    private void Update()
    {
        Set_MousePos();
    }

    #endregion

    #region Mouse

    private void Set_MousePos()
    {
        Vector2 localPoint;

        MousePos = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            MousePointerCanvas.transform as RectTransform,
            MousePos,
            MousePointerCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : MousePointerCanvas.worldCamera,
            out localPoint);

        MousePointerRT.anchoredPosition = localPoint;
    }

    public void Play_MousePointerClick()
    {
        DevTool.Set_KillTween(MousePointerRT);

        Sequence seq = DOTween.Sequence();
        seq.Append(MousePointerRT.DOScale(1.4f, 0.05f));
        seq.Append(MousePointerRT.DOScale(1f, 0.05f));
    }

    public Vector2 Get_AnchorMousePos()
    {
        Vector2 input = MousePointerRT.anchoredPosition;
        float clampedX = Mathf.Clamp(input.x, -1440f, 1440f);
        float clampedY = Mathf.Clamp(input.y, -810f, 810f);
        return new Vector2(clampedX, clampedY);
    }

    #endregion

    #region Input Set

    public void SetOn_InputAction()
    {
        if (DevTool.Get_ComponentTType(gameObject, out PlayerInput input))
            PlayerInput = input;

        // Title
        PlayerInput.actions["TU_Select"].performed += Input_TU_Select;
        PlayerInput.actions["TU_OutPanel"].performed += Input_TU_OutPanel;
    }

    public void SetOff_InputAction()
    {
        if (DevTool.Get_ComponentTType(gameObject, out PlayerInput input))
            PlayerInput = input;

        // Title
        PlayerInput.actions["TU_Select"].performed -= Input_TU_Select;
        PlayerInput.actions["TU_OutPanel"].performed -= Input_TU_OutPanel;
    }

    #endregion

    #region Title UI

    private void Input_TU_Select(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            TitleLobbyUIManager.Instance.TitleLobby_UIController.Try_Interact();
    }

    private void Input_TU_OutPanel(InputAction.CallbackContext _InputValue)
    {
        if (_InputValue.ReadValueAsButton())
            TitleLobbyUIManager.Instance.TitleLobby_UIController.Try_OutInteract();
    }

    #endregion

    #endregion
}
