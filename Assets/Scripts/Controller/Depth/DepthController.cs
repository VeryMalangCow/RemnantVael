using UnityEngine;

public class DepthController : IDController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Depth")]

    [Space(10)]
    [Header("=== Shadow")]
    [SerializeField] public GameObject TargetObject;
    [HideInInspector] public SpriteRenderer ThisSR;
    [SerializeField] public float TargetRange = 0.4f;

    #endregion

    #region Framework

    protected virtual void Awake()
    {
        Offset();
    }

    #endregion

    #region Offset

    protected virtual void Offset()
    {
        StaticCaculator.Set_ComponentTType<SpriteRenderer>(ref ThisSR, TargetObject);
    }

    #endregion

    #region Sorting Order

    public virtual void Set_SortingOrder(int _SortingOrder)
    {
        //StaticCaculator.Set_ComponentTType<SpriteRenderer>(ref ThisSR, TargetObject);

        ThisSR.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Gen

    // Bettery Shrapnel
    protected void Gen_BS(int _Value)
    {
        Vector2 spawnPos = gameObject.transform.position;
        GameObject targetGO = PlayerManager.Instance.PlayerController.gameObject;

        PoolingManager.Instance.Get_OP_BetteryShrapnel().Set_State(spawnPos, targetGO, _Value);
    }

    // Module Shrapnel
    protected void Gen_MS(int _Value)
    {
        Vector2 spawnPos = gameObject.transform.position;
        GameObject targetGO = PlayerManager.Instance.PlayerController.gameObject;

        PoolingManager.Instance.Get_OP_ModuleShrapnel().SetState(spawnPos, targetGO, _Value);
    }

    #endregion
}
