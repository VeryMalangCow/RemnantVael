using UnityEngine;

public class EnergyShrapnelController : AbsorbItemController
{
    #region Value

    [Space(20)] [Header("<><><><><> Energy Shrapnel")]

    [Header("=== State")]
    [SerializeField] private float EnergyValue = 1f;

    #endregion

    #region State

    public void SetState(Vector2 _SpawnPos, GameObject _TargetObject, float _Value)
    {
        base.SetState(_SpawnPos, _TargetObject);
        EnergyValue = _Value;
    }

    #endregion

    #region Get Item

    protected override void GetItem()
    {
        base.GetItem();

        PlayerManager.Instance.PlayerController.AddCurrentEP(EnergyValue);
        PoolingManager.Instance.EnergyShrapnel.Queue.Enqueue(this);
        this.gameObject.SetActive(false);
    }

    #endregion
}
