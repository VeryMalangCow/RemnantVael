using UnityEngine;

public class EnemyExplosionController : ExplosionController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]
    [HideInInspector] public EnemyController Enemy;

    #endregion

    #region Remove

    protected override void Remove_Condition()
    {
        PoolingManager.Instance.enemyExplosions.Enqueue(this);
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D _Col)
    {
        Try_Hit_Player(_Col);

        base.OnTriggerEnter2D(_Col);
    }


    protected void Try_Hit_Player(Collider2D _Col)
    {
        if (DevTool.Can_Collding(_Col, "Player",
            HittedObjectList, out PlayerController pc))
        {
            //Damage
            PlayerManager.Instance.playerController.Try_Hitted(this);
            HittedObjectList.Add(pc);
        }
    }

    #endregion
}
