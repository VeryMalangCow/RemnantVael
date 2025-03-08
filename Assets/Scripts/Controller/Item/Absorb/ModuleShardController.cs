using UnityEngine;

public class ModuleShardController : AbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Module Shard")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float AbsorbRange = 1f;
    [SerializeField] private int ModuleValue = 1;

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

    public void Set_State(Vector2 _SpawnPos, int _Value)
    {
        base.Set_State(_SpawnPos);

        ModuleValue = _Value;

        transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        gameObject.SetActive(true);
    }

    #endregion

    #region Get Item

    protected override void Gain_Item()
    {
        base.Gain_Item();

        PlayerManager.Instance.PlayerController.Add_CurrentMS(ModuleValue);
        PoolingManager.Instance.ModuleShrapnel.Queue.Enqueue(this);
    }

    #endregion
}
