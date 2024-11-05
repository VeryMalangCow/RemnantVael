using UnityEngine;

public class BetteryShrapnelController : AbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Field Bettery")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float AbsorbRange = 1f;
    [SerializeField] private int BetteryValue = 1;

    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        if (!IsAbsorbing)
        {
            IsAbsorbing = 
                Vector2.Distance(PlayerManager.Instance.PlayerController.gameObject.transform.position, this.gameObject.transform.position) 
                <= AbsorbRange;
        }
    }

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
    }

    #endregion
}
