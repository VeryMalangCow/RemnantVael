using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectController : MovableDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Movable Object")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Rigidbody2D ThisRb;

    [Space(10)]
    [Header("=== Movement")]
    [SerializeField] protected float AccelerationSpeed = 12;

    [HideInInspector] protected List<CurrentKnockbackState> KnockbackStateList = new List<CurrentKnockbackState>();
   
    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();
        Update_Knockback(Time.deltaTime);
    }

    #endregion

    #region Movement

    protected void Play_Walk(Vector2 _MoveDir, float _MoveSpeed, float _DeltaTime)
    {
        ThisRb.velocity = Vector2.Lerp(ThisRb.velocity, _MoveDir * _MoveSpeed, AccelerationSpeed * _DeltaTime);
    }

    #endregion

    #region Knockback

    private void Update_Knockback(float _DeltaTime)
    {
        if (KnockbackStateList.Count > 0)
        {
            for (int i = 0; i < KnockbackStateList.Count; i++)
            {
                ThisRb.velocity += KnockbackStateList[i].Get_Knockback() * _DeltaTime * 10;
            }
        }
    }

    protected void Gain_Knockback(CurrentKnockbackState _KnockbackState)
    {
        KnockbackStateList.Add(_KnockbackState);
        _KnockbackState.Start_Knockback()
            .OnComplete(() =>
            {
                KnockbackStateList.Remove(_KnockbackState);
            });
    }

    #endregion
}