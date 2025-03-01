using DG.Tweening;
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

    private void Play_MoveToTarget()
    {
        MainGameUIManager.Instance.Play_Dark(3f);
        this.transform.DOLocalMove(new Vector2(this.transform.localPosition.x, EndYPos), 3f)
            .SetEase(Ease.InQuart)
            .OnStart(() =>
            {
                PlayerManager.Instance.PlayerController.Set_EndStage();
                ThisSR.sortingOrder = 3000;

                EventManager.Instance.Set_Input(false);
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

    public void Play_Interact()
    {
        if (IsOn)
        {
            Play_MoveToTarget();
        }
    }

    #endregion
}
