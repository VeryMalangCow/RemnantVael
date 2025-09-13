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
    [SerializeField] public List<Vector2Int> RoomVec;


    [Space(10)]
    [Header("=== Wall")]
    [SerializeField] private Transform InRoom_UpperWallParentTF;
    [SerializeField] private Transform InRoom_LowerWallParentTF;
    [SerializeField] private Transform InRoom_UpperGateParentTF;
    [SerializeField] private Transform InRoom_LowerGateParentTF;
    [SerializeField] private Transform InRoom_FieldObjSpawnerParentTF;

    [Space(10)]
    [Header("=== Room Static ID")]
    [SerializeField] public int RoomStaticID;

    #endregion

    #region - Hide

    // Rule
    [HideInInspector] public RoomRuleController RoomRuleController;
    [HideInInspector] public bool SettedPos = false;

    // Wall
    [HideInInspector] private List<StaticDepthController> InRoom_UpperWalls = new List<StaticDepthController>();
    [HideInInspector] private List<StaticDepthController> InRoom_LowerWalls = new List<StaticDepthController>();

    // Gate
    [HideInInspector] public List<GateController> InRoom_UpperGates = new List<GateController>();
    [HideInInspector] public List<GateController> InRoom_LowerGates = new List<GateController>();
    [HideInInspector] public List<GateController> InRoom_AllGate = new List<GateController>();

    // Minimap UI
    [HideInInspector] public MinimapCellEUIController ThisMME;
    [HideInInspector] public MinimapCellEUIController ThisIMME;


    #endregion

    #endregion

    #region Offset

    public override void Offset(int _ID)
    {
        base.Offset(_ID);

        // Wall
        InRoom_UpperWalls = DevTool.Get_ChildList<StaticDepthController>(InRoom_UpperWallParentTF);
        InRoom_LowerWalls = DevTool.Get_ChildList<StaticDepthController>(InRoom_LowerWallParentTF);
        InRoom_UpperGates = DevTool.Get_ChildList<GateController>(InRoom_UpperGateParentTF);
        InRoom_LowerGates = DevTool.Get_ChildList<GateController>(InRoom_LowerGateParentTF);

        InRoom_AllGate = DevTool.Get_CombineList(InRoom_UpperGates, InRoom_LowerGates);
        for (int i = 0; i < InRoom_AllGate.Count; i++)
            InRoom_AllGate[i].ThisRoom = this;
        
        RoomRuleController.Offset();
    }

    #endregion

    #region Layer

    private List<SpriteRenderer> Get_SortingSrList(Transform _ParentTF)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        foreach (Transform childTF in _ParentTF)
        {
            if (DevTool.Get_ComponentTType(childTF.gameObject, out SpriteRenderer sr))
                result.Add(sr);
        }
        return result;
    }

    private List<SpriteRenderer> Get_Sorting_EachWalls(List<StaticDepthController> _StaticDepthList)
    {
        List<SpriteRenderer> srList = new List<SpriteRenderer>();

        for (int i = 0; i < _StaticDepthList.Count; i++)
        {
            srList.AddRange(Get_SortingSrList(_StaticDepthList[i].TargetObject.transform));
        }

        return srList;
    }

    private List<SpriteRenderer> Get_Sorting_EachGates(List<GateController> _Gates, bool _IsUpper)
    {
        List<SpriteRenderer> srList = new List<SpriteRenderer>();

        for (int i = 0; i < _Gates.Count; i++)
        {
            if (_IsUpper)
            {
                _Gates[i].Set_SortingOrder(LayerOrderManager.Order_BuildUpper);
            }
            else
            {
                if (InRoom_LowerGates[i].TargetObject.TryGetComponent(out SpriteRenderer sr))
                    srList.Add(sr); 
                if (InRoom_LowerGates[i].ExtraTargetObject.TryGetComponent(out SpriteRenderer extraSr))
                    srList.Add(extraSr); 
            }

            srList.AddRange(Get_SortingSrList(_Gates[i].TargetObject.transform));
            srList.AddRange(Get_SortingSrList(_Gates[i].ExtraTargetObject.transform));
        }

        return srList;
    }

    private void Set_Sorting(List<SpriteRenderer> _SrList, int _BaseSortingIndex)
    {
        _SrList = _SrList.OrderBy(obj => obj.transform.position.y).ToList();
        for (int i = 0; i < _SrList.Count; i++)
        {
            _SrList[i].sortingOrder = _BaseSortingIndex + (_SrList.Count - i);
        }
    }

    public void Set_SortingStaticObjects()
    {
        // Upper
        List<SpriteRenderer> upperSrs = new List<SpriteRenderer>();

        upperSrs.AddRange(Get_Sorting_EachWalls(InRoom_UpperWalls));
        upperSrs.AddRange(Get_Sorting_EachGates(InRoom_UpperGates, true));

        Set_Sorting(upperSrs, LayerOrderManager.Order_BuildUpper);


        //Lower
        List<SpriteRenderer> lowerSrs = new List<SpriteRenderer>();

        lowerSrs.AddRange(Get_Sorting_EachWalls(InRoom_LowerWalls));
        lowerSrs.AddRange(Get_Sorting_EachGates(InRoom_LowerGates, false));

        Set_Sorting(lowerSrs, LayerOrderManager.Order_BuildLower);

        RoomRuleController.Set_SortingStaticObjects();
    }

    #endregion

    #region Condition

    public void Play_RoomState()
    {
        if (RoomRuleController.IsAlreadyRoomClear == true) return;

        switch (RoomRuleController.RoomType)
        {
            case eRoomType.Completed:
                Set_Completed();
                RoomRuleController.Set_Completed();
                break;

            case eRoomType.KillAll:
                RoomRuleController.Set_KillAll();
                break;

            default:
                break;
        }
    }

    private void Set_Completed()
    {
        RoomRuleController.IsAlreadyRoomClear = true;

        // Gate
        for (int i = 0; i < InRoom_AllGate.Count; i++) 
        {
            if (InRoom_AllGate[i].ParterGate != null && !InRoom_AllGate[i].IsOpen)
            {
                InRoom_AllGate[i].Set_Open();
            }
        }

        StageManager.Instance.Set_SetSpriteClearly();
        StageManager.Instance.Set_SetAnimClearly();
    }


    #endregion

    #region Set

    // 게이트와 방의 Pos 세팅하기
    public void Set_CollectGatePos(int _Index, Vector2Int _InitVec)
    {
        Vector2Int targetVec = RoomVec[_Index];
        List<GateController> posNotSettedGates = Get_PosNotSettedGates(targetVec); // 아직 월드 위치값이 지정되어 있지않는 게이트
        for (int i = 0; i < posNotSettedGates.Count; i++)
        {
            posNotSettedGates[i].RoomPosGate = _InitVec;
            posNotSettedGates[i].SettedPos = true;
        }
        RoomVec[_Index] = _InitVec;
        SettedPos = true;
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
            connectedRooms.Add(existGates[i].ParterGate.ThisRoom);
            
        }
        return connectedRooms;
    }

    #endregion

    #region Gate 

    // 존재하는 (상호작용이 가능한)
    private List<GateController> Get_ExistGates()
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (InRoom_AllGate[i].ParterGate != null)
            {
                dirGates.Add(InRoom_AllGate[i]);
            }
        }
        return dirGates;
    }

    // 방향에 맞는
    private List<GateController> Get_DirGates(Vector2Int _Dir)
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (InRoom_AllGate[i].GateDir == _Dir)
            {
                dirGates.Add(InRoom_AllGate[i]);
            }
        }
        return dirGates;
    }

    // 위치에 맞는
    private List<GateController> Get_PosGates(Vector2Int _Pos)
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (InRoom_AllGate[i].RoomPosGate == _Pos)
            {
                dirGates.Add(InRoom_AllGate[i]);
            }
        }
        return dirGates;
    }

    // 위치 값이 대입되지 않은 (아직 대입받지 않은 게이트들)
    private List<GateController> Get_NotSettedGates()
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (!InRoom_AllGate[i].SettedPos)
            {
                dirGates.Add(InRoom_AllGate[i]);
            }
        }
        return dirGates;
    }

    // 열려있는
    private List<GateController> Get_OpenedGates()
    {
        List<GateController> dirGates = new List<GateController>();
        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (InRoom_AllGate[i].IsOpen)
            {
                dirGates.Add(InRoom_AllGate[i]);
            }
        }
        // 존재하는 건 기본 조건
        return DevTool.Get_IntersectionList(Get_ExistGates(), dirGates);
    }


    // 존재하고 + 방향에 맞는
    private List<GateController> Get_ExistDirGates(Vector2Int _Dir)
    {
        return DevTool.Get_IntersectionList(
            Get_DirGates(_Dir),
            Get_ExistGates());
    }

    // 위치에 맞고 + 아직 위치값이 대입되지 않은
    private List<GateController> Get_PosNotSettedGates(Vector2Int _Pos)
    {
        return DevTool.Get_IntersectionList(
            Get_PosGates(_Pos),
            Get_NotSettedGates());
    }



    // 열려있는 + 반대편도 열려있는
    private List<GateController> Get_BothOpenedGates()
    {
        List<GateController> singleOpenedGates = Get_OpenedGates();
        List<GateController> bothOpenedGates = new List<GateController>();
        for (int i = 0; i < singleOpenedGates.Count; i++)
        {
            if (singleOpenedGates[i].ParterGate.ThisRoom.RoomRuleController.RoomType == eRoomType.Completed)
            {
                bothOpenedGates.Add(singleOpenedGates[i]);
            }
        }
        return bothOpenedGates;
    }


    // 미니맵 상호작용에서 방향인풋 값으로 알맞는 게이트를 찾기
    public GateController Get_MinimapInteract_ShortcutGate(Vector2Int _TargetVec)
    {
        // 맞는 방향에 있는 모든 문
        List<GateController> gates =
            DevTool.Get_IntersectionList(
                Get_ExistDirGates(_TargetVec),
                Get_BothOpenedGates());

        if (gates.Count > 0)
        {
            return _TargetVec.x != 0 ?
                gates.OrderBy(obj => obj.transform.position.x).ToList()[0].ParterGate :
                gates.OrderBy(obj => obj.transform.position.y).ToList()[0].ParterGate;
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

        Debug.Log(data.Count);

        for (int i = 0; i < data.Count; i++)
            EachSpawn_FieldObj(data[i]);
    }

    private void EachSpawn_FieldObj(Vector2 _Pos)
    {
        if (Instantiate(ResourceManager.Instance.Get_RandomFieldObj_Prefab()).TryGetComponent(out DestructibleObjectController ddoc))
        {
            ddoc.gameObject.transform.SetParent(InRoom_FieldObjSpawnerParentTF);
            ddoc.gameObject.transform.position = _Pos;
        }
    }

    private List<Vector2> Get_FieldObjPos()
    {
        List<FieldObjectSpawnController> fieldObjSpawners = InRoom_FieldObjSpawnerParentTF != null &&
            InRoom_FieldObjSpawnerParentTF.childCount > 0 ?
            DevTool.Get_AllChildList<FieldObjectSpawnController>(InRoom_FieldObjSpawnerParentTF) : null;

        List<Vector2> result = new List<Vector2>();

        if (fieldObjSpawners != null)
            for (int i = 0; i < fieldObjSpawners.Count; i++)
                result.AddRange(fieldObjSpawners[i].Get_RandomPointsInSector_Self());

        result.AddRange(RoomRuleController.Get_FieldObjPos());

        return result.Distinct().ToList();
    }

    #endregion
}
