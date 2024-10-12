using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Room")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] public List<Vector2Int> RoomVec;
    [SerializeField] public eRoomType RoomType;

    [Space(10)]
    [Header("=== In Room")]
    [SerializeField] public List<BuildingController_AllLayer> InRoom_AllBuilding;


    [Space(10)]
    [Header("=== InitData")]
    [SerializeField] public int CurrentTempID;

    #endregion
}
