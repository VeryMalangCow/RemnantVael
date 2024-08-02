using UnityEngine;

public class BetteryShrapnelController : AbsorbItemController
{
    #region Value

    [Space(20)] [Header("<><><><><> Field Bettery")]

    [Header("=== State")]
    [SerializeField] private int BetteryValue = 1;

    #endregion

    #region State

    public void SetState(Vector2 _SpawnPos, GameObject _TargetObject, int _Value)
    {
        base.SetState(_SpawnPos, _TargetObject);
        BetteryValue = _Value;
    }

    #endregion

    #region Get Item

    protected override void GetItem()
    {
        base.GetItem();

        PlayerManager.Instance.PlayerController.AddCurrentBS(BetteryValue);
        PoolingManager.Instance.BetteryShrapnel.Queue.Enqueue(this);
        this.gameObject.SetActive(false);
    }

    #endregion
}
