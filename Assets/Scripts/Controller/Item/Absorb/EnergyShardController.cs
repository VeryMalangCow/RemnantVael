using UnityEngine;

public class EnergyShardController : AbsorbItemController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Energy Shard")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float AbsorbRange = 1f;
    [SerializeField] private float EnergyValue = 1f;

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

    public void Set_State(Vector2 _SpawnPos, float _Value)
    {
        base.Set_State(_SpawnPos);

        EnergyValue = _Value;

        transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        gameObject.SetActive(true);

        ThisSR.sprite = PlayerManager.Instance.PlayerController.ES_Sprite;
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        PlayerManager.Instance.PlayerController.Add_CurrentEP(EnergyValue);
        PoolingManager.Instance.EnergyShrapnel.Queue.Enqueue(this);
    }

    #endregion
}
