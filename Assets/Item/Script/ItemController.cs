using UnityEngine;

public class ItemController : SortingObjectController, IPoolable
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Item")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected Rigidbody2D rb;

    [Space(10)]
    [Header("=== Spread")]
    [SerializeField] private float spreadPower = 10f; // 퍼짐 속도 시작값
    protected float currentSpreadPower = 0f; // 현재 퍼짐 속도

    [SerializeField] private float decSpreadPowerSpeed = 1f; // 퍼짐 감속도
    protected Vector2 settedSpreadDir; // 방향

    protected bool isEndSpread = false;

    public int PoolIndex { get; set; } = -1;
    public int ActiveIndex { get; set; } = -1;

    #endregion

    #region State

    public virtual void Set_State(Vector2 spawnPos)
    {
        this.gameObject.transform.position = spawnPos;

        isEndSpread = false;
        currentSpreadPower = spreadPower;
        settedSpreadDir = DevTool.Get_RandomDir();
    }

    public void HandleSpread(float dt)
    {
        if (isEndSpread) return;

        if (currentSpreadPower > 0f)
        {
            currentSpreadPower -= decSpreadPowerSpeed * dt;
            rb.velocity = settedSpreadDir * currentSpreadPower;
        }
        else if (currentSpreadPower != 0f)
        {
            currentSpreadPower = 0f;
            rb.velocity = Vector2.zero;
            isEndSpread = true;
        }
    }

    #endregion

    #region Pool

    public virtual void PoolOffset()
    {
        gameObject.SetActive(false);
    }

    public virtual void SetActiveOn()
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
