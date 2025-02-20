using System.Collections;
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
    [SerializeField] public int TargetStageID;
    [SerializeField] private List<StageData> AllReso;
    [SerializeField] private Vector2 OffsetRoomSize;
    [SerializeField] public bool IsStartStage = true;
    [SerializeField] private GameObject BUShopPrefab;
    [SerializeField] private GameObject MUShopPrefab;

    [Space(10)]
    [Header("=== Current")]
    [SerializeField] private List<RoomController> CurrentAllRoomController = new List<RoomController>();
    public List<RoomController> GetAllRC() { return CurrentAllRoomController; }
    [SerializeField] public RoomController CurrentRoomController;

    // 이미 차지한 Vec
    [HideInInspector] private List<Vector2Int> alreadyExistList = new List<Vector2Int>();

    // 이미 차지한 보스 Vec
    [HideInInspector] private List<Vector2Int> alreadyExistBossList = new List<Vector2Int>();

    // 배치할 주변 Vec
    [HideInInspector] private List<Vector2Int> roundList = new List<Vector2Int>();

    [HideInInspector] List<int> BUShopIndexs;
    [HideInInspector] List<int> MUShopIndexs;

    #endregion

    #region Framework

    private void Start()
    {
        // 스테이지 소환
        GenStage(TargetStageID);

        // 처음 스타트맵
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

        // Shop 지정
        int normalRoomAmount = GetRoomAmount(_StageID);

#if UNITY_EDITOR
        if (normalRoomAmount <= (reso.BUShopAmount + reso.MUShopAmount) || 
            reso.BUShopAmount < 0 || reso.MUShopAmount < 0)
        { Debug.Log("상점 지정 수가 너무 많거나 음수 입니다."); }
#endif

        else
        {
            BUShopIndexs = new List<int>();
            MUShopIndexs = new List<int>();
            while (true)
            {
                if (BUShopIndexs.Count < reso.BUShopAmount)
                {
                    int BUIndex = Random.Range(1, normalRoomAmount + 1);

                    if (!BUShopIndexs.Contains(BUIndex) &&
                        !MUShopIndexs.Contains(BUIndex))
                    {
                        BUShopIndexs.Add(BUIndex);
                    }
                }
                if (MUShopIndexs.Count < reso.MUShopAmount)
                {
                    int MUIndex = Random.Range(1, normalRoomAmount + 1);

                    if (!BUShopIndexs.Contains(MUIndex) &&
                        !MUShopIndexs.Contains(MUIndex))
                    {
                        MUShopIndexs.Add(MUIndex);
                    }
                }

                // 조건 충족 시 BREAK
                if ((BUShopIndexs.Count + MUShopIndexs.Count) >= (reso.BUShopAmount + reso.MUShopAmount))
                {
                    break; 
                }
            }
        }

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
        for (int i = 0; i < reso.BossRoomList.Count; i++)
        {
            GenBossRoom(reso.BossRoomList[i], TempID);
            TempID++;
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
        BUShopIndexs.Clear();
        MUShopIndexs.Clear();

        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.GenMinimap();
        MainGameUIManager.Instance.PlayerHUD_UIController.SetStageDescription(reso.StageName, reso.StageDescription);

        MainGameUIManager.Instance.MapIntro_UIController.OnIntroLabel();

        // 적 객체 오브젝트 풀링 시스템 세팅하기
        PoolingManager.Instance.EnemiesPoolingSet(reso.StageEnemyList);
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
                GameObject rrcGO = Instantiate(AllReso[TargetStageID].GetCorrectRandomRoomRule(rc.RoomVec), rc.gameObject.transform);
                if (rrcGO.TryGetComponent(out RoomRuleController rrc))
                { rc.RoomRuleController = rrc; }

                rc.Offset();
                TryAddCaculateVec(rc);

                // 상점 소환
                if (BUShopIndexs.Contains(rc.CurrentTempID))
                {
                    rrc.SetShop(BUShopPrefab);
                    //Debug.Log("BU : " + rc.CurrentTempID);
                }
                else if (MUShopIndexs.Contains(rc.CurrentTempID))
                {
                    rrc.SetShop(MUShopPrefab);
                    //Debug.Log("MU : " + rc.CurrentTempID);
                }
            }
            else
            {
                GameObject rrcGO = Instantiate(AllReso[TargetStageID].StartRoomRulePrefab, rc.gameObject.transform);
                if (rrcGO.TryGetComponent(out RoomRuleController rrc))
                { rc.RoomRuleController = rrc; }

                rc.Offset();
                AddCaculateVec(new List<Vector2Int>() { Vector2Int.zero });
            }
        }
    }

    private void GenBossRoom(StageData.BossRoomData _BossRoomData, int _TempID)
    {
        GameObject room = Instantiate(_BossRoomData.RoomPrefab, Vector2.zero, Quaternion.identity, MapParentTF);
        if (room.TryGetComponent(out RoomController rc))
        {
            rc.CurrentTempID = _TempID;

            CurrentAllRoomController.Add(rc);

            GameObject rrcGO = Instantiate(_BossRoomData.RoomRulePrefabList[Random.Range(0, _BossRoomData.RoomRulePrefabList.Count)], rc.gameObject.transform);
            if (rrcGO.TryGetComponent(out RoomRuleController rrc))
            { rc.RoomRuleController = rrc; }

            rc.Offset();
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

        // Layer 초기화
        LayerOrderManager.Instance.NeedLayerObjects = new List<HaveShadowThing>();

        // 처음 엘베 레이어때문에 추가 하지않음
        if (!IsStartStage)
        { LayerOrderManager.Instance.NeedLayerObjects.Add(PlayerManager.Instance.PlayerController); }


        CurrentRoomController.gameObject.SetActive(true);

        // Layer 추가
        LayerOrderManager.Instance.NeedLayerObjects.AddRange(CurrentRoomController.RoomRuleController.InRoom_AllBuilding);
        LayerOrderManager.Instance.NeedLayerObjects.AddRange(CurrentRoomController.GetNeedAllLayer());

        // 현재 맵만 Sorting Layer 사용
        for (int i = 0; i < CurrentAllRoomController.Count; i++)
        {
            CurrentAllRoomController[i].SetCorrectWallSortOrder(CurrentRoomController);
        }

        // Minimap
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.SetState();

        yield return new WaitForSeconds(0.5f);

        _TargetRC.PlayRoomState();
        LayerOrderManager.Instance.NeedLayerObjects.AddRange(EnemyManager.Instance.CurrentEnemyList);

        // Minimap
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.SetState();
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.PlayEffect();
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
            CurrentRoomController.RoomRuleController.RoomType = eRoomType.Completed;
            CurrentRoomController.PlayRoomState();

            // Minimap
            MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.SetState();
        }
    }

    #endregion

    #region Get

    public StageData GetCollectStageData(int _StageID)
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
    
    private int GetRoomAmount(int _StageID)
    {
        int result = 0;

        StageData reso = GetCollectStageData(_StageID);
        if (reso != null)
        {
            for (int i = 0; i < reso.RoomPrefabList.Count; i++)
            {
                result += reso.RoomPrefabList[i].AmountInStage;
            }
        }
        return result;
    }

    private int GetBossRoomAmount(int _StageID)
    {
        return GetCollectStageData(_StageID).BossRoomList.Count;
    }

    private int GetAllRoomAmount(int _StageID)
    {
        return GetRoomAmount(_StageID) + GetBossRoomAmount(_StageID);
    }

    #endregion

    #region Detail Class

    [System.Serializable]
    public class StageData
    {
        [Space(20)]
        public int StageID;
        public string StageName;
        public string StageDescription;

        [Space(20)]
        public GameObject StartRoomPrefab;
        public GameObject StartRoomRulePrefab;

        [Space(20)]
        public List<RoomData> RoomPrefabList;
        public List<GameObject> RoomRulePrefabList;

        [Space(20)]
        public List<BossRoomData> BossRoomList;

        [Space(20)]
        public int BUShopAmount = 1;
        public int MUShopAmount = 1;

        [Space(20)]
        public List<GameObject> StageEnemyList;


        [System.Serializable]
        public class RoomData
        {
            public int AmountInStage;
            public GameObject RoomPrefab;
        }

        [System.Serializable]
        public class BossRoomData
        {
            public GameObject RoomPrefab;
            public List<GameObject> RoomRulePrefabList;
        }


        public GameObject GetCorrectRandomRoomRule(List<Vector2Int> _RoomVec)
        {
            List<GameObject> roomRulePrefabList = new List<GameObject>();
            for (int i = 0; i < RoomRulePrefabList.Count; i++)
            {
                if (RoomRulePrefabList[i].TryGetComponent(out RoomRuleController rrc) && rrc.RoomVec.SequenceEqual(_RoomVec))
                {
                    roomRulePrefabList.Add(RoomRulePrefabList[i]);
                }
            }

            return roomRulePrefabList[Random.Range(0, roomRulePrefabList.Count)];
        }
    }

    #endregion
}






