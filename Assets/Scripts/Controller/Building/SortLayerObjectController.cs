
public class SortLayerObjectController : HaveShadowThingStatic
{

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();
        LayerOrderManager.Instance.NeedLayerObjects.Add(this);
    }

    protected virtual void OnDisable()
    {
        LayerOrderManager.Instance.NeedLayerObjects.Remove(this);
    }

    #endregion
}
