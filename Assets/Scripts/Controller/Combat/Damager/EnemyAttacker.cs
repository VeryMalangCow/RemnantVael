using UnityEngine;

public class EnemyAttacker : Attacker
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]
    [SerializeField] private bool IsColliding = false;

    #endregion

    #region Framework

    private void Update()
    {
        if (IsColliding)
        { PlayerManager.Instance.PlayerController.TryHitted(this); }
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
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
