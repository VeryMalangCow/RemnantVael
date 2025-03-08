using UniRx;
using UnityEngine;

public class TitlePlayerController : MovableObjectController
{
    #region Value

    [Space(10)]
    [Header("=== Movement")]
    [SerializeField] public SolarSystemController HigherBody;
    [SerializeField] private float WalkSpeed = 1f;

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private IInteract CurrentInteractable;

    [Space(10)]
    [Header("=== Anim")]
    [SerializeField] private ReactiveProperty<int> CurrentIndex = new();

    #endregion

    #region Framework

    protected override void Start()
    {
        base.Start();
        CurrentIndex.Value = 5;
    }

    private void LateUpdate()
    {
        Play_Movement();

        Set_Img();
    }

    #endregion

    #region Movement

    private void Play_Movement()
    {
        Play_Walk(TitleInputManager.Instance.InputMoveDir, WalkSpeed, AccelerationSpeed);
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        if (CurrentInteractable != null)
        {
            TitleInputManager.Instance.InputMoveDir = Vector2.zero;
            CurrentInteractable.Play_Interact();
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

    private void Set_Img()
    {
        Vector2 dir = ThisRb.velocity;
        if (dir != Vector2.zero)
        {
            dir = new Vector2(-dir.x, dir.y);
            if (SolarSystemController.Get_Index(Quaternion.FromToRotation(Vector3.up, dir).eulerAngles.z) != CurrentIndex.Value)
            {
                CurrentIndex.Value = SolarSystemController.Get_Index(Quaternion.FromToRotation(Vector3.up, dir).eulerAngles.z);
            }

            HigherBody.PitchTF.transform.localRotation = HigherBody.Get_RotationSmooth(SolarSystemController.Get_NormalizedVec(CurrentIndex.Value));
            foreach (SatelliteController hand in HigherBody.Hands)
            {
                hand.SetPos(HigherBody.PlayerSR.sortingOrder);
            }
            //HigherBody.CenterSpriteRenderer.sortingOrder = PlayerController.ThisSR.sortingOrder + Hands[0].UpperOrder;
        }
    }

    #endregion
}
