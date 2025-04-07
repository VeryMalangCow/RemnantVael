
using UnityEngine;

public class CreditController : RangeAbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Credit")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private int GainAmount = 1;

    #endregion

    #region State

    public void Set_State(Vector2 _SpawnPos, int _Value)
    {
        base.Set_State(_SpawnPos);

        GainAmount = _Value;
        gameObject.SetActive(true);
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        IsSpawnNow = false;

        PlayerManager.Instance.PlayerController.Add_CurrentCredit(GainAmount);
        PoolingManager.Instance.Credit.Queue.Enqueue(this);
    }

    #endregion
}
