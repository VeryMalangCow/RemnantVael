using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MovableObject
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]

    [Header("=== Data")]
    [SerializeField] private string EnemyID;

    [Header("=== State")]
    [SerializeField] public EnemyLifeState LifeState;

    #endregion

    #region Damaged

    public void TakeDamage(eDamageType _DamageType, float _Damage)
    {
        LifeState.HealthPoint -= _Damage;
        Debug.Log(LifeState.HealthPoint);
        if (LifeState.HealthPoint <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
}


