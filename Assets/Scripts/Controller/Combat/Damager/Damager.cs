using UnityEngine;

public class Damager : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Damager")]
    [SerializeField] private bool IsColliding = false;

    [Space(10)]
    [Header("=== DMG")]
    [SerializeField] private float DmgValue;
    [SerializeField] private bool AbleKnockback;
    [SerializeField] private float KnockbackPower;

    #endregion

    #region Framework

    private void Update()
    {
        if (IsColliding)
        { PlayerManager.Instance.PlayerController.TryTakeDamage(DmgValue, this, AbleKnockback, KnockbackPower); }    
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
