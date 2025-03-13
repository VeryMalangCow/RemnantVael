using UnityEngine;

public class PlayerAttackerController : AttackerController
{
    #region Remove

    protected override void Remove_Condition()
    {
        PoolingManager.Instance.PlayerAttackers.Queue.Enqueue(this);
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
            ec.Take_Damaged(AttackerState, 
                DevTool.Is_ChanceSuccess(AttackerState.CriticalState.CC) ? true : false, 
                DevTool.Get_Dir(this.gameObject, _Col.gameObject));
            HittedObjectList.Add(ec);
        }
    }

    #endregion
}
