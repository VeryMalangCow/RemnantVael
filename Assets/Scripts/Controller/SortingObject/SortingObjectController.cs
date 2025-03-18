
public class SortingObjectController : StaticDepthController
{
    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        DevTool.Add_InList(LayerOrderManager.Instance.NeedSortingObjects, this);
    }

    protected virtual void OnDisable()
    {
        DevTool.Remove_InList(LayerOrderManager.Instance.NeedSortingObjects, this);
    }

    #endregion
}
