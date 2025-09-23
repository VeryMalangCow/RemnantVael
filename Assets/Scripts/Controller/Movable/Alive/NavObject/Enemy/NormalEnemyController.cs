

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
        PoolingManager.Instance.Set_EnqueueEnemy(this);

        AllyRequestManager.Instance.Play_KillNormalEnemy();
    }

    #endregion
}
