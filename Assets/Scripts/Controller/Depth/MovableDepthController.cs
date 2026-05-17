
public class MovableDepthController : StaticDepthController
{
    #region Framework

    protected override void Update()
    {
        base.Update();
        Set_TargetPos();
    }
    
    #endregion
}
