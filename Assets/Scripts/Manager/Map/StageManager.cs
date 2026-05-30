using NavMeshPlus.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager>, IMainGameInitializer
{
    #region Value

    #region - Inspector
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    [Space(20)]
    [Header("<><><><><> Stage Manager")]

    [Space(10)]
    [Header("=== Nav")]
    [SerializeField] private NavMeshSurface thisNav;

    [Space(10)]
    [Header("=== Generate")]
    [SerializeField] private Transform mapParentTF;
    [SerializeField] public int targetStageID = -1;
    [Space(10)] [SerializeField] private StageData lobbyStageData;
    [Space(10)][SerializeField] private List<StageData> allStageData;

    [Space(10)]
    [Header("=== Current")]
    [SerializeField] private List<RoomController> currentAllRoomController = new List<RoomController>();
    [SerializeField] private List<EntranceRuleController> currentAllEntranceRoomController = new List<EntranceRuleController>();
    [SerializeField] public RoomController currentRoomController;

    #endregion

    #region - Hide

    // Size
    [HideInInspector] private Vector2 offsetRoomSize = new Vector2(22, 14);

    // 처음 시작하는 Room인가?
    [HideInInspector] public bool isStartStage = true;


    // 스폰을 위한 리스트
    [HideInInspector] private List<int> shuffledRoomIndexList = new List<int>();

    // 이미 차지한 Vec
    [HideInInspector] private HashSet<Vector2Int> alreadyExistList = new HashSet<Vector2Int>();
    [HideInInspector] private HashSet<Vector2Int> alreadyExistSpeicalList = new HashSet<Vector2Int>();

    // 배치할 주변 Vec
    [HideInInspector] private List<Vector2Int> roundList = new List<Vector2Int>();


    // 클리어와 클리어 전 머터리얼 셋 
    [HideInInspector] private HashSet<BuildSetSpriteController> currentSetSprites = new HashSet<BuildSetSpriteController>();
    [HideInInspector] private HashSet<BuildSetAnimController> currentSetAnims = new HashSet<BuildSetAnimController>();

    [HideInInspector] private StageData currentStageData;

    // Passage
    [HideInInspector] private int beforeStageID = -1;
    [HideInInspector] private int afterStageID = -1;
    [HideInInspector] private StageData beforeStageData;
    [HideInInspector] private StageData afterStageData;

    [HideInInspector] private AllPassageMiddleSpriteData passageMiddleSpriteData;


    #endregion

    #region Init
    public IEnumerator Initialize()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Offset();
        sw.Stop();
        UnityEngine.Debug.Log($"StageManager: <color=orange>SpriteOffset</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");

        yield return null;
        // 스테이지 소환

        sw.Restart();
        Gen_Stage(targetStageID);
        sw.Stop();
        UnityEngine.Debug.Log($"StageManager: <color=orange>Generate</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
    }

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        lobbyStageData.Offset(ResourceManager.instance.Get_LobbyMapReso());

        for (int i = 0; i < allStageData.Count; i++)
        {
            allStageData[i].Offset(ResourceManager.instance.Get_StageMapReso(i));
        }

        Init_PassageMiddleData();
    }

    #endregion

    #region Framework
/*
    private void Start()
    {
        Offset();

        // 스테이지 소환
        Gen_Stage(targetStageID);
    }
*/
    #endregion

    #region Generate

    #region Stage

    // 스테이지 생성
    public void Gen_Stage(int stageId)
    {
        StageData stageData = Get_CollectStageData(stageId);

        // 전에 있는 데이터를 제거
        Remove_PassageStage();
        Remove_CurrentStage();
        currentStageData = stageData;

        // 처음 방
        if (stageData.infoData.stageId == 99) // 로비 시작 방
        {
            Gen_Type_LobbyStage();
        }
        else // 전투 스테이지 시작 방
        {
            Gen_Type_CombatStage(stageData);
            EliteEnemyController.isDroppedBossKeycard = false;
        }

        // Entrance 활성화
        Set_EntranceIndex(targetStageID);

        // 게이트 활성화
        Set_GateActiveOn();

        // UI 셋
        Set_StartUI(stageData);

        // Sound (BGM) 시작
        SoundManager.instance.Play_2D_BGM_Stage(stageData.infoData.stageId);

        Reset_GenStageData();

        if (targetStageID == 99)
            StartCoroutine(Play_LobbyStart_Cor(MainGameUIManager.instance.fadeOutTime));

        // 처음 스타트맵
        Play_CurrentRoom(Get_CorrectRoom(0));
    }

    // 통로 스테이지 생성
    public void Gen_PassageStage(int nextStageId)
    {
        // 전에 있는 데이터를 제거
        Remove_CurrentStage();

        // Gen
        Gen_PassageRoom(nextStageId);

        // 게이트 활성화
        Set_GateActiveOn();

        // UI 셋
        Set_StartPassageUI();

        // Sound (BGM) 시작
        //SoundManager.Instance.Play_2D_BGM("Stage" + DevTool.Get_LengthString(stageData.InfoData.StageID, 2) + "_BGM");

        Reset_GenStageData();

        // 처음 스타트맵
        Play_CurrentRoom(Get_CorrectRoom(0));
    }


    private void Set_EntranceIndex(int currentIndex)
    {
        List<int> indexList = ResourceManager.instance.Get_CorrectIndexList(currentIndex);

        if (indexList.Count > 1)
            indexList = DevTool.Get_ShuffledList(indexList);
        
        for (int i = 0; i < indexList.Count; i++)
            currentAllEntranceRoomController[i].Set_ElevatorData(indexList[i]);
    }

    // Lobby 스테이지 생성
    private void Gen_Type_LobbyStage()
    {
        int tempId = 0;

        Gen_LobbyRoom(ResourceManager.instance.roomPrefabArr[0], tempId);
        tempId++;

        Gen_LobbyEntranceRoom(tempId, new List<Vector2Int> { Vector2Int.up });
        tempId++;
    }

    // Combat 스테이지 생성
    private void Gen_Type_CombatStage(StageData stageData)
    {
        int tempId = 0;

        Gen_StartRoom(ResourceManager.instance.roomPrefabArr[0], tempId);
        tempId++;

        // 생성할 Room의 양을 계산에 1중 리스트로 변경 => 이들을 섞음
        shuffledRoomIndexList = DevTool.Get_ShuffledList(
            Get_ListInt_FromGenRoomAmount(stageData.roomData.roomAmount));

        // 기본 방 생성
        for (int i = 0; i < shuffledRoomIndexList.Count; i++)
        {
            Gen_NormalRoom(ResourceManager.instance.roomPrefabArr[shuffledRoomIndexList[i]], tempId);
            tempId++;
        }

        // 지정된 방 생성 (일반 룸과 같지만 특정 구성만 다름 ex.Elite)
        for (int i = 0; i < stageData.roomData.designatedRoom.Count; i++)
        {
            Gen_DesignatedRoom(stageData.roomData.designatedRoom[i], tempId);
            tempId++;
        }

        // 통과 방 생성
        for (int i = 0; i < stageData.roomData.entranceRoom.Count; i++)
        {
            Gen_EntranceRoom(stageData.roomData.entranceRoom[i], tempId);
            tempId++;
        }

        // 금고 방 생성
        for (int i = 0; i < stageData.roomData.vaultRoom.Count; i++)
        {
            Gen_VaultRoom(stageData.roomData.vaultRoom[i], tempId, out bool generated);
            if (generated) tempId++;
        }

        // 상점 방 생성
        for (int i = 0; i < stageData.roomData.shopRoom.Count; i++)
        {
            Gen_ShopRoom(stageData.roomData.shopRoom[i], tempId, out bool generated);
            if (generated) tempId++;
        }

        // Ally 상점 방 생성
        for (int i = 0; i < stageData.roomData.allyShopRoom.Count; i++)
        {
            Gen_AllyShopRoom(stageData.roomData.allyShopRoom[i], tempId, out bool generated);
            if (generated) tempId++;
        }

        // 감옥 방 생성
        for (int i = 0; i < stageData.roomData.prisonRoom.Count; i++)
        {
            Gen_PrisonRoom(stageData.roomData.prisonRoom[i], tempId, out bool generated);
            if (generated) tempId++;
        }
    }

    #endregion

    #region Room

    #region Start

    // 로비 방 생성
    private void Gen_LobbyRoom(GameObject prefab, int tempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(prefab, mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.lobbyRoomRulePrefab, room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            room.Offset(tempID);
            Add_RoundVec(new List<Vector2Int>() { Vector2Int.zero });
        }
    }

    // 시작 방 생성
    private void Gen_StartRoom(GameObject prefab, int tempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(prefab, mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.startRoomRulePrefab, room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            room.Offset(tempID);
            Add_RoundVec(new List<Vector2Int>() { Vector2Int.zero });

            Set_FieldObjPos(room);
        }
    }

    #endregion

    #region Normal

    // 기본 방 생성
    private void Gen_NormalRoom(GameObject prefab, int tempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(prefab, mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(Get_CorrectRandomRoomRule(room).gameObject, room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            room.Offset(tempID);
            Set_NormalRelativeVec(room, connectedRoomAmount: -1, applySpecialExist: false);
        }
    }

    #endregion

    #region Designated

    // 지정 방 생성
    private void Gen_DesignatedRoom(GenDesignatedRoom designatedRoomData, int tempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomPrefabArr[designatedRoomData.id], mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomDesignatedPrefabArr[designatedRoomData.ruleId], room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            room.Offset(tempID);
            Set_NormalRelativeVec(room, connectedRoomAmount: -1, applySpecialExist: false);
        }
    }

    #endregion

    #region Entrance

    // 통과 방 하나 생성
    private void Gen_EntranceRoom(GenSpecialRoomData entranceRoomData, int tempID)
    {
        if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomPrefabArr[entranceRoomData.id], mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomRuleEntrancePrefabArr[entranceRoomData.ruleId], room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            EntranceRuleController entranceRule = DevTool.Get_CastingTType<EntranceRuleController>(roomRule);
            currentAllEntranceRoomController.Add(entranceRule);

            room.Offset(tempID);
            Set_FurthestRelativeVec(room, connectedRoomAmount: 1, applySpecialExist: true);
        }
    }

    // 로비 통과 방 하나 생성
    private void Gen_LobbyEntranceRoom(int tempID, List<Vector2Int> relativePos)
    {
        if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.sRoomPrefabList[0], mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.lobbyEntranceRoomRulePrefab, room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            EntranceRuleController entranceRule = DevTool.Get_CastingTType<EntranceRuleController>(roomRule);
            currentAllEntranceRoomController.Add(entranceRule);

            room.Offset(tempID);
            entranceRule.Set_EntranceRuleInLobby();
            Set_NormalRelativeVec(room, relativePos);
        }
    }

    #endregion

    #region Vault

    // 금고 방 하나 생성
    private void Gen_VaultRoom(GenSpecialRoomData vaultRoomData, int tempID, out bool generated)
    {
        generated = false;
        GameProgressJsonData data = SaveDataManager.instance.jsonData.gameProgressData;

        if (!data.usableVault) return;

        if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomPrefabArr[vaultRoomData.id], mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomRuleVaultPrefabArr[vaultRoomData.ruleId], room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            VaultRuleController vaultRule = DevTool.Get_CastingTType<VaultRuleController>(roomRule);
            VaultController vault = DevTool.Get_ComponentTType<VaultController>(Instantiate(DevTool.Get_RandomInList(ResourceManager.instance.vaultPrefabArr), vaultRule.inRoom_vaultParentTf));
            vaultRule.vault = vault;
            vault.gameObject.transform.localPosition = Vector2.zero;
            vault.gameObject.SetActive(false);

            RepairOperatorController repairOper = DevTool.Get_ComponentTType<RepairOperatorController>(
                Instantiate(ResourceManager.instance.repairOperatorPrefab, vaultRule.inRoom_RepairOperactorParentTf));
            vaultRule.repairOperator = repairOper;
            repairOper.Set_TargetBuild(vault);
            repairOper.gameObject.transform.localPosition = Vector2.zero;
            repairOper.gameObject.SetActive(false);

            VaultRerollOperatorController rerollOper = DevTool.Get_ComponentTType<VaultRerollOperatorController>(
                Instantiate(ResourceManager.instance.vaultRerollOperatorPrefab, vaultRule.inRoom_RerollOperactorParentTf));
            vaultRule.rerollOperator = rerollOper;
            rerollOper.Set_TargetBuild(vault);
            rerollOper.gameObject.transform.localPosition = Vector2.zero;
            rerollOper.gameObject.SetActive(false);

            VaultUpgradeOperatorController upgradeOper = DevTool.Get_ComponentTType<VaultUpgradeOperatorController>(
                Instantiate(ResourceManager.instance.vaultUpgradeOperatorPrefab, vaultRule.inRoom_UpgradeOperactorParentTf));
            vaultRule.upgradeOperator = upgradeOper;
            upgradeOper.Set_TargetBuild(vault);
            upgradeOper.gameObject.transform.localPosition = Vector2.zero;
            upgradeOper.gameObject.SetActive(false);

            room.Offset(tempID);
            Set_NormalRelativeVec(room, connectedRoomAmount: 1, applySpecialExist: true);

            generated = true;
        }
    }

    #endregion

    #region Shop

    // 상점 방 하나 생성
    private void Gen_ShopRoom(GenSpecialRoomData shopRoomData, int tempID, out bool generated)
    {
        generated = false;
        GameProgressJsonData data = SaveDataManager.instance.jsonData.gameProgressData;

        if (!data.usableBU && !data.usableMU) return;

        if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomPrefabArr[shopRoomData.id], mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomRuleShopPrefabArr[shopRoomData.ruleId], room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            ShopRuleController shopRule = DevTool.Get_CastingTType<ShopRuleController>(roomRule);

            if (data.usableBU)
            {
                BaseUpgradeController BUShop = DevTool.Get_ComponentTType<BaseUpgradeController>(Instantiate(ResourceManager.instance.BUShopPrefab, shopRule.inRoom_buShopParentTf));
                shopRule.buShop = BUShop;
                BUShop.gameObject.transform.localPosition = Vector2.zero;
                BUShop.gameObject.SetActive(false);

                RepairOperatorController BURepairOper = DevTool.Get_ComponentTType<RepairOperatorController>(
                Instantiate(ResourceManager.instance.repairOperatorPrefab, shopRule.inRoom_buRepairOperactorParentTf));
                shopRule.buRepairOperator = BURepairOper;
                BURepairOper.Set_TargetBuild(BUShop);
                BURepairOper.gameObject.transform.localPosition = Vector2.zero;
                BURepairOper.gameObject.SetActive(false);
            }
            
            if (data.usableMU)
            {
                ModuleUpgradeController MUShop = DevTool.Get_ComponentTType<ModuleUpgradeController>(Instantiate(ResourceManager.instance.MUShopPrefab, shopRule.inRoom_muShopParentTf));
                shopRule.muShop = MUShop;
                MUShop.gameObject.transform.localPosition = Vector2.zero;
                MUShop.gameObject.SetActive(false);

                RepairOperatorController MURepairOper = DevTool.Get_ComponentTType<RepairOperatorController>(
                    Instantiate(ResourceManager.instance.repairOperatorPrefab, shopRule.inRoom_muRepairOperactorParentTf));
                shopRule.muRepairOperator = MURepairOper;
                MURepairOper.Set_TargetBuild(MUShop);
                MURepairOper.gameObject.transform.localPosition = Vector2.zero;
                MURepairOper.gameObject.SetActive(false);
            }
           
            room.Offset(tempID);
            Set_NormalRelativeVec(room, connectedRoomAmount: 1, applySpecialExist: true);

            generated = true;
        }
    }

    #endregion

    #region Ally Shop

    // 상점 방 하나 생성
    private void Gen_AllyShopRoom(GenSpecialRoomData shopRoomData, int tempID, out bool generated)
    {
        generated = false;
        GameProgressJsonData data = SaveDataManager.instance.jsonData.gameProgressData;

        if (!data.usableABU && !data.usableAMU) return;

        if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomPrefabArr[shopRoomData.id], mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomRuleAllyShopPrefabArr[shopRoomData.ruleId], room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            AllyShopRuleController shopRule = DevTool.Get_CastingTType<AllyShopRuleController>(roomRule);

            if (data.usableABU)
            {
                AllyBaseUpgradeController ABUShop = DevTool.Get_ComponentTType<AllyBaseUpgradeController>(Instantiate(ResourceManager.instance.ABUShopPrefab, shopRule.inRoom_buShopParentTf));
                shopRule.buShop = ABUShop;
                ABUShop.gameObject.transform.localPosition = Vector2.zero;
                ABUShop.gameObject.SetActive(false);

                RepairOperatorController BURepairOper = DevTool.Get_ComponentTType<RepairOperatorController>(
                    Instantiate(ResourceManager.instance.repairOperatorPrefab, shopRule.inRoom_buRepairOperactorParentTf));
                shopRule.buRepairOperator = BURepairOper;
                BURepairOper.Set_TargetBuild(ABUShop);
                BURepairOper.gameObject.transform.localPosition = Vector2.zero;
                BURepairOper.gameObject.SetActive(false);
            }

            if (data.usableAMU)
            {
                AllyModuleUpgradeController AMUShop = DevTool.Get_ComponentTType<AllyModuleUpgradeController>(Instantiate(ResourceManager.instance.AMUShopPrefab, shopRule.inRoom_muShopParentTf));
                shopRule.muShop = AMUShop;
                AMUShop.gameObject.transform.localPosition = Vector2.zero;
                AMUShop.gameObject.SetActive(false);

                RepairOperatorController MURepairOper = DevTool.Get_ComponentTType<RepairOperatorController>(
                    Instantiate(ResourceManager.instance.repairOperatorPrefab, shopRule.inRoom_muRepairOperactorParentTf));
                shopRule.muRepairOperator = MURepairOper;
                MURepairOper.Set_TargetBuild(AMUShop);
                MURepairOper.gameObject.transform.localPosition = Vector2.zero;
                MURepairOper.gameObject.SetActive(false);
            }

            room.Offset(tempID);
            Set_NormalRelativeVec(room, connectedRoomAmount: 1, applySpecialExist: true);

            generated = true;
        }
    }

    #endregion

    #region Prison

    // 감옥 방 하나 생성
    private void Gen_PrisonRoom(GenPrisonRoomData prisonRoomData, int tempID, out bool generated)
    {
        generated = false;
        GameProgressJsonData data = SaveDataManager.instance.jsonData.gameProgressData;

        switch (prisonRoomData.typeId)
        {
            case 0: if (!data.usableSTPrison) return; break;
            case 1: if (!data.usableUTPrison) return; break;
            case 2: if (!data.usableNTPrison) return; break;
        }

        if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomPrefabArr[prisonRoomData.id], mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.roomRulePrisonPrefabArr[prisonRoomData.ruleId], room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            PrisonRuleController prisonRule = DevTool.Get_CastingTType<PrisonRuleController>(roomRule);

            PrisonController prison = DevTool.Get_ComponentTType<PrisonController>(Instantiate(ResourceManager.instance.prisonPrefabArr[prisonRoomData.typeId], prisonRule.inRoom_PrisonParentTF));
            prisonRule.prison = prison;
            prison.gameObject.transform.localPosition = Vector2.zero;
            prison.gameObject.SetActive(false);

            PrisonPayOperatorController payOper = DevTool.Get_ComponentTType<PrisonPayOperatorController>(
               Instantiate(ResourceManager.instance.prisonPayOperatorPrefab, prisonRule.inRoom_PayOperactorParentTF));
            prisonRule.payOperator = payOper;
            payOper.Set_TargetBuild(prison);
            payOper.gameObject.SetActive(false);

            PrisonPuzzleOperatorController puzzleOper = DevTool.Get_ComponentTType<PrisonPuzzleOperatorController>(
               Instantiate(ResourceManager.instance.prisonPuzzleOperatorPrefab, prisonRule.inRoom_PuzzleOperactorParentTF));
            prisonRule.puzzleOperator = puzzleOper;
            puzzleOper.Set_TargetBuild(prison);
            puzzleOper.gameObject.SetActive(false);

            room.Offset(tempID);
            Set_NormalRelativeVec(room, connectedRoomAmount: 1, applySpecialExist: true);

            generated = true;
        }
    }

    #endregion

    #region Passage

    private void Gen_PassageRoom(int nextStageId)
    {
        if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.passageRoomPrefab, mapParentTF), out RoomController room))
        {
            currentAllRoomController.Add(room);

            if (DevTool.Get_ComponentTType(Instantiate(ResourceManager.instance.passageRulePrefab, room.gameObject.transform), out RoomRuleController roomRule))
                room.roomRule = roomRule;

            PassageRuleController passageRule = DevTool.Get_CastingTType<PassageRuleController>(roomRule);
            passageRule.Set_ElevatorData(nextStageId);

            room.Offset(0);
        }
    }

    #endregion

    #endregion

    #endregion

    #region Reset (StageData)

    private void Remove_CurrentStage()
    {
        isStartStage = true;

        currentSetSprites.Clear();
        currentSetAnims.Clear();

        currentStageData = null;

        int allRoomAmount = currentAllRoomController.Count;
        for (int i = allRoomAmount - 1; i >= 0; i--)
            Destroy(currentAllRoomController[i].gameObject);
        currentAllRoomController.Clear();

        int allEntranceRoomAmount = currentAllEntranceRoomController.Count;
        currentAllEntranceRoomController.Clear();

        currentRoomController = null;

        MainGameUIManager.instance.playerHud.minimapEui.Remove_AllMinimapCell();
    }

    private void Remove_PassageStage()
    {
        beforeStageID = -1;
        afterStageID = -1;

        beforeStageData = null;
        afterStageData = null;
    }

    #endregion

    #region Set State

    public void Play_CurrentRoom(RoomController targetRoom)
    {
        StartCoroutine(Play_CurrentRoom_Cor(targetRoom));
    }

    private IEnumerator Play_CurrentRoom_Cor(RoomController targetRoom)
    {
        if (targetRoom == null) yield break;

        // 필요없는 유닛 제거
        Remove_SetSprites();
        Remove_SetAnims();

        // 현재 방 선택
        currentRoomController = targetRoom;

        DevTool.Set_Active(currentAllRoomController, false);

        // Layer 초기화
        // LayerOrderManager.instance.ClearNeedSortObj();


        // 처음 엘베 레이어때문에 추가 하지않음
        if (!isStartStage)
        { 
            // PlayerManager.instance.playerController.AddSortingLayer();
        }
        
        currentRoomController.gameObject.SetActive(true);
        currentRoomController.Set_SortingStaticObjects();

        // Ally
        AllyManager.instance.Set_AllAllyPlayerNearPos();
        AllyManager.instance.Stop_AllAllies_Combat();

        // Minimap
        MainGameUIManager.instance.playerHud.minimapEui.Set_State();

        yield return new WaitForSeconds(0.2f);

        PlayerManager.instance.playerController.SetOn_Trail();
        targetRoom.Play_RoomState();
        Set_NavBake();

        //LayerOrderManager.instance.AddNeedSortObj(EnemyManager.instance.currentEnemyList); 
        //LayerOrderManager.instance.AddNeedSortObj(AllyManager.instance.allAlly);

        // Minimap
        MainGameUIManager.instance.playerHud.minimapEui.Set_State();
        MainGameUIManager.instance.playerHud.minimapEui.Play_Effect();

        // Ally
        AllyManager.instance.Start_AllAllies_Combat();
    }


    public void Play_CompleteKillAll()
    {
        if (currentRoomController == null || currentRoomController.roomRule.roomType == eRoomType.Completed) return;

        StartCoroutine(Play_CompleteKillAll_Cor());
    }

    // 모든 적을 처치했을 시, Complete로 바뀌는 부분
    public IEnumerator Play_CompleteKillAll_Cor()
    {
        yield return new WaitForSeconds(0.5f);

        if (EnemyManager.instance.currentEnemyList.Count <= 0)
        {
            currentRoomController.PlaySet_RoomStateComplete();

            // 상호작용 UI 변경 (문이나 아이템에 붙어있을 때, 상황을 바꾸어줌)
            MainGameUIManager.instance.playerHud.Set_InteractUI(); 
            MainGameUIManager.instance.interactAnnoUi.Set_UI();

            // Minimap
            MainGameUIManager.instance.playerHud.minimapEui.Set_State();
        }
    }

    #endregion

    #region Reset

    private void Reset_GenStageData()
    {
        shuffledRoomIndexList.Clear();

        alreadyExistList.Clear();
        alreadyExistSpeicalList.Clear();

        roundList.Clear();
    }

    #endregion

    #region Set

    #region Stage

    private void Set_StartUI(StageData stageData)
    {
        MainGameUIManager.instance.mapIntroUi.Play_IntroLabel();
        MainGameUIManager.instance.playerHud.minimapEui.Gen_Minimap();
        MainGameUIManager.instance.playerHud.stageIcon.gameObject.SetActive(true);
        MainGameUIManager.instance.playerHud.stageIcon.sprite = ResourceManager.instance.Get_StageIcon(targetStageID);
        MainGameUIManager.instance.playerHud.Set_StageDescription();
    }

    private void Set_StartPassageUI()
    {
        MainGameUIManager.instance.playerHud.minimapEui.Gen_Minimap();
        MainGameUIManager.instance.playerHud.stageIcon.gameObject.SetActive(false);
    }

    #endregion

    #region Room

    // 방 위치 세팅
    private void Set_RoomPos(RoomController room)
    {
        room.gameObject.transform.position = new Vector2(room.roomVec[0].x * offsetRoomSize.x, room.roomVec[0].y * offsetRoomSize.y);
    }

    #endregion

    #region Field Obj

    private void Set_FieldObjPos(RoomController room)
    {
        room.Spawn_FieldObj();
    }

    #endregion

    #region Gate

    // 게이트에 모든 짝꿍 게이트 지정과 세팅
    private void Set_ParterAllGate()
    {
        List<GateController> allGate = Get_AllGate(currentAllRoomController);
         
        for (int i = 0; i < allGate.Count - 1; i++)
        {
            // 이미 파트너 게이트가 있다면
            if (allGate[i].parterGate != null) continue;

            for (int j = i + 1; j < allGate.Count; j++)
            {
                if (Is_PartnerGate(allGate[i], allGate[j]))
                {
                    allGate[i].parterGate = allGate[j];
                    allGate[j].parterGate = allGate[i];
                }
            }
        }
    }


    // 현재 게이트 모두 활성화
    private void Set_GateActiveOn()
    {
        Set_ParterAllGate();
        List<GateController> allGate = Get_AllGate(currentAllRoomController);
        for (int i = 0; i < allGate.Count; i++)
        {
            if (allGate[i].parterGate != null)
            {
                // 게이트 활성화
                allGate[i].Set_ExistDoorState(true);

                // 게이트가 특정 방의 게이트라면 특정 필요 키카드 삽입
                int needKeyCardID = allGate[i].thisRoom.roomRule.Get_NeedKeyCardID();
                if (needKeyCardID != -1)
                {
                    allGate[i].Set_NeedKeyCard(needKeyCardID);
                    allGate[i].parterGate.Set_NeedKeyCard(needKeyCardID);
                }

                // Next Map Icon
                allGate[i].Set_NextMap();
            }
            else
            {
                // 게이트 비활성화
                allGate[i].Set_ExistDoorState(false);
            }
        }
    }

    #endregion

    #region Round

    // 해당 월드 좌표값을 Room에 적용
    private void Set_RelativeVec(RoomController room, List<Vector2Int> worldVecList)
    {
        // 벡터값을 넣어주고 (실제 좌표 값에 비례되는 값을 넣어줌 + Gate도)
        for (int i = 0; i < room.roomVec.Count; i++)
            room.Set_CollectGatePos(i, worldVecList[i]);
    }

    #endregion

    #region Relative

    // 월드 기준: 상대적인 좌표 직접 지정
    private void Set_NormalRelativeVec(RoomController room, List<Vector2Int> relativePos)
    {
        Set_RelativeVec(room, relativePos);

        Set_RoomPos(room);
        Set_FieldObjPos(room);
        Add_RoundVec(room.roomVec);
    }

    // 월드 기준: 상대적인 좌표 삽입
    private void Set_NormalRelativeVec(RoomController room, int connectedRoomAmount = -1, bool applySpecialExist = false)
    {
        Set_RelativeVec(room, Get_FindCorrectWorldVec_Normal(room, connectedRoomAmount, applySpecialExist));

        Set_RoomPos(room);
        Set_FieldObjPos(room);
        Add_RoundVec(room.roomVec);
    }

    // 월드 기준: 상대적인 좌표 삽입: 가장 멀고, 특별 Round 포함
    private void Set_FurthestRelativeVec(RoomController room, int connectedRoomAmount = -1, bool applySpecialExist = true)
    {
        Set_RelativeVec(room, Get_FindCorrectWorldVec_Furthest(room, connectedRoomAmount, applySpecialExist));

        Set_RoomPos(room);
        Set_FieldObjPos(room);
        Add_RoundVec(room.roomVec);

        Add_RoundSpecialVec(room.roomVec);
    }

    #endregion

    #region SR

    public void Set_PassageMiddleSprite(SpriteRenderer sr, string key)
    {
        Sprite data = passageMiddleSpriteData.Get_CorrectSprite(key, out int materialIndex);
        if (data == null) return;

        sr.sprite = data;
        sr.material = ResourceManager.instance.Get_PassageMiddleMaterial(materialIndex);
    }

    public void Set_CurrentMapSprite(SpriteRenderer sr, string spriteKey)
    {
        Set_MapUnclearSprite(currentStageData, sr, spriteKey);
    }

    public void Set_BeforeMapSprite(SpriteRenderer sr, string spriteKey)
    {
        Set_MapClearSprite(beforeStageData, sr, spriteKey);
    }

    public void Set_AfterMapSprite(SpriteRenderer sr, string spriteKey)
    {
        Set_MapClearSprite(afterStageData, sr, spriteKey);
    }

    public void Set_SetSpriteClearly()
    {
        if (currentSetSprites == null || currentSetSprites.Count <= 0) return;

        foreach(BuildSetSpriteController setSprite in currentSetSprites)
        {
            if (DevTool.Get_ComponentTType(setSprite.gameObject, out SpriteRenderer sr))
            {
                int index = currentStageData.mapMaterialUnclear.IndexOf(sr.sharedMaterial);
                if (index == -1)
                { UnityEngine.Debug.Log(sr.gameObject.name + " / " + sr.gameObject.transform.parent.gameObject.name); continue; }
                sr.material = currentStageData.mapMaterialClear[index];
            }
        }
    }

    private void Set_MapUnclearSprite(StageData stageData, SpriteRenderer sr, string spriteKey)
    {
        if (!stageData.mapSpriteReso.mapSprite.ContainsKey(spriteKey)) { UnityEngine.Debug.Log(spriteKey); return; }

        SpriteMaterial spriteMatrial = stageData.mapSpriteReso.mapSprite[spriteKey];
        sr.sprite = spriteMatrial.sprite;
        sr.material = stageData.mapMaterialUnclear[spriteMatrial.materialIndex];
    }

    private void Set_MapClearSprite(StageData stageData, SpriteRenderer sr, string spriteKey)
    {
        if (!stageData.mapSpriteReso.mapSprite.ContainsKey(spriteKey)) { UnityEngine.Debug.Log(spriteKey); return; }

        SpriteMaterial spriteMatrial = stageData.mapSpriteReso.mapSprite[spriteKey];
        sr.sprite = spriteMatrial.sprite;
        sr.material = stageData.mapMaterialClear[spriteMatrial.materialIndex];
    }

    #endregion

    #region Anim

    public void Set_StageDoorAnim(GateController gate, SpriteRenderer sr, Vector2Int doorDir)
    {
        List<StageDoorAnim> doorAnim = currentStageData.mapDoorAnim;
        for (int i = 0; i < doorAnim.Count; i++)
            if (doorAnim[i].dir == doorDir)
            {
                gate.ac = doorAnim[i].doorAnim;
                sr.material = currentStageData.mapMaterialUnclear[doorAnim[i].materialIndex];
            }
    }

    public void Set_SetAnimClearly()
    {
        if (currentSetAnims == null || currentSetAnims.Count <= 0) return;

        foreach (BuildSetAnimController setAnim in currentSetAnims)
        {
            if (DevTool.Get_ComponentTType(setAnim.gameObject, out SpriteRenderer sr))
            {
                int index = currentStageData.mapMaterialUnclear.IndexOf(sr.sharedMaterial);
                if (index == -1)
                { UnityEngine.Debug.Log(sr.material.name + " / " + sr.gameObject.transform.parent.gameObject.name); continue; }
                sr.material = currentStageData.mapMaterialClear[index];
            }
        }
    }

    #endregion

    #endregion

    #region Get

    #region Stage

    // 올바른 Stage 데이터 구하기
    public StageData Get_CollectStageData(int stageId)
    {
        if (stageId == 99)
            return lobbyStageData;

        for (int i = 0; i < allStageData.Count; i++)
            if (allStageData[i].infoData.stageId == stageId)
                return allStageData[i];

        return null;
    }

    public StageData Get_CurrentStageData()
    {
        return Get_CollectStageData(targetStageID);
    }

    #endregion

    #region Room

    // Room Amount List를 List<int>형인 기본 리스트로 변경
    private List<int> Get_ListInt_FromGenRoomAmount(List<GenRoomData> genRoomAmountList)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < genRoomAmountList.Count; i++)
            for (int j = 0; j < genRoomAmountList[i].amount; j++)
                result.Add(genRoomAmountList[i].id);
            
        return result;
    }

    // 현재 모든 방 구하기
    public List<RoomController> Get_AllRoom()
    {
        return currentAllRoomController;
    }

    // (ID가 지정된 후) 알맞는 Room 찾기
    public RoomController Get_CorrectRoom(int id)
    {
        return IDController.Get_CorrectIDObject<RoomController>(id, new List<IDController>(currentAllRoomController));
    }

    #endregion

    #region RoomRule

    // Room 인덱스에 맞는 모든 RoomRule 가져오기
    private List<RoomRuleController> Get_CorrectRoomRuleList(RoomController room)
    {
        List<Vector2Int> roomIndex = room.roomVec;
        List<RoomRuleController> result = new List<RoomRuleController>();
        for (int i = 0; i < ResourceManager.instance.roomRulePrefabArr.Length; i++)
            if (ResourceManager.instance.roomRulePrefabArr[i].TryGetComponent(out RoomRuleController rrc) && rrc.roomVec.SequenceEqual(roomIndex))
                result.Add(rrc);
            
        return result;
    }

    // 인덱스가 같은 RoomRule 찾기 (마지막엔 랜덤)
    private RoomRuleController Get_CorrectRandomRoomRule(RoomController room)
    {
        List<RoomRuleController> roomRuleList = Get_CorrectRoomRuleList(room);
        return roomRuleList[UnityEngine.Random.Range(0, roomRuleList.Count)];
    }

    #endregion

    #region Gate 

    private List<GateController> Get_AllGate(List<RoomController> roomList)
    {
        List<GateController> allGate = new List<GateController>();
        for (int i = 0; i < roomList.Count; i++)
            allGate.AddRange(roomList[i].inRoom_AllGate);

        return allGate;
    }

    #endregion

    #region Round

    // 현재 Round에서, 타겟 Room에 맞춘 위치 값 반환
    private List<Vector2Int> Get_FindCorrectWorldVec_Normal(RoomController targetRoom, int connectedRoomAmount = -1, bool applySpecialExist = false)
    {
        List<Vector2Int> randomRoundList = DevTool.Get_ShuffledList(roundList);

        List<Vector2Int> existList = new List<Vector2Int>(alreadyExistList);
        if (applySpecialExist) existList.AddRange(alreadyExistSpeicalList);

        return Get_FindCorrectWorldVec(
            targetRoom.roomVec, roundList, existList, connectedRoomAmount);
    }

    // 현재 Round에서, 타겟 Room에 맞춘 위치 값 + 가장 먼 위치 값 반환
    private List<Vector2Int> Get_FindCorrectWorldVec_Furthest(RoomController targetRoom, int connectedRoomAmount = -1, bool applySpecialExist = false)
    {
        List<Vector2Int> farRoundList = roundList.OrderByDescending(obj => Vector2Int.Distance(Vector2Int.zero, obj)).ToList();

        List<Vector2Int> existList = new List<Vector2Int>(alreadyExistList);
        if (applySpecialExist) existList.AddRange(alreadyExistSpeicalList);

        return Get_FindCorrectWorldVec(
            targetRoom.roomVec, farRoundList, existList, connectedRoomAmount);
    }

    // 계산
    private List<Vector2Int> Get_FindCorrectWorldVec(List<Vector2Int> roomVec, List<Vector2Int> roundList, List<Vector2Int> existList, int connectedRoomAmount = -1)
    {
        List<Vector2Int> WorldVecList = new List<Vector2Int>();

        // 찾을 때까지 실행

        int randomIndex = 0;

        while (true)
        {
            WorldVecList = new List<Vector2Int>();
            bool wrongPlace = false;

            // 주변 공간에 배치할 시 배치 할 수 있는지에 대한
            for (int i = 0; i < roomVec.Count; i++)
            {
                if (roundList.Count <= randomIndex || roomVec.Count <= i)
                {
                    break;
                }

                WorldVecList.Add(roundList[randomIndex] + roomVec[i]);

                if (existList.Contains(WorldVecList[i]))
                {
                    wrongPlace = true;
                    break;
                }
            }

            if (connectedRoomAmount != -1 && Get_AdjacentRoomAmount(WorldVecList, existList) != connectedRoomAmount)
                wrongPlace = true;

            randomIndex++;

            if (randomIndex > 100)
            {
                UnityEngine.Debug.Assert(false, "생성에 문제!");
            }

            // 안된다면 다시 시작
            if (!wrongPlace) break;
        }

        return WorldVecList;
    }



    private int Get_AdjacentRoomAmount(List<Vector2Int> targetRoomVec, List<Vector2Int> existRoomVec)
    {
        List<Vector2Int> targetRoomVecRound = DevTool.Get_RoundVec(targetRoomVec);

        return DevTool.Get_IntersectionAmount(targetRoomVecRound, existRoomVec);
    }

    #endregion

    #region Minimap

    public CoupleData<Sprite> Get_CorrectMinimapIcon(RoomRuleController roomRule)
    {
        switch(roomRule)
        {
            case VaultRuleController:
                return ResourceManager.instance.vault_Icon;

            case EntranceRuleController:
                return ResourceManager.instance.elevator_Icon;

            case ShopRuleController:
                return ResourceManager.instance.shop_Icon;

            case AllyShopRuleController:
                return ResourceManager.instance.allyShop_Icon;

            case PrisonRuleController prisonRule:
                if (prisonRule.prison is StrikeTeamPrisonController)
                    return ResourceManager.instance.st_Prison_Icon;
                else if (prisonRule.prison is UplinkTeamPrisonController)
                    return ResourceManager.instance.ut_Prison_Icon;
                else if (prisonRule.prison is NeoTeamPrisonController)
                    return ResourceManager.instance.nt_Prison_Icon;
                else
                    return null;

            default:
                return null;
        }

    }

    #endregion

    #region Vault

    public GameObject Get_VaultCorrectType(Type typeVault) 
    {
        for (int i = 0; i < ResourceManager.instance.vaultPrefabArr.Length; i++)
            if (DevTool.Get_ComponentTType<VaultController>(ResourceManager.instance.vaultPrefabArr[i]).GetType() == typeVault)
                return ResourceManager.instance.vaultPrefabArr[i];

        return null;
    }

    #endregion

    #endregion

    #region Add

    #region Round

    // 존재하는 방의 좌표와 Round 좌표를 초기화
    private void Add_RoundVec(List<Vector2Int> addVecList)
    {
        foreach (var vec in addVecList)
            alreadyExistList.Add(vec);

        roundList = DevTool.Get_RoundVec(alreadyExistList).ToList();
    }

    // 특수 방의 좌표를 넣어줌
    private void Add_RoundSpecialVec(List<Vector2Int> addVecList)
    {
        foreach (var vec in addVecList)
        {
            alreadyExistSpeicalList.Add(vec);
            List<Vector2Int> eachRound = DevTool.Get_RoundVec(vec);
            for (int i = 0; i < eachRound.Count; i++)
                alreadyExistSpeicalList.Add(eachRound[i]);
        }

    }

    #endregion

    #region SR

    public void Add_SetSprite(BuildSetSpriteController setSprite)
    {
        currentSetSprites.Add(setSprite);
    }

    #endregion

    #region Anim

    public void Add_SetAnim(BuildSetAnimController setAnim)
    {
        currentSetAnims.Add(setAnim);
    }

    #endregion

    #endregion

    #region Remove

    #region SR

    private void Remove_SetSprites()
    {
        currentSetSprites.Clear();
    }

    #endregion

    #region Anim

    private void Remove_SetAnims()
    {
        currentSetAnims.Clear();
    }

    #endregion

    #endregion

    #region Is

    #region Gate 

    private bool Is_PartnerGate(GateController gate1, GateController gate2)
    {
        return ((gate1.roomPosGate + gate1.gateDir) == gate2.roomPosGate) && // 반대편에 방에 존재하는 게이트인지
            (gate1.gateDir * -1) == gate2.gateDir; // 서로 바라보고 있는지
    }

    #endregion

    #endregion

    #region Play (Lobby)

    private IEnumerator Play_LobbyStart_Cor(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        PlayerManager.instance.playerController.Set_StartStage();
        EventManager.instance.Set_Input(true);
    }

    #endregion

    #region Play (Spawn another Stage)

    public void Play_GenStage(int stageId)
    {
        targetStageID = stageId;
        StartCoroutine(Play_GenStage_Cor());
    }

    private IEnumerator Play_GenStage_Cor()
    {
        yield return new WaitForSeconds(1f);

        Gen_Stage(targetStageID);
    }

    #endregion

    #region Play (Spawn another Passage Stage)

    public void Play_GenPassageStage(int afterStageID)
    {
        beforeStageID = targetStageID;
        this.afterStageID = afterStageID;

        beforeStageData = Get_CollectStageData(beforeStageID);
        afterStageData = Get_CollectStageData(this.afterStageID);

        StartCoroutine(Play_GenPassageStage_Cor());
    }

    private IEnumerator Play_GenPassageStage_Cor()
    {
        yield return new WaitForSeconds(1f);

        Gen_PassageStage(afterStageID);
    }

    public int Get_BeforeStageID()
    {
        return beforeStageID;
    }

    public int Get_AfterStageID()
    {
        return afterStageID;
    }

    #endregion

    #region Play (Boss Gate)
    
    public void Play_GoInBossRoom(GateController gate, EliteEnemyController enemy)
    {
        MainGameUIManager.instance.battleProdUi.Play_BattleOnProd(
            PlayerManager.instance.playerController, enemy, out float durTime);

        StartCoroutine(Play_GoInBattleRoom_Cor(gate, durTime));
    }

    public void Play_GoInBossRoom(GateController gate, BossEnemyController enemy)
    {
        MainGameUIManager.instance.battleProdUi.Play_BattleOnProd(
            PlayerManager.instance.playerController, enemy, out float durTime);

        StartCoroutine(Play_GoInBattleRoom_Cor(gate, durTime));
    }

    private IEnumerator Play_GoInBattleRoom_Cor(GateController gate, float durTime)
    {
        EventManager.instance.Set_Input(false);

        yield return new WaitForSeconds(durTime);

        gate.EnterGate();
        MainGameUIManager.instance.battleProdUi.Play_BattleOffProd(out float outDurTime);

        yield return new WaitForSeconds(outDurTime);

        EventManager.instance.Set_Input(true);
    }

    #endregion

    #region Nav

    public void Set_NavBake()
    {
        thisNav.BuildNavMesh();
    }

    #endregion

    #region Init

    // => ResoucreManager에서 리소스를 가져오고 난 다음, 호출문
    public void Init_PassageMiddleData()
    {
        passageMiddleSpriteData = new AllPassageMiddleSpriteData(ResourceManager.instance.Get_PassageMapReso());
    }

    #endregion
}