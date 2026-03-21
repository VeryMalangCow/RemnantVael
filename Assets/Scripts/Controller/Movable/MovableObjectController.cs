using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovableObjectController : MovableDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Movable Object")]

    [Space(10)]
    [Header("=== Component")]
    [FormerlySerializedAs("ThisRb")][SerializeField] public Rigidbody2D rb;

    [Space(10)]
    [Header("=== Movement")]
    [FormerlySerializedAs("AccelerationSpeed")][SerializeField] protected float accelerationSpeed = 12;

    [HideInInspector] protected List<CurrentKnockbackState> kbStateList = new List<CurrentKnockbackState>();

    #endregion

    #region Framework

    protected virtual void FixedUpdate()
    {
        Update_Knockback(Time.deltaTime);
    }

    #endregion

    #region Movement

    protected void Play_Walk(Vector2 moveDir, float moveSpeed, float deltaTime)
    {
        rb.velocity = Vector2.Lerp(rb.velocity, moveDir * moveSpeed, accelerationSpeed * deltaTime);
    }

    #endregion

    #region Knockback

    private void Update_Knockback(float deltaTime)
    {
        if (kbStateList.Count > 0)
        {
            for (int i = 0; i < kbStateList.Count; i++)
            {
                rb.velocity += kbStateList[i].Get_Knockback() * deltaTime * 10;
            }
        }
    }

    protected void Gain_Knockback(CurrentKnockbackState kbState)
    {
        kbStateList.Add(kbState);
        kbState.Start_Knockback()
            .OnComplete(() =>
            {
                kbStateList.Remove(kbState);
            });
    }

    #endregion

    #region Trigger

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "FieldObj", out DestructibleObjectController doc))
        {
            doc.Destruct();
        }
    }

    #endregion
}