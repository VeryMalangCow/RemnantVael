using UnityEngine;

public class MovableDepthController : StaticDepthController
{
    #region Framework

    protected virtual void Update()
    {
        TargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }
    
    #endregion
}
