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
    [SerializeField] protected float TargetRange = 0.4f;

    #endregion

    private void Awake()
    {
        if (ThisSR == null && TargetObject.TryGetComponent(out SpriteRenderer sr))
        { ThisSR = sr; }
    }


    #region Sorting Order

    public void SetSortingOrder(int _SortingOrder)
    {
        if (ThisSR == null)
        { return; } 

        ThisSR.sortingOrder = _SortingOrder;
    }

    #endregion
}
