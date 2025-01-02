using System.Collections.Generic;
using UnityEngine;

public class RoomRuleController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Room")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] public List<Vector2Int> RoomVec;
    [SerializeField] public eRoomType RoomType;

    [Space(10)]
    [Header("=== In Room _ Building")]
    [SerializeField] private Transform InRoom_AllBuildingParentTF;
    [HideInInspector] public List<SortLayerObjectController> InRoom_AllBuilding;
    [SerializeField] private InteractableBuildingController InRoom_BuildThing;
    [SerializeField] private DownstartElevatorController InRoom_Elevator;

    [Space(10)]
    [Header("=== In Room _ Enemy")]
    [SerializeField] private List<EnemySpot> InRoom_AllEnemy;
    [SerializeField] private Transform InRoom_WayPointParentTF;
    [HideInInspector] public List<WayPoint> InRoom_AllWayPoint;


    #endregion

    #region Basic

    public void Offset()
    {
        // Obstacle
        InRoom_AllBuilding = new List<SortLayerObjectController>();
        if (InRoom_AllBuildingParentTF != null && InRoom_AllBuildingParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_AllBuildingParentTF)
            {
                if (chile.gameObject.TryGetComponent(out SortLayerObjectController BC))
                {
                    InRoom_AllBuilding.Add(BC);
                }
            }
        }
        // Building
        if (InRoom_BuildThing != null)
        {
            InRoom_BuildThing.gameObject.SetActive(false);
        }

        // Enemy
        if (InRoom_WayPointParentTF != null && InRoom_WayPointParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_WayPointParentTF)
            {
                if (chile.gameObject.TryGetComponent(out WayPoint wp))
                { InRoom_AllWayPoint.Add(wp); }
            }

            // Waypoint Set
            
        }

    }

    #endregion

    #region Set

    public void Set_Completed()
    {
        // Waypoint
        SetOutData_WayPoint();

        // Extra Building
        if (InRoom_BuildThing != null && !InRoom_BuildThing.gameObject.activeSelf)
        {
            InRoom_BuildThing.gameObject.SetActive(true);
        }
        if (InRoom_Elevator != null && !InRoom_Elevator.IsOn)
        {
            InRoom_Elevator.IsOn = true;
        }
    }


    public void Set_KillAll()
    {
        SetInData_WayPoint();

        for (int i = 0; i < InRoom_AllEnemy.Count; i++)
        {
            if (InRoom_AllEnemy[i].EnemySpawnTF != null &&
                InRoom_AllEnemy[i].EnemyPrefab != null)
            {
                EnemyController enemy = PoolingManager.Instance.GetOP_Enemy(InRoom_AllEnemy[i].EnemyPrefab);

                EnemyManager.Instance.CurrentEnemyList.Add(enemy);

                enemy.transform.position = InRoom_AllEnemy[i].EnemySpawnTF.transform.position;
                enemy.gameObject.SetActive(true);
            }
        }
    }


    private void SetInData_WayPoint()
    {
        for (int i = 0; i < InRoom_AllWayPoint.Count; i++)
        {
            InRoom_AllWayPoint[i].AdjacentWPList.Clear();
            InRoom_AllWayPoint[i].AdjacentWPList = new List<WayPoint>();

            for (int j = 0; j < InRoom_AllWayPoint.Count; j++)
            {
                if (InRoom_AllWayPoint[i] != InRoom_AllWayPoint[j])
                {
                    if (!IsExistWall(InRoom_AllWayPoint[i].transform, InRoom_AllWayPoint[j].transform))
                    {
                        InRoom_AllWayPoint[i].AdjacentWPList.Add(InRoom_AllWayPoint[j]);
                    }
                }
            }
        }
    }
    private void SetOutData_WayPoint()
    {
        for (int i = 0; i < InRoom_AllWayPoint.Count; i++)
        {
            InRoom_AllWayPoint[i].AdjacentWPList.Clear();
            InRoom_AllWayPoint[i].AdjacentWPList = null;
        }
    }

    private bool IsExistWall(Transform _StartTF, Transform _EndTF)
    {
        //Vector2 dirVec = _EndTF.position - _StartTF.position;
        RaycastHit2D hit = Physics2D.Linecast(_StartTF.position, _EndTF.position, LayerMask.GetMask("Wall"));

        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Enemy

    [System.Serializable]
    public class EnemySpot
    {
        [SerializeField] public GameObject EnemyPrefab;
        [SerializeField] public Transform EnemySpawnTF;
    }

    #endregion
}
