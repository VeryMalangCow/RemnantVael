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
        if (base.IsDead) 
        { return; }

        if(_DamageType == eDamageType.Physics)
        {
            SetIsDead(LifeState.HealthPoint, _Damage);
            LifeState.HealthPoint -= _Damage;
            if (LifeState.HealthPoint <= 0f)
            {
                base.IsDead = true;
                SpawnBS(4);
                Die();
            }
        }
        else
        {
            SpawnES(_Damage);
        }


    }

    private void Die()
    {
        

        this.gameObject.SetActive(false);
    }

    #endregion

    #region Spawn Item

    private void SpawnES(float _Value)
    {
        EnergyShrapnelController ESC = PoolingManager.Instance.GetOP_EnergyShrapnel();
        ESC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            _Value);
        ESC.gameObject.SetActive(true);
    }

    private void SpawnBS(int _Value)
    {
        BetteryShrapnelController BSC = PoolingManager.Instance.GetOP_BetteryShrapnel();
        BSC.SetState(
            this.gameObject.transform.position,
            PlayerManager.Instance.PlayerController.gameObject,
            _Value);
        BSC.gameObject.SetActive(true);
    }

    #endregion
}


