

public class NormalEnemyController : EnemyController
{
    #region Value

    #endregion

    #region Pattern


    #endregion

    #region Die

    protected override void Set_Die_Enqueue()
    {
        PoolingManager.Instance.Set_EnqueueEnemy(this);
    }

    #endregion
}
