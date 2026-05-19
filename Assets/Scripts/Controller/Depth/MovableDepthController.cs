
public class MovableDepthController : StaticDepthController
{
    #region Framework

    protected virtual void Update()
    {
        Set_TargetPos();
    }
    
    #endregion
}
