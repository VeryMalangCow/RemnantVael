using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class LowerController : SatelliteController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Lower")]

    [Header("-- Component")]
    [SerializeField] private Rigidbody2D ThisRb;
    [SerializeField] private List<Animator> ThisAnimatorList;
    [SerializeField] private SpriteRenderer CenterSpriteRenderer;
    [SerializeField] private ReactiveProperty<int> CurrentIndex = new();

    Tween MoveTween = null;

    #endregion

    #region Fremework

    private void Start()
    {
        CurrentIndex.Value = 5;
        CurrentIndex.Subscribe(index =>
        {
            for (int i = 0; i < ThisAnimatorList.Count; i++) 
            {
                ThisAnimatorList[i].SetTrigger(index.ToString());
            }
        });
    }

    protected void LateUpdate()
    {
        Vector2 dir = ThisRb.velocity;
        if (dir != Vector2.zero) 
        { 
            SetAll(dir);
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
            
        SetAnimSpeed(Vector2.Distance(Vector2.zero, dir));
    }

    #endregion

    #region Set

    private void SetAll(Vector2 _Dir)
    {
        // Animation
        Vector2 dirModifyX = new Vector2(-_Dir.x, _Dir.y);
        SetAnim(dirModifyX);

        // Rotate
        PitchTF.transform.localRotation = RotateSmooth(GetNormalizedVec(CurrentIndex.Value), PitchTF, rotateSpeed);
        foreach (Satellite hand in Hands)
        {
            hand.SetPos(PlayerSR.sortingOrder);
        }
        CenterSpriteRenderer.sortingOrder = PlayerController.ThisSR.sortingOrder + (10 * Hands[0].UpperOrder);
    
        // Tween
        if (MoveTween == null)
        {
            MoveTween = this.transform.DOShakePosition(1f, 0.01f, 20, 0, false, false)
                .SetLoops(-1, LoopType.Restart); 
        }

    }

    private void SetAnim(Vector2 _DirModifyX)
    {
        if (GetIndex(Quaternion.FromToRotation(Vector3.up, _DirModifyX).eulerAngles.z) != CurrentIndex.Value)
        {
            CurrentIndex.Value = GetIndex(Quaternion.FromToRotation(Vector3.up, _DirModifyX).eulerAngles.z);
        }
    }

    private void SetAnimSpeed(float _AnimSpeed)
    {
        for (int i = 0; i < ThisAnimatorList.Count; i++)
        {
            ThisAnimatorList[i].speed = _AnimSpeed;
        }
    }

    #endregion

    #region Get

    private int GetIndex(float _EulerAngleY)
    {
        int index = 0;
        float angle = _EulerAngleY + 67.5f;
        angle = angle >= 360 ? angle -= 360 : angle;

        index = (int)(angle / 45);
        return index;
    }

    private Vector2Int GetNormalizedVec(int _Index)
    {
        switch (_Index)
        { 
            case 0:
                return new Vector2Int(-1, 1);
            case 1:
                return new Vector2Int(0, 1);
            case 2:
                return new Vector2Int(1, 1);
            case 3:
                return new Vector2Int(1, 0);
            case 4:
                return new Vector2Int(1, -1);
            case 5:
                return new Vector2Int(0, -1);
            case 6:
                return new Vector2Int(-1, -1);
            case 7:
                return new Vector2Int(-1, 0);

        }
        return Vector2Int.zero;
    }

    #endregion
}
