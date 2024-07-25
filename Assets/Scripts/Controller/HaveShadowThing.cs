using UnityEngine;

public class HaveShadowThing : MonoBehaviour
{
    #region Value

    [Header("=== Shadow")]
    [SerializeField] private GameObject ShadowObject;
    [SerializeField] private float ShadowRange = 0.35f;

    #endregion

    #region Framework

    protected virtual void Update()
    {
        ShadowObject.transform.position = (Vector2)this.transform.position + (Vector2.down * ShadowRange);
    }

    #endregion
}
