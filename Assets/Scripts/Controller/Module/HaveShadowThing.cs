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

    protected virtual void Awake()
    {
        TryFindSR();
    }

    protected void TryFindSR()
    {
        if (ThisSR == null && TargetObject.TryGetComponent(out SpriteRenderer sr))
        { ThisSR = sr; }
    }


    #region Sorting Order

    public virtual void SetSortingOrder(int _SortingOrder)
    {
        if (ThisSR == null)
        { return; }

        ThisSR.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Spawn


    // Bettery Shrapnel
    protected void SpawnBS(int _Value)
    {
        BetteryShrapnelController BSC = PoolingManager.Instance.GetOP_BetteryShrapnel();
        BSC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            _Value);
        BSC.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        BSC.gameObject.SetActive(true);
    }

    // Module Shrapnel
    protected void SpawnMS(int _Value)
    {
        ModuleShrapnelController MSC = PoolingManager.Instance.GetOP_ModuleShrapnel();
        MSC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            _Value);
        MSC.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        MSC.gameObject.SetActive(true);
    }

    #endregion
}
