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
    [SerializeField] private Transform ThisTF;
    [SerializeField] private List<WayPoint> AdjacentWPList;

    #endregion

    #region Framework

    private void Awake()
    {
        ThisTF = this.transform;
    }

    #endregion

    #region Class

    public WayPoint()
    {

    }

    public WayPoint(Transform _ThisTF)
    {
        ThisTF = _ThisTF;
    }

    #endregion
}
