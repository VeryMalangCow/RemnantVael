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
    [SerializeField] public BulletState ThisBulletState_forSendData;
    [SerializeField] private float ROF;
    [SerializeField] private float SpreadMaxAngle = 0;
    [SerializeField] protected float CurrentDelayROF = 0;

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
        CurrentDelayROF += Time.deltaTime * ROF;

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
                PBC.SetState(BulletSpawnTF.position, randomAngle, ThisBulletState_forSendData);
                PBC.gameObject.SetActive(true);
            }

        }

        CurrentDelayROF = 0;
    }


    #endregion
}
