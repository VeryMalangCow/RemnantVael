
public class SortingObjectController : StaticDepthController
{
    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        AddSortingLayer(false);
    }

    protected virtual void OnDisable()
    {
        RemoveSortingLayer();
    }

    #endregion
}
