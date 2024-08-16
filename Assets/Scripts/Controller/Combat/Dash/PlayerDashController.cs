using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDashController : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Player")]
    [SerializeField] private PlayerController PlayerController;

    [Space(10)]
    [Header("=== Dash")]
    [SerializeField] protected float CurrentDashProcessTime = 0;
    [SerializeField] public float NeedEP_ForDash = 5f;
    [SerializeField] public Vector2 DashTargetDir;
    [SerializeField] public float DashSpeed = 7f;
    [SerializeField] public float DashDur = 0.2f;

    #endregion


    public void Dash(Vector2 _DashDir, float _TargetDashProcessTime)
    {
        if (CurrentDashProcessTime < _TargetDashProcessTime)
        {
            CurrentDashProcessTime += Time.deltaTime;
            PlayerController.ThisRb.velocity = _DashDir * DashSpeed;
        }
        else
        {
            CurrentDashProcessTime = 0;
            PlayerController.MovementState = eMovementState.IdleOrWalk;
            PlayerController.MakeAfterImage.EndGen();
        }
    }
}
