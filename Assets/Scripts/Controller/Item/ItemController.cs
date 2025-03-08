using UnityEngine;

public class ItemController : StaticDepthController
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

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        LayerOrderManager.Instance.NeedLayerObjects.Add(this);
    }

    protected void OnDisable()
    {
        LayerOrderManager.Instance.NeedLayerObjects.Remove(this);
    }

    #endregion
}
