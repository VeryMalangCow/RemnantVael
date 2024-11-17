using UnityEngine;

public class TitlePlayerController : MovableObject
{
    #region Value

    [Space(10)]
    [Header("=== Movement")]
    [SerializeField] SatelliteController HigherBody;
    [SerializeField] private float WalkSpeed = 1f;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private IInteract CurrentInteractable;

    #endregion

    #region Framework

    private void LateUpdate()
    {
        Movement();

        //InputTitleManager.Instance.DirFromPlayerPos.normalized;
        Vector2 v = Vector2.zero;
        HigherBody.PitchTF.transform.localRotation = HigherBody.RotateSmooth(v);
    }

    #endregion

    #region Movement

    private void Movement()
    {
        Walk(InputTitleManager.Instance.InputMoveDir, WalkSpeed, AccelerationSpeed);
    }

    #endregion

    #region Interact

    public void TryInteract()
    {
        if (CurrentInteractable != null)
        {
            InputTitleManager.Instance.InputMoveDir = Vector2.zero;
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
}
