using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : HaveShadowThingMovable
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Movable Object")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] protected bool IsDead = false;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public Rigidbody2D ThisRb;
    [SerializeField] private List<SpriteRenderer> ThisExtraSrs;

    [Space(10)]
    [Header("=== Movement")]
    [Header("-- Walk")]
    [SerializeField] protected float AccelerationSpeed = 12;

    [Space(10)]
    [Header("=== Anim")]
    [Header("-- Idle")]
    [SerializeField] private float BaseYLimit = 0.06f;
    [SerializeField] private float BaseTweenReTime = 0.3f;
    [SerializeField] List<HaveShadowThingMovable> ThisComponentGOList;

    [HideInInspector] private Sequence BaseSeq = null;

    #endregion

    #region Movement

    protected void Walk(Vector2 _MoveDir, float _MoveSpeed, float _AccelerationSpeed)
    {
        Vector2 moveVelocity = _MoveDir * _MoveSpeed;
        Vector2 currentVelocity = ThisRb.velocity;

        moveVelocity = Vector2.Lerp(currentVelocity, moveVelocity, _AccelerationSpeed * Time.deltaTime);
        ThisRb.velocity = moveVelocity;
    }

    #endregion

    #region Life

    protected void SetIsDead(float _Life, float _Damage)
    {
        if(_Life <= _Damage)
        {
            IsDead = true;
        }
        else
        {
            IsDead = false;
        }
    }

    #endregion
    
    #region Have Shadow Thing

    protected void SetBaseAnimTween()
    {
        BaseSeq = DOTween.Sequence();

        BaseSeq.Join(DOTween.To(() => TargetRange, x => TargetRange = x, TargetRange + BaseYLimit, BaseTweenReTime)
            .SetEase(Ease.Linear));

        if (ThisComponentGOList != null && ThisComponentGOList.Count > 0)
        {
            for (int i = 0; i < ThisComponentGOList.Count; i++)
            {
                HaveShadowThingMovable HSTM = ThisComponentGOList[i];
                BaseSeq.Join(DOTween.To(() => HSTM.TargetRange, x => HSTM.TargetRange = x, HSTM.TargetRange + BaseYLimit, BaseTweenReTime)
                    .SetEase(Ease.Linear));
            }
        }
        

        BaseSeq.SetLoops(-1, LoopType.Yoyo);
    }

    #endregion
}