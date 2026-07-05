using System;
using System.Collections.Generic;
using UniRx;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public abstract class AliveObjectController : MovableObjectController
{
    #region Value


    [Space(20)]
    [Header("<><><><><> Alive")]


    [Space(10)]
    [Header("=== Dead")]
    [SerializeField] protected bool isDead = false;

    [Space(10)]
    [Header("=== Point")]
    [SerializeField] protected ReactiveProperty<float> currentSP = new();
    [SerializeField] protected ReactiveProperty<float> currentHP = new();
    public float currentEp { get; private set; } = 0f;

    [Space(10)]
    [Header("=== Dead Particle")]
    [SerializeField] private List<DeadParticleElement> brokenParticleData;
    [SerializeField] private float particleThrowDis = 1f;

    #endregion

    public void SetCurrentEp(float value)
        => currentEp = value;
    
    protected void AddCurrentEp(float addValue, float max)
        => currentEp = Math.Clamp(currentEp + addValue, 0, max);
    
    protected void AddPercentEp(float percent, float max)
        => currentEp = Math.Clamp(currentEp + DevTool.GetPercent(percent, max), 0, max);
    

   
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
   

    // Point
    private void Add_PercentPoint(ref ReactiveProperty<float> refValue, float percent, float max)
    {
        refValue.Value = Math.Min(refValue.Value + DevTool.GetPercent(percent, max), max);
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
    protected virtual void CheckIsDead(float life)
    {
        if (life <= 0)
        {
            if (isDead) return;
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
            DeadParticleController particle = VFXManager.instance.SpawnDeadParticle();
            particle.transform.SetParent(StageManager.instance.currentRoomController.transform);

            particle.Play_DeadParticle(brokenParticleData[i], transform.position,
                startY: targetRange, throwDis: particleThrowDis, durTime: 1.5f, disappointTime: 3f);
        }
    }

    #endregion
}
