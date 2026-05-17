
public class SortingObjectController : StaticDepthController
{
    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        AddSortingLayer();
    }

    protected virtual void OnDisable()
    {
        RemoveSortingLayer();
    }

    #endregion
}
