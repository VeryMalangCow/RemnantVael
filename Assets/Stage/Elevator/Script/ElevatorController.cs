using DG.Tweening;
using UnityEngine;

public class ElevatorController : StaticDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Elevator")]
    [Header("=== Data")]
    [SerializeField] protected float endYPos;
    [SerializeField] public bool isOn = false;

    #endregion

    #region Tween

    protected virtual void Tween_Start()
    {
        isOn = true;
    }

    protected virtual void Tween_Update()
    {
        PlayerManager.instance.playerController.transform.position =
                        this.transform.position;
    }

    protected virtual void Tween_Complete()
    {
        isOn = false;
    }

    #endregion

    #region Elevator

    public virtual void Play_MoveToTarget()
    {
        DevTool.Play_Tween(
            this.transform.DOLocalMove(new Vector2(this.transform.localPosition.x, endYPos), 3f),
            new Dele(Tween_Start),
            new Dele(Tween_Update),
            new Dele(Tween_Complete));
    }

    #endregion
}
