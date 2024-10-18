
using UnityEngine;

public class NormalEnemyController : EnemyController
{
    #region Framework

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FindWay(CurrentRoomController.InRoom_AllWayPoint);
        }
    }



    #endregion
}
