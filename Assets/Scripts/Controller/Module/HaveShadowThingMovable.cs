using UnityEngine;

public class HaveShadowThingMovable : HaveShadowThing
{
    #region Framework
    protected virtual void OnEnable()
    {
        TargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }


    protected virtual void Update()
    {
        TargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }

    #endregion
}
