using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerManager : Singleton<AttackerManager>, IMainGameInitializer
{
    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    // Pool
    [SerializeField] private PoolSystem<PlayerAttackerController> playerAttackerPool;

    [SerializeField] private PoolSystem<EnemyAttackerController> enemyAttackerPool;



    // Init
    public IEnumerator Initialize()
    {
        yield return playerAttackerPool.InitAsync(32, 8f);
        yield return enemyAttackerPool.InitAsync(32, 8f);

        enabled = true;
    }



    #region Player

    public PlayerAttackerController SpawnPlayerAttacker() => playerAttackerPool.Dequeue();
    public void RemovePlayerAttacker(PlayerAttackerController attacker) => playerAttackerPool.Enqueue(attacker);

    #endregion

    #region Enemy 

    public EnemyAttackerController SpawnEnemyAttacker() => enemyAttackerPool.Dequeue();
    public void RemoveEnemyAttacker(EnemyAttackerController attacker) => enemyAttackerPool.Enqueue(attacker);

    #endregion
}
