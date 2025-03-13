using System;
using UniRx;
using UnityEngine;

public class AliveObjectController : MovableObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Alive")]

    [Space(10)]
    [Header("=== Dead")]
    [SerializeField] protected bool IsDead = false;

    [Header("=== Point")]
    [SerializeField] protected ReactiveProperty<float> CurrentSP = new();
    [SerializeField] protected ReactiveProperty<float> CurrentHP = new();
    [SerializeField] protected ReactiveProperty<float> CurrentEP = new();

    #endregion

    #region Point

    // Shield
    protected virtual void Add_CurrentSP(float _AddValue, float _Max)
    {
        Add_CurrentPoint(ref CurrentSP, _AddValue, 0, _Max);
    }

    // Health
    protected virtual void Add_CurrentHP(float _AddValue, float _Max)
    {
        Add_CurrentPoint(ref CurrentHP, _AddValue, 0, _Max);
    }

    // Energy
    protected virtual void Add_CurrentEP(float _AddValue, float _Max)
    {
        Add_CurrentPoint(ref CurrentEP, _AddValue, 0, _Max);
    }


    // Point
    private void Add_CurrentPoint(ref ReactiveProperty<float> _Value, float _AddValue, float _Min, float _Max)
    {
        _Value.Value = Math.Clamp(_Value.Value + _AddValue, _Min, _Max);
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

    }

    #endregion
}
