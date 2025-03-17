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
    [HideInInspector] public List<SortingObjectController> InRoom_AllBuilding;
    [SerializeField] private Transform InRoom_ShopTF;
    [SerializeField] private Transform InRoom_PrisonTF;
    [HideInInspector] private InteractableBuildController InRoom_BuildThing;
    // [HideInInspector] private 감옥 지정될 변수 
    [SerializeField] private EndingElevatorController InRoom_Elevator;

    [Space(10)]
    [Header("=== In Room _ Enemy")]
    [SerializeField] private List<EnemySpot> InRoom_AllEnemy;
    [SerializeField] private Transform InRoom_WayPointParentTF;
    [SerializeField] public List<WayPointController> InRoom_AllWayPoint;


    #endregion

    #region Offset

    public void Offset()
    {
        // Obstacle
        InRoom_AllBuilding = new List<SortingObjectController>();
        if (InRoom_AllBuildingParentTF != null && InRoom_AllBuildingParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_AllBuildingParentTF)
            {
                if (chile.gameObject.TryGetComponent(out SortingObjectController BC))
                {
                    InRoom_AllBuilding.Add(BC);
                }
            }
        }

        // Enemy
        if (InRoom_WayPointParentTF != null && InRoom_WayPointParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_WayPointParentTF)
            {
                if (chile.gameObject.TryGetComponent(out WayPointController wp))
                { InRoom_AllWayPoint.Add(wp); }
            }
        }
    }

    #endregion

    #region Shop

    public void Set_Shop(GameObject _ShopObject)
    {
        GameObject shop = Instantiate(_ShopObject, InRoom_ShopTF);
        if (shop != null)
        {
            shop.transform.localPosition = Vector3.zero;
            if (shop.TryGetComponent(out InteractableBuildController IBC))
            { InRoom_BuildThing = IBC; }
            shop.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Set

    public void Set_Completed()
    {
        // Waypoint
        SetOff_WayPointData();

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
        SetOn_WayPointData();

        for (int i = 0; i < InRoom_AllEnemy.Count; i++)
        {
            if (InRoom_AllEnemy[i].EnemySpawnTF != null)
            {
                EnemyController enemy = PoolingManager.Instance.Get_OP_Enemy(InRoom_AllEnemy[i].EnemyID);
                EnemyManager.Instance.CurrentEnemyList.Add(enemy);

                enemy.transform.position = InRoom_AllEnemy[i].EnemySpawnTF.transform.position;
                enemy.gameObject.SetActive(true);

                // VFX
                UnitManager.Instance.Enemy_ExplImgGenerator.Expl_Enemy(
                    (Vector2)InRoom_AllEnemy[i].EnemySpawnTF.transform.position + (Vector2.up * enemy.TargetRange));
            }
        }
    }


    private void SetOn_WayPointData()
    {
        for (int i = 0; i < InRoom_AllWayPoint.Count; i++)
        {
            InRoom_AllWayPoint[i].AdjacentWPList.Clear();
            InRoom_AllWayPoint[i].AdjacentWPList = new List<WayPointController>();

            for (int j = 0; j < InRoom_AllWayPoint.Count; j++)
            {
                if (InRoom_AllWayPoint[i] != InRoom_AllWayPoint[j])
                {
                    if (!Is_ExistWall(InRoom_AllWayPoint[i].transform, InRoom_AllWayPoint[j].transform))
                    {
                        InRoom_AllWayPoint[i].AdjacentWPList.Add(InRoom_AllWayPoint[j]);
                    }
                }
            }
        }
    }

    private void SetOff_WayPointData()
    {
        for (int i = 0; i < InRoom_AllWayPoint.Count; i++)
        {
            InRoom_AllWayPoint[i].AdjacentWPList.Clear();
        }
    }

    private bool Is_ExistWall(Transform _StartTF, Transform _EndTF)
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
        [SerializeField] public int EnemyID;
        [SerializeField] public Transform EnemySpawnTF;
    }

    #endregion
}
