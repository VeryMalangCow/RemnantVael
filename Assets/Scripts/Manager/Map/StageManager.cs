using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    [SerializeField] private Vector2 OffsetRoomSize;

    [Space(10)]
    [Header("=== Current")]
    [SerializeField] private List<RoomController> CurrentAllRoomController = new List<RoomController>();
    [SerializeField] public RoomController CurrentRoomController;

    // 이미 차지한 Vec
    [HideInInspector] private List<Vector2Int> alreadyExistList = new List<Vector2Int>();

    // 이미 차지한 보스 Vec
    [HideInInspector] private List<Vector2Int> alreadyExistBossList = new List<Vector2Int>();

    // 배치할 주변 Vec
    [HideInInspector] private List<Vector2Int> roundList = new List<Vector2Int>();

    #endregion

    #region Framework

    private void Start()
    {
        GenStage(0);

        StartCurrentRoom(GetCollectRoomController(0));
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

        // 섞기
        reso.RoomPrefabList = GameManager.ShuffleList(reso.RoomPrefabList);

        // 방 생성
        for (int i = 0; i < reso.RoomPrefabList.Count; i++)
        {
            for (int j = 0; j < reso.RoomPrefabList[i].AmountInStage; j++)
            {
                GenRoom(reso.RoomPrefabList[i].RoomPrefab, TempID, false);
                TempID++;
            }
        }

        // 보스 방 생성
        for (int i = 0; i < reso.BossRoomPrefabList.Count; i++)
        {
            for (int j = 0; j < reso.BossRoomPrefabList[i].AmountInStage; j++)
            {
                GenFurthestRoom(reso.BossRoomPrefabList[i].RoomPrefab, TempID);
                TempID++;
            }
        }

        // 게이트 활성화
        SetParterAllGate();
        List<GateController> allGate = GetAllGate();
        for (int i = 0; i < allGate.Count; i++)
        {
            if (allGate[i].HadParter == true)
            {
                allGate[i].IsExistDoor(true);
            }
            else
            {
                allGate[i].IsExistDoor(false);
            }
        }

        alreadyExistList.Clear();
        alreadyExistBossList.Clear();
        roundList.Clear();
    }

    private void GenRoom(GameObject _Prefab, int _TempID, bool _IsStartRoom)
    {
        GameObject room = Instantiate(_Prefab, Vector2.zero, Quaternion.identity, MapParentTF);
        if (room.TryGetComponent(out RoomController rc))
        {
            rc.CurrentTempID = _TempID;
            rc.Offset();
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

    private void GenFurthestRoom(GameObject _Prefab, int _TempID)
    {
        GameObject room = Instantiate(_Prefab, Vector2.zero, Quaternion.identity, MapParentTF);
        if (room.TryGetComponent(out RoomController rc))
        {
            rc.CurrentTempID = _TempID;
            rc.Offset();
            CurrentAllRoomController.Add(rc);

            TryAddCaculateFurthestVec(rc);
        }
    }


    #endregion

    #region Room Caculate

    private void TryAddCaculateVec(RoomController _RC)
    {
        while (true)
        {
            int randomIndex = Random.Range(0, roundList.Count);

            List<Vector2Int> WorldVecList = new List<Vector2Int>();
            bool IsNeedReset = false;

            // 주변 공간에 배치할 시 배치 할 수 있는지에 대한
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

            // 된다면 벡터값을 넣어주고 (실제 좌표 값에 비례되는 값을 넣어줌 + Gate도)
            for (int i = 0; i < _RC.RoomVec.Count; i++)
            {
                _RC.SetCollectGateVec(i, WorldVecList[i]);
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
        _RC.gameObject.transform.position = new Vector2(_RC.RoomVec[0].x * OffsetRoomSize.x, _RC.RoomVec[0].y * OffsetRoomSize.y);
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

    #region Boss Room Caculate

    private void TryAddCaculateFurthestVec(RoomController _RC)
    {
        List<Vector2Int> tempRoundList = roundList.OrderByDescending(obj => OriginalToTarget(obj)).ToList();
        for (int i = 0; i < tempRoundList.Count; i++)
        {
            List<Vector2Int> WorldVecList = new List<Vector2Int>();
            bool IsNeedReset = false;

            for (int j = 0; j < _RC.RoomVec.Count; j++)
            {
                WorldVecList.Add(tempRoundList[i] + _RC.RoomVec[j]);
                if (alreadyExistList.Contains(WorldVecList[j]) ||
                    alreadyExistBossList.Contains(WorldVecList[j]))
                {
                    IsNeedReset = true;
                    break;
                }
            }

            // 안된다면 다시 시작
            if (IsNeedReset)
            { continue; }

            // 된다면 벡터값을 넣어주고 (실제 좌표 값에 비례되는 값을 넣어줌 + Gate도)
            for (int j = 0; j < _RC.RoomVec.Count; j++)
            {
                _RC.SetCollectGateVec(j, WorldVecList[j]);
            }

            // 위치를 지정해주며
            SetRCPos(_RC);

            // 결과값을 넣어줌.
            AddCaculateBossVec(_RC.RoomVec);

            return;
        }

    }

    private void AddCaculateBossVec(List<Vector2Int> _AddVecList)
    {
        List<Vector2Int> bossRoundAllList = new List<Vector2Int>();
        for (int i = 0;  i < _AddVecList.Count;  i++)
        {
            List<Vector2Int> round = GetRoundVec(_AddVecList[i]);
            round.Add(_AddVecList[i]);

            for (int j = 0; j < round.Count; j++)
            {
                if (!bossRoundAllList.Contains(round[j]))
                {
                    bossRoundAllList.Add(round[j]);
                }
            }
        }

        alreadyExistBossList.AddRange(bossRoundAllList);

        AddCaculateVec(_AddVecList);
    }

    #endregion

    #region Gate Caculate

    private void SetParterAllGate()
    {
        List<GateController> allGate = GetAllGate();

        for (int i = 0; i < allGate.Count - 1; i++)
        {
            // 이미 파트너 게이트가 있다면
            if (allGate[i].HadParter)
            {
                continue;
            }

            for (int j = i + 1; j < allGate.Count; j++)
            {
                if (((allGate[i].RoomPosGate + allGate[i].GateDir) == allGate[j].RoomPosGate) &&
                    (allGate[i].GateDir * -1) == allGate[j].GateDir)
                {
                    allGate[i].ParterGate = allGate[j];
                    allGate[i].HadParter = true;

                    allGate[j].ParterGate = allGate[i];
                    allGate[j].HadParter = true;
                }
            }
        }
    }

    #endregion

    #region Set State

    public void StartCurrentRoom(RoomController _TargetRC)
    {
        StartCoroutine(StartCurrentRoom_Cor(_TargetRC));
    }

    private IEnumerator StartCurrentRoom_Cor(RoomController _TargetRC)
    {
        if (_TargetRC == null)
        { yield return null; }

        // 현재 방 선택
        CurrentRoomController = _TargetRC; 
        
        for (int i = 0; i < CurrentAllRoomController.Count; i++)
        {
            CurrentAllRoomController[i].gameObject.SetActive(false);
        }
        CurrentRoomController.gameObject.SetActive(true);

        // 현재 맵만 Sorting Layer 사용
        for (int i = 0; i < CurrentAllRoomController.Count; i++)
        {
            CurrentAllRoomController[i].SetCorrectWallSortOrder(CurrentRoomController);
        }

        yield return new WaitForSeconds(0.5f);

        _TargetRC.PlayRoomState();

        // Layer 초기화
        LayerOrderManager.Instance.NeedLayerObjects = new List<HaveShadowThing>()
        { PlayerManager.Instance.PlayerController }; // 전 방 리셋

        // Layer 추가
        LayerOrderManager.Instance.NeedLayerObjects.AddRange(CurrentRoomController.InRoom_AllBuilding);
        LayerOrderManager.Instance.NeedLayerObjects.AddRange(CurrentRoomController.GetNeedAllLayer());
        LayerOrderManager.Instance.NeedLayerObjects.AddRange(EnemyManager.Instance.CurrentEnemyList);

    }

    public void Complete_KillAll()
    {
        StartCoroutine(Complete_KillAll_Cor());
    }

    public IEnumerator Complete_KillAll_Cor()
    {
        if (CurrentRoomController == null)
        { yield return null; }

        yield return new WaitForSeconds(0.5f);

        if (EnemyManager.Instance.CurrentEnemyList.Count <= 0)
        {
            CurrentRoomController.RoomType = eRoomType.Completed;
            CurrentRoomController.PlayRoomState();
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

    private List<GateController> GetAllGate()
    {
        List<GateController> allGate = new List<GateController>();
        for (int i = 0; i < CurrentAllRoomController.Count; i++)
        {
            allGate.AddRange(CurrentAllRoomController[i].InRoom_AllGate);
        }
        return allGate;
    }

    private float OriginalToTarget(Vector2Int _TargetVec)
    {
        return Vector2Int.Distance(Vector2Int.zero, _TargetVec);
    }

    #endregion

    #region Detail Class

    [System.Serializable]
    public class StageData
    {
        public int StageID;
        public GameObject StartRoomPrefab;
        public List<RoomData> RoomPrefabList;
        public List<RoomData> BossRoomPrefabList;

        [System.Serializable]
        public class RoomData
        {
            public int AmountInStage;
            public GameObject RoomPrefab;
        }

    }

    #endregion
}






