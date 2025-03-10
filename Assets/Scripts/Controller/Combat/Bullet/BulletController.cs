using System.Collections.Generic;
using UnityEngine;

public abstract class BulletController : MovableDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Bullet Controller")]
    [SerializeField] protected string PoolingString = "";

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public BulletState State;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected Rigidbody2D ThisRb;

    // Alive Time
    [HideInInspector] protected float CurrentAliveTime = 0;
    [HideInInspector] private static float BaseBulletSpeed = 200f; 


    [Space(10)]
    [Header("=== Judg")]
    [SerializeField] protected List<string> DestroyTagList;


    // Extra
    [Space(10)]
    [Header("=== Guided")]
    [SerializeField] protected bool IsGuided = false;
    [SerializeField] protected EnemyController TargetEnemyController = null;
    [SerializeField] protected float RotateSpeed = 1f; // Guided Power


    #endregion

    #region Framework

    protected virtual void FixedUpdate()
    {
        Play_InAlive(Time.fixedDeltaTime);
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
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        ThisRb.simulated = false;
        CurrentAliveTime = 0;
        TargetEnemyController = null;
    }

    protected virtual void Reset_Other()
    {

    }

    #endregion

    #region State

    public void Set_State(
        BulletState _State, 
        BulletState_PosAndRot _State_PosAndRot, 
        BulletState_Size? _State_Size, 
        BulletState_Anim? _State_Anim, 
        float _TargetRange = 0.4f)
    {
        Set_State_Base(_State, _TargetRange);
        Set_State_PosAndRot(_State_PosAndRot);
        Set_State_Size(_State_Size);
        Set_State_Anim(_State_Anim);
        Set_State_Extra();

        SetOn_State();
    }


    public virtual void Set_State_Base(BulletState _State, float _TargetRange = 0.4f)
    {
        this.State = new BulletState(_State, true);

        TargetRange = _TargetRange;
    }

    public virtual void Set_State_PosAndRot(BulletState_PosAndRot _State_PosAndRot)
    {
        this.transform.position = _State_PosAndRot.SpawnPos;
        this.transform.localRotation = DevTool.Get_RotFromDir(_State_PosAndRot.Dir);

        DevTool.Add_RotZValue(transform, _State_PosAndRot.SpreadAngle);
    }

    public virtual void Set_State_Size(BulletState_Size? _State_Size) { }

    public virtual void Set_State_Anim(BulletState_Anim? _State_Anim) { }

    public virtual void Set_State_Extra() { }


    private void SetOn_State()
    {
        ThisRb.simulated = true;
        gameObject.SetActive(true);
    }

    #endregion

    #region Alive


    // 살아있는 경우의 계산
    protected void Play_InAlive(float _FixedDeltaTime)
    {
        CurrentAliveTime += _FixedDeltaTime;

        if (Is_Alive() && DevTool.Is_Usable(ThisRb))
        {
            if (IsGuided)
            { 
                Play_Guided(_FixedDeltaTime); // 유도 기능
            }
            Play_FlyForward(State.MuzzleSpeed, BaseBulletSpeed, _FixedDeltaTime);
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
        return CurrentAliveTime < State.AliveTime;
    }

    // 날아가는 기능
    private void Play_FlyForward(float _MuzzleSpeed, float _StaticValue, float _FixedDeltaTime)
    {
        ThisRb.velocity = ((_MuzzleSpeed * _StaticValue * _FixedDeltaTime) * this.transform.up);
    }

    #endregion

    #region Guided

    // 유도가 가능한가? (상위 조건을 만족 시에 실조건)
    private bool Is_ExistTarget()
    {
        return DevTool.Is_Usable(TargetEnemyController) && // 타겟이 있는가
            TargetEnemyController.gameObject.activeSelf; // 타켓이 켜져있는가
    }

    // 유도 기능
    protected void Play_Guided(float _FixedDeltaTime)
    {
        if (Is_ExistTarget()) // 타겟이 검색되어 있다면, 타겟을 따라감
        {
            Set_RotToTarget(RotateSpeed, _FixedDeltaTime);
        }
        else // 타겟이 검색되어 있지않다면, 타겟을 찾음
        {
            Try_FindTarget();
        }
    }
    
    // 유도 적 찾기
    protected void Try_FindTarget()
    {
        TargetEnemyController = null;
        TargetEnemyController = EnemyManager.Instance.Get_ClosestEnemy(this.transform.position);
    }

    // 유도 적에게 (천천히, 스무스) 방향 돌리기
    protected void Set_RotToTarget(float _RotSpeed, float _FixedDeltaTime)
    {
        Quaternion fromRot = this.transform.rotation;
        Quaternion toRot = DevTool.Get_RotFromDir((TargetEnemyController.transform.position - this.transform.position).normalized);

        this.transform.rotation = Quaternion.Slerp(fromRot, toRot, _RotSpeed * _FixedDeltaTime);
    }

    #endregion

    #region Remove

    // 오브젝트 풀링 시스템과 추가 효과 등을 추상
    protected abstract void Remove_Condition();

    // 오브젝트 파괴될 때, 항상 실행
    protected void Remove_Object()
    {
        Remove_Condition();
        Reset_State();
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Trigger Judg

    protected void Try_Hit_DestructibleObject(Collider2D _Col)
    {
        if (_Col.tag == "DestructibleObject")
        {
            if (_Col.transform.parent.TryGetComponent(out DestructibleBuildController DBC))
            {
                DBC.Take_Damage(true);
            }
        }
    }

    protected void Try_Remove(string _Tag)
    {
        if (DestroyTagList.Contains(_Tag))
        {
            Remove_Object();
        }
    }

    #endregion
}