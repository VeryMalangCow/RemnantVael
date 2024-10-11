using UnityEngine;

public class TitlePlayerController : MovableObject
{
    [SerializeField] private float WalkSpeed = 1f;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private IInteract CurrentInteractable;

    private void LateUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        Walk(InputTitleManager.Instance.InputMoveDir, WalkSpeed, AccelerationSpeed);
    }

    public void TryInteract()
    {
        if (CurrentInteractable != null)
        {
            InputTitleManager.Instance.InputMoveDir = Vector2.zero;
            CurrentInteractable.Interact();
        }
    }

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
}
