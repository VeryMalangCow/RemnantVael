using UnityEngine;

public class EnergyShrapnelController : AbsorbItemController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Energy Shrapnel")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float EnergyValue = 1f;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        IsAbsorbing = true;
        LayerOrderManager.Instance.NeedLayerObjects.Add(this);
    }

    protected void OnDisable()
    {
        LayerOrderManager.Instance.NeedLayerObjects.Remove(this);
    }


    #endregion

    #region State

    public void SetState(Vector2 _SpawnPos, GameObject _TargetObject, float _Value)
    {
        base.SetState(_SpawnPos, _TargetObject);
        ThisSR.sprite = PlayerManager.Instance.PlayerController.ES_Sprite;
        EnergyValue = _Value;
    }

    #endregion

    #region Get Item

    protected override void GetItem()
    {
        base.GetItem();

        PlayerManager.Instance.PlayerController.AddCurrentEP(EnergyValue);
        PoolingManager.Instance.EnergyShrapnel.Queue.Enqueue(this);
    }

    #endregion
}
