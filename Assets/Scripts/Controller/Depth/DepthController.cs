using UnityEngine;

public class DepthController : IDController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Depth")]

    [Space(10)]
    [Header("=== Shadow")]
    [SerializeField] public GameObject TargetObject;
    [SerializeField] public SpriteRenderer ThisSR;
    [SerializeField] public float TargetRange = 0.4f;

    public int CurrentOrder { get; private set; } = int.MinValue;

    #endregion

    #region Offset

    protected virtual void Offset() { }
   

    #endregion

    #region Framework

    protected void Start()
    {
        Offset();
    }

    #endregion

    #region Sprite Renderer

    public virtual void Set_SortingOrder(int _SortingOrder)
    {
        if (CurrentOrder == _SortingOrder) return;

        CurrentOrder = _SortingOrder;

#if UNITY_EDITOR
        if (ThisSR == null)
        { Debug.Log(this.gameObject.name); return; }
#endif

        ThisSR.sortingOrder = _SortingOrder;
    }

    #endregion

    #region Set TargetPos

    protected Vector2 Get_TargetPos()
    {
        return (Vector2)transform.position + (Vector2.up * TargetRange);
    }

    protected void Set_TargetPos()
    {
        TargetObject.transform.position = Get_TargetPos();
    }

    #endregion

    #region Gen


    // Bettery Shard
    protected void Gen_BS(int _Value)
    {
        PoolingManager.Instance.Get_OP_BetteryShard().Set_State(
            _SpawnPos: gameObject.transform.position, 
            _Value);
    }

    // Bettery Shard: Random
    protected void Gen_RandomBS(int _Min, int _Max, int _Value = 1)
    {
        int amount = Random.Range(_Min, _Max + 1);

        for (int i = 0; i < amount; i++) Gen_BS(_Value);
    }

    // Module Shard
    protected void Gen_MS(int _Value)
    {
        PoolingManager.Instance.Get_OP_ModuleShard().Set_State(
            _SpawnPos: gameObject.transform.position, 
            _Value);
    }

    // Module Shard: Random
    protected void Gen_RandomMS(int _Min, int _Max, int _Value = 1)
    {
        int amount = Random.Range(_Min, _Max + 1);

        for (int i = 0; i < amount; i++) Gen_MS(_Value);
    }

    // Joule
    protected void Gen_J(float _Value)
    {
        PoolingManager.Instance.Get_OP_Joule().Set_State(
            _SpawnPos: gameObject.transform.position, 
            _Value);
    }

    // Overrider
    protected void Gen_Overrider(int _Value)
    {
        PoolingManager.Instance.Get_OP_Overrider().Set_State(
            _SpawnPos: gameObject.transform.position,
            _Value);
    }

    // Credit
    protected void Gen_Credit(int _Value)
    {
        PoolingManager.Instance.Get_OP_Credit().Set_State(
            _SpawnPos: gameObject.transform.position,
            _Value);
    }

    // Module Item
    protected void Gen_ModuleItem(int _Rank)
    {
        ModuleItemController MIC = PoolingManager.Instance.Get_OP_ModuleItem();
        MIC.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        MIC.Set_State(this.transform.position);
        MIC.Set_RankState(_Rank);
    }

    // Keycard Item
    protected void Gen_KeycardItem(int _ID)
    {
        KeycardItemController KIC = PoolingManager.Instance.Get_OP_KeycardItem();
        KIC.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        KIC.Set_State(this.transform.position);
        KIC.Set_TypeState(_ID);
    }

    #endregion
}
