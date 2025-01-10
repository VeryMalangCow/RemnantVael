using UnityEngine;

public class HaveShadowThingStatic : HaveShadowThing
{
    #region Framework

    protected virtual void OnEnable()
    {
        TargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }

    #endregion
}
