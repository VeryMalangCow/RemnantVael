using UnityEngine;

public abstract class DroppingBombController : DroppingDepthController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Dropping Bullet")]
    [SerializeField] protected string poolingString = "";

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public CombatState state;
    [SerializeField] protected AttackSizeState sizeState;
    // 스탯을 Drop형으로 바꾸던 아니면, 스탯을 추가하던 하셈

    #endregion

    #endregion

    #region Reset

    public void Reset_State()
    {
        state.Reset_State();

        Reset_BaseBullet();
        Reset_Other();
    }

    private void Reset_BaseBullet()
    {
        transform.position = new Vector3(1000, 0, 0);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    protected virtual void Reset_Other()
    {

    }

    #endregion

    #region State

    public void Set_State(
        CombatState state, float droppingTime, float topYPos, float bottomYPos,
        BulletState_PosAndRot state_PosAndRot,
        BulletState_Size state_Size)
    {
        UnitManager.instance.Add_Unit(this);

        Set_State_Base(state, droppingTime, topYPos, bottomYPos);

        Set_State_PosAndRot(state_PosAndRot);
        Set_State_ShadowSize(state_Size);

        SetOn_State();
    }


    public override void Set_State_Base(CombatState state, float droppingTime, float topYPos = 5f, float dropBottomYPos = 0f)
    {
        base.Set_State_Base(state, droppingTime, topYPos, dropBottomYPos);

        this.state = new CombatState(state);
    }

    public virtual void Set_State_PosAndRot(BulletState_PosAndRot state_PosAndRot)
    {
        this.transform.position = state_PosAndRot.spawnPos + (state_PosAndRot.dir * state_PosAndRot.dis);
        this.transform.localRotation = DevTool.Get_RotFromDir(state_PosAndRot.dir);

        DevTool.Add_RotZValue(transform, state_PosAndRot.spreadAngle);
    }

    public override void Set_State_ShadowSize(BulletState_Size state_Size)
    {
        base.Set_State_ShadowSize(state_Size);

        sizeState = new AttackSizeState(state_Size.objSize.x);
    }

    protected override void SetOn_State()
    {
        gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);
        gameObject.SetActive(true);

        SetOn_Trail();

        base.SetOn_State();
    }

    #endregion

    #region Explosion

    protected override void Active()
    {
        Gen_Explosion();

        Remove_Object();
    }

    protected abstract void Gen_Explosion();

    #endregion

    #region Sorting Order

    public override void Set_SortingOrder(int sortingOrder)
    {
        base.Set_SortingOrder(sortingOrder);

        trail.sortingOrder = sortingOrder - 1;
    }

    #endregion

    #region Trail

    protected virtual void SetOn_Trail()
    {
        trail.Clear();

        trail.emitting = true;
        trail.enabled = true;
    }

    private void SetOff_Trail()
    {
        trail.emitting = false;
        trail.enabled = false;
    }

    #endregion

    #region Remove


    // 오브젝트 파괴될 때, 항상 실행
    private void Remove_Object()
    {
        UnitManager.instance.Remove_Unit(this);

        RemoveForce_Object();
    }

    public void RemoveForce_Object()
    {
        SetOff_Trail();

        Reset_State();
        PoolingSet();

        this.gameObject.SetActive(false);
    }

    #endregion

    #region Pooling

    // 오브젝트 풀링 시스템과 추가 효과 등을 추상
    protected abstract void PoolingSet();

    #endregion
}
