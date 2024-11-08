using UnityEngine;

public class Damager : MonoBehaviour
{
    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if (_Col.tag == "Player")
        {
            PlayerManager.Instance.PlayerController.TryTakeDamage();
        }
    }

    #endregion
}
