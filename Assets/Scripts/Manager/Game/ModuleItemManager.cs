using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ModuleItemManager : Singleton<ModuleItemManager>, IMainGameInitializer
{
    #region Value

    // Init
    public int InitOrder { get { return initOrder; } }
    [SerializeField] private int initOrder;
    public string InitPregressText { get { return initPregressText; } }
    [SerializeField] private string initPregressText;

    #region - Inspector

    #endregion

    #region - Hide

    // Data
    [HideInInspector] public ItemData[] itemDataArr;
    [HideInInspector] private MainChipData[] mainChipDataArr;

    // Module State
    [HideInInspector] public static readonly int rowAmount = 5;
    [HideInInspector] public static readonly int columnAmount = 20;
    [HideInInspector] private ModuleState[][] allModuleData;

    [HideInInspector] public static readonly int equipedAmount = 6;
    [HideInInspector] private CoupleData<int>[] equippedIndex;

    [HideInInspector] private CoupleData<int> decompositionIndex = new CoupleData<int>(-1, -1);

    [HideInInspector] public static readonly int fusionAmount = 2;
    [HideInInspector] private CoupleData<int>[] fusionIndex;

    // Main Chip
    [HideInInspector] private Dictionary<int, int> mainChipAmalgamationDict = new Dictionary<int, int>();
    [HideInInspector] private List<SynchoronyState> currentAllMainChipState = new List<SynchoronyState>();

    [HideInInspector] public static int synchoronyOneTierRange = 3;
    [HideInInspector] public static int synchoronyMaxLv = 3;

    #endregion

    #region - Interface

    // Module Base
    private List<IWhen_Hit> iWhen_HitList = new List<IWhen_Hit>();
    private List<IWhen_CriticalHit> iWhen_CriticalHitList = new List<IWhen_CriticalHit>();
    private List<IWhen_Fire> iWhen_FireList = new List<IWhen_Fire>();

    // Sync
    private List<IWhenSync_Start> iWhenSync_StartList = new List<IWhenSync_Start>();

    private List<IWhenSync_Fire> iWhenSync_FireList = new List<IWhenSync_Fire>();
    private List<IWhenSync_AfterFire> iWhenSync_AfterFireList = new List<IWhenSync_AfterFire>();

    private List<IWhenSync_Hit> iWhenSync_HitList = new List<IWhenSync_Hit>();
    private List<IWhenSync_CriticalHit> iWhenSync_CriticalHitList = new List<IWhenSync_CriticalHit>();

    private List<IWhenSync_GetFire> iWhenSync_GetFireList = new List<IWhenSync_GetFire>();
    private List<IWhenSync_GetCold> iWhenSync_GetColdList = new List<IWhenSync_GetCold>();
    private List<IWhenSync_GetElectricity> iWhenSync_GetElectricityList = new List<IWhenSync_GetElectricity>();
    private List<IWhenSync_GetCorrosion> iWhenSync_GetCorrosionList = new List<IWhenSync_GetCorrosion>();


    #endregion

    #endregion

    #region Init

    public IEnumerator Initialize()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        itemDataArr = ResourceManager.instance.Get_ItemDataArr();
        mainChipDataArr = ResourceManager.instance.Get_MainChipDataArr();

        // Inven
        allModuleData = new ModuleState[columnAmount][];
        for (int i = 0; i < allModuleData.Length; i++)
            allModuleData[i] = new ModuleState[rowAmount];

        // Equip
        equippedIndex = new CoupleData<int>[equipedAmount];
        for (int i = 0; i < equipedAmount; i++)
            equippedIndex[i] = new CoupleData<int>(-1, -1);

        // Fuison
        fusionIndex = new CoupleData<int>[fusionAmount];
        for (int i = 0; i < fusionAmount; i++)
            fusionIndex[i] = new CoupleData<int>(-1, -1);

        sw.Stop();
        UnityEngine.Debug.Log($"ModuleUpgradeManager: <color=orange>DataInit</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
        yield return null;
    }

    #endregion

    #region Reset

    private void Reset_Interface()
    {
        BuffManager.instance.Init_SyncSetBuff();

        Reset_InterfaceMU();
        Reset_InterfaceMC();

        ActiveSync_Start();
    }


    private void Clear_InterfaceMU()
    {
        iWhen_HitList.Clear();
        iWhen_FireList.Clear();
        iWhen_CriticalHitList.Clear();
    }

    private void Clear_InterfaceMC()
    {
        currentAllMainChipState.Clear();

        iWhenSync_StartList.Clear();

        iWhenSync_FireList.Clear();
        iWhenSync_AfterFireList.Clear();

        iWhenSync_HitList.Clear();
        iWhenSync_CriticalHitList.Clear();

        iWhenSync_GetFireList.Clear();
        iWhenSync_GetColdList.Clear();
        iWhenSync_GetElectricityList.Clear();
        iWhenSync_GetCorrosionList.Clear();
    }


    private void Reset_InterfaceMU()
    {
        Clear_InterfaceMU();

        for (int i = 0; i < equippedIndex.Length; i++)
        {
            CoupleData<int> colRow = equippedIndex[i];
            if (colRow.typeBase == -1 || colRow.typeSpecial == -1)
                continue; 
            else
                Try_AddIWhen(Get_EquippedModuleState(i));
        }
    }

    private void Try_AddIWhen(ModuleState moduleState)
    {
        if (moduleState is IWhen_Hit iHit) DevTool.Add_InList(iWhen_HitList, iHit);
        else if (moduleState is IWhen_Fire iFire) DevTool.Add_InList(iWhen_FireList, iFire);
        else if (moduleState is IWhen_CriticalHit iCriticalHit) DevTool.Add_InList(iWhen_CriticalHitList, iCriticalHit);
    }


    private void Reset_InterfaceMC()
    {
        Clear_InterfaceMC();

        Set_MainChipData();

        for (int i = 0; i < currentAllMainChipState.Count; i++)
            Try_AddIWhenSync(currentAllMainChipState[i]);
    }

    private void Try_AddIWhenSync(SynchoronyState synchoronyState)
    {
        if (synchoronyState is IWhenSync_Start iStart) DevTool.Add_InList(iWhenSync_StartList, iStart);

        else if (synchoronyState is IWhenSync_Fire iFire) DevTool.Add_InList(iWhenSync_FireList, iFire);
        else if (synchoronyState is IWhenSync_AfterFire iAfterFire) DevTool.Add_InList(iWhenSync_AfterFireList, iAfterFire);

        else if (synchoronyState is IWhenSync_Hit iHit) DevTool.Add_InList(iWhenSync_HitList, iHit);
        else if (synchoronyState is IWhenSync_CriticalHit iCriticalHit) DevTool.Add_InList(iWhenSync_CriticalHitList, iCriticalHit);

        else if (synchoronyState is IWhenSync_GetFire iGetFire) DevTool.Add_InList(iWhenSync_GetFireList, iGetFire);
        else if (synchoronyState is IWhenSync_GetCold iGetCold) DevTool.Add_InList(iWhenSync_GetColdList, iGetCold);
        else if (synchoronyState is IWhenSync_GetElectricity iGetElectricity) DevTool.Add_InList(iWhenSync_GetElectricityList, iGetElectricity);
        else if (synchoronyState is IWhenSync_GetCorrosion iGetCorrosion) DevTool.Add_InList(iWhenSync_GetCorrosionList, iGetCorrosion);
    }


    #endregion

    #region Get

    #region ItemData

    // 랜덤한 아이템
    public ItemData_Field Get_RandomInteractItem()
    {
        var data = new ItemData_Field(itemDataArr[Random.Range(0, itemDataArr.Length)]);
        //Debug.Log("Module Rank Test");
        //data.Rank = 5;
        return data;
    }

    #endregion

    #region Module

    // 선택한 아이템의 모듈 스탯 찾기
    public ModuleState Get_ModuleState(int col, int row)
    {
        return allModuleData[col][row];
    }

    public ModuleState Get_ModuleState(CoupleData<int> index)
    {
        return allModuleData[index.typeBase][index.typeSpecial];
    }

    public ModuleState Get_ModuleState(InventoryItemEUIController itemEUI)
    {
        return allModuleData[itemEUI.slot.col][itemEUI.slot.row];
    }

    // 모든 아이템의 Vector값(PlayerModuleUI기준) 리스트로 가져오기
    public List<CopyModuleState> Get_ExistModuleState(List<CopyModuleState> excludeModuleState)
    {
        List<CopyModuleState> result = new List<CopyModuleState>();
        List<ModuleState> excludeMsList = new List<ModuleState>();

        for (int i = 0; i < excludeModuleState.Count; i++)
            excludeMsList.Add(excludeModuleState[i].state);

        for (int i = 0; i < allModuleData.Length; i++)
        {
            for (int j = 0; j < allModuleData[i].Length; j++)
            {
                ModuleState ms = allModuleData[i][j];
                if (ms != null && !excludeMsList.Contains(ms))
                {
                    CopyModuleState copyMs = new CopyModuleState(allModuleData[i][j], new CoupleData<int>(i, j), false);
                    result.Add(copyMs);
                }
            }
        }
                
        return result;
    }


    #endregion

    #region Equipped

    // 비어있는 장착 ModuleState 찾기
    public CoupleData<int> Get_EmptyModuleState()
    {
        for (int i = 0; i < columnAmount; i++)
        {
            for (int j = 0; j < rowAmount; j++)
            {
                if (allModuleData[i][j] == null)
                {
                    return new CoupleData<int>(i, j);
                }
            }
        }

        return new CoupleData<int>(-1, -1);
    }

    // 비어 있는 장착 슬롯 인덱스 찾기
    public int Get_EmptyEquippedIndex(CoupleData<int> exclude)
    {
        for (int i = 0; i < equippedIndex.Length; i++)
        {
            if (equippedIndex[i].typeBase == exclude.typeBase &&
                equippedIndex[i].typeSpecial == exclude.typeSpecial)
                return -1;

            if (equippedIndex[i].typeBase == -1 && equippedIndex[i].typeSpecial == -1)
                return i;
        }
        return -1;
    }


    // 모든 장착 모듈 스탯 (빈 공간도 포함)
    public ModuleState Get_EquippedModuleState(int index)
    {
        CoupleData<int> colRow = equippedIndex[index];
        return Get_ModuleState(colRow.typeBase, colRow.typeSpecial);
    }

    // 장착되어 있는 아이템의 인덱스들만
    public CoupleData<int>[] Get_OnlyEquippedIndex()
    {
        List<CoupleData<int>> result = new List<CoupleData<int>>();
        for (int i = 0; i < equippedIndex.Length; i++)
            if (equippedIndex[i].typeBase != -1 && equippedIndex[i].typeSpecial != -1)
                result.Add(equippedIndex[i]);

        return result.ToArray();
    }

    // 장착되어 있는 아이템의 모듈
    public List<CopyModuleState> Get_EquippedModuleState()
    {
        List<CopyModuleState> result = new List<CopyModuleState>();

        for (int i = 0; i < equippedIndex.Length; i++)
        {
            if (equippedIndex[i].typeBase != -1 && equippedIndex[i].typeSpecial != -1)
            {
                result.Add(
                    new CopyModuleState(
                        Get_ModuleState(equippedIndex[i].typeBase, equippedIndex[i].typeSpecial),
                        new CoupleData<int>(equippedIndex[i].typeBase, equippedIndex[i].typeSpecial), 
                        true));
            }
        }

        return result;
    }


    #endregion

    #region MainChip

    // 메인칩 데이터 찾기
    public MainChipData Get_CorrectMainChip(int id)
    {
        return mainChipDataArr[id];
    }

    // 현재 메인 칩의 양 구하기
    public int Get_SynchronyAmount(int id)
    {
        return mainChipAmalgamationDict[id];
    }

    // 메인 칩의 랭크 가져오기
    // 1~3 / 4~6 / 7~9
    public int Get_SynchronyRank(int amalgamation)
    {
        return Mathf.Min((amalgamation - 1) > 0 ? amalgamation / synchoronyOneTierRange : 0, synchoronyMaxLv);
    }

    // 모든 메인 칩 딕셔너리로 메인칩스탯 리스트 반환
    private List<SynchoronyState> Get_CurrentMainChipState()
    {
        List<SynchoronyState> result = new List<SynchoronyState>();

        foreach(KeyValuePair<int, int> mainChipAmalgamation in mainChipAmalgamationDict) // ID, Amount
        {
            int id = mainChipAmalgamation.Key;
            int rank = Get_SynchronyRank(mainChipAmalgamation.Value);
            if (rank <= 0) continue;

            SynchoronyState mcs = SynchoronyState.Get_AllSynchoronyState()[id];
            mcs.Set_State(id, rank);

            result.Add(mcs);
        }

        return result;
    }

    // 현재 모든 Sync int, int 딕셔너리로 반환
    public Dictionary<int, int> Get_CurrentSyncData()
    {
        Dictionary<int, int> result = new Dictionary<int, int>();

        foreach (SynchoronyState stateData in currentAllMainChipState)
        {
            result.Add(stateData.ID, stateData.SynergyRank);
        }

        return result;
    }

    #endregion

    #region Sprite

    public Sprite Get_CorrectItemIcon(int id)
    {
        return itemDataArr[id].itemIcon;
    }

    #endregion

    #region Forge

    // 분해 슬롯의 인덱스
    public CoupleData<int> Get_DecompositionIndex()
    {
        return new CoupleData<int>(decompositionIndex);
    }

    // 합성 슬롯 인덱스
    public List<CoupleData<int>> Get_FusionIndex()
    {
        List<CoupleData<int>> result = new List<CoupleData<int>>();
        for (int i = 0; i < fusionIndex.Length; i++)
            result.Add(fusionIndex[i]);

        return result;
    }

    #endregion

    #region Item

    public static int Get_MS_ByDecomposition(ModuleState moduleState)
    {
        return (moduleState.thisItemData.rank * 2);
    }

    public static int Get_BC_ByDescomposition(ModuleState moduleState)
    {
        return moduleState.thisItemData.rank;
    }

    public static int Get_MS_ForFusion(ModuleState moduleState)
    {
        return (moduleState.thisItemData.rank + 1);
    }

    public static int Get_MS_ForMake()
    {
        return 7;
    }

    public static int Get_CB_ForMake()
    {
        return 4;
    }

    #endregion

    #endregion

    #region Set

    #region Inventory

    public void Set_ChangeInventorySlot(CoupleData<int> index0, CoupleData<int> index1)
    {
        ModuleState moduleState0 = allModuleData[index0.typeBase][index0.typeSpecial];
        ModuleState moduleState1 = allModuleData[index1.typeBase][index1.typeSpecial];

        // 기본 벨류
        int listIndex0 = -1;
        int listIndex1 = -1;
        CoupleData<int> newIndex0 = new CoupleData<int>(-1, -1);
        CoupleData<int> newIndex1 = new CoupleData<int>(-1, -1);

        // 모듈 스탯이 실존하고, 장착 중인 것이라면
        if (moduleState0 != null && Is_IncludeEquipped(index0, out int _ListIndex0))
        {
            listIndex0 = _ListIndex0;
            newIndex0 = new CoupleData<int>(index1);
        }
        if (moduleState1 != null && Is_IncludeEquipped(index1, out int _ListIndex1))
        {
            listIndex1 = _ListIndex1;
            newIndex1 = new CoupleData<int>(index0);
        }

        if (listIndex0 != -1) equippedIndex[listIndex0] = newIndex0;
        if (listIndex1 != -1) equippedIndex[listIndex1] = newIndex1;

        allModuleData[index0.typeBase][index0.typeSpecial] = moduleState1;
        allModuleData[index1.typeBase][index1.typeSpecial] = moduleState0;

        MainGameUIManager.instance.muUi.Set_InventoryUI(allModuleData);
        MainGameUIManager.instance.muUi.Set_EquipedUI(allModuleData, equippedIndex);
        Reset_Interface();
    }

    // 장착되어 있는 아이템의 위치값이 변경됨

    #endregion

    #region Equip

    // 아이템 장착
    public void Set_Equip(int equipedIndex, CoupleData<int> interactIndex)
    {
        equippedIndex[equipedIndex] = interactIndex;

        MainGameUIManager.instance.muUi.Set_EquipedUI(allModuleData, equippedIndex);
        
        Reset_Interface();
        AllyManager.instance.Set_AllAlliesSync();
    }

    public void Set_UnEquip(int equipedIndex)
    {
        equippedIndex[equipedIndex] = new CoupleData<int>(-1, -1);

        MainGameUIManager.instance.muUi.Set_EquipedUI(allModuleData, equippedIndex);

        Reset_Interface(); 
        AllyManager.instance.Set_AllAlliesSync();
    }

    public void Set_SwitchEquipment(int listIndex0, int listIndex1)
    {
        CoupleData<int> temp = new CoupleData<int>(equippedIndex[listIndex0]);
        equippedIndex[listIndex0] = new CoupleData<int>(equippedIndex[listIndex1]);
        equippedIndex[listIndex1] = new CoupleData<int>(temp);

        MainGameUIManager.instance.muUi.Set_InventoryUI(allModuleData);
        MainGameUIManager.instance.muUi.Set_EquipedUI(allModuleData, equippedIndex);
        Reset_Interface();
    }

    #endregion

    #region Decomposition

    // 분해 슬롯 장착
    public void Set_DecompositionSlot(CoupleData<int> interactIndex)
    {
        decompositionIndex = interactIndex;
    }

    public void Set_UnDecompositionSlot()
    {
        decompositionIndex = new CoupleData<int>(-1, -1);
    }

    #endregion

    #region Fusion

    // 합성
    public void Set_FusionSlot(int index, CoupleData<int> interactIndex)
    {
        fusionIndex[index] = interactIndex;
        MainGameUIManager.instance.muUi.Set_FusionUI(allModuleData, fusionIndex);
    }

    public void Set_UnFusionSlot(int index)
    {
        fusionIndex[index] = new CoupleData<int>(-1, -1);
        MainGameUIManager.instance.muUi.Set_FusionUI(allModuleData, fusionIndex);
    }
    public void Set_UnFusionSlotAll()
    {
        for (int i = 0; i < fusionIndex.Length; i++)
            fusionIndex[i] = new CoupleData<int>(-1, -1);
    }
    public void Set_SwitchFusion(int listIndex0, int listIndex1)
    {
        CoupleData<int> temp = new CoupleData<int>(fusionIndex[listIndex0]);
        fusionIndex[listIndex0] = new CoupleData<int>(fusionIndex[listIndex1]);
        fusionIndex[listIndex1] = new CoupleData<int>(temp);

        MainGameUIManager.instance.muUi.Set_InventoryUI(allModuleData);
        MainGameUIManager.instance.muUi.Set_FusionUI(allModuleData, fusionIndex);
        Reset_Interface();
    }

    #endregion

    #region Upgrade

    #endregion

    #endregion

    #region Is

    // 분해 슬롯
    public bool Is_EmptyDecompositionSlot()
    {
        return decompositionIndex.typeBase == -1 && decompositionIndex.typeSpecial == -1;
    }

    // 퓨전 슬롯
    public bool Is_EmptyFusionSlot()
    {
        for (int i = 0; i < fusionIndex.Length; i++)
        {
            if (fusionIndex[i].typeBase == -1 && fusionIndex[i].typeSpecial == -1)
            {
                return true;
            }
        }
        return false;
    }

    public bool Is_EmptyFusionSlot(out int emptyIndex)
    {
        for (int i = 0; i < fusionIndex.Length; i++)
        {
            if (fusionIndex[i].typeBase == -1 && fusionIndex[i].typeSpecial == -1)
            {
                emptyIndex = i;
                return true;
            }
        }
        emptyIndex = -1;
        return false;
    }

    public bool Is_EmptyFusionSlot(int index)
    {
        if (fusionIndex[index].typeBase == -1 && fusionIndex[index].typeSpecial == -1)
        {
            return true;
        }
        return false;
    }

    // 퓨전 스롯에 이미 가지고 있는가?
    public bool Is_IncludeFusionSlots(CoupleData<int> index)
    {
        return Is_Include(index, fusionIndex);
    }

    public bool Is_IncludeFusionSlots(CoupleData<int> index, out int listIndex)
    {
        bool result = Is_Include(index, fusionIndex, out int outListIndex);
        listIndex = outListIndex;
        return result;
    }

    // 모두 비었는가?
    public bool Is_AllEmptyFusionSlot()
    {
        bool result = true;
        for (int i = 0; i < fusionIndex.Length; i++)
        {
            if (fusionIndex[i].typeBase != -1 || fusionIndex[i].typeSpecial != -1)
            {
                result = false;
            }
        }
        return result;
    }

    // 퓨전 슬롯의 랭크가 같은가?
    public bool Is_SameRankFusionSlots()
    {
        for (int i = 1; i < fusionIndex.Length; i++)
        {
            // 랭크가 다르다면
            if (Get_ModuleState(fusionIndex[0].typeBase, fusionIndex[0].typeSpecial).thisItemData.rank !=
                Get_ModuleState(fusionIndex[i].typeBase, fusionIndex[i].typeSpecial).thisItemData.rank)
            {
                return false;
            }
        }
        return true;
    }

    // 장착된 인덱스들 중에서 포함되어있는지
    public bool Is_IncludeOnlyEquipped(CoupleData<int> index)
    {
        return Is_Include(index, Get_OnlyEquippedIndex());
    }

    // 모든 장착 인덱스 중에 포함되어 있는가
    public bool Is_IncludeEquipped(CoupleData<int> index, out int listIndex)
    {
        bool result = Is_Include(index, equippedIndex, out int outListIndex);
        listIndex = outListIndex;
        return result;
    }

    // 이미 가지고 있는가
    public bool Is_Include(CoupleData<int> index, CoupleData<int>[] indexList)
    {
        for (int i = 0; i < indexList.Length; i++)
        {
            if (indexList[i].typeBase == index.typeBase &&
                indexList[i].typeSpecial == index.typeSpecial)
            {
                return true;
            }
        }

        return false;
    }

    public bool Is_Include(CoupleData<int> index, CoupleData<int>[] indexList, out int includeListIndex)
    {
        for (int i = 0; i < indexList.Length; i++)
        {
            if (indexList[i].typeBase == index.typeBase &&
                indexList[i].typeSpecial == index.typeSpecial)
            {
                includeListIndex = i;
                return true;
            }
        }

        includeListIndex = -1;
        return false;
    }

    #endregion

    #region Rank

    public void Set_UpRank(CoupleData<int> index)
    {
        allModuleData[index.typeBase][index.typeSpecial].thisItemData.rank++;
        MainGameUIManager.instance.muUi.Set_InventoryUI(allModuleData);
        Reset_Interface();
    }

    #endregion

    #region Remove

    public void Remove_ModuleState(CoupleData<int> index)
    {
        if (Get_ModuleState(index.typeBase, index.typeSpecial) == null) return;

        // 장착되어 있다면 제거
        if (Is_IncludeEquipped(index, out int listIndex))
            equippedIndex[listIndex] = new CoupleData<int>(-1, -1);

        allModuleData[index.typeBase][index.typeSpecial] = null;

        MainGameUIManager.instance.muUi.Set_InventoryUI(allModuleData); 
        MainGameUIManager.instance.muUi.Set_EquipedUI(allModuleData, equippedIndex);
        Reset_Interface();
    }

    public void Remove_ModuleState(List<CoupleData<int>> indexList)
    {
        for (int i = 0; i < indexList.Count; i++)
        {
            if (Get_ModuleState(indexList[i].typeBase, indexList[i].typeSpecial) == null) return;
            
            // 장착되어 있다면 제거
            if (Is_IncludeEquipped(indexList[i], out int listIndex))
                equippedIndex[listIndex] = new CoupleData<int>(-1, -1);

            allModuleData[indexList[i].typeBase][indexList[i].typeSpecial] = null;
        }

        MainGameUIManager.instance.muUi.Set_InventoryUI(allModuleData);
        MainGameUIManager.instance.muUi.Set_EquipedUI(allModuleData, equippedIndex);
        Reset_Interface();
    }

    #endregion

    #region Gain

    // 아이템 상호작용해 획득
    public void Gain_ModuleState(ItemData_Field itemDataField)
    {
        // 빈 공간이 있어야 획득 가능
        CoupleData<int> index = Get_EmptyModuleState();
        if (index.typeBase == -1 || index.typeSpecial == -1) return;
       
        // 아이템 데이터 초기화
        ModuleState newModuleState = ModuleState.Get_AllModuleState()[itemDataField.id];
        newModuleState.Set_State(itemDataArr[itemDataField.id]);
        newModuleState.thisItemData.rank = itemDataField.rank;

        ItemData_UIVisual stateUI = new ItemData_UIVisual(
            Get_CorrectItemIcon(itemDataField.id),
            itemDataField.rank);

        allModuleData[index.typeBase][index.typeSpecial] = newModuleState;

        MainGameUIManager.instance.muUi.Set_InventoryUI(allModuleData);
    }

    // 아이템을 랜덤하게 획득
    public void Gain_ModuleState()
    {
        Gain_ModuleState(Get_RandomInteractItem());
    }

    #endregion

    #region Interface (Base)

    public void Active_Hit(EnemyController enemy)
    {
        Active(iWhen_HitList, enemy);
    }

    public void Active_CriticalHit(EnemyController enemy)
    {
        Active(iWhen_CriticalHitList, enemy);
    }

    public void Active_Fire()
    {
        Active(iWhen_FireList);
    }


    // Base
    private void Active<T>(List<T> iWhenList, EnemyController enemy = null) where T : IWhen
    {
        if (iWhenList.Count <= 0) return;

        for (int i = 0; i < iWhenList.Count; i++)
            iWhenList[i].Play_When(enemy);
    }

    #endregion

    #region Interface (Synchrony)

    public void ActiveSync_Start()
    {
        ActiveSync(iWhenSync_StartList);
    }


    public void ActiveSync_Fire(BulletController bullet)
    {
        ActiveSync(iWhenSync_FireList, null, bullet);
    }

    public void ActiveSync_AfterFire()
    {
        ActiveSync(iWhenSync_AfterFireList);
    }


    public void ActiveSync_Hit()
    {
        ActiveSync(iWhenSync_HitList);
    }

    public void ActiveSync_CriticalHit()
    {
        ActiveSync(iWhenSync_CriticalHitList);
    }


    public void ActiveSync_EnemyTakingFire(EnemyController enemy)
    {
        ActiveSync(iWhenSync_GetFireList, enemy);
    }

    public void ActiveSync_EnemyTakingCold(EnemyController enemy)
    {
        ActiveSync(iWhenSync_GetColdList, enemy);
    }

    public void ActiveSync_EnemyTakingElectricity(EnemyController enemy)
    {
        ActiveSync(iWhenSync_GetElectricityList, enemy);
    }

    public void ActiveSync_EnemyTakingCorrosion(EnemyController enemy)
    {
        ActiveSync(iWhenSync_GetCorrosionList, enemy);
    }


    // Base
    private void ActiveSync<T>(List<T> iWhenList, EnemyController enemy = null, BulletController bullet = null) where T : IWhenSync
    {
        if (iWhenList.Count <= 0) return;

        for (int i = 0; i < iWhenList.Count; i++)
            iWhenList[i].Play_When(enemy, bullet);
    }

    #endregion

    #region MainChip

    public Dictionary<int, int> Get_CurrentMainChipData()
    {
        return mainChipAmalgamationDict;
    }

    public List<int> Get_MainChipIDData(ModuleState moduleState)
    {
        return new List<int>
        {
            moduleState.thisItemData.r1_MainChipID,
            moduleState.thisItemData.r3_MainChipID,
            moduleState.thisItemData.r5_MainChipID
        };
    }

    private void Add_MainChipData(ModuleState moduleState)
    {
        List<int> synergyIDList = Get_MainChipIDData(moduleState);

        if (moduleState.thisItemData.rank >= 5)
        {
            DevTool.Add_AmountForDict(ref mainChipAmalgamationDict, synergyIDList[0], 3);
            DevTool.Add_AmountForDict(ref mainChipAmalgamationDict, synergyIDList[1], 2);
            DevTool.Add_AmountForDict(ref mainChipAmalgamationDict, synergyIDList[2], 1);
        }
        else if (moduleState.thisItemData.rank >= 3)
        {
            DevTool.Add_AmountForDict(ref mainChipAmalgamationDict, synergyIDList[0], 2);
            DevTool.Add_AmountForDict(ref mainChipAmalgamationDict, synergyIDList[1], 1);
        }
        else
        {
            DevTool.Add_AmountForDict(ref mainChipAmalgamationDict, synergyIDList[0], 1);
        }
    }

    public void Set_MainChipData()
    {
        mainChipAmalgamationDict = new Dictionary<int, int>();
        for (int i = 0; i < equippedIndex.Length; i++)
        {
            int col = equippedIndex[i].typeBase;
            int row = equippedIndex[i].typeSpecial;
            if (col != -1 && row != -1)
            {
                ModuleState moduleState = Get_ModuleState(col, row);
                if (moduleState != null) Add_MainChipData(moduleState);
            }
        }

        if (MainGameUIManager.instance.muUi != null)
            MainGameUIManager.instance.muUi.Set_SynergySlots(mainChipAmalgamationDict);

        currentAllMainChipState = Get_CurrentMainChipState();
    }

    public int Get_MainChipAmount(int id)
    {
        if (mainChipAmalgamationDict.ContainsKey(id))
        {
            return mainChipAmalgamationDict[id];
        }
        else
        {
            return -1;
        }
    }

    #endregion

    #region Set (Language)

    public void Set_DataLanguage()
    {
        Set_ItemDataLanguage();
        Set_MainChipDataLanguage();

        MainGameUIManager.instance.muUi.Set_EquipedUI(allModuleData, equippedIndex);
    }

    private void Set_ItemDataLanguage()
    {
        for (int i = 0; i < itemDataArr.Length; i++)
            ResourceManager.instance.Set_DataLanguage(itemDataArr[i], i);

        for (int i = 0; i < columnAmount; i++)
            for (int j = 0; j < rowAmount; j++)
                if (allModuleData[i][j] != null)
                {
                    allModuleData[i][j].thisItemData.Set_LanguageTxt(itemDataArr[allModuleData[i][j].thisItemData.id]);
                }
    }

    private void Set_MainChipDataLanguage()
    {
        for (int i = 0; i < mainChipDataArr.Length; i++)
            ResourceManager.instance.Set_DataLanguage(mainChipDataArr[i], i);
    }


    #endregion
}