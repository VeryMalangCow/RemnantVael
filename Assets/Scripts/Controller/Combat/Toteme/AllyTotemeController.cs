using UnityEngine;

public class AllyTotemeController : TotemeController
{

    protected override void PoolingSet()
    {
        PoolingManager.Instance.AllyTotemes.Enqueue(this);
    }


}
