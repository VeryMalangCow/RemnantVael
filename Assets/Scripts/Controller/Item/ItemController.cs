using UnityEngine;

public class ItemController : HaveShadowThing
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Item")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected Rigidbody2D ThisRb;

    #endregion

    #region State

    protected void SetState(Vector2 _SpawnPos)
    {
        this.gameObject.transform.position = _SpawnPos;
    }

    #endregion
}
