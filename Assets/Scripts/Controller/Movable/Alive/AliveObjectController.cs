using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AliveObjectController : MovableObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Alive Object")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] protected bool IsDead = false;
    [SerializeField] protected ReactiveProperty<float> CurrentSP = new();
    [SerializeField] protected ReactiveProperty<float> CurrentHP = new();
    [SerializeField] protected ReactiveProperty<float> CurrentEP = new();

    #endregion


    #region Dead

    protected virtual void Set_IsDead(float _Life, float _Damage)
    {
        if (_Life <= _Damage)
        {
            IsDead = true;
        }
        else
        {
            IsDead = false;
        }
    }

    #endregion
}
