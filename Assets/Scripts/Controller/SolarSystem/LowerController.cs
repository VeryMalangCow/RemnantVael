using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LowerController : SolarSystemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Lower")]

    [Header("=== Component")]
    [SerializeField] public Rigidbody2D ThisRb;
    [SerializeField] private List<DirectionalAnimController> ThisAnimatorList;
    [SerializeField] private SpriteRenderer CenterSpriteRenderer;

    [HideInInspector] private bool IsTweening = false;
    [HideInInspector] private Tween MoveTween = null;

    #endregion

    #region Fremework

    protected override void LateUpdate()
    {
        Vector2 dir = ThisRb.velocity;
        float time = Time.deltaTime;

        Set_RotSmooth(dir.normalized, time);
        Set_Tween(dir);
        DevTool.Set_AnimSpeed(ThisAnimatorList, dir.sqrMagnitude);

        base.LateUpdate();
    }

    #endregion

    #region Tween

    private void Set_Tween(Vector2 _RbVel)
    {
        if (_RbVel != Vector2.zero && IsTweening == false) // On
        {
            SetOn_Tween();
        }
        else if (_RbVel == Vector2.zero && IsTweening == true) // Off
        {
            SetOff_Tween();
        }
    }

    private void SetOn_Tween()
    {
        IsTweening = true;
        MoveTween = this.transform.DOShakePosition(1f, 0.01f, 20, 0, false, false)
            .SetLoops(-1, LoopType.Restart);
    }

    private void SetOff_Tween()
    {
        IsTweening = false;
        DOTween.Kill(MoveTween);
        MoveTween = null;
    }

    #endregion
}
