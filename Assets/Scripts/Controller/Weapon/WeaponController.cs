using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region Value

    [Header("=== State")]
    [SerializeField] private BulletState thisBulletState;
    [SerializeField] private float rateOfFire; 
    [SerializeField] protected float currentDelay = 0;

    //Other

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
        currentDelay += Time.deltaTime * rateOfFire;

        if (currentDelay > 1)
        {
            currentDelay = 1;
        }
    }

    protected void Fire()
    {
        Debug.Log("Rifle Fire");
        PlayerBulletController PBC = PoolingManager.Instance.GetOP_PlayerBullet();
        PBC.SetState(
            this.gameObject.transform.position,
            thisBulletState.thisDamageType,
            thisBulletState.baseDamage,
            thisBulletState.muzzleSpeed,
            thisBulletState.aliveTime
            );

        PBC.gameObject.SetActive(true);

        currentDelay = 0;
    }
    

    #endregion
}
