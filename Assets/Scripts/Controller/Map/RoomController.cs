using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomController : IDController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Room")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] public RoomRuleController RoomRuleController;
    [SerializeField] public List<Vector2Int> RoomVec;

    [HideInInspector] public bool SettedPos = false;

    [Space(10)]
    [Header("=== In Room _ Wall")]
    [SerializeField] private Transform InRoom_UpperWallParentTF;
    [HideInInspector] private List<StaticDepthController> InRoom_UpperWalls;
    [SerializeField] private Transform InRoom_LowerWallParentTF;
    [HideInInspector] private List<StaticDepthController> InRoom_LowerWalls;

    [Space(10)]
    [Header("=== In Room _ Building")]
    [SerializeField] public List<GateController> InRoom_AllUpperGate;
    [SerializeField] public List<GateController> InRoom_AllLowerGate;
    [HideInInspector] public List<GateController> InRoom_AllGate;

    [Space(10)]
    [Header("=== Camera")]
    [SerializeField] public Transform RoomCameraCenter;

    [Space(10)]
    [Header("=== UI")]
    [SerializeField] public Sprite ThisSpriteMM;
    [SerializeField] public Sprite ThisSpriteMMO;
    [SerializeField] public Sprite ThisSpriteMMI;
    [SerializeField] public Sprite ThisSpriteMMIO;
    [SerializeField] public Vector2 SpritePivot;
    [HideInInspector] public MinimapCellEUIController ThisMME;
    [HideInInspector] public MinimapCellEUIController ThisIMME;

    #endregion

    #region Offset

    public override void Offset(int _ID)
    {
        base.Offset(_ID);

        // Wall
        InRoom_UpperWalls = new List<StaticDepthController>();
        if (InRoom_UpperWallParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_UpperWallParentTF)
            {
                if (chile.gameObject.TryGetComponent(out StaticDepthController HSTS))
                {
                    InRoom_UpperWalls.Add(HSTS);
                }
            }
        }
        InRoom_LowerWalls = new List<StaticDepthController>();
        if (InRoom_LowerWallParentTF.childCount > 0)
        {
            foreach (Transform chile in InRoom_LowerWallParentTF)
            {
                if (chile.gameObject.TryGetComponent(out StaticDepthController HSTS))
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

        RoomRuleController.Offset();
    }

    #endregion

    #region Layer

    public void Set_CorrectWallSortOrder(RoomController _RC)
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
            List<StaticDepthController> HSTSs = new List<StaticDepthController>();
            HSTSs.AddRange(InRoom_UpperWalls);
            HSTSs.AddRange(InRoom_LowerWalls);
            for (int i = 0; i < HSTSs.Count; i++)
            {
                if (HSTSs[i].TargetObject.TryGetComponent(out SpriteRenderer sr))
                { sr.sortingOrder = 1; }
            }
        }
        
    }

    public List<DepthController> Get_NeedAllLayer()
    {
        List<DepthController> HST = new List<DepthController>();

        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (InRoom_AllGate[i].NeedSetAllLayer != null && InRoom_AllGate[i].NeedSetAllLayer.Count > 0)
            { HST.AddRange(InRoom_AllGate[i].NeedSetAllLayer); }
        }

        return HST;
    }

    #endregion

    #region Gate

    public void Set_CollectGateVec(int _Index, Vector2Int _InitVec)
    {
        Vector2Int targetVec = RoomVec[_Index];
        List<GateController> gates = Get_CollectGateList(targetVec);
        for (int i = 0; i < gates.Count; i++)
        {
            gates[i].RoomPosGate = _InitVec;
            gates[i].SettedPos = true;
        }
        RoomVec[_Index] = _InitVec;
        SettedPos = true;
    }

    private List<GateController> Get_CollectGateList(Vector2Int _TargetVec)
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

    public GateController Get_CollectGate(Vector2Int _TargetVec)
    {
        // 맞는 방향에 있는 모든 문
        List<GateController> collectDirGateList = new List<GateController>();
        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (InRoom_AllGate[i].GateDir == _TargetVec &&
                InRoom_AllGate[i].ParterGate != null)
            {
                collectDirGateList.Add(InRoom_AllGate[i]);
            }
        }
        List <GateController> gateList = new List<GateController>();
        if (collectDirGateList.Count > 0)
        {
            // 반대편이 갈 수 있는 (이미 성공한 방) 상태인지 판별한 방 리스트
            for (int i = 0; i < collectDirGateList.Count; i++)
            {
                if (collectDirGateList[i].ParterGate.ThisRoom.RoomRuleController.RoomType
                    == eRoomType.Completed)
                {
                    gateList.Add(collectDirGateList[i].ParterGate);
                }
            }

            // XY 값을 기준으로 적은 값을 우선적으로 리턴.
            if (gateList.Count > 0)
            {
                gateList = gateList.Distinct().ToList();
                if (_TargetVec.x != 0)
                {
                    return gateList.OrderBy(obj => obj.transform.position.x).ToList()[0];
                }
                else
                {
                    return gateList.OrderBy(obj => obj.transform.position.y).ToList()[0];
                }
            }
            else
            {
                return null;
            }
        }

        return null; 
    }

    #endregion

    #region Condition

    public void Play_RoomState()
    {
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
        // Gate
        for (int i = 0; i < InRoom_AllGate.Count; i++) 
        {
            if (InRoom_AllGate[i].HadParter)
            {
                if (!InRoom_AllGate[i].IsOpen)
                {
                    InRoom_AllGate[i].Set_OpenClose(true);
                }
            }
        }
    }


    #endregion

    #region Get

    public List<RoomController> Get_ConnectedRoomList()
    {
        List<RoomController> result = new List<RoomController>();
        for (int i = 0; i < InRoom_AllGate.Count; i++)
        {
            if (InRoom_AllGate[i].ParterGate != null && InRoom_AllGate[i].ParterGate.ThisRoom != null)
            {
                result.Add(InRoom_AllGate[i].ParterGate.ThisRoom);
            }
        }
        return result;
    }

    #endregion
}
