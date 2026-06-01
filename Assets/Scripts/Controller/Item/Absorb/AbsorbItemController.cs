using System;
using UnityEngine;

public class AbsorbItemController : ItemController
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Absorb")]

    [Space(10)]
    [Header("=== Target")]
    [SerializeField] Transform targetGo;

    [Space(10)]
    [Header("=== Absorb")]
    [SerializeField] private float absorbRange = 1f;

    [SerializeField] protected bool isAbsorbing = false;
    [SerializeField] private float absorbStartPower = 300f;
    [SerializeField] private float absorbPower = 5f;
    [SerializeField] private float rotPower = 10f;


    #endregion

    #region Pool

    public override void PoolOffset()
    {
        base.PoolOffset();
        targetGo = PlayerManager.instance.playerController.gameObject.transform;
    }

    #endregion

    #region Absorb

    public virtual void HandleAbsorb(float dt)
    {
        if (isAbsorbing)
        {
            rb.velocity = Get_AbsorbDir(dt) * Get_AbsorbPower();
        }
        else
        {
            if (!isEndSpread)
            {
                HandleSpread(dt);
            }
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 10 * Time.deltaTime);
            }

            isAbsorbing =
                Vector2.Distance(targetGo.position, transform.position) <= absorbRange;
        }
    }

    #endregion

    #region State

    public override void Set_State(Vector2 spawnPos)
    {
        base.Set_State(spawnPos);

        isAbsorbing = false;
        transform.SetParent(StageManager.instance.currentRoomController.transform);
    }

    #endregion

    #region Absorb

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
