using UnityEngine;

public class ItemController : SortingObjectController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Item")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected Rigidbody2D ThisRb;

    #endregion

    #region State

    public virtual void Set_State(Vector2 _SpawnPos)
    {
        this.gameObject.transform.position = _SpawnPos;
    }

    #endregion
}
