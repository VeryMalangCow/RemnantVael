
public class SortingObjectController : StaticDepthController
{
    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        LayerOrderManager.Instance.Add_NeedSortObj(this);
    }

    protected virtual void OnDisable()
    {
        LayerOrderManager.Instance.Remove_NeedSortObj(this);
    }

    #endregion
}
