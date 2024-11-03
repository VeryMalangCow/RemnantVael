using UnityEngine;

public class PlayerDashController : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Player")]
    [SerializeField] private PlayerController PlayerController;

    [Space(10)]
    [Header("=== Dash")]

    [Header("-- Type")]
    [SerializeField] private eDashStyle ThisDashStyle = eDashStyle.OneWay;

    [Header("-- Input")]
    [SerializeField] public float NeedEP_ForDash = 5f;
    [SerializeField] public BaseUpgradeState<float> DashSpeed;
    [SerializeField] public float DashDur = 0.2f;
    [SerializeField] private float RotateLerpValue = 1f;

    [Header("-- Only Caculate")]
    [SerializeField] protected float CurrentDashProcessTime = 0;
    [SerializeField] private Vector2 CaculateDir = Vector2.zero;

    #endregion

    #region Dash

    public void Dash()
    {
        switch (ThisDashStyle)
        {
            case eDashStyle.OneWay:
                if (CaculateDir == Vector2.zero)
                { CaculateDir = InputManager.Instance.DirFromPlayerPos.normalized; }
                Dash_Framework(CaculateDir, DashDur);
                break;

            case eDashStyle.CanInputWay:
                Vector2 targetDir = Vector2.Lerp(PlayerController.ThisRb.velocity.normalized, InputManager.Instance.InputMoveDir, RotateLerpValue * Time.deltaTime);
                Dash_Framework(targetDir, DashDur);
                break;

            case eDashStyle.Teleport:
                Teleport(DashDur);
                break;

            default:
                break;
        }
    }

    #endregion

    #region One Way

    public void Dash_Framework(Vector2 _DashDir, float _TargetDashProcessTime)
    {
        if (CurrentDashProcessTime < _TargetDashProcessTime)
        {
            CurrentDashProcessTime += Time.deltaTime;
            PlayerController.ThisRb.velocity = _DashDir * DashSpeed.ActualState.Value;
        }
        else
        {
            EndDash();
        }
    }

    public void Teleport(float _TargetDashProcessTime)
    {
        if (CurrentDashProcessTime < _TargetDashProcessTime)
        {
            if (CaculateDir == Vector2.zero)
            {
                PlayerController.ThisRb.velocity = Vector2.zero;
                CaculateDir = InputManager.Instance.MousePosByWorld;
            }

            CurrentDashProcessTime += Time.deltaTime;

            if (CurrentDashProcessTime >= _TargetDashProcessTime / 2)
            {
                if (CaculateDir != Vector2.one)
                {
                    PlayerController.gameObject.transform.position = CaculateDir;
                    CaculateDir = Vector2.one;
                }
            }
        }
        else if (CurrentDashProcessTime >= _TargetDashProcessTime)
        {
            EndDash();
        }
    }

    void EndDash()
    {
        CaculateDir = Vector2.zero;
        CurrentDashProcessTime = 0;
        PlayerController.MovementState = eMovementState.IdleOrWalk;
        PlayerController.PlayerMAI.EndGen();

        InputManager.Instance.IsPlayingSkill = false;
    }

    #endregion
}
