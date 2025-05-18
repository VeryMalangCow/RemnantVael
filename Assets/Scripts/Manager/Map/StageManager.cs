using NavMeshPlus.Components;
using System;
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
    [Header("=== Nav")]
    [SerializeField] private NavMeshSurface ThisNav;

    [Space(10)]
    [Header("=== Generate")]
    [SerializeField] private Transform MapParentTF;
    [SerializeField] public int TargetStageID = -1;
    [Space(10)] [SerializeField] private StageData LobbyStageData;
    [Space(10)] [SerializeField] private List<StageData> AllStageData;

    [Space(10)]
    [Header("=== Reso")]

    [Space(5)]
    [Header("-- Room")]
    [SerializeField] private GameObject LobbyRoomRulePrefab;
    [SerializeField] private GameObject StartRoomRulePrefab;

    [Space(3)]
    [Header("* Small Room")]
    [SerializeField] private List<GameObject> SRoomPrefabList;
    [SerializeField] private GameObject LobbyEntranceRoomRulePrefab;

    [Space(3)]
    [Header("* Normal Room")]
    [SerializeField] private List<GameObject> RoomPrefabList;
    [SerializeField] private List<GameObject> RoomRulePrefabList;
    [SerializeField] private List<GameObject> RoomRuleEntrancePrefabList;
    [SerializeField] private List<GameObject> RoomRuleVaultPrefabList;
    [SerializeField] private List<GameObject> RoomRuleShopPrefabList;
    [SerializeField] private List<GameObject> RoomRulePrisonPrefabList;

    [Space(5)]
    [Header("-- Build / Actual")]
    [SerializeField] private GameObject BUShopPrefab;
    [SerializeField] private GameObject MUShopPrefab;
    [SerializeField] private List<GameObject> VaultPrefabList;
    [SerializeField] private List<GameObject> PrisonPrefabList;

    [Space(5)]
    [Header("-- Build / Operator")]
    [SerializeField] private GameObject RepairOperatorPrefab;
    [SerializeField] private GameObject VaultRerollOperatorPrefab;
    [SerializeField] private GameObject VaultUpgradeOperatorPrefab;
    [SerializeField] private GameObject PrisonPayOperatorPrefab;
    [SerializeField] private GameObject PrisonPuzzleOperatorPrefab;

    [Space(5)]
    [Header("-- Icon")]
    [SerializeField] private CoupleData<Sprite> Vault_Icon;
    [SerializeField] private CoupleData<Sprite> Elevator_Icon;
    [SerializeField] private CoupleData<Sprite> Shop_Icon;
    [SerializeField] private CoupleData<Sprite> ST_Prison_Icon;
    [SerializeField] private CoupleData<Sprite> UT_Prison_Icon;
    [SerializeField] private CoupleData<Sprite> NT_Prison_Icon;
    [SerializeField] public List<MinimapIcon> MinimapIcons;

    [Space(10)]
    [Header("=== Value0")]
    [SerializeField] private Vector2 OffsetRoomSize;

    [Space(10)]
    [Header("=== Current")]
    [SerializeField] private List<RoomController> CurrentAllRoomController = new List<RoomController>();
    [SerializeField] private List<EntranceRuleController> CurrentAllEntranceRoomController = new List<EntranceRuleController>();
    [SerializeField] public RoomController CurrentRoomController;

    #region - Hide

    // 처음 시작하는 Room인가?
    [HideInInspector] public bool IsStartStage = true;


    // 스폰을 위한 리스트
    [HideInInspector] private List<int> ShuffledRoomIndexList = new List<int>();

    // 이미 차지한 Vec
    [HideInInspector] private HashSet<Vector2Int> alreadyExistList = new HashSet<Vector2Int>();
    [HideInInspector] private HashSet<Vector2Int> alreadyExistSpeicalList = new HashSet<Vector2Int>();

    // 배치할 주변 Vec
    [HideInInspector] private List<Vector2Int> roundList = new List<Vector2Int>();


    // 클리어와 클리어 전 머터리얼 셋 
    [HideInInspector] private HashSet<BuildSetSpriteController> CurrentSetSprites = new HashSet<BuildSetSpriteController>();
    [HideInInspector] private HashSet<BuildSetAnimController> CurrentSetAnims = new HashSet<BuildSetAnimController>();

    [HideInInspector] private StageData CurrentStageData;

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        LobbyStageData.Offset(ResourceManager.Instance.Get_LobbyStageMapSpriteList(), ResourceManager.Instance.Get_LobbyStageMapMaterialList());

        for (int i = 0; i < AllStageData.Count; i++)
        {
            AllStageData[i].Offset(ResourceManager.Instance.Get_StageMapSpriteList(i), ResourceManager.Instance.Get_StageMapMaterialList(i));
        }
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();

        // 스테이지 소환
        Gen_Stage(TargetStageID);
    }

    #endregion

    #region Generate

    #region Stage

    // 스테이지 생성
    public void Gen_Stage(int _StageID)
    {
        StageData stageData = Get_CollectStageData(_StageID);

        // 전에 있는 데이터를 제거
        Remove_CurrentStage();
        CurrentStageData = stageData;

        // 처음 방
        if (stageData.InfoData.StageID == 99) // 로비 시작 방
        {
            Gen_LobbyStage();
        }
        else // 전투 스테이지 시작 방
        {
            Gen_CombatStage(stageData);
        }

        // 게이트 활성화
        Set_GateActiveOn();

        // Entrance 활성화
        Set_EntranceIndex(TargetStageID);

        // UI 셋
        Set_StartUI(stageData);

        // 적 객체 오브젝트 풀링 시스템 세팅하기
        PoolingManager.Instance.Offset_EnemiesPooling(stageData.EnemyData.StageEnemyList);

        // Sound (BGM) 시작
        SoundManager.Instance.Play_2D_BGM("Stage" + DevTool.Get_LengthString(stageData.InfoData.StageID, 2) + "_BGM");

        Reset_GenStageData();

        if (TargetStageID == 99)
            StartCoroutine(Play_LobbyStart_Cor(MainGameUIManager.Instance.FadeOutTime));

        // 처음 스타트맵
        Play_CurrentRoom(Get_CorrectRoom(0));
    }

    private void Set_EntranceIndex(int _CurrentIndex)
    {
        List<int> indexList = ResourceManager.Instance.Get_CorrectIndexList(_CurrentIndex);

        if (indexList.Count > 1)
            indexList = DevTool.Get_ShuffledList(indexList);
        
        for (int i = 0; i < indexList.Count; i++)
            CurrentAllEntranceRoomController[i].Set_ElevatorData(indexList[i]);
    }

    // Lobby 스테이지 생성
    private void Gen_LobbyStage()
    {
        int TempID = 0;

        Gen_LobbyRoom(RoomPrefabList[0], TempID);
        TempID++;

        Gen_LobbyEntranceRoom(TempID, new List<Vector2Int> { Vector2Int.up });
        TempID++;
    }

    // Combat 스테이지 생성
    private void Gen_CombatStage(StageData _StageData)
    {
        int TempID = 0;

        Gen_StartRoom(RoomPrefabList[0], TempID);
        TempID++;

        // 생성할 Room의 양을 계산에 1중 리스트로 변경 => 이들을 섞음
        ShuffledRoomIndexList = DevTool.Get_ShuffledList(
            Get_ListInt_FromGenRoomAmount(_StageData.RoomData.RoomAmount));

        // 기본 방 생성
        for (int i = 0; i < ShuffledRoomIndexList.Count; i++)
        {
            Gen_NormalRoom(RoomPrefabList[ShuffledRoomIndexList[i]], TempID);
            TempID++;
        }

        // 통과 방 생성
        for (int i = 0; i < _StageData.RoomData.EntranceRoom.Count; i++)
        {
            Gen_EntranceRoom(_StageData.RoomData.EntranceRoom[i], TempID);
            TempID++;
        }

        // 금고 방 생성
        for (int i = 0; i < _StageData.RoomData.VaultRoom.Count; i++)
        {
            Gen_VaultRoom(_StageData.RoomData.VaultRoom[i], TempID);
            TempID++;
        }

        // 상점 방 생성
        for (int i = 0; i < _StageData.RoomData.ShopRoom.Count; i++)
        {
            Gen_ShopRoom(_StageData.RoomData.ShopRoom[i], TempID);
            TempID++;
        }

        // 감옥 방 생성
        for (int i = 0; i < _StageData.RoomData.PrisonRoom.Count; i++)
        {
            Gen_PrisonRoom(_StageData.RoomData.PrisonRoom[i], TempID);
            TempID++;
        }
    }

    #endregion

    #region Room

    #region Start

    // 로비 방 생성
    private void Gen_LobbyRoom(GameObject _Prefab, int _TempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(_Prefab, MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(LobbyRoomRulePrefab, room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule;

            room.Offset(_TempID);
            Add_RoundVec(new List<Vector2Int>() { Vector2Int.zero });
        }
    }

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

    #endregion

    #region Normal

    // 기본 방 생성
    private void Gen_NormalRoom(GameObject _Prefab, int _TempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(_Prefab, MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(Get_CorrectRandomRoomRule(room).gameObject, room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule;

            room.Offset(_TempID);
            Set_NormalRelativeVec(room, _ConnectedRoomAmount: -1, _ApplySpecialExist: false);
        }
    }

    #endregion

    #region Entrance

    // 통과 방 하나 생성
    private void Gen_EntranceRoom(GenSpecialRoomData _EntranceRoomData, int _TempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(RoomPrefabList[_EntranceRoomData.ID], MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(RoomRuleEntrancePrefabList[_EntranceRoomData.RuleID], room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule;

            EntranceRuleController entranceRule = DevTool.Get_CastingTType<EntranceRuleController>(roomRule);
            CurrentAllEntranceRoomController.Add(entranceRule);

            room.Offset(_TempID);
            Set_FurthestRelativeVec(room, _ConnectedRoomAmount: 1, _ApplySpecialExist: true);
        }
    }

    // 로비 통과 방 하나 생성
    private void Gen_LobbyEntranceRoom(int _TempID, List<Vector2Int> _RelativePos)
    {
        if (DevTool.Get_ComponentTType(Instantiate(SRoomPrefabList[0], MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(LobbyEntranceRoomRulePrefab, room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule;

            EntranceRuleController entranceRule = DevTool.Get_CastingTType<EntranceRuleController>(roomRule);
            CurrentAllEntranceRoomController.Add(entranceRule);

            room.Offset(_TempID);
            Set_NormalRelativeVec(room, _RelativePos);
        }
    }

    #endregion

    #region Vault

    // 금고 방 하나 생성
    private void Gen_VaultRoom(GenSpecialRoomData _VaultRoomData, int _TempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(RoomPrefabList[_VaultRoomData.ID], MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(RoomRuleVaultPrefabList[_VaultRoomData.RuleID], room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule;

            VaultRuleController vaultRule = DevTool.Get_CastingTType<VaultRuleController>(roomRule);
            VaultController vault = DevTool.Get_ComponentTType<VaultController>(Instantiate(DevTool.Get_RandomInList(VaultPrefabList), vaultRule.InRoom_VaultParentTF));
            vaultRule.Vault = vault;
            vault.gameObject.transform.localPosition = Vector2.zero;
            vault.gameObject.SetActive(false);

            RepairOperatorController repairOper = DevTool.Get_ComponentTType<RepairOperatorController>(
                Instantiate(RepairOperatorPrefab, vaultRule.InRoom_RepairOperactorParentTF));
            vaultRule.RepairOperator = repairOper;
            repairOper.Set_TargetBuild(vault);
            repairOper.gameObject.transform.localPosition = Vector2.zero;
            repairOper.gameObject.SetActive(false);

            VaultRerollOperatorController rerollOper = DevTool.Get_ComponentTType<VaultRerollOperatorController>(
                Instantiate(VaultRerollOperatorPrefab, vaultRule.InRoom_RerollOperactorParentTF));
            vaultRule.RerollOperator = rerollOper;
            rerollOper.Set_TargetBuild(vault);
            rerollOper.gameObject.transform.localPosition = Vector2.zero;
            rerollOper.gameObject.SetActive(false);

            VaultUpgradeOperatorController upgradeOper = DevTool.Get_ComponentTType<VaultUpgradeOperatorController>(
                Instantiate(VaultUpgradeOperatorPrefab, vaultRule.InRoom_UpgradeOperactorParentTF));
            vaultRule.UpgradeOperator = upgradeOper;
            upgradeOper.Set_TargetBuild(vault);
            upgradeOper.gameObject.transform.localPosition = Vector2.zero;
            upgradeOper.gameObject.SetActive(false);

            room.Offset(_TempID);
            Set_NormalRelativeVec(room, _ConnectedRoomAmount: 1, _ApplySpecialExist: true);
        }
    }

    #endregion

    #region Shop

    // 상점 방 하나 생성
    private void Gen_ShopRoom(GenSpecialRoomData _ShopRoomData, int _TempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(RoomPrefabList[_ShopRoomData.ID], MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(RoomRuleShopPrefabList[_ShopRoomData.RuleID], room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule;

            ShopRuleController shopRule = DevTool.Get_CastingTType<ShopRuleController>(roomRule);

            BaseUpgradeController BUShop = DevTool.Get_ComponentTType<BaseUpgradeController>(Instantiate(BUShopPrefab, shopRule.InRoom_BUShopParentTF));
            shopRule.BUShop = BUShop;
            BUShop.gameObject.transform.localPosition = Vector2.zero;
            BUShop.gameObject.SetActive(false);

            ModuleUpgradeController MUShop = DevTool.Get_ComponentTType<ModuleUpgradeController>(Instantiate(MUShopPrefab, shopRule.InRoom_MUShopParentTF));
            shopRule.MUShop = MUShop;
            MUShop.gameObject.transform.localPosition = Vector2.zero;
            MUShop.gameObject.SetActive(false);

            RepairOperatorController BURepairOper = DevTool.Get_ComponentTType<RepairOperatorController>(
                Instantiate(RepairOperatorPrefab, shopRule.InRoom_BURepairOperactorParentTF));
            shopRule.BURepairOperator = BURepairOper;
            BURepairOper.Set_TargetBuild(BUShop);
            BURepairOper.gameObject.transform.localPosition = Vector2.zero;
            BURepairOper.gameObject.SetActive(false);

            RepairOperatorController MURepairOper = DevTool.Get_ComponentTType<RepairOperatorController>(
                Instantiate(RepairOperatorPrefab, shopRule.InRoom_MURepairOperactorParentTF));
            shopRule.MURepairOperator = MURepairOper;
            MURepairOper.Set_TargetBuild(MUShop);
            MURepairOper.gameObject.transform.localPosition = Vector2.zero;
            MURepairOper.gameObject.SetActive(false);

            room.Offset(_TempID);
            Set_NormalRelativeVec(room, _ConnectedRoomAmount: 1, _ApplySpecialExist: true);
        }
    }

    #endregion

    #region Prison

    // 감옥 방 하나 생성
    private void Gen_PrisonRoom(GenPrisonRoomData _PrisonRoomData, int _TempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(RoomPrefabList[_PrisonRoomData.ID], MapParentTF), out RoomController room))
        {
            CurrentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(RoomRulePrisonPrefabList[_PrisonRoomData.RuleID], room.gameObject.transform), out RoomRuleController roomRule))
                room.RoomRuleController = roomRule;

            PrisonRuleController prisonRule = DevTool.Get_CastingTType<PrisonRuleController>(roomRule);

            Debug.Log(_PrisonRoomData.TypeID);
            PrisonController prison = DevTool.Get_ComponentTType<PrisonController>(Instantiate(PrisonPrefabList[_PrisonRoomData.TypeID], prisonRule.InRoom_PrisonParentTF));
            prisonRule.Prison = prison;
            prison.gameObject.transform.localPosition = Vector2.zero;
            prison.gameObject.SetActive(false);

            PrisonPayOperatorController payOper = DevTool.Get_ComponentTType<PrisonPayOperatorController>(
               Instantiate(PrisonPayOperatorPrefab, prisonRule.InRoom_PayOperactorParentTF));
            prisonRule.PayOperator = payOper;
            payOper.Set_TargetBuild(prison);
            payOper.gameObject.SetActive(false);

            PrisonPuzzleOperatorController puzzleOper = DevTool.Get_ComponentTType<PrisonPuzzleOperatorController>(
               Instantiate(PrisonPuzzleOperatorPrefab, prisonRule.InRoom_PuzzleOperactorParentTF));
            prisonRule.PuzzleOperator = puzzleOper;
            puzzleOper.Set_TargetBuild(prison);
            puzzleOper.gameObject.SetActive(false);

            room.Offset(_TempID);
            Set_NormalRelativeVec(room, _ConnectedRoomAmount: 1, _ApplySpecialExist: true);
        }
    }

    #endregion

    #endregion

    #endregion

    #region Reset (StageData)

    private void Remove_CurrentStage()
    {
        IsStartStage = true;

        CurrentSetSprites.Clear();
        CurrentSetAnims.Clear();

        CurrentStageData = null;

        int allRoomAmount = CurrentAllRoomController.Count;
        for (int i = allRoomAmount - 1; i >= 0; i--)
            Destroy(CurrentAllRoomController[i].gameObject);
        CurrentAllRoomController.Clear();

        int allEntranceRoomAmount = CurrentAllEntranceRoomController.Count;
        CurrentAllEntranceRoomController.Clear();

        CurrentRoomController = null;

        PoolingManager.Instance.Remove_AllQueue();

        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Remove_AllMinimapCell();
    }


    #endregion

    #region Set State

    public void Play_CurrentRoom(RoomController _TargetRC)
    {
        StartCoroutine(Play_CurrentRoom_Cor(_TargetRC));
    }

    private IEnumerator Play_CurrentRoom_Cor(RoomController _TargetRC)
    {
        if (_TargetRC == null) yield break;

        Remove_SetSprites();
        Remove_SetAnims();

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

        // Ally
        AllyManager.Instance.Set_AllAllyPlayerNearPos();
        AllyManager.Instance.Stop_AllAllies_Combat();

        // Minimap
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Set_State();

        yield return new WaitForSeconds(0.2f);

        _TargetRC.Play_RoomState();
        Set_NavBake();

        LayerOrderManager.Instance.NeedSortingObjects.AddRange(EnemyManager.Instance.CurrentEnemyList);
        LayerOrderManager.Instance.NeedSortingObjects.AddRange(AllyManager.Instance.AllAllies);

        // Minimap
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Set_State();
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Play_Effect();

        // Ally
        AllyManager.Instance.Start_AllAllies_Combat();
    }


    public void Play_CompleteKillAll()
    {
        if (CurrentRoomController == null || CurrentRoomController.RoomRuleController.RoomType == eRoomType.Completed) return;

        StartCoroutine(Play_CompleteKillAll_Cor());
    }

    public IEnumerator Play_CompleteKillAll_Cor()
    {
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
    }

    #endregion

    #region Set

    #region Stage

    private void Set_StartUI(StageData _StageData)
    {
        MainGameUIManager.Instance.MapIntro_UIController.Play_IntroLabel();
        MainGameUIManager.Instance.PlayerHUD_UIController.ThisMinimap.Gen_Minimap();
        MainGameUIManager.Instance.PlayerHUD_UIController.Set_StageDescription();
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


    // 현재 게이트 모두 활성화
    private void Set_GateActiveOn()
    {
        Set_ParterAllGate();
        List<GateController> allGate = Get_AllGate(CurrentAllRoomController);
        for (int i = 0; i < allGate.Count; i++)
            if (allGate[i].ParterGate != null)
                allGate[i].Set_ExistDoorState(true);
            else
                allGate[i].Set_ExistDoorState(false);
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

    // 월드 기준: 상대적인 좌표 직접 지정
    private void Set_NormalRelativeVec(RoomController _Room, List<Vector2Int> _RelativePos)
    {
        Set_RelativeVec(_Room, _RelativePos);

        Set_RoomPos(_Room);
        Add_RoundVec(_Room.RoomVec);
    }

    // 월드 기준: 상대적인 좌표 삽입
    private void Set_NormalRelativeVec(RoomController _Room, int _ConnectedRoomAmount = -1, bool _ApplySpecialExist = false)
    {
        Set_RelativeVec(_Room, Get_FindCorrectWorldVec_Normal(_Room, _ConnectedRoomAmount, _ApplySpecialExist));

        Set_RoomPos(_Room);
        Add_RoundVec(_Room.RoomVec);
    }

    // 월드 기준: 상대적인 좌표 삽입: 가장 멀고, 특별 Round 포함
    private void Set_FurthestRelativeVec(RoomController _Room, int _ConnectedRoomAmount = -1, bool _ApplySpecialExist = true)
    {
        Set_RelativeVec(_Room, Get_FindCorrectWorldVec_Furthest(_Room, _ConnectedRoomAmount, _ApplySpecialExist));

        Set_RoomPos(_Room);
        Add_RoundVec(_Room.RoomVec);

        Add_RoundSpecialVec(_Room.RoomVec);
    }

    #endregion

    #region SR

    public void Set_MapSprite(SpriteRenderer _SR, string _SpriteKey)
    {
        if (!CurrentStageData.MapSpriteReso.MapSprite.ContainsKey(_SpriteKey)) { Debug.Log(_SpriteKey); return; }

        SpriteMaterial spriteMatrial = CurrentStageData.MapSpriteReso.MapSprite[_SpriteKey];
        _SR.sprite = spriteMatrial.Sprite;
        _SR.material = CurrentStageData.MapMaterialUnclear[spriteMatrial.MaterialIndex];

        return;
    }

    public void Set_SetSpriteClearly()
    {
        if (CurrentSetSprites == null || CurrentSetSprites.Count <= 0) return;

        foreach(BuildSetSpriteController setSprite in CurrentSetSprites)
        {
            if (DevTool.Get_ComponentTType(setSprite.gameObject, out SpriteRenderer sr))
            {
                int index = CurrentStageData.MapMaterialUnclear.IndexOf(sr.sharedMaterial);
                if (index == -1)
                { Debug.Log(sr.gameObject.name + " / " + sr.gameObject.transform.parent.gameObject.name); continue; }
                sr.material = CurrentStageData.MapMaterialClear[index];
            }
        }
    }

    #endregion

    #region Anim

    public void Set_StageDoorAnim(GateController _Gate, SpriteRenderer _SR, Vector2Int _DoorDir)
    {
        List<StageDoorAnim> doorAnim = CurrentStageData.MapDoorAnim;
        for (int i = 0; i < doorAnim.Count; i++)
            if (doorAnim[i].Dir == _DoorDir)
            {
                _Gate.ThisAC = doorAnim[i].DoorAnim;
                _SR.material = CurrentStageData.MapMaterialUnclear[doorAnim[i].MaterialIndex];
            }
    }

    public void Set_SetAnimClearly()
    {
        if (CurrentSetAnims == null || CurrentSetAnims.Count <= 0) return;

        foreach (BuildSetAnimController setAnim in CurrentSetAnims)
        {
            if (DevTool.Get_ComponentTType(setAnim.gameObject, out SpriteRenderer sr))
            {
                int index = CurrentStageData.MapMaterialUnclear.IndexOf(sr.sharedMaterial);
                if (index == -1)
                { Debug.Log(sr.material.name + " / " + sr.gameObject.transform.parent.gameObject.name); continue; }
                sr.material = CurrentStageData.MapMaterialClear[index];
            }
        }
    }

    #endregion

    #endregion

    #region Get

    #region Stage

    // 올바른 Stage 데이터 구하기
    public StageData Get_CollectStageData(int _StageID)
    {
        if (_StageID == 99)
            return LobbyStageData;

        for (int i = 0; i < AllStageData.Count; i++)
            if (AllStageData[i].InfoData.StageID == _StageID)
                return AllStageData[i];

        return null;
    }

    #endregion

    #region Room

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
        return roomRuleList[UnityEngine.Random.Range(0, roomRuleList.Count)];
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

    #region Round

    // 현재 Round에서, 타겟 Room에 맞춘 위치 값 반환
    private List<Vector2Int> Get_FindCorrectWorldVec_Normal(RoomController _TargetRoom, int _ConnectedRoomAmount = -1, bool _ApplySpecialExist = false)
    {
        List<Vector2Int> randomRoundList = DevTool.Get_ShuffledList(roundList);

        List<Vector2Int> existList = new List<Vector2Int>(alreadyExistList);
        if (_ApplySpecialExist) existList.AddRange(alreadyExistSpeicalList);

        return Get_FindCorrectWorldVec(
            _TargetRoom.RoomVec, roundList, existList, _ConnectedRoomAmount);
    }

    // 현재 Round에서, 타겟 Room에 맞춘 위치 값 + 가장 먼 위치 값 반환
    private List<Vector2Int> Get_FindCorrectWorldVec_Furthest(RoomController _TargetRoom, int _ConnectedRoomAmount = -1, bool _ApplySpecialExist = false)
    {
        List<Vector2Int> farRoundList = roundList.OrderByDescending(obj => Vector2Int.Distance(Vector2Int.zero, obj)).ToList();

        List<Vector2Int> existList = new List<Vector2Int>(alreadyExistList);
        if (_ApplySpecialExist) existList.AddRange(alreadyExistSpeicalList);

        return Get_FindCorrectWorldVec(
            _TargetRoom.RoomVec, farRoundList, existList, _ConnectedRoomAmount);
    }

    // 계산
    private List<Vector2Int> Get_FindCorrectWorldVec(List<Vector2Int> _RoomVec, List<Vector2Int> _RoundList, List<Vector2Int> _ExistList, int _ConnectedRoomAmount = -1)
    {
        List<Vector2Int> WorldVecList = new List<Vector2Int>();

        // 찾을 때까지 실행

        int randomIndex = 0;

        while (true)
        {
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

            if (_ConnectedRoomAmount != -1 && Get_AdjacentRoomAmount(WorldVecList, _ExistList) != _ConnectedRoomAmount)
                wrongPlace = true;

            randomIndex++;

            // 안된다면 다시 시작
            if (!wrongPlace) break;
        }

        return WorldVecList;
    }



    private int Get_AdjacentRoomAmount(List<Vector2Int> _TargetRoomVec, List<Vector2Int> _ExistRoomVec)
    {
        List<Vector2Int> targetRoomVecRound = DevTool.Get_RoundVec(_TargetRoomVec);

        return DevTool.Get_IntersectionAmount(targetRoomVecRound, _ExistRoomVec);
    }

    #endregion

    #region Minimap

    public MinimapIcon Get_CorrectMinimapIcon(RoomController _Room)
    {
        return MinimapIcons[_Room.RoomStaticID];
    }

    public CoupleData<Sprite> Get_CorrectMinimapIcon(RoomRuleController _RoomRule)
    {
        switch(_RoomRule)
        {
            case VaultRuleController:
                return Vault_Icon;

            case EntranceRuleController:
                return Elevator_Icon;

            case ShopRuleController:
                return Shop_Icon;

            case PrisonRuleController prisonRule:
                if (prisonRule.Prison is StrikeTeamPrisonController)
                    return ST_Prison_Icon;
                else if (prisonRule.Prison is UplinkTeamPrisonController)
                    return UT_Prison_Icon;
                else if (prisonRule.Prison is NeoTeamPrisonController)
                    return NT_Prison_Icon;
                else
                    return null;

            default:
                return null;
        }

    }

    #endregion

    #region Vault

    public GameObject Get_VaultCorrectType(Type _TypeVault) 
    {
        for (int i = 0; i < VaultPrefabList.Count; i++)
            if (DevTool.Get_ComponentTType<VaultController>(VaultPrefabList[i]).GetType() == _TypeVault)
                return VaultPrefabList[i];

        return null;
    }

    #endregion

    #endregion

    #region Add

    #region Round

    // 존재하는 방의 좌표와 Round 좌표를 초기화
    private void Add_RoundVec(List<Vector2Int> _AddVecList)
    {
        foreach (var vec in _AddVecList)
            alreadyExistList.Add(vec);

        roundList = DevTool.Get_RoundVec(alreadyExistList).ToList();
    }

    // 특수 방의 좌표를 넣어줌
    private void Add_RoundSpecialVec(List<Vector2Int> _AddVecList)
    {
        foreach (var vec in _AddVecList)
        {
            alreadyExistSpeicalList.Add(vec);
            List<Vector2Int> eachRound = DevTool.Get_RoundVec(vec);
            for (int i = 0; i < eachRound.Count; i++)
                alreadyExistSpeicalList.Add(eachRound[i]);
        }

    }

    #endregion

    #region SR

    public void Add_SetSprite(BuildSetSpriteController _SetSprite)
    {
        CurrentSetSprites.Add(_SetSprite);
    }

    #endregion

    #region Anim

    public void Add_SetAnim(BuildSetAnimController _SetAnim)
    {
        CurrentSetAnims.Add(_SetAnim);
    }

    #endregion

    #endregion

    #region Remove

    #region SR

    private void Remove_SetSprites()
    {
        CurrentSetSprites.Clear();
    }

    #endregion

    #region Anim

    private void Remove_SetAnims()
    {
        CurrentSetAnims.Clear();
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

    #region Play (Lobby)

    private IEnumerator Play_LobbyStart_Cor(float _DelayTime)
    {
        yield return new WaitForSeconds(_DelayTime);

        PlayerManager.Instance.PlayerController.Set_StartStage();
        EventManager.Instance.Set_Input(true);
    }

    #endregion

    #region Play (Spawn another Stage)

    public void Play_GenStage(int _StageID)
    {
        TargetStageID = _StageID;
        StartCoroutine(Play_GenStage_Cor());
    }

    private IEnumerator Play_GenStage_Cor()
    {
        yield return new WaitForSeconds(1f);

        Gen_Stage(TargetStageID);
    }

    #endregion

    #region Nav

    public void Set_NavBake()
    {
        ThisNav.BuildNavMesh();
    }

    #endregion
}