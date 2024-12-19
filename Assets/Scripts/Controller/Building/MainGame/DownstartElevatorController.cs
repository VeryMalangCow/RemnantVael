using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownstartElevatorController : HaveShadowThingStatic, IInteract
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Elevator")]
    [Header("=== Pos")]
    [SerializeField] private float EndYPos;

    [Space(10)]
    [Header("=== Data")]
    //[SerializeField] private int NextStageID = 0;
    [SerializeField] public bool IsOn = false;

    #endregion

    #region Move

    private void MoveToTarget()
    {
        MainGameUIManager.Instance.PlayDark(3f);
        this.transform.DOLocalMove(new Vector2(this.transform.localPosition.x, EndYPos), 3f)
            .SetEase(Ease.InQuart)
            .OnStart(() =>
            {
                PlayerManager.Instance.PlayerController.SetEndStage();
                ThisSR.sortingOrder = 3000;

                InputManager.Instance.SetAimAllOff();
                InputManager.Instance.CanMouseInput = false;
            })
            .OnUpdate(() =>
            {
                PlayerManager.Instance.PlayerController.transform.position =
                        this.transform.position;

            })
            .OnComplete(() =>
            {

            });
    }

    #endregion

    #region Interact

    public void Interact()
    {
        if (IsOn)
        {
            MoveToTarget();
        }
    }

    #endregion
}
