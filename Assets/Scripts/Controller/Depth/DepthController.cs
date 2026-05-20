using UnityEngine;
using System;

public class DepthController : IDController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Depth")]

    [Space(10)]
    [Header("=== Shadow")]
    [SerializeField] public GameObject targetObject;
    [SerializeField] public SpriteRenderer thisSr;
    [SerializeField] public float targetRange = 0.4f;
    [SerializeField] private bool isSortingMover;

    public int currentOrder { get; private set; } = int.MinValue;
    public int sortIndex { get; private set; } = -1;


    #endregion

    #region MonoBehaviour

    protected void Start()
    {
        Offset();
    }

    #endregion

    #region Offset

    protected virtual void Offset() { }

    #endregion

    #region Sprite Renderer

    public virtual void SetSortingOrder(int sortingOrder)
    {
        if (currentOrder == sortingOrder) return;

        currentOrder = sortingOrder;
        if (thisSr == null)
            Debug.Log(gameObject);
        else
            thisSr.sortingOrder = sortingOrder;
    }

    #endregion

    #region Sort

    public void SetSortIndex(int index)
    {
        sortIndex = index;
    }

    public bool IsSortingMover()
    {
        return isSortingMover;
    }

    public void AddSortingLayer(bool isSortingMover = true)
    {
        if (SortingOrderManager.instance == null)
            throw new Exception("LayerOrderManager's instance is NULL");
        this.isSortingMover = isSortingMover;
        SetSortIndex(-1);
        SortingOrderManager.instance.RequestAddSortObj(this);
    }

    protected void RemoveSortingLayer()
    {
        if (SortingOrderManager.instance == null)
            throw new Exception("LayerOrderManager's instance is NULL");

        SortingOrderManager.instance.RequestRemoveSortObj(this);
    }


    public float GetPosY()
    {
        return transform.position.y;
    }

    #endregion

    #region Set TargetPos

    protected Vector2 Get_TargetPos()
    {
        return (Vector2)transform.position + (Vector2.up * targetRange);
    }

    protected void Set_TargetPos()
    {
        targetObject.transform.position = Get_TargetPos();
    }

    #endregion

    #region Gen


    // Bettery Shard
    protected void Gen_BS(int value)
    {
        if (value <= 0) return;

        PoolingManager.instance.Get_OP_BetteryShard().Set_State(
            spawnPos: gameObject.transform.position, 
            value);
    }

    // Bettery Shard: Random
    protected void Gen_RandomBS(int min, int max, int value = 1)
    {
        if (value <= 0) return;

        int amount = UnityEngine.Random.Range(min, max + 1);

        for (int i = 0; i < amount; i++) Gen_BS(value);
    }

    // Module Shard
    protected void Gen_MS(int value)
    {
        if (value <= 0) return;

        PoolingManager.instance.Get_OP_ModuleShard().Set_State(
            spawnPos: gameObject.transform.position, 
            value);
    }

    // Module Shard: Random
    protected void Gen_RandomMS(int min, int max, int value = 1)
    {
        if (value <= 0) return;

        int amount = UnityEngine.Random.Range(min, max + 1);

        for (int i = 0; i < amount; i++) Gen_MS(value);
    }

    // Joule
    protected void Gen_J(float value)
    {
        if (value <= 0) return;

        PoolingManager.instance.Get_OP_Joule().Set_State(
            spawnPos: gameObject.transform.position, 
            value);
    }

    // Overrider
    protected void Gen_Overrider(int value)
    {
        if (value <= 0) return;

        PoolingManager.instance.Get_OP_Overrider().Set_State(
            spawnPos: gameObject.transform.position,
            value);
    }

    // Credit
    protected void Gen_Credit(int value)
    {
        if (value <= 0) return;

        PoolingManager.instance.Get_OP_Credit().Set_State(
            spawnPos: gameObject.transform.position,
            value);
    }

    // Module Item
    protected void Gen_ModuleItem(int rank)
    {
        ModuleItemController module = PoolingManager.instance.Get_OP_ModuleItem();
        module.transform.SetParent(StageManager.instance.currentRoomController.transform);
        module.Set_State(this.transform.position);
        module.Set_RankState(rank);
    }

    // Keycard Item
    protected void Gen_KeycardItem(int id)
    {
        KeycardItemController keycard = PoolingManager.instance.Get_OP_KeycardItem();
        keycard.transform.SetParent(StageManager.instance.currentRoomController.transform);
        keycard.Set_State(this.transform.position);
        keycard.Set_TypeState(id);
    }

    // Core Item
    protected void Gen_CoreItem(int id)
    {
        CoreItemController core = PoolingManager.instance.Get_OP_CoreItem();
        core.transform.SetParent(StageManager.instance.currentRoomController.transform);
        core.Set_State(this.transform.position);
        core.Set_TypeState(id);
    }

    #endregion
}
