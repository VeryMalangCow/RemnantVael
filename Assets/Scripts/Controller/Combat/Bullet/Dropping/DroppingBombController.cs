using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class DroppingBombController : DroppingDepthController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Dropping Bullet")]
    [SerializeField] protected string PoolingString = "";

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public CombatState State;
    // 스탯을 Drop형으로 바꾸던 아니면, 스탯을 추가하던 하셈

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected TrailRenderer ThisTrail;
    [SerializeField] protected Light2D ThisLight;

    #endregion

    #region - Hide

    #endregion

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        DevTool.Add_InList(LayerOrderManager.Instance.NeedSortingObjects, this);
    }

    protected void OnDisable()
    {
        DevTool.Remove_InList(LayerOrderManager.Instance.NeedSortingObjects, this);
    }

    protected override void Update()
    {
        base.Update();


    }

    #endregion

    #region Reset

    public void Reset_State()
    {
        State.Reset_State();

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
        CombatState _State, float _DroppingTime, float _TopYPos,
        BulletState_PosAndRot _State_PosAndRot,
        BulletState_Size _State_Size)
    {
        Set_State_Base(_State, _DroppingTime, _TopYPos);

        Set_State_PosAndRot(_State_PosAndRot);
        Set_State_ShadowSize(_State_Size);

        SetOn_State();
    }


    public override void Set_State_Base(CombatState _State, float _DroppingTime, float _TopYPos = 5f)
    {
        base.Set_State_Base(_State, _DroppingTime, _TopYPos);

        this.State = new CombatState(_State);
    }

    public virtual void Set_State_PosAndRot(BulletState_PosAndRot _State_PosAndRot)
    {
        this.transform.position = _State_PosAndRot.SpawnPos + (_State_PosAndRot.Dir * _State_PosAndRot.Dis);
        this.transform.localRotation = DevTool.Get_RotFromDir(_State_PosAndRot.Dir);

        DevTool.Add_RotZValue(transform, _State_PosAndRot.SpreadAngle);
    }

    protected override void SetOn_State()
    {
        gameObject.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        gameObject.SetActive(true);

        SetOn_Trail();
        SetOn_Light();

        base.SetOn_State();
    }

    #endregion

    #region Active

    protected override void Active()
    {
        Debug.Log("실행");
    }

    #endregion

    #region Sorting Order

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);

        ThisTrail.sortingOrder = _SortingOrder - 1;
    }

    #endregion

    #region Light

    protected virtual void SetOn_Light()
    {

    }

    private void SetOff_Light()
    {

    }

    #endregion

    #region Trail

    protected virtual void SetOn_Trail()
    {
        ThisTrail.Clear();

        ThisTrail.emitting = true;
        ThisTrail.enabled = true;
    }

    private void SetOff_Trail()
    {
        ThisTrail.emitting = false;
        ThisTrail.enabled = false;
    }

    #endregion

    #region Remove

    // 오브젝트 풀링 시스템과 추가 효과 등을 추상
    protected abstract void Remove_Condition();

    // 오브젝트 파괴될 때, 항상 실행
    protected void Remove_Object()
    {
        SetOff_Trail();
        SetOff_Light();

        Remove_Condition();
        Reset_State();

        this.gameObject.SetActive(false);
    }

    #endregion
}
