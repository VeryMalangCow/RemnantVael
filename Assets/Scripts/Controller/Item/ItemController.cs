using UnityEngine;

public class ItemController : HaveShadowThingStatic
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
        TargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }

    #endregion
}
