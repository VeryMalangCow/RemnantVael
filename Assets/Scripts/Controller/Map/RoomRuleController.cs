using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoomController;

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
    [HideInInspector] public List<BuildingController_AllLayer> InRoom_AllBuilding;
    [SerializeField] private BuildingController_OnlyPlayerLayer InRoom_BuildThing;


    [Space(10)]
    [Header("=== In Room _ Enemy")]
    [SerializeField] private List<EnemySpot> InRoom_AllEnemy;
    [SerializeField] private Transform InRoom_WayPointParentTF;
    [HideInInspector] public List<Transform> InRoom_AllWayPoint;

    #endregion

    #region Basic

    public void Offset()
    {
        // Obstacle
        InRoom_AllBuilding = new List<BuildingController_AllLayer>();
        if (InRoom_AllBuildingParentTF != null && InRoom_AllBuildingParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_AllBuildingParentTF)
            {
                if (chile.gameObject.TryGetComponent(out BuildingController_AllLayer BC))
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
                InRoom_AllWayPoint.Add(chile);
            }
        }
    }

    #endregion

    #region Set

    public void Set_Completed()
    {
        // Extra Building
        if (InRoom_BuildThing != null && !InRoom_BuildThing.gameObject.activeSelf)
        {
            InRoom_BuildThing.gameObject.SetActive(true);
            InRoom_BuildThing.MEI.GenExplosionImgs(
                    InRoom_BuildThing.MEI.gameObject.transform.position, InRoom_BuildThing.ThisSR.sortingOrder + 1,
                    16, 0.15f, 0.75f,
                    2.0f, 0.05f, 0.1f,
                    1.0f, 0.5f, 1.0f,
                    0, StageManager.Instance.GetCurrentStageMaterial());
        }
    }


    public void Set_KillAll()
    {
        for (int i = 0; i < InRoom_AllEnemy.Count; i++)
        {
            if (InRoom_AllEnemy[i].EnemySpawnTF != null &&
                InRoom_AllEnemy[i].EnemyPrefab != null)
            {
                EnemyController enemy = PoolingManager.Instance.GetOP_Enemy(InRoom_AllEnemy[i].EnemyPrefab);

                EnemyManager.Instance.CurrentEnemyList.Add(enemy);

                enemy.transform.position = InRoom_AllEnemy[i].EnemySpawnTF.transform.position;
                enemy.gameObject.SetActive(true);
                enemy.MEI.GenExplosionImgs(
                    enemy.MEI.gameObject.transform.position, enemy.ThisSR.sortingOrder + 1,
                    16, 0.15f, 0.75f,
                    1.6f, 0.05f, 0.1f,
                    0.8f, 0.5f, 1.0f,
                    0, StageManager.Instance.GetCurrentStageMaterial());
            }
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
