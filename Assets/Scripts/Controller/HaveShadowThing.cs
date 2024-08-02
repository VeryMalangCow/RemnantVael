using UnityEngine;

public class HaveShadowThing : MonoBehaviour
{
    #region Value
    [Space(20)] [Header("<><><><><> Have Shadow Thing")]

    [Header("=== Shadow")]
    [SerializeField] public GameObject TargetObject;
    [SerializeField] private float TargetRange = 0.4f;

    #endregion

    #region Framework

    protected virtual void Update()
    {
        TargetObject.transform.position = (Vector2)this.transform.position + (Vector2.up * TargetRange);
    }

    #endregion
}
