using System;
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

    [SerializeField] private float SpreadMaxAngle = 0;
    [SerializeField] public float CurrentDelayROF = 0;
    [SerializeField] private float fireMinDisLimit = 4;

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


    protected void Fire<T>(List<T> _Ts)
    {
        float randomAngle = UnityEngine.Random.Range(-SpreadMaxAngle, SpreadMaxAngle);
        foreach (Transform BulletSpawnTF in BulletSpawnTFs)
        {
            int index = BulletSpawnTFs.IndexOf(BulletSpawnTF);
            PlayerBulletController PBC = GameManager.CastIfPossible<PlayerBulletController>(_Ts[index]);
            if (PBC)
            {
                // Critical
                float rcc = UnityEngine.Random.Range(0f, 1f);
                bool isCritical = false;
                if (rcc < CC.ActualState.Value)
                {
                    isCritical = true;
                }

                // Base State
                BulletState bulletState = new BulletState(DamageType, BaseDamage.ActualState.Value, MuzzleSpeed.ActualState.Value, AliveTime.ActualState.Value);
                PBC.SetState(BulletSpawnTF.position, randomAngle, bulletState, fireMinDisLimit, isCritical, CD.ActualState.Value);
                PBC.gameObject.SetActive(true);
            }

        }

        CurrentDelayROF = 0;
    }


    #endregion
}
