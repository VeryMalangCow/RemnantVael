using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomController : IDController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Room")]

    [Space(10)]
    [Header("=== Data")]

    [Space(5)]
    [Header("-- Vec")]
    [SerializeField] public List<Vector2Int> roomVec;


    [Space(10)]
    [Header("=== Wall")]
    [SerializeField] private Transform inRoom_UpperWallParentTF;
    [SerializeField] private Transform inRoom_LowerWallParentTF;
    [SerializeField] private Transform inRoom_UpperGateParentTF;
    [SerializeField] private Transform inRoom_LowerGateParentTF;
    [SerializeField] private Transform inRoom_FieldObjSpawnerParentTF;

    [Space(10)]
    [Header("=== Room Static ID")]
    [SerializeField] public int roomStaticId;

    #endregion

    #region - Hide

    // Rule
    [HideInInspector] public RoomRuleController roomRule;
    [HideInInspector] public bool settedPos = false;

    // Wall
    [HideInInspector] private List<StaticDepthController> inRoom_UpperWalls = new List<StaticDepthController>();
    [HideInInspector] private List<StaticDepthController> inRoom_LowerWalls = new List<StaticDepthController>();

    // Gate
    [HideInInspector] public List<GateController> inRoom_UpperGates = new List<GateController>();
    [HideInInspector] public List<GateController> inRoom_LowerGates = new List<GateController>();
    [HideInInspector] public List<GateController> inRoom_AllGate = new List<GateController>();

    // Minimap UI
    [HideInInspector] public MinimapCellEUIController thisMME;
    [HideInInspector] public MinimapCellEUIController thisIMME;


    #endregion

    #endregion

    #region Offset

    public override void Offset(int id)
    {
        base.Offset(id);

        // Wall
        inRoom_UpperWalls = DevTool.Get_ChildList<StaticDepthController>(inRoom_UpperWallParentTF);
        inRoom_LowerWalls = DevTool.Get_ChildList<StaticDepthController>(inRoom_LowerWallParentTF);
        inRoom_UpperGates = DevTool.Get_ChildList<GateController>(inRoom_UpperGateParentTF);
        inRoom_LowerGates = DevTool.Get_ChildList<GateController>(inRoom_LowerGateParentTF);

        inRoom_AllGate = DevTool.Get_CombineList(inRoom_UpperGates, inRoom_LowerGates);
        for (int i = 0; i < inRoom_AllGate.Count; i++)
            inRoom_AllGate[i].thisRoom = this;
        
        roomRule.Offset();
    }

    #endregion

    #region Layer

    private List<SpriteRenderer> Get_SortingSrList(Transform parentTF)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        foreach (Transform childTF in parentTF)
        {
            if (DevTool.Get_ComponentTType(childTF.gameObject, out SpriteRenderer sr))
                result.Add(sr);
        }
        return result;
    }

    private List<SpriteRenderer> Get_Sorting_EachWalls(List<StaticDepthController> staticDepthList)
    {
        List<SpriteRenderer> srList = new List<SpriteRenderer>();

        for (int i = 0; i < staticDepthList.Count; i++)
        {
            srList.AddRange(Get_SortingSrList(staticDepthList[i].targetObject.transform));
        }

        return srList;
    }

    private List<SpriteRenderer> Get_Sorting_EachGates(List<GateController> gates, bool isUpper)
    {
        List<SpriteRenderer> srList = new List<SpriteRenderer>();

        for (int i = 0; i < gates.Count; i++)
        {
            if (isUpper)
            {
                gates[i].SetSortingOrder(SortingOrderManager.order_BuildUpper);
            }
            else
            {
                if (inRoom_LowerGates[i].targetObject.TryGetComponent(out SpriteRenderer sr))
                    srList.Add(sr); 
                if (inRoom_LowerGates[i].extraTargetObject.TryGetComponent(out SpriteRenderer extraSr))
                    srList.Add(extraSr); 
            }

            srList.AddRange(Get_SortingSrList(gates[i].targetObject.transform));
            srList.AddRange(Get_SortingSrList(gates[i].extraTargetObject.transform));
        }

        return srList;
    }

    private void Set_Sorting(List<SpriteRenderer> srList, int baseSortingIndex)
    {
        srList = srList.OrderBy(obj => obj.transform.position.y).ToList();
        for (int i = 0; i < srList.Count; i++)
        {
            srList[i].sortingOrder = baseSortingIndex + (srList.Count - i);
        }
    }

    public void Set_SortingStaticObjects()
    {
        // Upper
        List<SpriteRenderer> upperSrs = new List<SpriteRenderer>();

        upperSrs.AddRange(Get_Sorting_EachWalls(inRoom_UpperWalls));
        upperSrs.AddRange(Get_Sorting_EachGates(inRoom_UpperGates, true));

        Set_Sorting(upperSrs, SortingOrderManager.order_BuildUpper);


        //Lower
        List<SpriteRenderer> lowerSrs = new List<SpriteRenderer>();

        lowerSrs.AddRange(Get_Sorting_EachWalls(inRoom_LowerWalls));
        lowerSrs.AddRange(Get_Sorting_EachGates(inRoom_LowerGates, false));

        Set_Sorting(lowerSrs, SortingOrderManager.order_BuildLower);
    }

    #endregion

    #region Condition

    public void Play_RoomState()
    {
        if (roomRule.isAlreadyRoomClear == true) return;

        switch (roomRule.roomType)
        {
            case eRoomType.Completed: Set_Completed(); break;
            case eRoomType.KillAll: Set_KillAll(); break;
            case eRoomType.Safe: Set_Safe(); break;
            case eRoomType.Prison: Set_Prison(); break;

            default: break;
        }
    }

    public void PlaySet_RoomStateComplete()
    {
        switch (roomRule.roomType)
        {
            case eRoomType.KillAll:
                if (roomRule.enemyType == eEnemy.Normal) 
                     SoundManager.instance.Play_2D_SFX_Room("Complete_KillAll"); 
                else
                    SoundManager.instance.Play_2D_SFX_Room("BattleWin");
                break;

            default: break;
        }
        roomRule.roomType = eRoomType.Completed;

        Play_RoomState();
    }

    private void Set_KillAll()
    {
        SoundManager.instance.Play_2D_SFX_Room("Start_KillAll");

        roomRule.Set_KillAll();
    }

    private void Set_Safe()
    {
        SoundManager.instance.Play_2D_SFX_Room("Start_Safe");

        roomRule.roomType = eRoomType.Completed;
        Set_Completed();
    }

    private void Set_Prison()
    {
        SoundManager.instance.Play_2D_SFX_Room("Start_Prison");

        roomRule.roomType = eRoomType.Completed;
        Set_Completed();
    }

    private void Set_Completed()
    {
        roomRule.isAlreadyRoomClear = true;

        // Gate
        for (int i = 0; i < inRoom_AllGate.Count; i++) 
        {
            if (inRoom_AllGate[i].parterGate != null && !inRoom_AllGate[i].isOpen)
            {
                inRoom_AllGate[i].Set_Open();
            }
        }

        StageManager.instance.Set_SetSpriteClearly();
        StageManager.instance.Set_SetAnimClearly();

        roomRule.Set_Completed();
    }

    #endregion

    #region Set

    // 게이트와 방의 Pos 세팅하기
    public void Set_CollectGatePos(int index, Vector2Int initVec)
    {
        Vector2Int targetVec = roomVec[index];
        List<GateController> posNotSettedGates = Get_PosNotSettedGates(targetVec); // 아직 월드 위치값이 지정되어 있지않는 게이트
        for (int i = 0; i < posNotSettedGates.Count; i++)
        {
            posNotSettedGates[i].roomPosGate = initVec;
            posNotSettedGates[i].settedPos = true;
        }
        roomVec[index] = initVec;
        settedPos = true;
    }

    #endregion

    #region Get

    #region Room

    // 연결된 방을 모두 가져오기
    public List<RoomController> Get_ConnectedRooms()
    {
        List<RoomController> connectedRooms = new List<RoomController>();
        List<GateController> existGates = Get_ExistGates();
        for (int i = 0; i < existGates.Count; i++)
        {
            connectedRooms.Add(existGates[i].parterGate.thisRoom);
            
        }
        return connectedRooms;
    }

    #endregion

    #region Gate 

    // 존재하는 (상호작용이 가능한)
    private List<GateController> Get_ExistGates()
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < inRoom_AllGate.Count; i++)
        {
            if (inRoom_AllGate[i].parterGate != null)
            {
                dirGates.Add(inRoom_AllGate[i]);
            }
        }
        return dirGates;
    }

    // 방향에 맞는
    private List<GateController> Get_DirGates(Vector2Int dir)
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < inRoom_AllGate.Count; i++)
        {
            if (inRoom_AllGate[i].gateDir == dir)
            {
                dirGates.Add(inRoom_AllGate[i]);
            }
        }
        return dirGates;
    }

    // 위치에 맞는
    private List<GateController> Get_PosGates(Vector2Int pos)
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < inRoom_AllGate.Count; i++)
        {
            if (inRoom_AllGate[i].roomPosGate == pos)
            {
                dirGates.Add(inRoom_AllGate[i]);
            }
        }
        return dirGates;
    }

    // 위치 값이 대입되지 않은 (아직 대입받지 않은 게이트들)
    private List<GateController> Get_NotSettedGates()
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < inRoom_AllGate.Count; i++)
        {
            if (!inRoom_AllGate[i].settedPos)
            {
                dirGates.Add(inRoom_AllGate[i]);
            }
        }
        return dirGates;
    }

    // 열려있는
    private List<GateController> Get_OpenedGates()
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < inRoom_AllGate.Count; i++)
        {
            if (inRoom_AllGate[i].isOpen)
            {
                dirGates.Add(inRoom_AllGate[i]);
            }
        }
        // 존재하는 건 기본 조건
        return DevTool.Get_IntersectionList(Get_ExistGates(), dirGates);
    }


    // 존재하고 + 방향에 맞는
    private List<GateController> Get_ExistDirGates(Vector2Int dir)
    {
        return DevTool.Get_IntersectionList(
            Get_DirGates(dir),
            Get_ExistGates());
    }

    // 위치에 맞고 + 아직 위치값이 대입되지 않은
    private List<GateController> Get_PosNotSettedGates(Vector2Int pos)
    {
        return DevTool.Get_IntersectionList(
            Get_PosGates(pos),
            Get_NotSettedGates());
    }



    // 열려있는 + 반대편도 열려있는
    private List<GateController> Get_BothOpenedGates()
    {
        List<GateController> singleOpenedGates = Get_OpenedGates();
        List<GateController> bothOpenedGates = new List<GateController>();
        for (int i = 0; i < singleOpenedGates.Count; i++)
        {
            if (singleOpenedGates[i].parterGate.thisRoom.roomRule.roomType == eRoomType.Completed &&
                singleOpenedGates[i].parterGate.isOpen)
            {
                bothOpenedGates.Add(singleOpenedGates[i]);
            }
        }
        return bothOpenedGates;
    }


    // 미니맵 상호작용에서 방향인풋 값으로 알맞는 게이트를 찾기
    public GateController Get_MinimapInteract_ShortcutGate(Vector2Int targetVec)
    {
        // 맞는 방향에 있는 모든 문
        List<GateController> gates =
            DevTool.Get_IntersectionList(
                Get_ExistDirGates(targetVec),
                Get_BothOpenedGates());

        if (gates.Count > 0)
        {
            return targetVec.x != 0 ?
                gates.OrderBy(obj => obj.transform.position.x).ToList()[0].parterGate :
                gates.OrderBy(obj => obj.transform.position.y).ToList()[0].parterGate;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #endregion

    #region FieldObj

    public void Spawn_FieldObj()
    {
        List<Vector2> data = Get_FieldObjPos();

        for (int i = 0; i < data.Count; i++)
            EachSpawn_FieldObj(data[i]);
    }

    private void EachSpawn_FieldObj(Vector2 pos)
    {
        if (Instantiate(ResourceManager.instance.Get_RandomFieldObj_Prefab()).TryGetComponent(out DestructibleObjectController ddoc))
        {
            ddoc.gameObject.transform.SetParent(inRoom_FieldObjSpawnerParentTF);
            ddoc.gameObject.transform.position = pos;
        }
    }

    private List<Vector2> Get_FieldObjPos()
    {
        List<FieldObjectSpawnController> fieldObjSpawners = inRoom_FieldObjSpawnerParentTF != null &&
            inRoom_FieldObjSpawnerParentTF.childCount > 0 ?
            DevTool.Get_AllChildList<FieldObjectSpawnController>(inRoom_FieldObjSpawnerParentTF) : null;

        List<Vector2> result = new List<Vector2>();

        if (fieldObjSpawners != null)
            for (int i = 0; i < fieldObjSpawners.Count; i++)
                result.AddRange(fieldObjSpawners[i].Get_RandomPointsInSector_Self());

        result.AddRange(roomRule.Get_FieldObjPos());

        return result.Distinct().ToList();
    }

    #endregion
}
