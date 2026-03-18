using UnityEngine;

public class AllyTotemeController : TotemeController
{

    protected override void PoolingSet()
    {
        PoolingManager.Instance.allyTotemes.Enqueue(this);
    }


}
