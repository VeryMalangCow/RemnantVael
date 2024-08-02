using UnityEngine;

public class EnergyShrapnelController : AbsorbItemController
{
    #region Value

    [Space(20)] [Header("<><><><><> Energy Shrapnel")]

    [Header("=== State")]
    [SerializeField] private float EnergyValue = 1f;

    #endregion

    #region State

    public void SetState(Vector2 _SpawnPos, float _Value)
    {
        base.SetState(_SpawnPos);
        EnergyValue = _Value;
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Collision)
    {
        if (_Collision.tag == "Player")
        {
            PlayerManager.Instance.PlayerController.SetCurrentEP(EnergyValue);
            PoolingManager.Instance.EnergyShrapnel.Queue.Enqueue(this);
            this.gameObject.SetActive(false);
        }
    }

    #endregion
}
