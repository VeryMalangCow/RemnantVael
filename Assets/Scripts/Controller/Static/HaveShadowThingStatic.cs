using UnityEngine;

public class HaveShadowThingStatic : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Have Shadow Thing")]

    [Space(10)]
    [Header("=== Shadow")]
    [SerializeField] public GameObject TargetObject;
    [SerializeField] protected float TargetRange = 0.4f;

    #endregion

    #region Framework

    protected virtual void OnEnable()
    {
        TargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }

    #endregion
}
