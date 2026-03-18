
public class SortingObjectController : StaticDepthController
{
    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        LayerOrderManager.instance.Add_NeedSortObj(this);
    }

    protected virtual void OnDisable()
    {
        LayerOrderManager.instance.Remove_NeedSortObj(this);
    }

    #endregion
}
