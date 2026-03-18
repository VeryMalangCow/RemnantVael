using DG.Tweening;
using UnityEngine;

public class ElevatorController : StaticDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Elevator")]
    [Header("=== Data")]
    [SerializeField] protected float EndYPos;
    [SerializeField] public bool IsOn = false;

    #endregion

    #region Tween

    protected virtual void Tween_Start()
    {
        IsOn = true;
    }

    protected virtual void Tween_Update()
    {
        PlayerManager.Instance.playerController.transform.position =
                        this.transform.position;
    }

    protected virtual void Tween_Complete()
    {
        IsOn = false;
    }

    #endregion

    #region Elevator

    protected virtual void Play_MoveToTarget()
    {
        DevTool.Play_Tween(
            this.transform.DOLocalMove(new Vector2(this.transform.localPosition.x, EndYPos), 3f),
            new Dele(Tween_Start),
            new Dele(Tween_Update),
            new Dele(Tween_Complete));
    }

    #endregion
}
