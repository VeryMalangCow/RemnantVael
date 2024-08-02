using System.Collections.Generic;
using UnityEngine;

public class MovableObject : HaveShadowThing
{
    #region Value
    [Space(20)] [Header("<><><><><> Movable Object")]

    [Header("=== Component")]
    [SerializeField] protected Rigidbody2D ThisRb;
    [SerializeField] public SpriteRenderer ThisSr;
    [SerializeField] private List<SpriteRenderer> ThisExtraSrs;

    [Header("=== Movement")]
    [Header("-- Walk")]
    [SerializeField] protected float AccelerationSpeed = 12;

    [Header("-- Dash")]
    [SerializeField] protected float CurrentDashProcessTime = 0;

    #endregion

    #region Movement

    protected void Walk(Vector2 _MoveDir, float _MoveSpeed, float _AccelerationSpeed)
    {
        Vector2 moveVelocity = _MoveDir * _MoveSpeed;
        Vector2 currentVelocity = ThisRb.velocity;

        moveVelocity = Vector2.Lerp(currentVelocity, moveVelocity, _AccelerationSpeed * Time.deltaTime);
        ThisRb.velocity = moveVelocity;
    }

    protected virtual void Dash(Vector2 _DashDir, float _TargetDashProcessTime)
    {
        if (CurrentDashProcessTime < _TargetDashProcessTime)
        {
            CurrentDashProcessTime += Time.deltaTime;
            ThisRb.velocity = _DashDir * PlayerManager.Instance.MovementState.DashSpeed;
        }
        else
        {
            CurrentDashProcessTime = 0;
        }
    }

    #endregion

    #region Sorting Order

    public void SetSortingOrder(int _SortingOrder)
    {
        ThisSr.sortingOrder = _SortingOrder;
    }

    #endregion
}