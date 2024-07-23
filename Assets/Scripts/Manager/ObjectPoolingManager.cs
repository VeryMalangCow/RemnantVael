using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{
    #region Value

    [Header("=== Player Bullet")]
    [SerializeField] private GameObject PlayerBulletPrefab;
    [SerializeField] private Transform PlayerBulletParentTF;
    [SerializeField] private Queue<PlayerBulletController> PlayerBulletQueue = new Queue<PlayerBulletController>();

    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();
    }


    #endregion

    #region Get

    public PlayerBulletController GetOP_PlayerBulletController()
    {
        // No Object
        if (PlayerBulletQueue.Count <= 0)
        {
            Debug.Log("생성");
            GameObject GenGO = Instantiate(PlayerBulletPrefab, PlayerBulletParentTF);
            GenGO.TryGetComponent(out PlayerBulletController PBC);
            GenGO.SetActive(false);
            return PBC;
        }

        Debug.Log("활성화");
        PlayerBulletController GetPlayerBulletController = PlayerBulletQueue.Dequeue();
        return GetPlayerBulletController;
    }

    public void SetOP_PlayerBulletController(PlayerBulletController playerBulletController)
    {
        PlayerBulletQueue.Enqueue(playerBulletController);
    }

    #endregion
}
