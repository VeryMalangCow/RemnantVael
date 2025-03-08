using System.Collections.Generic;
using UnityEngine;

public class BulletController : MovableDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Bullet Controller")]
    [SerializeField] private string PoolingString = "";

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public BulletState State;
    [SerializeField] protected float CurrentAliveTime = 0;
    [HideInInspector] private static float BaseBulletSpeed = 200f; 

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] protected Rigidbody2D ThisRb;

    [Space(10)]
    [Header("=== Judg")]
    [SerializeField] protected List<string> DestroyTagList;


    // Extra
    [Space(10)]
    [Header("=== Target")]
    [SerializeField] protected bool IsGuided = false;
    [SerializeField] protected EnemyController TargetEnemyController = null;
    [SerializeField] protected float RotateSpeed = 1f;

    #endregion

    #region State

    public void Reset_State()
    {
        State.Reset_State();
        
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        CurrentAliveTime = 0;
        ThisRb.simulated = false;

        TargetEnemyController = null;
    }

    public virtual void Set_State(Vector2 _SpawnVec, float _SpreadAngle, BulletState _BulletState, float _TargetRange)
    {
        CurrentAliveTime = 0;

        this.transform.position = _SpawnVec;

        this.State = new BulletState(_BulletState, true);


        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z += _SpreadAngle;
        transform.eulerAngles = currentRotation;

        TargetRange = _TargetRange;

    }

    #endregion

    #region Framework

    protected void FixedUpdate()
    {
        CurrentAliveTime += Time.fixedDeltaTime;

        if (CurrentAliveTime >= State.AliveTime)
        {
            Remove_Object();
            return;
        }

        if (ThisRb != null)
        {
            if (IsGuided)
            {
                if (TargetEnemyController != null && TargetEnemyController.gameObject.activeSelf)
                {
                    Set_TargetDir();
                }
                else
                {
                    Set_Target();
                }
            }

            ThisRb.velocity = ((State.MuzzleSpeed * BaseBulletSpeed * Time.fixedDeltaTime) * this.transform.up);
        }
    }
    /*

    protected override void Update()
    {
        base.Update();

        CurrentAliveTime += Time.deltaTime;

        if (CurrentAliveTime >= State.AliveTime)
        {
            Remove_Object();
            return;
        }

        if (ThisRb != null)
        {
            if (IsGuided)
            {
                if (TargetEnemyController != null && TargetEnemyController.gameObject.activeSelf)
                {
                    Set_TargetDir();
                }
                else
                {
                    Set_Target();
                }
            }

            ThisRb.velocity = ((State.MuzzleSpeed * BaseBulletSpeed * Time.deltaTime) * this.transform.up);
        }

    }*/

    #endregion

    #region Remove

    protected virtual void Remove_Object()
    {
        switch (PoolingString)
        {
            case "BaseBullet":
                if (this is PlayerBulletController pbc)
                PoolingManager.Instance.PlayerBullet.Queue.Enqueue(pbc);
                break;

            case "EnemyBullet":
                if (this is EnemyBulletController ebc)
                    PoolingManager.Instance.EnemyBullets.Queue.Enqueue(ebc);
                break;

            case "MissileBullet":
                if (this is MissileBulletController mbc)
                    PoolingManager.Instance.MissileBullet.Queue.Enqueue(mbc);
                break;

            case "MI_000_Bullet":
                if (this is PlayerBulletController MI_000_pbc)
                    PoolingManager.Instance.MI_000_Bullets.Queue.Enqueue(MI_000_pbc);
                break;

            case "MI_001_Bullet":
                if (this is PlayerBulletController MI_001_pbc)
                    PoolingManager.Instance.MI_001_Bullets.Queue.Enqueue(MI_001_pbc);
                break;

            default:
                break;
        }

        Reset_State();
        this.gameObject.SetActive(false);
    }

    #endregion

    #region Angle Vector Things

    protected Vector2 Get_DirByAngle(float _Angle)
    {
        return new Vector2(
                    Mathf.Cos((_Angle + 90) * Mathf.Deg2Rad),
                    Mathf.Sin((_Angle + 90) * Mathf.Deg2Rad)).normalized;
    }

    protected Quaternion Get_RotByVec2(Vector2 _Dir)
    {
        return Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, _Dir));
    }

    #endregion

    #region Induction

    protected void Set_Target()
    {
        TargetEnemyController = null;
        TargetEnemyController = EnemyManager.Instance.Get_ClosestEnemy(this.transform.position);
    }

    protected void Set_TargetDir()
    {
        if (TargetEnemyController != null)
        {
            this.transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, ((TargetEnemyController.transform.position - this.transform.position).normalized))),
                RotateSpeed * Time.deltaTime);
        }
    }

    #endregion
}



