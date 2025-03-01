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
        EventManager.Instance.Set_BlackUpDownCover(true);
        Play_MoveToTarget();
    }

    #endregion

    #region Move

    private void Play_MoveToTarget()
    {
        this.transform.DOLocalMove(new Vector2(this.transform.localPosition.x, EndYPos), 3f)
            .SetEase(Ease.OutQuart)
            .OnStart(() =>
            {
                PlayerManager.Instance.PlayerController.Set_PastStartStage();
            })
            .OnUpdate(() =>
            {
                PlayerManager.Instance.PlayerController.transform.position =
                    this.transform.position;

            })
            .OnComplete(() =>
            {
                PlayerManager.Instance.PlayerController.Set_StartStage();

                EventManager.Instance.Set_Input(true);
                EventManager.Instance.Set_BlackUpDownCover(false);
            });
    }

    #endregion
}
