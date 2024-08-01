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
        if(_DamageType == eDamageType.Physics)
        {
            LifeState.HealthPoint -= _Damage;
            if (LifeState.HealthPoint <= 0f)
            {
                Die();
            }
        }
        else
        {
            AbsorbItemController AIC = PoolingManager.Instance.GetOP_AbsorbItem();
            AIC.SetState(
                eItemType.Absorb, 
                this.gameObject.transform.position,
                _Damage);
            AIC.gameObject.SetActive(true);
        }

        
    }

    private void Die()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
}


