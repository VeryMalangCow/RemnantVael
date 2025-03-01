using UnityEngine;

public class HaveShadowThing : MonoBehaviour
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Have Shadow Thing")]

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
        if (ThisSR == null && TargetObject.TryGetComponent(out SpriteRenderer sr))
        { ThisSR = sr; }
    }

    #endregion

    #region Sorting Order

    public virtual void Set_SortingOrder(int _SortingOrder)
    {
        if (ThisSR == null)
        { Offset(); }

        ThisSR.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Gen


    // Bettery Shrapnel
    protected void Gen_BS(int _Value)
    {
        BetteryShrapnelController BSC = PoolingManager.Instance.Get_OP_BetteryShrapnel();
        BSC.Set_State(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            _Value);
        BSC.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        BSC.gameObject.SetActive(true);
    }

    // Module Shrapnel
    protected void Gen_MS(int _Value)
    {
        ModuleShrapnelController MSC = PoolingManager.Instance.Get_OP_ModuleShrapnel();
        MSC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            _Value);
        MSC.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        MSC.gameObject.SetActive(true);
    }

    #endregion
}
