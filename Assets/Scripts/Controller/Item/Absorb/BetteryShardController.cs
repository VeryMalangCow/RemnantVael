using UnityEngine;

public class BetteryShardController : RangeAbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Bettery Shard")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private int BetteryValue = 1;

    #endregion

    #region State

    public void Set_State(Vector2 _SpawnPos, int _Value)
    {
        base.Set_State(_SpawnPos);

        BetteryValue = _Value;
        gameObject.SetActive(true);
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        IsSpawnNow = false;

        PlayerManager.Instance.PlayerController.Add_CurrentBetteryShard(BetteryValue);
        PoolingManager.Instance.BetteryShard.Queue.Enqueue(this);
    }

    #endregion
}
