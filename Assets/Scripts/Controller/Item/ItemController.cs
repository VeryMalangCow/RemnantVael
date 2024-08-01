using UnityEngine;

public class ItemController : HaveShadowThing
{
    #region Value

    [Space(20)] [Header("<><><><><> Item")]

    [Header("=== Data")]
    [SerializeField] private string ItemID;

    [Header("=== Base Item")]
    [SerializeField] protected eItemType ItemType;

    [Header("=== Component")]
    [SerializeField] protected Rigidbody2D ThisRb;

    #endregion

    #region State

    protected void SetState(eItemType _ItemType, Vector2 _SpawnPos)
    {
        ItemType = _ItemType;
        this.gameObject.transform.position = _SpawnPos;
    }

    #endregion
}
