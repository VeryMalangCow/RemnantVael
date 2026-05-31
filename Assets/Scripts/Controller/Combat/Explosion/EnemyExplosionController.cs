using UnityEngine;

public class EnemyExplosionController : ExplosionController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]
    [HideInInspector] public EnemyController enemy;

    #endregion

    #region Remove

    public override void RemoveObject()
    {
        ExplosionManager.instance.RemoveEnemyExplosion(this);
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_Player(col);

        base.OnTriggerEnter2D(col);
    }


    protected void Try_Hit_Player(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "Player",
            hittedObjectList, out PlayerController pc))
        {
            //Damage
            PlayerManager.instance.playerController.Try_Hitted(this);
            hittedObjectList.Add(pc);
        }
    }

    #endregion
}
