using UniRx;
using UnityEngine;

public class TitlePlayerController : MovableObject
{
    #region Value

    [Space(10)]
    [Header("=== Movement")]
    [SerializeField] public SatelliteController HigherBody;
    [SerializeField] private float WalkSpeed = 1f;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private IInteract CurrentInteractable;

    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] private ReactiveProperty<int> CurrentIndex = new();

    #endregion

    #region Framework

    private void Start()
    {
        CurrentIndex.Value = 5;
    }

    private void LateUpdate()
    {
        Movement();

        SetImg();
    }

    #endregion

    #region Movement

    private void Movement()
    {
        Walk(TitleInputManager.Instance.InputMoveDir, WalkSpeed, AccelerationSpeed);
    }

    #endregion

    #region Interact

    public void TryInteract()
    {
        if (CurrentInteractable != null)
        {
            TitleInputManager.Instance.InputMoveDir = Vector2.zero;
            CurrentInteractable.Interact();
        }
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if (_Col.gameObject.transform.parent.TryGetComponent(out IInteract II))
        {
            CurrentInteractable = II;
        }
    }
    private void OnTriggerExit2D(Collider2D _Col)
    {
        if (_Col.gameObject.transform.parent.TryGetComponent(out IInteract II) &&
            CurrentInteractable == II)
        {
            CurrentInteractable = null;
        }
    }

    #endregion

    #region Anim

    private void SetImg()
    {
        Vector2 dir = ThisRb.velocity;
        if (dir != Vector2.zero)
        {
            dir = new Vector2(-dir.x, dir.y);
            if (SatelliteController.GetIndex(Quaternion.FromToRotation(Vector3.up, dir).eulerAngles.z) != CurrentIndex.Value)
            {
                CurrentIndex.Value = SatelliteController.GetIndex(Quaternion.FromToRotation(Vector3.up, dir).eulerAngles.z);
            }

            HigherBody.PitchTF.transform.localRotation = HigherBody.RotateSmooth(SatelliteController.GetNormalizedVec(CurrentIndex.Value));
            foreach (Satellite hand in HigherBody.Hands)
            {
                hand.SetPos(HigherBody.PlayerSR.sortingOrder);
            }
            //HigherBody.CenterSpriteRenderer.sortingOrder = PlayerController.ThisSR.sortingOrder + Hands[0].UpperOrder;
        }
    }

    #endregion
}
