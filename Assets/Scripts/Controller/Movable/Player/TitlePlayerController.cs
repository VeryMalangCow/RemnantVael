using UnityEngine;

public class TitlePlayerController : MovableObject
{
    private float WalkSpeed = 1f;


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
        //InputTitleManager.Instance.InputMoveDir = Vector2.zero;
    }
}
