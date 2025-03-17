
public class SortingObjectController : StaticDepthController
{
    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        LayerOrderManager.Instance.NeedSortingObjects.Add(this);
    }

    protected virtual void OnDisable()
    {
        LayerOrderManager.Instance.NeedSortingObjects.Remove(this);
    }

    #endregion
}
