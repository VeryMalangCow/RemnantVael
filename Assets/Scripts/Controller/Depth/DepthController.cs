using UnityEngine;

public class DepthController : IDController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Depth")]

    [Space(10)]
    [Header("=== Shadow")]
    [SerializeField] public GameObject TargetObject;
    [SerializeField] public SpriteRenderer ThisSR;
    [SerializeField] public float TargetRange = 0.4f;

    #endregion

    #region Offset

    protected virtual void Offset() { }
   

    #endregion

    #region Framework

    protected void Start()
    {
        Offset();
    }

    #endregion

    #region Sprite Renderer

    public virtual void Set_SortingOrder(int _SortingOrder)
    {
        if (ThisSR == null)
        { Debug.Log(this.gameObject.name); return; }

        ThisSR.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Set TargetPos

    protected Vector2 Get_TargetPos()
    {
        return (Vector2)transform.position + (Vector2.up * TargetRange);
    }

    protected void Set_TargetPos()
    {
        TargetObject.transform.position = Get_TargetPos();
    }

    #endregion

    #region Gen


    // Bettery Shrapnel
    protected void Gen_BS(int _Value)
    {
        Vector2 spawnPos = gameObject.transform.position;
        GameObject targetGO = PlayerManager.Instance.PlayerController.gameObject;

        PoolingManager.Instance.Get_OP_BetteryShrapnel().Set_State(spawnPos, _Value);
    }

    // Random BS
    protected void Gen_RandomBS(int _Min, int _Max)
    {
        int amount = Random.Range(_Min, _Max + 1);
        for (int i = 0; i < amount; i++)
        {
            Gen_BS(1);
        }
    }

    // Module Shrapnel
    protected void Gen_MS(int _Value)
    {
        Vector2 spawnPos = gameObject.transform.position;
        GameObject targetGO = PlayerManager.Instance.PlayerController.gameObject;

        PoolingManager.Instance.Get_OP_ModuleShrapnel().Set_State(spawnPos, _Value);
    }

    // Random MS
    protected void Gen_RandomMS(int _Min, int _Max)
    {
        int amount = Random.Range(_Min, _Max + 1);
        for (int i = 0; i < amount; i++)
        {
            Gen_MS(1);
        }
    }


    // Module Interact Item
    protected void Gen_II(int _Rank)
    {
        InteractItemController IIC = PoolingManager.Instance.Get_OP_InteractableItem();
        IIC.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        IIC.Set_State(this.transform.position);
        IIC.Set_RankState(_Rank);
    }

    // Energy Shrapnel
    protected void Gen_ES(float _Value)
    {
        EnergyShardController ESC = PoolingManager.Instance.Get_OP_EnergyShrapnel();
        ESC.Set_State(this.gameObject.transform.position, _Value);
    }


    #endregion
}
