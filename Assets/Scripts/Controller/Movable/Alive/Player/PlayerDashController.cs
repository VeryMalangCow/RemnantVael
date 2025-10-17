using UnityEngine;

public class PlayerDashController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dash")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private eDashStyle ThisDashStyle = eDashStyle.OneWay;

    [Space(10)]
    [Header("=== BU State")]
    [SerializeField] public BUState<float> DashSpeed;

    // Readonly Value
    [HideInInspector] private readonly float NeedEP_ForDash = 1f; // 임시
    [HideInInspector] private readonly float DashDur = 0.25f;

    // Caculate
    [HideInInspector] private float CurrentDashProcessTime = 0;
    [HideInInspector] private bool Booked = false;
    [HideInInspector] private Vector2 BookCaculateVec = Vector2.zero;

    // Type: CanInputWay
    [HideInInspector] private readonly float RotateLerpValue = 30f;

    // Type: Teleport
    [HideInInspector] private readonly float TpDur = 0.15f;
    [HideInInspector] private bool AlreadyTp = false;

    // Controller (Owner)
    [HideInInspector] private PlayerController PlayerController;

    #endregion

    #region Offset

    public void Offset()
    {
        PlayerController = PlayerManager.Instance.PlayerController;
    }

    #endregion

    #region Dash

    public void Play_Dash(float _DeltaTime)
    {
        switch (ThisDashStyle)
        {
            case eDashStyle.OneWay:
                Update_OneWay(_DeltaTime);
                break;

            case eDashStyle.CanInputWay:
                Update_CanInputWay(_DeltaTime);
                break;

            case eDashStyle.Teleport:
                Update_Teleport(_DeltaTime);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Type Dash


    // 직진 대시
    private void Update_OneWay(float _DeltaTime)
    {
        // 직진 예약 방향이 정하기
        if (!Booked)
        {
            BookCaculateVec = InputManager.Instance.DirFromPlayerPos.normalized;
            SoundManager.Instance.Play_2D_SFX_Player(PlayerController.Get_AS(), "Dash");
            Booked = true;
        }

        if (Is_Dashing()) // 진행
        {
            PlayerController.ThisRb.velocity = 
                BookCaculateVec * DashSpeed.ActualState.Value;

            CurrentDashProcessTime += _DeltaTime;
        }
        else // 종료
        {
            End_Dash();
        }
    }

    // 빠른 이동 대시
    private void Update_CanInputWay(float _DeltaTime)
    {
        if (Is_Dashing())
        {
            PlayerController.ThisRb.velocity = Vector2.Lerp(
                a: PlayerController.ThisRb.velocity.normalized,
                b: InputManager.Instance.InputMoveDir,
                t: RotateLerpValue * _DeltaTime) * DashSpeed.ActualState.Value;

            CurrentDashProcessTime += _DeltaTime;
        }
        else
        {
            End_Dash();
        }
    }
    
    // 순간 이동
    public void Update_Teleport(float _DeltaTime)
    {
        if (!Booked)
        {
            PlayerController.ThisRb.velocity = Vector2.zero;
            BookCaculateVec = InputManager.Instance.MousePosByWorld;
        }
        Booked = true;

        if (Is_Dashing())
        {
            if (CurrentDashProcessTime >= TpDur && !AlreadyTp) // 텔포 시점
            {
                PlayerController.gameObject.transform.position = BookCaculateVec;
                AlreadyTp = true;
            }

            CurrentDashProcessTime += _DeltaTime;
        }
        else
        {
            End_Dash();
        }
    }

    private bool Is_Dashing()
    {
        return CurrentDashProcessTime < DashDur;
    }

    void End_Dash()
    {
        PlayerController.AfterImgGenerator.End_Gen();
        PlayerController.MovementState = eMovementState.IdleOrWalk;

        // 예약 좌표
        Booked = false;
        BookCaculateVec = Vector2.zero;
        CurrentDashProcessTime = 0;

        InputManager.Instance.IsPlayingBuffered = false;
    }

    #endregion

    #region Get

    public bool Is_EnoughEP()
    {
        return Get_ActualNeedEP() <= PlayerController.Get_CurrentEP().Value;
    }

    public float Get_ActualNeedEP()
    {
        return NeedEP_ForDash * PlayerController.NeedEP_ForSkillMultiple.ActualState.Value;
    }

    #endregion
}
