using System.Collections.Generic;
using UnityEngine;

public class RoomRuleController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Room Rule")]
    
    [Space(10)]
    [Header("=== Data")]

    [Space(5)]
    [Header("-- Vec")]
    [SerializeField] public List<Vector2Int> RoomVec;

    [Space(5)]
    [Header("-- Type")]
    [SerializeField] public eRoomType RoomType;
    [SerializeField] public bool IsAlreadyRoomClear = false;


    [Space(10)]
    [Header("=== Parent TF")]

    [Space(5)]
    [Header("-- Build")]
    [SerializeField] private Transform InRoom_ObstacleParentTF;

    [Space(5)]
    [Header("-- WayPoint")]
    [SerializeField] private Transform InRoom_WayPointParentTF;

    [Space(5)]
    [Header("-- Enemy")]
    [SerializeField] private Transform InRoom_EnemySpawnParentTF;

    #endregion

    #region - Hide

    [HideInInspector] private List<EnemySpawnContoller> InRoom_AllEnemySpawn;
    [HideInInspector] public List<WayPointController> InRoom_AllWayPoint;
    [HideInInspector] public List<SortingObjectController> InRoom_AllObstacle;
    [HideInInspector] private InteractableBuildController InRoom_ShopBuild;

    // Kill All
    [HideInInspector] private Dele EndDele = null;

    #endregion

    #endregion

    #region Offset

    public virtual void Offset()
    {
        InRoom_AllObstacle = InRoom_ObstacleParentTF != null && 
            InRoom_ObstacleParentTF.childCount > 0 ?
            DevTool.Get_ChildList<SortingObjectController>(InRoom_ObstacleParentTF) : null;

        InRoom_AllWayPoint = InRoom_WayPointParentTF != null &&
            InRoom_WayPointParentTF.childCount > 0 ?
            DevTool.Get_ChildList<WayPointController>(InRoom_WayPointParentTF) : null;

        InRoom_AllEnemySpawn = InRoom_EnemySpawnParentTF != null &&
            InRoom_EnemySpawnParentTF.childCount > 0 ?
            DevTool.Get_ChildList<EnemySpawnContoller>(InRoom_EnemySpawnParentTF) : null;
    }

    #endregion

    #region Set

    #region Completed

    public virtual void Set_Completed()
    {
        // Waypoint
        if (EndDele != null) EndDele(); 

        SetOn_Shop();
    }

    private void SetOn_Shop()
    {
        if (InRoom_ShopBuild != null && 
            !InRoom_ShopBuild.gameObject.activeSelf)
        {
            InRoom_ShopBuild.gameObject.SetActive(true);
        }
    }

    #endregion

    #region Kill All

    public void Set_KillAll()
    {
        // Way Point
        StageManager.Instance.Set_NavBake();
        SetOn_WayPointData();
        EndDele = new Dele(SetOff_WayPointData);

        Spawn_AllEnemy();
    }

    private void Spawn_AllEnemy()
    {
        for (int i = 0; i < InRoom_AllEnemySpawn.Count; i++)
        {
            Vector2 spawnPos = InRoom_AllEnemySpawn[i].transform.position;

            EnemyController enemy = PoolingManager.Instance.Get_OP_Enemy(InRoom_AllEnemySpawn[i].SpawnID);
            
            enemy.transform.position = spawnPos;
            enemy.gameObject.SetActive(true);

            // VFX
            UnitManager.Instance.Enemy_ExplImgGenerator.Expl_Enemy(spawnPos + (Vector2.up * enemy.TargetRange));
        }
    }

    private void SetOn_WayPointData()
    {
        for (int i = 0; i < InRoom_AllWayPoint.Count; i++)
        {
            InRoom_AllWayPoint[i].AdjacentWPList =
                DevTool.Get_AdjPoint_UseLine(InRoom_AllWayPoint[i], InRoom_AllWayPoint);
        }
    }

    private void SetOff_WayPointData()
    {
        for (int i = 0; i < InRoom_AllWayPoint.Count; i++)
        {
            InRoom_AllWayPoint[i].AdjacentWPList.Clear();
        }
    }

    #endregion

    #endregion

    #region Sorting

    public void Set_SortingStaticObjects()
    {
        if (InRoom_AllObstacle != null && InRoom_AllObstacle.Count > 0)
            LayerOrderManager.Instance.NeedSortingObjects.AddRange(InRoom_AllObstacle);
    }

    #endregion
}