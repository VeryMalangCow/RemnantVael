using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomController : MonoBehaviour
{
    #region Value

    // Id
    public int id { get; private set; }
    public int roomTypeId { get; private set; }
    private bool isAlreadyRoomClear  = false;

    [Space(10)]
    [Header("=== Pos")]
    [SerializeField] private Vector2Int[] roomVec;
    [SerializeField] private Vector2Int[] roomVecWorld;
    public Vector2Int[] RoomVec => roomVec;
    public Vector2Int[] RoomVecWorld => roomVecWorld;

    [Space(10)]
    [Header("=== Wall")]
    [SerializeField] private Transform inRoom_UpperWallParentTF;
    [SerializeField] private Transform inRoom_LowerWallParentTF;
    private List<StaticDepthController> inRoom_UpperWalls = new List<StaticDepthController>();
    private List<StaticDepthController> inRoom_LowerWalls = new List<StaticDepthController>();

    [Space(10)]
    [Header("=== Gate")]
    [SerializeField] private Transform inRoom_UpperGateParentTF;
    [SerializeField] private Transform inRoom_LowerGateParentTF;
    private List<GateController> inRoom_UpperGates = new List<GateController>();
    private List<GateController> inRoom_LowerGates = new List<GateController>();
    public List<GateController> inRoom_AllGate { get; private set; } = new List<GateController>();


    [Space(10)]
    [Header("=== FieldObj")]
    [SerializeField] private Transform inRoom_FieldObjSpawnerParentTF;


    // Visual
    private StageThemeSO stageThemeSO;
    private List<RoomVisualSprite> clearModeVisualSprites;

    // Rule
    public RoomRuleController roomRule { get; private set; }

    // Minimap UI
    [HideInInspector] public MinimapCellEUIController thisMME;
    [HideInInspector] public MinimapCellEUIController thisIMME;

    #endregion

    #region Offset

    public void Offset(RoomRuleController rule, int instanceId, int typeId, Vector2Int worldGridPivot, StageThemeSO stageThemeSO)
    {
        id = instanceId;
        roomTypeId = typeId;
        roomRule = rule;
        roomVecWorld = new Vector2Int[roomVec.Length];
        for (int i = 0; i < roomVecWorld.Length; i++)
            roomVecWorld[i] = roomVec[i] + worldGridPivot;

        transform.position = (Vector2)(worldGridPivot * StageObjectGenerator.offsetRoomSize);

        // Wall
        inRoom_UpperWalls = DevTool.Get_ChildList<StaticDepthController>(inRoom_UpperWallParentTF);
        inRoom_LowerWalls = DevTool.Get_ChildList<StaticDepthController>(inRoom_LowerWallParentTF);
        inRoom_UpperGates = DevTool.Get_ChildList<GateController>(inRoom_UpperGateParentTF);
        inRoom_LowerGates = DevTool.Get_ChildList<GateController>(inRoom_LowerGateParentTF);

        inRoom_AllGate = DevTool.Get_CombineList(inRoom_UpperGates, inRoom_LowerGates);
        for (int i = 0; i < inRoom_AllGate.Count; i++)
            inRoom_AllGate[i].thisRoom = this;

        roomRule.Offset();

        this.stageThemeSO = stageThemeSO;

        InitVisualSprite();
    }

    #endregion

    #region Visual

    private void InitVisualSprite()
    {
        clearModeVisualSprites = new List<RoomVisualSprite>();
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.TryGetComponent(out RoomVisualSprite visualSprite))
            {
                visualSprite.SetSprite(stageThemeSO);
                if (visualSprite.IsClearVisualMode)
                    clearModeVisualSprites.Add(visualSprite);
            }
        }
    }

    private void SetClearModeVisualSprites()
    {
        for (int i = 0; i < clearModeVisualSprites.Count; i++)
        {
            clearModeVisualSprites[i].SetClearMaterial(stageThemeSO);
        }
    }

    #endregion

    #region Layer

    private List<SpriteRenderer> GetSortingSprites(Transform parentTF)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        foreach (Transform childTF in parentTF)
        {
            if (DevTool.Get_ComponentTType(childTF.gameObject, out SpriteRenderer sr))
                result.Add(sr);
        }
        return result;
    }

    private List<SpriteRenderer> GetSortingEachWalls(List<StaticDepthController> staticDepthList)
    {
        List<SpriteRenderer> srList = new List<SpriteRenderer>();

        for (int i = 0; i < staticDepthList.Count; i++)
        {
            srList.AddRange(GetSortingSprites(staticDepthList[i].targetObject.transform));
        }

        return srList;
    }

    private List<SpriteRenderer> GetSortingEachGates(List<GateController> gates, bool isUpper)
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

            srList.AddRange(GetSortingSprites(gates[i].targetObject.transform));
            srList.AddRange(GetSortingSprites(gates[i].extraTargetObject.transform));
        }

        return srList;
    }

    private void SetSorting(List<SpriteRenderer> srList, int baseSortingIndex)
    {
        srList = srList.OrderBy(obj => obj.transform.position.y).ToList();
        for (int i = 0; i < srList.Count; i++)
        {
            srList[i].sortingOrder = baseSortingIndex + (srList.Count - i);
        }
    }

    public void SetSortingStaticObjects()
    {
        // Upper
        List<SpriteRenderer> upperSrs = new List<SpriteRenderer>();

        upperSrs.AddRange(GetSortingEachWalls(inRoom_UpperWalls));
        upperSrs.AddRange(GetSortingEachGates(inRoom_UpperGates, true));

        SetSorting(upperSrs, SortingOrderManager.order_BuildUpper);


        //Lower
        List<SpriteRenderer> lowerSrs = new List<SpriteRenderer>();

        lowerSrs.AddRange(GetSortingEachWalls(inRoom_LowerWalls));
        lowerSrs.AddRange(GetSortingEachGates(inRoom_LowerGates, false));

        SetSorting(lowerSrs, SortingOrderManager.order_BuildLower);
    }

    #endregion

    #region Condition

    public void Play_RoomState()
    {
        if (isAlreadyRoomClear == true) return;

        switch (roomRule.roomType)
        {
            case eRoomType.Completed: Complete(); break;
            case eRoomType.KillAll: KillAll(); break;
            case eRoomType.Safe: Safe(); break;
            case eRoomType.Prison: Prison(); break;

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

    private void KillAll()
    {
        SoundManager.instance.Play_2D_SFX_Room("Start_KillAll");

        roomRule.KillAll();
    }

    private void Safe()
    {
        SoundManager.instance.Play_2D_SFX_Room("Start_Safe");

        roomRule.roomType = eRoomType.Completed;
        Complete();
    }

    private void Prison()
    {
        SoundManager.instance.Play_2D_SFX_Room("Start_Prison");

        roomRule.roomType = eRoomType.Completed;
        Complete();
    }

    private void Complete()
    {
        isAlreadyRoomClear = true;

        // Gate
        for (int i = 0; i < inRoom_AllGate.Count; i++) 
        {
            if (inRoom_AllGate[i].parterGate != null && !inRoom_AllGate[i].isOpen)
            {
                inRoom_AllGate[i].SetOpen();
            }
        }

        SetClearModeVisualSprites();

        roomRule.Complete();
    }

    #endregion

    #region Room

    // 연결된 방을 모두 가져오기
    public List<RoomController> GetConnectedRooms()
    {
        List<RoomController> connectedRooms = new List<RoomController>();
        List<GateController> existGates = GetExistGates();
        for (int i = 0; i < existGates.Count; i++)
        {
            connectedRooms.Add(existGates[i].parterGate.thisRoom);
            
        }
        return connectedRooms;
    }

    #endregion

    #region Gate 

    // 게이트와 방의 Pos 세팅하기
    public void SetCollectGatePos(int index, Vector2Int initVec)
    {
        Vector2Int targetVec = roomVec[index];
        List<GateController> posNotSettedGates = GetPosNotSettedGates(targetVec); // 아직 월드 위치값이 지정되어 있지않는 게이트
        for (int i = 0; i < posNotSettedGates.Count; i++)
        {
            posNotSettedGates[i].roomPosGate = initVec;
            posNotSettedGates[i].settedPos = true;
        }
        roomVec[index] = initVec;
    }

    // 존재하는 (상호작용이 가능한)
    private List<GateController> GetExistGates()
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
    private List<GateController> GetDirGates(Vector2Int dir)
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
    private List<GateController> GetPosGates(Vector2Int pos)
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
    private List<GateController> GetNotSettedGates()
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
    private List<GateController> GetOpenedGates()
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
        return DevTool.Get_IntersectionList(GetExistGates(), dirGates);
    }


    // 존재하고 + 방향에 맞는
    private List<GateController> GetExistDirGates(Vector2Int dir)
    {
        return DevTool.Get_IntersectionList(
            GetDirGates(dir),
            GetExistGates());
    }

    // 위치에 맞고 + 아직 위치값이 대입되지 않은
    private List<GateController> GetPosNotSettedGates(Vector2Int pos)
    {
        return DevTool.Get_IntersectionList(
            GetPosGates(pos),
            GetNotSettedGates());
    }



    // 열려있는 + 반대편도 열려있는
    private List<GateController> GetBothOpenedGates()
    {
        List<GateController> singleOpenedGates = GetOpenedGates();
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
    public GateController GetMinimapInteractShortcutGate(Vector2Int targetVec)
    {
        // 맞는 방향에 있는 모든 문
        List<GateController> gates =
            DevTool.Get_IntersectionList(
                GetExistDirGates(targetVec),
                GetBothOpenedGates());

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

    #region FieldObj

    public void Spawn_FieldObj()
    {
        List<Vector2> data = Get_FieldObjPos();

        for (int i = 0; i < data.Count; i++)
            EachSpawn_FieldObj(data[i]);
    }

    private void EachSpawn_FieldObj(Vector2 pos)
    {
        DestructibleObjectController ddoc = Instantiate(StageManager.instance.StageTheme.GetRandomFieldObjPrefab());
        if (ddoc == null)
            return;

        ddoc.gameObject.transform.SetParent(inRoom_FieldObjSpawnerParentTF);
        ddoc.gameObject.transform.position = pos;
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
