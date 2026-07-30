
public class NormalEnemyController : EnemyController
{
    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        // Pattern
        //Start_PatternFromNone();
    }

    #endregion

    #region Die

    protected override void Set_Die()
    {
        base.Set_Die();
        AllyRequestManager.instance.Play_KillNormalEnemy();
    }

    #endregion
}
