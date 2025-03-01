using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LowerController : SatelliteController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Lower")]

    [Header("-- Component")]
    [SerializeField] public Rigidbody2D ThisRb;
    [SerializeField] private List<Animator> ThisAnimatorList;
    [SerializeField] private SpriteRenderer CenterSpriteRenderer;

    Tween MoveTween = null;

    #endregion

    #region Fremework

    protected void LateUpdate()
    {
        Vector2 dir = ThisRb.velocity;
        if (dir != Vector2.zero) 
        { 
            Set_All(dir);
        }
        else
        {
            // Tween
            if (MoveTween != null)
            {
                DOTween.Kill(MoveTween);
                MoveTween = null;
            }
        }
            
        Set_AnimSpeed(Vector2.Distance(Vector2.zero, dir));
    }

    #endregion

    #region Set

    public void Set_All(Vector2 _Dir)
    {
        // Rotate
        PitchTF.transform.localRotation = Get_RotationSmooth(_Dir.normalized);
        foreach (Satellite hand in Hands)
        {
            hand.SetPos(PlayerSR.sortingOrder);
        }
        CenterSpriteRenderer.sortingOrder = PlayerController.ThisSR.sortingOrder + Hands[0].UpperOrder;
    
        // Tween
        if (MoveTween == null)
        {
            MoveTween = this.transform.DOShakePosition(1f, 0.01f, 20, 0, false, false)
                .SetLoops(-1, LoopType.Restart); 
        }
    }

    private void Set_AnimSpeed(float _AnimSpeed)
    {
        for (int i = 0; i < ThisAnimatorList.Count; i++)
        {
            ThisAnimatorList[i].speed = _AnimSpeed;
        }
    }

    #endregion

    
}
