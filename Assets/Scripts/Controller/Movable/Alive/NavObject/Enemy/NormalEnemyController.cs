

public class NormalEnemyController : EnemyController
{
    #region Value

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        // Pattern
        Start_PatternFromNone();
    }

    #endregion

    #region Pattern


    #endregion

    #region Die

    protected override void Set_Die_Extra()
    {
        PoolingManager.instance.Set_EnqueueEnemy(this);

        AllyRequestManager.instance.Play_KillNormalEnemy();
    }

    #endregion
}
