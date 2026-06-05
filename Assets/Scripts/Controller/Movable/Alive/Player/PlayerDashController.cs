using UnityEngine;

public class PlayerDashController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dash")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private eDashStyle dashStyle = eDashStyle.OneWay;

    [Space(10)]
    [Header("=== BU State")]
    [SerializeField] public BUState<float> dashSpeed;

    // Readonly Value
    [HideInInspector] private readonly float needEP_ForDash = 1f; // 임시
    [HideInInspector] private readonly float dashDur = 0.25f;

    // Caculate
    [HideInInspector] private float currentDashProcessTime = 0;
    [HideInInspector] private bool booked = false;
    [HideInInspector] private Vector2 bookCaculateVec = Vector2.zero;

    // Type: CanInputWay
    [HideInInspector] private readonly float rotLerpValue = 30f;

    // Type: Teleport
    [HideInInspector] private readonly float tpDur = 0.15f;
    [HideInInspector] private bool alreadyTp = false;

    // Controller (Owner)
    [HideInInspector] private PlayerController playerController;

    #endregion

    #region Offset

    public void Offset()
    {
        playerController = PlayerManager.instance.playerController;
    }

    #endregion

    #region Dash

    public void Play_Dash(float deltaTime)
    {
        switch (dashStyle)
        {
            case eDashStyle.OneWay:
                Update_OneWay(deltaTime);
                break;

            case eDashStyle.CanInputWay:
                Update_CanInputWay(deltaTime);
                break;

            case eDashStyle.Teleport:
                Update_Teleport(deltaTime);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Type Dash


    // 직진 대시
    private void Update_OneWay(float deltaTime)
    {
        // 직진 예약 방향이 정하기
        if (!booked)
        {
            // 움직이는 방향으로 대시
            bookCaculateVec = InputManager.instance.inputMoveDir;

            // 마우스 방향으로 대시
            //BookCaculateVec = InputManager.Instance.DirFromPlayerPos.normalized;

            SoundManager.instance.Play_2D_SFX_Player(playerController.Get_AS(), "Dash");
            booked = true;
        }

        if (Is_Dashing()) // 진행
        {
            playerController.rb.velocity = 
                bookCaculateVec * dashSpeed.actualState;

            currentDashProcessTime += deltaTime;
        }
        else // 종료
        {
            End_Dash();
        }
    }

    // 빠른 이동 대시
    private void Update_CanInputWay(float deltaTime)
    {
        if (Is_Dashing())
        {
            playerController.rb.velocity = Vector2.Lerp(
                a: playerController.rb.velocity.normalized,
                b: InputManager.instance.inputMoveDir,
                t: rotLerpValue * deltaTime) * dashSpeed.actualState;

            currentDashProcessTime += deltaTime;
        }
        else
        {
            End_Dash();
        }
    }
    
    // 순간 이동
    public void Update_Teleport(float deltaTime)
    {
        if (!booked)
        {
            playerController.rb.velocity = Vector2.zero;
            bookCaculateVec = InputManager.instance.mousePosByWorld;
        }
        booked = true;

        if (Is_Dashing())
        {
            if (currentDashProcessTime >= tpDur && !alreadyTp) // 텔포 시점
            {
                playerController.gameObject.transform.position = bookCaculateVec;
                alreadyTp = true;
            }

            currentDashProcessTime += deltaTime;
        }
        else
        {
            End_Dash();
        }
    }

    private bool Is_Dashing()
    {
        return currentDashProcessTime < dashDur;
    }

    void End_Dash()
    {
        playerController.afterImgGenerator.End_Gen();
        playerController.movementState = eMovementState.IdleOrWalk;

        // 예약 좌표
        booked = false;
        bookCaculateVec = Vector2.zero;
        currentDashProcessTime = 0;

        InputManager.instance.isPlayingBuffered = false;
    }

    #endregion

    #region Get

    public bool Is_EnoughEP()
    {
        return Get_ActualNeedEP() <= playerController.currentEp;
    }

    public float Get_ActualNeedEP()
    {
        return needEP_ForDash * playerController.needEP_ForSkillMultiple.actualState;
    }

    #endregion
}
