using DG.Tweening;
using UnityEngine;

public class UpendElevatorController : HaveShadowThingStatic
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Elevator")]
    [Header("=== Pos")]
    [SerializeField] private float EndYPos;


    #endregion

    #region Framework

    private void Start()
    {
        EventManager.Instance.SetBlackUpDownCover(true);
        MoveToTarget();
    }

    #endregion

    #region Move

    private void MoveToTarget()
    {
        this.transform.DOLocalMove(new Vector2(this.transform.localPosition.x, EndYPos), 3f)
            .SetEase(Ease.OutQuart)
            .OnStart(() =>
            {
                PlayerManager.Instance.PlayerController.SetPastStartStage();
            })
            .OnUpdate(() =>
            {
                PlayerManager.Instance.PlayerController.transform.position =
                    this.transform.position;

            })
            .OnComplete(() =>
            {
                PlayerManager.Instance.PlayerController.SetStartStage();

                EventManager.Instance.SetInputSetting(true);
                EventManager.Instance.SetBlackUpDownCover(false);
            });
    }

    #endregion
}
