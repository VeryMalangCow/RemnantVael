using System;
using UniRx;
using UnityEngine;

public class AllyController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Ally")]

    [SerializeField] private float MaxEP = 100f;
    [SerializeField] private ReactiveProperty<float> CurrentEP;

    #endregion

    #region Framework

    private void OnEnable()
    {
        if (!AllyManager.Instance.AllAllies.Contains(this))
        {
            AllyManager.Instance.AllAllies.Add(this);
        }
        CurrentEP.Value = MaxEP;
    }

    private void OnDisable()
    {
        if (AllyManager.Instance.AllAllies.Contains(this))
        {
            AllyManager.Instance.AllAllies.Remove(this);
        }
    }

    #endregion

    #region

    public void AddCurrentEP(float _AddValue)
    {
        float result = CurrentEP.Value + _AddValue;
        result = Math.Max(result, 0);
        result = Math.Min(result, MaxEP);

        CurrentEP.Value = result;
    }

    public void TakeDamage(float _DmgValue)
    {
        AddCurrentEP(-_DmgValue);
    }

    #endregion

}
