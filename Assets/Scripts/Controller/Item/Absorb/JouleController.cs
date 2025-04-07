using UnityEngine;

public class JouleController : RangeAbsorbItemController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Joule")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float EnergyValue = 1f;

    #endregion

    #region State

    public void Set_State(Vector2 _SpawnPos, float _Value)
    {
        base.Set_State(_SpawnPos);

        EnergyValue = _Value;

        gameObject.SetActive(true);
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        IsSpawnNow = false;

        PlayerManager.Instance.PlayerController.Add_CurrentEP(EnergyValue);
        PoolingManager.Instance.Joule.Queue.Enqueue(this);
    }

    #endregion
}
