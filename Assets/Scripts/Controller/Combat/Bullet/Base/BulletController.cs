using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class BulletController : MovableDepthController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Bullet")]
    [SerializeField] protected string poolingString = "";

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public BulletState state;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected TrailRenderer trail;
    [SerializeField] protected Light2D light2d;

    [Space(10)]
    [Header("=== Judg")]
    [SerializeField] protected List<string> destroyTagList;

    // Extra
    [Space(10)]
    [Header("=== Guided")]
    [SerializeField] protected bool isGuided = false;
    [SerializeField] protected EnemyController enemy = null;
    [SerializeField] protected float rotSpeed = 1f; // Guided Power

    #endregion

    #region - Hide

    // Alive Time
    [HideInInspector] protected float currentAliveTime = 0;
    [HideInInspector] private static float baseBulletSpeed = 200f; 

    #endregion

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        AddSortingLayer();
    }

    protected void OnDisable()
    {
        RemoveSortingLayer();
    }


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
        Set_Guided(false);

        rb.simulated = false;
        currentAliveTime = 0;
        enemy = null;
    }

    protected virtual void Reset_Other()
    {

    }

    #endregion

    #region State

    public void Set_State(
        BulletState state, 
        BulletState_PosAndRot state_PosAndRot, 
        BulletState_Size? state_Size, 
        State_Anim? state_Anim,
        BulletState_Effect? state_Effect,
        float targetRange = 0.4f)
    {
        //UnitManager.instance.Add_Unit(this);

        Set_State_Base(state, targetRange);
        Set_State_PosAndRot(state_PosAndRot);
        Set_State_Size(state_Size);
        Set_State_Anim(state_Anim);
        Set_State_Effect(state_Effect);
        Set_State_Extra();

        rb.simulated = true;
        //SetOn_State();
    }


    public virtual void Set_State_Base(BulletState state, float targetRange = 0.4f)
    {
        this.state = new BulletState(state, false);

        base.targetRange = targetRange;
    }

    public virtual void Set_State_PosAndRot(BulletState_PosAndRot state_PosAndRot)
    {
        this.transform.position = state_PosAndRot.spawnPos + (state_PosAndRot.dir * state_PosAndRot.dis);
        this.transform.localRotation = DevTool.Get_RotFromDir(state_PosAndRot.dir);

        DevTool.Add_RotZValue(transform, state_PosAndRot.spreadAngle);
    }

    public virtual void Set_State_Size(BulletState_Size? state_Size) { }

    public virtual void Set_State_Anim(State_Anim? state_Anim) { }

    public virtual void Set_State_Effect(BulletState_Effect? state_Effect) { }

    public virtual void Set_State_Extra() { }


    protected void SetOn_State()
    {
        currentAliveTime = 0;

        gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);
        gameObject.SetActive(true);

        SetOn_Trail();
        SetOn_Light();
    }

    #endregion

    #region Sorting Order

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);

        trail.sortingOrder = sortingOrder - 1;
    }

    #endregion

    #region Light

    protected virtual void SetOn_Light()
    {

    }

    protected void SetOff_Light()
    {

    }

    #endregion

    #region Trail

    protected virtual void SetOn_Trail()
    {
        trail.Clear();

        trail.emitting = true;
        trail.enabled = true;
    }

    protected void SetOff_Trail()
    {
        trail.emitting = false;
        trail.enabled = false;
    }

    #endregion

    #region Alive


    // 살아있는 경우의 계산
    public void PlayInAlive(float fixedDeltaTime)
    {
        currentAliveTime += fixedDeltaTime;

        if (Is_Alive() && DevTool.Is_Usable(rb))
        {
            if (isGuided)
            { 
                Play_Guided(fixedDeltaTime); // 유도 기능
            }
            Play_FlyForward(state.muzzleSpeed, baseBulletSpeed, fixedDeltaTime);
        }
        else
        {
            Remove_Object();
            return;
        }
    }

    // 살아있는가? (AliveTime)
    private bool Is_Alive()
    {
        return currentAliveTime < state.aliveTime;
    }

    // 날아가는 기능
    private void Play_FlyForward(float muzzleSpeed, float staticValue, float fixedDeltaTime)
    {
        rb.velocity = ((muzzleSpeed * staticValue * fixedDeltaTime) * this.transform.up);
    }

    #endregion

    #region Guided

    // 유도가 가능한가? (상위 조건을 만족 시에 실조건)
    private bool Is_ExistTarget()
    {
        return DevTool.Is_Usable(enemy) && // 타겟이 있는가
            enemy.gameObject.activeSelf; // 타켓이 켜져있는가
    }

    // 유도 기능
    protected void Play_Guided(float fixedDeltaTime)
    {
        if (Is_ExistTarget()) // 타겟이 검색되어 있다면, 타겟을 따라감
        {
            Set_RotToTarget(rotSpeed, fixedDeltaTime);
        }
        else // 타겟이 검색되어 있지않다면, 타겟을 찾음
        {
            Try_FindTarget();
        }
    }
    
    // 유도 적 찾기
    protected void Try_FindTarget()
    {
        enemy = null;
        enemy = EnemyManager.instance.Get_ClosestEnemy(this.gameObject);
    }

    // 유도 적에게 (천천히, 스무스) 방향 돌리기
    protected void Set_RotToTarget(float rotSpeed, float fixedDeltaTime)
    {
        Quaternion fromRot = this.transform.rotation;
        Quaternion toRot = DevTool.Get_RotFromDir((enemy.transform.position - this.transform.position).normalized);

        this.transform.rotation = Quaternion.Slerp(fromRot, toRot, rotSpeed * fixedDeltaTime);
    }

    #endregion

    #region Remove


    // 오브젝트 파괴될 때, 항상 실행
    protected abstract void Remove_Object();

    #endregion

    #region Trigger

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_DestructibleObject(col);

        Try_Remove(col.tag);
    }

    protected void Try_Hit_DestructibleObject(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "DestructibleObject", out DestructibleBuildController dbc))
        {
            dbc.Take_Damage(spawnItem: true, soundOn: true);
        }

        else if (DevTool.Can_Collding(col, "FieldObj", out DestructibleObjectController doc))
        {
            doc.Destruct();
        }
    }

    protected void Try_Remove(string tag)
    {
        if (destroyTagList.Contains(tag))
        {
            ExtraEffect();
            Remove_Object();
        }
    }

    #endregion

    #region Set

    public void Set_Guided(bool onOff, float power = 0, EnemyController targetEC = null)
    {
        isGuided = onOff;
        rotSpeed = power;
        enemy = targetEC;
    }

    #endregion

    #region Effect

    protected abstract void ExtraEffect();

    #endregion

    #region Pooling

    protected abstract void PoolingSet();

    #endregion
}