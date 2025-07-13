
public class AllyDroppingBombController : DroppingBombController
{

    #region Remove

    protected override void Remove_Condition()
    {
        PoolingManager.Instance.DroppingAllyBullet.Queue.Enqueue(this);
    }

    #endregion

}
