using System;
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

    [Header("=== Point")]
    [SerializeField] protected ReactiveProperty<float> CurrentSP = new();
    [SerializeField] protected ReactiveProperty<float> CurrentHP = new();
    [SerializeField] protected ReactiveProperty<float> CurrentEP = new();

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_FirstSetting();
        Offset_Subscribe();
        Offset_Controller();
    }

    protected abstract void Offset_Subscribe();

    protected abstract void Offset_Controller();

    protected abstract void Offset_FirstSetting();

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
        _Value.Value = Math.Min(_Value.Value + _AddValue, _Max);
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

    }

    #endregion
}
