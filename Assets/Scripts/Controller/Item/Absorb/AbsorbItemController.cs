using System;
using UnityEngine;

public class AbsorbItemController : ItemController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Absorb")]

    [Space(10)]
    [Header("=== Target")]
    [SerializeField] GameObject TargetGO;

    [Space(10)]
    [Header("=== Absorb")]
    [SerializeField] protected bool IsAbsorbing = false;
    [SerializeField] private float AbsorbStartPower = 300f;
    [SerializeField] private float AbsorbPower = 5f;
    [SerializeField] private float RotatePower = 10f;

    // Limit OnEnable
    [HideInInspector] protected bool IsSpawnNow = false;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        if (IsSpawnNow) ThisRb.AddForce(DevTool.Get_RandomDir() * AbsorbStartPower);
    }

    protected virtual void Update()
    {
        Set_Absorb(Time.deltaTime);
    }

    #endregion

    #region State

    public override void Set_State(Vector2 _SpawnPos)
    {
        base.Set_State(_SpawnPos);

        TargetGO = PlayerManager.Instance.PlayerController.gameObject;
        IsAbsorbing = false;

        transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
    }

    #endregion

    #region Absorb

    private void Set_Absorb(float _DeltaTime)
    {
        if (IsAbsorbing)
        {
            ThisRb.velocity = Get_AbsorbDir(_DeltaTime) * Get_AbsorbPower();
        }
        else
        {
            ThisRb.velocity = Vector2.Lerp(ThisRb.velocity, Vector2.zero, 10 * Time.deltaTime);
        }
    }

    private Vector2 Get_AbsorbDir(float _DeltaTime)
    {
        Vector2 fromDir = ThisRb.velocity.normalized;
        Vector2 toDir = (Vector2)(TargetGO.transform.position - this.transform.position).normalized;

        return  Vector2.Lerp(fromDir, toDir, RotatePower * _DeltaTime).normalized;
        
    }

    private float Get_AbsorbPower()
    {
        return Math.Max(Vector2.Distance(Vector2.zero, ThisRb.velocity) * 0.99f, AbsorbPower);
    }

    #endregion

    #region Get Item

    protected virtual void Gain_Item()
    {
        SoundManager.Instance.Play_2D_SFX_Item_Random(PlayerManager.Instance.PlayerController.Get_AS(), "Absorb", 2);

        this.gameObject.SetActive(false);
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Collision)
    {
        if (_Collision.tag == "Player")
        {
            Gain_Item();
        }
    }

    #endregion

}
