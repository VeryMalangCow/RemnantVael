using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> WayPoint")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] public Transform ThisTF;
    [SerializeField] public List<WayPoint> AdjacentWPList;

    #endregion

    #region Framework

    private void Awake()
    {
        ThisTF = this.transform;
    }

    #endregion
}
