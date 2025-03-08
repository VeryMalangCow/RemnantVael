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

    protected virtual void Start()
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

    #region Gen

    // Bettery Shrapnel
    protected void Gen_BS(int _Value)
    {
        Vector2 spawnPos = gameObject.transform.position;
        GameObject targetGO = PlayerManager.Instance.PlayerController.gameObject;

        PoolingManager.Instance.Get_OP_BetteryShrapnel().Set_State(spawnPos, _Value);
    }

    // Module Shrapnel
    protected void Gen_MS(int _Value)
    {
        Vector2 spawnPos = gameObject.transform.position;
        GameObject targetGO = PlayerManager.Instance.PlayerController.gameObject;

        PoolingManager.Instance.Get_OP_ModuleShrapnel().Set_State(spawnPos, _Value);
    }

    #endregion
}
