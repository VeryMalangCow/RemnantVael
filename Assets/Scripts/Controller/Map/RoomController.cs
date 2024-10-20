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

    [HideInInspector] public bool SettedPos = false;

    [Space(10)]
    [Header("=== In Room _Building")]
    [SerializeField] private Transform InRoom_AllGateParentTF;
    [HideInInspector] public List<GateController> InRoom_AllGate;
    [SerializeField] private Transform InRoom_AllBuildingParentTF;
    [HideInInspector] public List<BuildingController_AllLayer> InRoom_AllBuilding;

    [SerializeField] private BuildingController_OnlyPlayerLayer InRoom_BuildThing;


    [Space(10)]
    [Header("=== In Room _Enemy")]
    [SerializeField] private List<EnemySpot> InRoom_AllEnemy;
    [SerializeField] private Transform InRoom_WayPointParentTF;
    [HideInInspector] public List<Transform> InRoom_AllWayPoint;


    [Space(10)]
    [Header("=== InitData")]
    [SerializeField] public int CurrentTempID;


    #endregion

    #region Basic

    public void Offset()
    {
        // Gate
        InRoom_AllGate = new List<GateController>();
        if (InRoom_AllGateParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_AllGateParentTF)
            {
                if (chile.gameObject.TryGetComponent(out GateController GC))
                {
                    InRoom_AllGate.Add(GC);
                    GC.ThisRoom = this;
                    GC.gameObject.SetActive(false);
                }
            }
        }

        // Obstacle
        InRoom_AllBuilding = new List<BuildingController_AllLayer>();
        if (InRoom_AllBuildingParentTF.childCount > 0)
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

    #region Gate

    public void SetCollectGateVec(int _Index, Vector2Int _InitVec)
    {
        Vector2Int targetVec = RoomVec[_Index];
        List<GateController> gates = GetCollectGateList(targetVec);
        for (int i = 0; i < gates.Count; i++)
        {
            gates[i].RoomPosGate = _InitVec;
            gates[i].SettedPos = true;
        }
        RoomVec[_Index] = _InitVec;
        SettedPos = true;
    }

    private List<GateController> GetCollectGateList(Vector2Int _TargetVec)
    {
        List<GateController> resultList = new List<GateController>();
        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (InRoom_AllGate[i].RoomPosGate == _TargetVec &&
                !InRoom_AllGate[i].SettedPos)
            {
                resultList.Add(InRoom_AllGate[i]);
            }
        }
        return resultList;
    }

    #endregion

    #region Condition

    public void PlayRoomState()
    {
        switch (RoomType)
        {
            case eRoomType.Completed:
                Set_Completed();
                break;

            case eRoomType.KillAll:
                Set_KillAll();
                break;

            default:
                break;
        }
    }

    private void Set_Completed()
    {
        for (int i = 0; i < InRoom_AllGate.Count; i++) 
        {
            if (InRoom_AllGate[i].HadParter && !InRoom_AllGate[i].gameObject.activeSelf)
            {
                InRoom_AllGate[i].gameObject.SetActive(true);
                InRoom_AllGate[i].MEI.GenExplosionImgs(
                    InRoom_AllGate[i].MEI.gameObject.transform.position,
                    36, 0.15f, 0.75f,
                    0.5f, 0.05f, 0.1f,
                    0.0f, 0.5f, 1.0f);
            }
        }

        if (InRoom_BuildThing != null && !InRoom_BuildThing.gameObject.activeSelf)
        {
            InRoom_BuildThing.gameObject.SetActive(true);
            InRoom_BuildThing.MEI.GenExplosionImgs(
                    InRoom_BuildThing.MEI.gameObject.transform.position,
                    36, 0.15f, 0.75f,
                    0.5f, 0.05f, 0.1f,
                    0.0f, 0.5f, 1.0f);
        }
    }

    private void Set_KillAll()
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
                    enemy.MEI.gameObject.transform.position,
                    36, 0.15f, 0.75f,
                    0.5f, 0.05f, 0.1f,
                    0.0f, 0.5f, 1.0f);
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
