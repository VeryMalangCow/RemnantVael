using UnityEngine;

public class ItemController : SortingObjectController, IPoolable
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Item")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected Rigidbody2D rb;

    public int PoolIndex { get; set; } = -1;
    public int ActiveIndex { get; set; } = -1;

    #endregion

    #region State

    public virtual void Set_State(Vector2 spawnPos)
    {
        this.gameObject.transform.position = spawnPos;
    }

    #endregion

    #region Pool
    public void PoolOffset()
    {
        gameObject.SetActive(false);
    }

    public void SetActiveOn()
    {
        gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);
        gameObject.SetActive(true);
    }

    public void SetActiveOff()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
