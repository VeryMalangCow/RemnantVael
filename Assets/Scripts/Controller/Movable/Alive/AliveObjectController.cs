using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class AliveObjectController : MovableObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Alive")]

    [Space(10)]
    [Header("=== Dead")]
    [FormerlySerializedAs("IsDead")][SerializeField] protected bool isDead = false;

    [Space(10)]
    [Header("=== Point")]
    [FormerlySerializedAs("CurrentSP")][SerializeField] protected ReactiveProperty<float> currentSP = new();
    [FormerlySerializedAs("CurrentHP")][SerializeField] protected ReactiveProperty<float> currentHP = new();
    [FormerlySerializedAs("CurrentEP")][SerializeField] protected ReactiveProperty<float> currentEP = new();

    [Space(10)]
    [Header("=== Dead Particle")]
    [FormerlySerializedAs("BrokenParticleData")][SerializeField] private List<DeadParticleElement> brokenParticleData;
    [FormerlySerializedAs("ParticleThrowDis")][SerializeField] private float particleThrowDis = 1f;

    #endregion

    #region Add Point (Percent)

    // Shield
    protected void Add_PercentSP(float percent, float max)
    {
        Add_PercentPoint(ref currentSP, percent, max);
    }
    // Health
    protected void Add_PercentHP(float percent, float max)
    {
        Add_PercentPoint(ref currentHP, percent, max);
    }
    // Energy
    protected void Add_PercentEP(float percent, float max)
    {
        Add_PercentPoint(ref currentEP, percent, max);
    }

    // Point
    private void Add_PercentPoint(ref ReactiveProperty<float> refValue, float percent, float max)
    {
        refValue.Value = Math.Min(refValue.Value + DevTool.Get_Percent(percent, max), max);
    }

    #endregion

    #region Add Point (Value)

    // Shield
    protected void Add_CurrentSP(float addValue, float max)
    {
        Add_CurrentPoint(ref currentSP, addValue, max);
    }

    // Health
    protected void Add_CurrentHP(float addValue, float max)
    {
        Add_CurrentPoint(ref currentHP, addValue, max);
    }

    // Energy
    protected void Add_CurrentEP(float addValue, float max)
    {
        Add_CurrentPoint(ref currentEP, addValue, max);
    }


    // Point
    private void Add_CurrentPoint(ref ReactiveProperty<float> refValue, float addValue, float max)
    {
        refValue.Value = Math.Clamp(refValue.Value + addValue, 0, max);
    }

    #endregion

    #region Set Point (Value)

    protected void Set_CurrentSP(float setValue, float max, bool lesserIsOk = false)
    {
        Set_CurrentPoint(ref currentSP, setValue, max, lesserIsOk);
    }

    private void Set_CurrentPoint(ref ReactiveProperty<float> refValue, float setValue, float max, bool lesserIsOk = false)
    {
        refValue.Value = refValue.Value > setValue && lesserIsOk ?
            Math.Min(setValue, max) : refValue.Value;
    }

    #endregion

    #region Dead

    // Is Dead?
    protected virtual void Check_IsDead(float life)
    {
        if (life <= 0)
        {
            isDead = true;
            Set_Die();
        }
        else
        {
            isDead = false;
        }
    }

    // Dead!
    protected virtual void Set_Die()
    {
        Play_DeadParticle();
    }

    #endregion

    #region Dead (Particle)

    protected void Play_DeadParticle()
    {
        for (int i = 0; i < brokenParticleData.Count; i++)
        {
            DeadParticleController particle = PoolingManager.instance.Get_OP_DeadParticle();
            particle.transform.SetParent(StageManager.instance.currentRoomController.transform);

            particle.Play_DeadParticle(
                brokenParticleData[i].sprite, brokenParticleData[i].shadowSize, transform.position,
                startY: targetRange, throwDis: particleThrowDis, durTime: 1.5f, disappointTime: 3f);
        }
    }

    #endregion
}
