using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AbsorbItemController : ItemController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Absorb")]

    [Space(10)]
    [Header("=== Target")]
    [FormerlySerializedAs("TargetGO")][SerializeField] GameObject targetGo;

    [Space(10)]
    [Header("=== Absorb")]
    [FormerlySerializedAs("IsAbsorbing")][SerializeField] protected bool isAbsorbing = false;
    [FormerlySerializedAs("AbsorbStartPower")][SerializeField] private float absorbStartPower = 300f;
    [FormerlySerializedAs("AbsorbPower")][SerializeField] private float absorbPower = 5f;
    [FormerlySerializedAs("RotatePower")][SerializeField] private float rotPower = 10f;

    // Limit OnEnable
    [HideInInspector] protected bool isSpawnNow = false;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        if (isSpawnNow) rb.AddForce(DevTool.Get_RandomDir() * absorbStartPower);
    }

    protected virtual void Update()
    {
        Set_Absorb(Time.deltaTime);
    }

    #endregion

    #region State

    public override void Set_State(Vector2 spawnPos)
    {
        base.Set_State(spawnPos);

        targetGo = PlayerManager.instance.playerController.gameObject;
        isAbsorbing = false;

        transform.SetParent(StageManager.instance.currentRoomController.transform);
    }

    #endregion

    #region Absorb

    private void Set_Absorb(float deltaTime)
    {
        if (isAbsorbing)
        {
            rb.velocity = Get_AbsorbDir(deltaTime) * Get_AbsorbPower();
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 10 * Time.deltaTime);
        }
    }

    private Vector2 Get_AbsorbDir(float deltaTime)
    {
        Vector2 fromDir = rb.velocity.normalized;
        Vector2 toDir = (Vector2)(targetGo.transform.position - this.transform.position).normalized;

        return  Vector2.Lerp(fromDir, toDir, rotPower * deltaTime).normalized;
        
    }

    private float Get_AbsorbPower()
    {
        return Math.Max(Vector2.Distance(Vector2.zero, rb.velocity) * 0.99f, absorbPower);
    }

    #endregion

    #region Get Item

    protected virtual void Gain_Item()
    {
        SoundManager.instance.Play_2D_SFX_Item_Random(PlayerManager.instance.playerController.Get_AS(), "Absorb", 2);

        this.gameObject.SetActive(false);
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            Gain_Item();
        }
    }

    #endregion

}
