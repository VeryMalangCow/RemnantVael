using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    [Header("=== In Room _ Wall")]
    [SerializeField] private Transform InRoom_UpperWallParentTF;
    [HideInInspector] private List<HaveShadowThingStatic> InRoom_UpperWalls;
    [SerializeField] private Transform InRoom_LowerWallParentTF;
    [HideInInspector] private List<HaveShadowThingStatic> InRoom_LowerWalls;

    [Space(10)]
    [Header("=== In Room _ Building")]
    [SerializeField] public List<GateController> InRoom_AllUpperGate;
    [SerializeField] public List<GateController> InRoom_AllLowerGate;
    [HideInInspector] public List<GateController> InRoom_AllGate;
    [SerializeField] private Transform InRoom_AllBuildingParentTF;
    [HideInInspector] public List<BuildingController_AllLayer> InRoom_AllBuilding;

    [SerializeField] private BuildingController_OnlyPlayerLayer InRoom_BuildThing;


    [Space(10)]
    [Header("=== In Room _ Enemy")]
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
        // Wall
        InRoom_UpperWalls = new List<HaveShadowThingStatic>();
        if (InRoom_UpperWallParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_UpperWallParentTF)
            {
                if (chile.gameObject.TryGetComponent(out HaveShadowThingStatic HSTS))
                {
                    InRoom_UpperWalls.Add(HSTS);
                }
            }
        }
        InRoom_LowerWalls = new List<HaveShadowThingStatic>();
        if (InRoom_LowerWallParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_LowerWallParentTF)
            {
                if (chile.gameObject.TryGetComponent(out HaveShadowThingStatic HSTS))
                {
                    InRoom_LowerWalls.Add(HSTS);
                }
            }
        }

        // Gate
        InRoom_AllGate = new List<GateController>();
        InRoom_AllGate.AddRange(InRoom_AllUpperGate);
        InRoom_AllGate.AddRange(InRoom_AllLowerGate);
        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            InRoom_AllGate[i].ThisRoom = this;
        }


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

    #region Layer

    public void SetCorrectWallSortOrder(RoomController _RC)
    {
        if (_RC == this)
        {
            // Upper
            List<SpriteRenderer> upperSrs = new List<SpriteRenderer>();
            for (int i = 0; i < InRoom_UpperWalls.Count; i++)
            {
                foreach (Transform tf in InRoom_UpperWalls[i].TargetObject.transform)
                {
                    if (tf.TryGetComponent(out SpriteRenderer upperSr))
                    { upperSrs.Add(upperSr); }
                }
            }
            for (int i = 0; i < InRoom_AllUpperGate.Count; i++)
            {
                if (InRoom_AllUpperGate[i].TargetObject.TryGetComponent(out SpriteRenderer upperSr))
                { upperSr.sortingOrder = 1; }

                foreach (Transform tf in InRoom_AllUpperGate[i].TargetObject.transform)
                {
                    if (tf.TryGetComponent(out SpriteRenderer childUpperSr))
                    { upperSrs.Add(childUpperSr); }
                }

                foreach (Transform tf in InRoom_AllUpperGate[i].ExtraTargetObject.transform)
                {
                    if (tf.TryGetComponent(out SpriteRenderer childUpperSr))
                    { upperSrs.Add(childUpperSr); }
                }
            }

            upperSrs = upperSrs.OrderBy(obj => obj.transform.position.y).ToList();
            for (int i = 0; i < upperSrs.Count; i++)
            {
                upperSrs[i].sortingOrder = 1 + (upperSrs.Count - i);
            }


            //Lower
            List<SpriteRenderer> lowerSrs = new List<SpriteRenderer>();
            for (int i = 0; i < InRoom_LowerWalls.Count; i++)
            {
                foreach (Transform tf in InRoom_LowerWalls[i].TargetObject.transform)
                {
                    if (tf.TryGetComponent(out SpriteRenderer lowerSr))
                    { lowerSrs.Add(lowerSr); }
                }
            }
            for (int i = 0; i < InRoom_AllLowerGate.Count; i++)
            {
                if (InRoom_AllLowerGate[i].TargetObject.TryGetComponent(out SpriteRenderer lowerSr))
                { lowerSrs.Add(lowerSr); }

                foreach (Transform tf in InRoom_AllLowerGate[i].TargetObject.transform)
                {
                    if (tf.TryGetComponent(out SpriteRenderer childLowerSr))
                    { lowerSrs.Add(childLowerSr); }
                }


                if (InRoom_AllLowerGate[i].ExtraTargetObject.TryGetComponent(out SpriteRenderer extraLowerSr))
                { lowerSrs.Add(extraLowerSr); }

                foreach (Transform tf in InRoom_AllLowerGate[i].ExtraTargetObject.transform)
                {
                    if (tf.TryGetComponent(out SpriteRenderer childLowerSr))
                    { lowerSrs.Add(childLowerSr); }
                }
            }


            lowerSrs = lowerSrs.OrderBy(obj => obj.transform.position.y).ToList();
            for (int i = 0; i < lowerSrs.Count; i++)
            {
                lowerSrs[i].sortingOrder = 2000 + (lowerSrs.Count - i);
            }
            
        }
        else
        {
            List<HaveShadowThingStatic> HSTSs = new List<HaveShadowThingStatic>();
            HSTSs.AddRange(InRoom_UpperWalls);
            HSTSs.AddRange(InRoom_LowerWalls);
            for (int i = 0; i < HSTSs.Count; i++)
            {
                if (HSTSs[i].TargetObject.TryGetComponent(out SpriteRenderer sr))
                { sr.sortingOrder = 1; }
            }
        }
        
    }

    public List<HaveShadowThing> GetNeedAllLayer()
    {
        List<HaveShadowThing> HST = new List<HaveShadowThing>();

        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (InRoom_AllGate[i].NeedSetAllLayer != null && InRoom_AllGate[i].NeedSetAllLayer.Count > 0)
            { HST.AddRange(InRoom_AllGate[i].NeedSetAllLayer); }
        }

        return HST;
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
        // Gate
        for (int i = 0; i < InRoom_AllGate.Count; i++) 
        {
            if (InRoom_AllGate[i].HadParter)
            {
                if (!InRoom_AllGate[i].IsOpen)
                {
                    InRoom_AllGate[i].SetOnOff(true);
                }
            }
        }

        // Extra Building
        if (InRoom_BuildThing != null && !InRoom_BuildThing.gameObject.activeSelf)
        {
            InRoom_BuildThing.gameObject.SetActive(true);
            InRoom_BuildThing.MEI.GenExplosionImgs(
                    InRoom_BuildThing.MEI.gameObject.transform.position,
                    36, 0.15f, 0.75f,
                    1.5f, 0.05f, 0.1f,
                    0.0f, 0.5f, 1.0f,
                    0, StageManager.Instance.GetCurrentStageMaterial());
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
                    1.5f, 0.05f, 0.1f,
                    0.0f, 0.5f, 1.0f,
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
