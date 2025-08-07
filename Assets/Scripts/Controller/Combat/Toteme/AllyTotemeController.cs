using UnityEngine;

public class AllyTotemeController : TotemeController
{
    protected override void Remove_Condition()
    {
        PoolingManager.Instance.AllyTotemes.Queue.Enqueue(this);
    }


}
