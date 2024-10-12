using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Stage Manager")]

    [Space(10)]
    [Header("=== Generate")]
    [SerializeField] private Transform MapParentTF;
    [SerializeField] private List<StageData> AllReso;

    [Space(10)]
    [Header("=== Current")]
    [SerializeField] private List<RoomController> CurrentAllRoomController = new List<RoomController>();
    [SerializeField] private RoomController CurrentRoomController;

    // 이미 차지한 Vec
    [HideInInspector] private List<Vector2Int> alreadyExistList = new List<Vector2Int>();
    // 배치할 주변 Vec
    [HideInInspector] private List<Vector2Int> roundList = new List<Vector2Int>();

    #endregion

    #region Framework

    private void Start()
    {
        GenStage(0);
    }

    #endregion

    #region Generate

    public void GenStage(int _StageID)
    {
        StageData reso = GetCollectStageData(_StageID);
        if (reso == null) 
        { return; }


        // 처음 방
        int TempID = 0;
        GenRoom(reso.StartRoomPrefab, TempID, true);
        TempID++;


        // 방 생성
        for (int i = 0; i < reso.RoomPrefabList.Count; i++)
        {
            for (int j = 0; j < reso.RoomPrefabList[i].AmountInStage; j++)
            {
                GenRoom(reso.RoomPrefabList[i].RoomPrefab, TempID, false);
                TempID++;
            }
        }

        alreadyExistList.Clear();
        roundList.Clear();

        SetCurrentRoom(GetCollectRoomController(0));
    }

    private void GenRoom(GameObject _Prefab, int _TempID, bool _IsStartRoom)
    {
        GameObject room = Instantiate(_Prefab, Vector2.zero, Quaternion.identity, MapParentTF);
        if (room.TryGetComponent(out RoomController rc))
        {
            rc.CurrentTempID = _TempID;
            CurrentAllRoomController.Add(rc);

            if (!_IsStartRoom)
            {
                TryAddCaculateVec(rc);
            }
            else
            {
                AddCaculateVec(new List<Vector2Int>() { Vector2Int.zero });
            }
        }
    }

    private void SetCurrentRoom(RoomController _TargetRC)
    {
        if (_TargetRC == null)
        { return; }

        // 전 방 리셋
        LayerOrderManager.Instance.NeedLayerObjects = new List<HaveShadowThing>()
        { PlayerManager.Instance.PlayerController };

        // 현재 방 선택
        CurrentRoomController = _TargetRC;
        LayerOrderManager.Instance.NeedLayerObjects.AddRange(CurrentRoomController.InRoom_AllBuilding);
    }

    #endregion

    #region Vector Caculate

    private void TryAddCaculateVec(RoomController _RC)
    {
        while (true)
        {
            int randomIndex = Random.Range(0, roundList.Count);

            List<Vector2Int> WorldVecList = new List<Vector2Int>();

            // 주변 공간에 배치할 시 배치 할 수 있는지에 대한
            bool IsNeedReset = false;
            for (int i = 0; i < _RC.RoomVec.Count; i++)
            {
                WorldVecList.Add(roundList[randomIndex] + _RC.RoomVec[i]);
                if (alreadyExistList.Contains(WorldVecList[i]))
                {
                    IsNeedReset = true;
                    break;
                }
            }

            // 안된다면 다시 시작
            if (IsNeedReset)
            { continue; }

            // 된다면 벡터값을 넣어주고           
            for (int i = 0; i < _RC.RoomVec.Count; i++)
            {
                _RC.RoomVec[i] = WorldVecList[i];
            }

            // 위치를 지정해주며
            SetRCPos(_RC);

            // 결과값을 넣어줌.
            AddCaculateVec(_RC.RoomVec);

            break;
        }
    }

    private void SetRCPos(RoomController _RC)
    {
        _RC.gameObject.transform.position = new Vector2(_RC.RoomVec[0].x * 21f, _RC.RoomVec[0].y * 13.5f);
    }

    private void AddCaculateVec(List<Vector2Int> _AddVecList)
    {
        alreadyExistList.AddRange(_AddVecList);
        roundList = new List<Vector2Int>();

        for (int i = 0; i < alreadyExistList.Count; i++)
        {
            foreach(Vector2Int vec in GetRoundVec(alreadyExistList[i]))
            {
                if (!roundList.Contains(vec) && !alreadyExistList.Contains(vec))
                {
                    roundList.Add(vec);
                }
            }
        }
    }


    #endregion

    #region Get

    private StageData GetCollectStageData(int _StageID)
    {
        for (int i = 0; i < AllReso.Count; i++)
        {
            if (AllReso[i].StageID == _StageID)
            {
                return AllReso[i];
            }
        }

        return null;
    }
    
    private RoomController GetCollectRoomController(int _TempID)
    {
        for (int i = 0; i < CurrentAllRoomController.Count; i++)
        {
            if (CurrentAllRoomController[i].CurrentTempID == _TempID)
            {
                return CurrentAllRoomController[i];
            }
        }
        
        return null;
    }

    private List<Vector2Int> GetRoundVec(Vector2Int _CenterVec)
    {
        return new List<Vector2Int>()
        {
            (_CenterVec + Vector2Int.up),
            (_CenterVec + Vector2Int.down),
            (_CenterVec + Vector2Int.left),
            (_CenterVec + Vector2Int.right)
        };
    }

    #endregion

    #region Detail Class

    [System.Serializable]
    public class StageData
    {
        public int StageID;
        public GameObject StartRoomPrefab;
        public List<RoomData> RoomPrefabList;

        [System.Serializable]
        public class RoomData
        {
            public int AmountInStage;
            public GameObject RoomPrefab;
        }
    }

    #endregion
}






