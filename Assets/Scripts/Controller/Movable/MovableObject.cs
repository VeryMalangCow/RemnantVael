using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MovableDepthController
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
    [Header("=== Knockback")]
    [SerializeField] protected List<KnockbackState> KnockbackStateList = new List<KnockbackState>();

    [Space(10)]
    [Header("=== Anim")]
    [Header("-- Idle")]
    [SerializeField] private float BaseYLimit = 0.06f;
    [SerializeField] private float BaseTweenReTime = 0.3f;
    [SerializeField] List<MovableDepthController> ThisComponentGOList;

    [HideInInspector] private Sequence BaseSeq = null;

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();
        Play_Knockback();
    }

    #endregion

    #region Movement

    protected void Play_Walk(Vector2 _MoveDir, float _MoveSpeed, float _AccelerationSpeed)
    {
        Vector2 moveVelocity = _MoveDir * _MoveSpeed;
        Vector2 currentVelocity = ThisRb.velocity;

        moveVelocity = Vector2.Lerp(currentVelocity, moveVelocity, _AccelerationSpeed * Time.fixedDeltaTime);
        ThisRb.velocity = moveVelocity;
    }

    #endregion

    #region Life

    protected void Set_IsDead(float _Life, float _Damage)
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

    protected void Set_BaseAnimTween()
    {
        BaseSeq = DOTween.Sequence();

        BaseSeq.Join(DOTween.To(() => TargetRange, x => TargetRange = x, TargetRange + BaseYLimit, BaseTweenReTime)
            .SetEase(Ease.Linear));

        if (ThisComponentGOList != null && ThisComponentGOList.Count > 0)
        {
            for (int i = 0; i < ThisComponentGOList.Count; i++)
            {
                MovableDepthController HSTM = ThisComponentGOList[i];
                BaseSeq.Join(DOTween.To(() => HSTM.TargetRange, x => HSTM.TargetRange = x, HSTM.TargetRange + BaseYLimit, BaseTweenReTime)
                    .SetEase(Ease.Linear));
            }
        }
        

        BaseSeq.SetLoops(-1, LoopType.Yoyo);
    }

    #endregion

    #region Knockback

    private void Play_Knockback()
    {
        if (KnockbackStateList.Count > 0)
        {
            for (int i = 0; i < KnockbackStateList.Count; i++)
            {
                ThisRb.velocity += KnockbackStateList[i].Get_Knockback() * Time.deltaTime * 10;
            }
        }
    }

    protected void Get_Knockback(KnockbackState _KnockbackState)
    {
        KnockbackStateList.Add(_KnockbackState);
        KnockbackStateList[KnockbackStateList.Count - 1].Start_Knockback()
            .OnComplete(() =>
            {
                KnockbackStateList.Remove(KnockbackStateList[KnockbackStateList.Count - 1]);
            });
    }

    [System.Serializable]
    protected class KnockbackState
    {
        public Vector2 Dir;
        public float Power;
        public float Time;

        public KnockbackState(Vector2 _KnockbackDir, float _KnockbackPower, float _KnockbackTime)
        {
            Dir = _KnockbackDir;
            Power = _KnockbackPower;
            Time = _KnockbackTime;
        }

        public Tween Start_Knockback()
        {
            return DOTween.To(() => Power, x => Power = x, 0, Time);
        }

        public Vector2 Get_Knockback()
        {
            return Dir.normalized * Power;
        }
    }

    #endregion
}