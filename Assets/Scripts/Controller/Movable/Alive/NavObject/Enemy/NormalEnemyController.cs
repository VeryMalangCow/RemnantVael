
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

    #region Die

    protected override void Set_Die_Extra()
    {
        EnemyManager.instance.RemoveNormalEnemy(this, Get_ID());

        AllyRequestManager.instance.Play_KillNormalEnemy();
    }

    #endregion
}
