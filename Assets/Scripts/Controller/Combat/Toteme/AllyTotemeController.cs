
public class AllyTotemeController : TotemeController
{

    protected override void PoolingSet()
    {
        PoolingManager.instance.allyTotemes.Enqueue(this);
    }


}
