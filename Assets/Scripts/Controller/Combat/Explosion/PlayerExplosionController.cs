using UnityEngine;

public class PlayerExplosionController : ExplosionController
{
    #region Remove

    protected override void Remove_Condition()
    {
        PoolingManager.Instance.PlayerExplosions.Queue.Enqueue(this);
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D _Col)
    {
        Try_Hit_Enemy(_Col);

        base.OnTriggerEnter2D(_Col);
    }

    protected void Try_Hit_Enemy(Collider2D _Col)
    {
        if (DevTool.Can_Collding(_Col, "Enemy",
            HittedObjectList, out EnemyController ec))
        {
            //Damage
            ec.Try_Hitted(this);
            HittedObjectList.Add(ec);
        }
    }

    #endregion
}
