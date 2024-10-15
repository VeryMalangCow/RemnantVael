using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region Value

    [Space(20)] 
    [Header("<><><><><> Weapon")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] public eDamageType DamageType;
    [SerializeField] public BaseUpgradeState<float> BaseDamage;
    [SerializeField] public BaseUpgradeState<float> AliveTime;
    [SerializeField] public BaseUpgradeState<float> MuzzleSpeed;
    [SerializeField] public BaseUpgradeState<float> ROF;
    [SerializeField] public BaseUpgradeState<float> CC;
    [SerializeField] public BaseUpgradeState<float> CD;
    [SerializeField] public BaseUpgradeState<float> AccuracyRate;

    [SerializeField] public float CurrentDelayROF = 0;

    [Space(10)]
    [Header("=== GunPos")]
    [SerializeField] protected List<Transform> BulletSpawnTFs;

    #endregion

    #region Fremework

    protected virtual void Update()
    {
        CaculateROF();
    }

    #endregion

    #region Fire

    private void CaculateROF()
    {
        CurrentDelayROF += Time.deltaTime * ROF.ActualState.Value;

        if (CurrentDelayROF > 1)
        {
            CurrentDelayROF = 1;
        }
    }

    #endregion
}
