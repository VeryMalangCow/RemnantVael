using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class AliveObjectController : MovableObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Alive")]

    [Space(10)]
    [Header("=== Dead")]
    [SerializeField] protected bool IsDead = false;

    [Space(10)]
    [Header("=== Point")]
    [SerializeField] protected ReactiveProperty<float> CurrentSP = new();
    [SerializeField] protected ReactiveProperty<float> CurrentHP = new();
    [SerializeField] protected ReactiveProperty<float> CurrentEP = new();

    [Space(10)]
    [Header("=== Dead Particle")]
    [SerializeField] private List<DeadParticleElement> BrokenParticleData;
    [SerializeField] private float ParticleThrowDis = 1f;

    #endregion

    #region Add Point (Percent)

    // Shield
    protected void Add_PercentSP(float _Percent, float _Max)
    {
        Add_PercentPoint(ref CurrentSP, _Percent, _Max);
    }
    // Health
    protected void Add_PercentHP(float _Percent, float _Max)
    {
        Add_PercentPoint(ref CurrentHP, _Percent, _Max);
    }
    // Energy
    protected void Add_PercentEP(float _Percent, float _Max)
    {
        Add_PercentPoint(ref CurrentEP, _Percent, _Max);
    }

    // Point
    private void Add_PercentPoint(ref ReactiveProperty<float> _Value, float _Percent, float _Max)
    {
        _Value.Value = Math.Min(_Value.Value + DevTool.Get_Percent(_Percent, _Max), _Max);
    }

    #endregion

    #region Add Point (Value)

    // Shield
    protected void Add_CurrentSP(float _AddValue, float _Max)
    {
        Add_CurrentPoint(ref CurrentSP, _AddValue, _Max);
    }

    // Health
    protected void Add_CurrentHP(float _AddValue, float _Max)
    {
        Add_CurrentPoint(ref CurrentHP, _AddValue, _Max);
    }

    // Energy
    protected void Add_CurrentEP(float _AddValue, float _Max)
    {
        Add_CurrentPoint(ref CurrentEP, _AddValue, _Max);
    }


    // Point
    private void Add_CurrentPoint(ref ReactiveProperty<float> _Value, float _AddValue, float _Max)
    {
        _Value.Value = Math.Clamp(_Value.Value + _AddValue, 0, _Max);
    }

    #endregion

    #region Set Point (Value)

    protected void Set_CurrentSP(float _SetValue, float _Max, bool _LesserIsOk = false)
    {
        Set_CurrentPoint(ref CurrentSP, _SetValue, _Max, _LesserIsOk);
    }

    private void Set_CurrentPoint(ref ReactiveProperty<float> _Value, float _SetValue, float _Max, bool _LesserIsOk = false)
    {
        _Value.Value = _Value.Value > _SetValue && _LesserIsOk ?
            Math.Min(_SetValue, _Max) : _Value.Value;
    }

    #endregion

    #region Dead

    // Is Dead?
    protected virtual void Check_IsDead(float _Life)
    {
        if (_Life <= 0)
        {
            IsDead = true;
            Set_Die();
        }
        else
        {
            IsDead = false;
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
        for (int i = 0; i < BrokenParticleData.Count; i++)
        {
            DeadParticleController particle = PoolingManager.Instance.Get_OP_DeadParticle();
            particle.transform.SetParent(StageManager.Instance.currentRoomController.transform);

            particle.Play_DeadParticle(
                BrokenParticleData[i].Sprite, BrokenParticleData[i].ShadowSize, transform.position,
                _StartY: TargetRange, _ThrowDis: ParticleThrowDis, _DurTime: 1.5f, _DisappointTime: 3f);
        }
    }

    #endregion
}
