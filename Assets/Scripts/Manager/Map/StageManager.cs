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
    [SerializeField] private List<StageData> AllStageData;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] private GameObject StartRoomRulePrefab;
    [SerializeField] private GameObject BUShopPrefab;
    [SerializeField] private GameObject MUShopPrefab;
    [SerializeField] private List<GameObject> RoomPrefabList;
    [SerializeField] private List<GameObject> RoomRulePrefabList;
    [SerializeField] private List<GameObject> RoomRuleEntrancePrefabList;

    [Space(10)]
    [Header("=== Value0")]
    [SerializeField] private Vector2 OffsetRoomSize;

    [Space(10)]
    [Header("=== Current")]
    [SerializeField] private List<RoomController> CurrentAllRoomController = new List<RoomController>();
    [SerializeField] public RoomController CurrentRoomController;

    #region - Hide

    // 처음 시작하는 Room인가?
    [HideInInspector] public bool IsStartStage = true;

    // 스폰을 위한 리스트
    [HideInInspector] private List<int> ShuffledRoomIndexList = new List<int>();

    // 이미 차지한 Vec
    [HideInInspector] private List<Vector2Int> alreadyExistList = new List<Vector2Int>();
    [HideInInspector] private List<Vector2Int> alreadyExistSpeicalList = new List<Vector2Int>();

    // 배치할 주변 Vec
    [HideInInspector] private List<Vector2Int> roundList = new List<Vector2Int>();

    // 상점 스폰 ID를 저장힉 위함
    [HideInInspector] List<int> BUShopIndexs;
    [HideInInspector] List<int> MUShopIndexs;

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        for (int i = 0; i < AllStageData.Count; i++)
        {
            AllStageData[i].Offset(CSVManager.Instance.MapImgList_Data[i], CSVManager.Instance.MapMaterialIndexList_Data[i]);
        }

        CSVManager.Instance.MapImgList_Data.Clear();
        CSVManager.Instance.MapMaterialIndexList_Data.Clear();
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();

        // 스테이지 소환
        Gen_Stage(TargetStageID);

        // 처음 스타트맵
        Play_CurrentRoom(Get_CorrectRoom(0));
    }

    #endregion

    #region Generate

    #region Stage

    // 스테이지 생성
    public void Gen_Stage(int _StageID)
    {
        StageData stageData = Get_CollectStageData(_StageID);
        if (stageData == null) return; 

        // 처음 방
        int TempID = 0;
        Gen_StartRoom(RoomPrefabList[0], TempID);
        TempID++;

        // Shop이 스폰될 ID 지정
        Set_ShopData(_StageID, stageData.ShopData.BUShopAmount, stageData.ShopData.MUShopAmount);

        // 생성할 Room의 양을 계산에 1중 리스트로 변경 => 이들을 섞음
        ShuffledRoomIndexList = DevTool.Get_ShuffledList(
            Get_ListInt_FromGenRoomAmount(stageData.RoomData.RoomAmount));

        // 기본 방 생성
        for (int i = 0; i < ShuffledRoomIndexList.Count; i++)
        {
            Gen_NormalRoom(RoomPrefabList[ShuffledRoomIndexList[i]], TempID);
            TempID++;
        }

        // 통과 방 생성
        for (int i = 0; i < stageData.RoomData.EntranceRoom.Count; i++)
        {
            Gen_EntranceRoom(stageData.RoomData.EntranceRoom[i], TempID);
            TempID++;
        }

        // 게이트 활성화
        Set_ParterAllGate();
        List<GateController> allGate = Get_AllGate(CurrentAllRoomController);
        for (int i = 0; i < allGate.Count; i++)
            if (allGate[i].ParterGate != null) 
                allGate[i].Set_ExistDoorState(true);
            else 
                allGate[i].Set_ExistDoorState(false);
        

        Set_StartUI(stageData);

        // 적 객체 오브젝트 풀링 시스템 세팅하기
        PoolingManager.Instance.Offset_EnemiesPooling(stageData.EnemyData.StageEnemyList);

        Reset_GenStageData();
    }

    #endregion

    #region Room

    // 시작 방 생성
    private void Gen_StartRoom(GameObject _Prefab, int _TempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(_Prefab, MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(StartRoomRulePrefab, room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule;

            room.Offset(_TempID);
            Add_RoundVec(new List<Vector2Int>() { Vector2Int.zero });
        }
    }

    // 기본 방 생성
    private void Gen_NormalRoom(GameObject _Prefab, int _TempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(_Prefab, MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(Get_CorrectRandomRoomRule(room).gameObject, room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule;

            room.Offset(_TempID);
            Set_NormalRelativeVec(room);

            // 상점 소환
            if (BUShopIndexs.Contains(_TempID)) roomRule.Spawn_CorretShop(BUShopPrefab);
            else if (MUShopIndexs.Contains(_TempID)) roomRule.Spawn_CorretShop(MUShopPrefab);
        }
            
    }

    // 통과 방 하나 생성
    private void Gen_EntranceRoom(GenEntranceRoomData _EntranceRoomData, int _TempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(RoomPrefabList[_EntranceRoomData.ID], MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(RoomRuleEntrancePrefabList[_EntranceRoomData.RuleID], room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule; 

            room.Offset(_TempID);
            Set_FurthestRelativeVec(room);
        }
    }

    #endregion

    #endregion

    #region Set State

    public void Play_CurrentRoom(RoomController _TargetRC)
    {
        StartCoroutine(Play_CurrentRoom_Cor(_TargetRC));
    }

    private IEnumerator Play_CurrentRoom_Cor(RoomController _TargetRC)
    {
        if (_TargetRC == null) yield break;

        // 현재 방 선택
        CurrentRoomController = _TargetRC;

        DevTool.Set_Active(CurrentAllRoomController, false);

        // Layer 초기화
        LayerOrderManager.Instance.NeedSortingObjects = new List<DepthController>();

        // 처음 엘베 레이어때문에 추가 하지않음
        if (!IsStartStage)
        { LayerOrderManager.Instance.NeedSortingObjects.Add(PlayerManager.Instance.PlayerController); }


        CurrentRoomController.gameObject.SetActive(true);
        CurrentRoomController.Set_SortingStaticObjects();

        // Minimap
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Set_State();

        yield return new WaitForSeconds(0.5f);

        _TargetRC.Play_RoomState();
        LayerOrderManager.Instance.NeedSortingObjects.AddRange(EnemyManager.Instance.CurrentEnemyList);

        // Minimap
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Set_State();
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Play_Effect();
    }


    public void Play_CompleteKillAll()
    {
        StartCoroutine(Play_CompleteKillAll_Cor());
    }

    public IEnumerator Play_CompleteKillAll_Cor()
    {
        if (CurrentRoomController == null) yield break;

        yield return new WaitForSeconds(0.5f);

        if (EnemyManager.Instance.CurrentEnemyList.Count <= 0)
        {
            CurrentRoomController.RoomRuleController.RoomType = eRoomType.Completed;
            CurrentRoomController.Play_RoomState();

            // Minimap
            MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Set_State();
        }
    }

    #endregion

    #region Reset

    private void Reset_GenStageData()
    {
        ShuffledRoomIndexList.Clear();

        alreadyExistList.Clear();
        alreadyExistSpeicalList.Clear();

        roundList.Clear();

        BUShopIndexs.Clear();
        MUShopIndexs.Clear();
    }

    #endregion

    #region Set

    #region Stage

    private void Set_StartUI(StageData _StageData)
    {
        MainGameUIManager.Instance.MapIntro_UIController.Play_IntroLabel();
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Gen_Minimap();
        MainGameUIManager.Instance.PlayerHUD_UIController.Set_StageDescription(_StageData.InfoData.StageName, _StageData.InfoData.StageDescription);
    }

    #endregion

    #region Room

    // 방 위치 세팅
    private void Set_RoomPos(RoomController _RC)
    {
        _RC.gameObject.transform.position = new Vector2(_RC.RoomVec[0].x * OffsetRoomSize.x, _RC.RoomVec[0].y * OffsetRoomSize.y);
    }

    #endregion

    #region Gate

    // 게이트에 모든 짝꿍 게이트 지정과 세팅
    private void Set_ParterAllGate()
    {
        List<GateController> allGate = Get_AllGate(CurrentAllRoomController);
         
        for (int i = 0; i < allGate.Count - 1; i++)
        {
            // 이미 파트너 게이트가 있다면
            if (allGate[i].ParterGate != null) continue;

            for (int j = i + 1; j < allGate.Count; j++)
            {
                if (Is_PartnerGate(allGate[i], allGate[j]))
                {
                    allGate[i].ParterGate = allGate[j];
                    allGate[j].ParterGate = allGate[i];
                }
            }
        }
    }

    #endregion

    #region Shop

    // 상점 스폰할 Room ID 지정하기
    private void Set_ShopData(int _StageID, int _BUShopAmount, int _MUShopAmount)
    {
        // Shop 지정
        int normalRoomAmount = Get_RoomAmount(_StageID);

        BUShopIndexs = Get_RandomIndexList(normalRoomAmount, _BUShopAmount, new List<int>());
        MUShopIndexs = Get_RandomIndexList(normalRoomAmount, _MUShopAmount, BUShopIndexs);
    }


    #endregion

    #region Round

    // 해당 월드 좌표값을 Room에 적용
    private void Set_RelativeVec(RoomController _Room, List<Vector2Int> _WorldVecList)
    {
        // 벡터값을 넣어주고 (실제 좌표 값에 비례되는 값을 넣어줌 + Gate도)
        for (int i = 0; i < _Room.RoomVec.Count; i++)
            _Room.Set_CollectGatePos(i, _WorldVecList[i]);
    }

    #endregion

    #region Relative

    // 월드 기준: 상대적인 좌표 삽입
    private void Set_NormalRelativeVec(RoomController _Room)
    {
        Set_RelativeVec(_Room, Get_FindCorrectWorldVec_Normal(_Room));

        Set_RoomPos(_Room);
        Add_RoundVec(_Room.RoomVec);
    }

    // 월드 기준: 상대적인 좌표 삽입: 가장 멀고, 특별 Round 포함
    private void Set_FurthestRelativeVec(RoomController _Room)
    {
        Set_RelativeVec(_Room, Get_FindCorrectWorldVec_Furthest(_Room, _ApplySpecialExist: true));

        Set_RoomPos(_Room);
        Add_RoundVec(_Room.RoomVec);
        Add_RoundSpecialVec(_Room.RoomVec);
    }

    #endregion

    #region SR
    public void Set_MapSprite(SpriteRenderer _SR, string _SpriteKey)
    {
        if (!AllStageData[TargetStageID].MapSpriteReso.MapSprite.ContainsKey(_SpriteKey)) { Debug.Log(_SpriteKey);  return; }

        SpriteMaterial spriteMatrial = AllStageData[TargetStageID].MapSpriteReso.MapSprite[_SpriteKey];
        _SR.sprite = spriteMatrial.Sprite;
        _SR.material = AllStageData[TargetStageID].MapMaterial[spriteMatrial.MaterialIndex];
    }

    #endregion

    #endregion

    #region Get

    #region Stage

    // 올바른 Stage 데이터 구하기
    public StageData Get_CollectStageData(int _StageID)
    {
        for (int i = 0; i < AllStageData.Count; i++)
            if (AllStageData[i].InfoData.StageID == _StageID)
                return AllStageData[i];

        return null;
    }

    #endregion

    #region Room

    // 지정 스테이지 모든 방 숫자 구하기
    private int Get_RoomAmount(int _StageID)
    {
        int result = 0;

        StageData reso = Get_CollectStageData(_StageID);

        if (reso != null)
            for (int i = 0; i < reso.RoomData.RoomAmount.Count; i++)
                result += reso.RoomData.RoomAmount[i].Amount;

        return result;
    }


    // Room Amount List를 List<int>형인 기본 리스트로 변경
    private List<int> Get_ListInt_FromGenRoomAmount(List<GenRoomData> _GenRoomAmountList)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < _GenRoomAmountList.Count; i++)
            for (int j = 0; j < _GenRoomAmountList[i].Amount; j++)
                result.Add(_GenRoomAmountList[i].ID);
            
        return result;
    }

    // 현재 모든 방 구하기
    public List<RoomController> Get_AllRoom()
    {
        return CurrentAllRoomController;
    }

    // (ID가 지정된 후) 알맞는 Room 찾기
    public RoomController Get_CorrectRoom(int _ID)
    {
        return IDController.Get_CorrectIDObject<RoomController>(_ID, new List<IDController>(CurrentAllRoomController));
    }

    #endregion

    #region RoomRule

    // Room 인덱스에 맞는 모든 RoomRule 가져오기
    private List<RoomRuleController> Get_CorrectRoomRuleList(RoomController _Room)
    {
        List<Vector2Int> roomIndex = _Room.RoomVec;
        List<RoomRuleController> result = new List<RoomRuleController>();
        for (int i = 0; i < RoomRulePrefabList.Count; i++)
            if (RoomRulePrefabList[i].TryGetComponent(out RoomRuleController rrc) && rrc.RoomVec.SequenceEqual(roomIndex))
                result.Add(rrc);
            
        return result;
    }

    // 인덱스가 같은 RoomRule 찾기 (마지막엔 랜덤)
    private RoomRuleController Get_CorrectRandomRoomRule(RoomController _Room)
    {
        List<RoomRuleController> roomRuleList = Get_CorrectRoomRuleList(_Room);
        return roomRuleList[Random.Range(0, roomRuleList.Count)];
    }

    #endregion

    #region Gate 

    private List<GateController> Get_AllGate(List<RoomController> _RoomList)
    {
        List<GateController> allGate = new List<GateController>();
        for (int i = 0; i < _RoomList.Count; i++)
            allGate.AddRange(_RoomList[i].InRoom_AllGate);

        return allGate;
    }

    #endregion

    #region Shop

    // 상점 갯수에 맞춰 생성할 ID 구하기
    private List<int> Get_RandomIndexList(int _ListRange, int _Amount, List<int> _ExcludeList)
    {
        List<int> result = new List<int>();
        while (true)
        {
            if (result.Count < _Amount)
            {
                // 랜덤 값 가져오기
                int index = Random.Range(1, _ListRange + 1);

                // 이미 포함되었거나, 무시할 리스트에 포함되어있다면
                if (!result.Contains(index) &&
                    !_ExcludeList.Contains(index))
                {
                    result.Add(index);
                }
            }
            break;
        }
        return result;
    }

    #endregion

    #region Round

    // 현재 Round에서, 타겟 Room에 맞춘 위치 값 반환
    private List<Vector2Int> Get_FindCorrectWorldVec_Normal(RoomController _TargetRoom, bool _ApplySpecialExist = false)
    {
        List<Vector2Int> existList = new List<Vector2Int>(alreadyExistList);
        if (_ApplySpecialExist) existList.AddRange(alreadyExistSpeicalList);

        return Get_FindCorrectWorldVec(
            _TargetRoom.RoomVec, roundList, existList);
    }

    // 현재 Round에서, 타겟 Room에 맞춘 위치 값 + 가장 먼 위치 값 반환
    private List<Vector2Int> Get_FindCorrectWorldVec_Furthest(RoomController _TargetRoom, bool _ApplySpecialExist = false)
    {
        List<Vector2Int> farRoundList = roundList.OrderByDescending(obj => Vector2Int.Distance(Vector2Int.zero, obj)).ToList();

        List<Vector2Int> existList = new List<Vector2Int>(alreadyExistList);
        if (_ApplySpecialExist) existList.AddRange(alreadyExistSpeicalList);

        return Get_FindCorrectWorldVec(
            _TargetRoom.RoomVec, farRoundList, existList);
    }

    private List<Vector2Int> Get_FindCorrectWorldVec(List<Vector2Int> _RoomVec, List<Vector2Int> _RoundList, List<Vector2Int> _ExistList)
    {
        List<Vector2Int> WorldVecList = new List<Vector2Int>();

        // 찾을 때까지 실행
        while (true)
        {
            int randomIndex = Random.Range(0, _RoundList.Count);

            WorldVecList = new List<Vector2Int>();
            bool wrongPlace = false;

            // 주변 공간에 배치할 시 배치 할 수 있는지에 대한
            for (int i = 0; i < _RoomVec.Count; i++)
            {
                WorldVecList.Add(_RoundList[randomIndex] + _RoomVec[i]);

                if (_ExistList.Contains(WorldVecList[i]))
                {
                    wrongPlace = true;
                    break;
                }
            }

            // 안된다면 다시 시작
            if (!wrongPlace) break;
        }

        return WorldVecList;
    }

    #endregion

    #region Public

    // 해당 백터의 주변을 구하기
    private List<Vector2Int> Get_RoundVec(Vector2Int _CenterVec)
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

    #endregion

    #region Add

    #region Round

    // 존재하는 방의 좌표와 Round 좌표를 초기화
    private void Add_RoundVec(List<Vector2Int> _AddVecList)
    {
        alreadyExistList.AddRange(_AddVecList);
        roundList = new List<Vector2Int>();

        for (int i = 0; i < alreadyExistList.Count; i++)
            foreach (Vector2Int vec in Get_RoundVec(alreadyExistList[i]))
                if (!roundList.Contains(vec) && !alreadyExistList.Contains(vec))
                    roundList.Add(vec);
    }

    // 특수 방의 좌표를 넣어줌
    private void Add_RoundSpecialVec(List<Vector2Int> _AddVecList)
    {
        List<Vector2Int> specialRoundAllList = new List<Vector2Int>(_AddVecList);
        for (int i = 0; i < _AddVecList.Count; i++)
        {
            specialRoundAllList.AddRange(Get_RoundVec(_AddVecList[i]));
        }
        alreadyExistSpeicalList.AddRange(specialRoundAllList);
        alreadyExistSpeicalList = alreadyExistSpeicalList.Distinct().ToList();
    }



    #endregion

    #endregion

    #region Is

    #region Gate 

    private bool Is_PartnerGate(GateController _Gate1, GateController _Gate2)
    {
        return ((_Gate1.RoomPosGate + _Gate1.GateDir) == _Gate2.RoomPosGate) && // 반대편에 방에 존재하는 게이트인지
            (_Gate1.GateDir * -1) == _Gate2.GateDir; // 서로 바라보고 있는지
    }

    #endregion

    #endregion
}