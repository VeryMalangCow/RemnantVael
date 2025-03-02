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

    protected override void OnEnable()
    {
        base.OnEnable();
        LayerOrderManager.Instance.NeedLayerObjects.Add(this);
    }

    protected void OnDisable()
    {
        LayerOrderManager.Instance.NeedLayerObjects.Remove(this);
    }

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

    public void Set_State(Vector2 _SpawnPos, GameObject _TargetObject, int _Value)
    {
        base.SetState(_SpawnPos, _TargetObject);
        BetteryValue = _Value;
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        PlayerManager.Instance.PlayerController.Add_CurrentBS(BetteryValue);
        PoolingManager.Instance.BetteryShrapnel.Queue.Enqueue(this);
    }

    #endregion
}
