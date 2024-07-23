using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region Value

    [Header("=== Input")]
    [SerializeField] public bool IsInputed = false;

    [Header("=== Base Weapon")]
    [SerializeField] private eWeaponType thisWeaponType;
    [SerializeField] private float baseDamage;
    [SerializeField] private float rateOfFire; 
    [SerializeField] protected float muzzleSpeed;
    [SerializeField] protected float aliveTime;
    [SerializeField] private float currentDelay = 0;

    //Other
    private delegate void FireDelegate();
    Dictionary<eWeaponType, FireDelegate> fireTypeDict;

    #endregion

    #region Fremework

    private void Awake()
    {
        fireTypeDict = new Dictionary<eWeaponType, FireDelegate>()
        {
            { eWeaponType.Pistol, PistolFire },
            { eWeaponType.AssaultRifle, AssaultRifleFire },
            { eWeaponType.Shotgun, ShotgunFire },
            { eWeaponType.Sniper, SniperFire }
        };
    }

    private void Update()
    {
        currentDelay += Time.deltaTime * rateOfFire;

        if(currentDelay > 1)
        {
            currentDelay = 1;
        }

        if (IsInputed && currentDelay >= 1)
        {
            fireTypeDict[thisWeaponType]();
        }
    }


    #endregion

    #region Fire

    private void PistolFire()
    {
        Debug.Log("ÇÇ½ºÅç Fire"); 

        currentDelay = 0;
    }

    private void AssaultRifleFire()
    {
        Debug.Log("¾î½äÆ® Fire");
        PlayerBulletController PBC = ObjectPoolingManager.Instance.GetOP_PlayerBulletController();
        PBC.SetState(
            this.gameObject.transform.position,
            baseDamage,
            muzzleSpeed,
            aliveTime
            );

        PBC.gameObject.SetActive(true);

        currentDelay = 0;
    }
    
    private void ShotgunFire()
    {
        Debug.Log("¼¦°Ç Fire"); 
        
        currentDelay = 0;
    }

    private void SniperFire()
    {
        Debug.Log("½º³ª Fire");
        
        currentDelay = 0;
    }

    #endregion
}

public enum eWeaponType
{
    Pistol, AssaultRifle, Shotgun, Sniper
}