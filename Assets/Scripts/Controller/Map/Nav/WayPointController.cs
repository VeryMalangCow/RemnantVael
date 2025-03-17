using System.Collections.Generic;
using UnityEngine;

public class WayPointController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> WayPoint")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] public List<WayPointController> AdjacentWPList = new List<WayPointController>();

    #endregion
}
