using UnityEngine;

public class StaticDepthController : DepthController
{
    #region Framework

    protected virtual void OnEnable()
    {
        TargetObject.transform.position = (Vector2)transform.position + (Vector2.up * TargetRange);
    }

    #endregion
}
