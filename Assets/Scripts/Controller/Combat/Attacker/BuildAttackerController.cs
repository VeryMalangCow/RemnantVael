using UnityEngine;

public class BuildAttackerController : AttackerController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]
    [SerializeField] private bool IsColliding = false;

    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        IsColliding = false;
    }

    #endregion

    #region Framework

    protected override void Update()
    {
        //base.Update();
        if (IsColliding)
        { PlayerManager.Instance.PlayerController.Try_Hitted(this); }
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D _Col)
    {
        if (_Col.tag == "Player")
        { IsColliding = true; }
    }

    private void OnTriggerExit2D(Collider2D _Col)
    {
        if (_Col.tag == "Player")
        { IsColliding = false; }
    }

    #endregion
}
