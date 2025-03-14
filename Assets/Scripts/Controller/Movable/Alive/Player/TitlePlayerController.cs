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

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        CurrentIndex.Value = 5;
    }

    #endregion

    #region Framework

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Play_Movement(Time.fixedDeltaTime);

        //Set_Img(Time.deltaTime);
    }

    #endregion

    #region Movement

    private void Play_Movement(float _DeltaTime)
    {
        Play_Walk(TitleInputManager.Instance.InputMoveDir, WalkSpeed, _DeltaTime);
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

    private void Set_Img(float _DeltaTime)
    {
        Vector2 dir = ThisRb.velocity;
        if (dir != Vector2.zero)
        {
            dir = new Vector2(-dir.x, dir.y);
            int index = DevTool.Get_Index(Quaternion.FromToRotation(Vector3.up, dir).eulerAngles.z);
            if (index != CurrentIndex.Value)
            {
                CurrentIndex.Value = index;
            }

            HigherBody.Set_RotSmooth(DevTool.Get_NormalizedVec(CurrentIndex.Value), _DeltaTime);
        }
    }

    #endregion
}
