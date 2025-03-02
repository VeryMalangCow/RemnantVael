
public class SortingObjectController : StaticDepthController
{
    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        LayerOrderManager.Instance.NeedLayerObjects.Add(this);
        LayerOrderManager.Instance.Update();
    }

    protected virtual void OnDisable()
    {
        LayerOrderManager.Instance.NeedLayerObjects.Remove(this);
    }

    #endregion
}
