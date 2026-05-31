using UnityEngine;

public class PlayerAttackerController : AttackerController
{
    #region Pooling

    public override void RemoveObject()
    {
        AttackerManager.instance.RemovePlayerAttacker(this);
    }

    #endregion

    #region Trigger

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        Try_Hit_Enemy(col);

        base.OnTriggerEnter2D(col);
    }

    protected void Try_Hit_Enemy(Collider2D col)
    {
        if (DevTool.Can_Collding(col, "Enemy", 
            hittedObjList, out EnemyController ec))
        {
            //Damage
            ec.Try_Hitted(this);
            hittedObjList.Add(ec);
        }
    }

    #endregion
}
